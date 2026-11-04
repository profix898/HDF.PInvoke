using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using HDF.PInvoke;

static class Program
{
    static int Main(string[] args)
    {
        try
        {
            if (LoaderScenarios.Run(args))
                return 0;
            string originalCwd = Environment.CurrentDirectory;
            string originalPath = Environment.GetEnvironmentVariable("PATH");
            if (args.Length > 0 && !args[0].StartsWith("--"))
                NativeDependencies.Configure(Path.GetFullPath(args[0]));
            if (args.Length > 0 && args[0] == "--globals-first")
                _ = H5P.FILE_ACCESS;
            string expectedRid = Environment.GetEnvironmentVariable("HDF5_EXPECTED_RID");
            if (expectedRid != null && expectedRid != NativeDependencies.RuntimeIdentifier)
                throw new Exception($"Expected {expectedRid}, selected {NativeDependencies.RuntimeIdentifier}");
            Console.WriteLine($"RID: {NativeDependencies.RuntimeIdentifier}");
            if (H5.open() < 0)
                throw new Exception("H5.open failed");
            uint major = 0, minor = 0, release = 0;
            H5.get_libversion(ref major, ref minor, ref release);
            Console.WriteLine($"HDF5: {major}.{minor}.{release}");
            var handle = (IntPtr)typeof(NativeDependencies).GetMethod("GetLibraryHandle", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, new object[] { false });
            var hl = (IntPtr)typeof(NativeDependencies).GetMethod("GetLibraryHandle", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, new object[] { true });
            if (args.Length > 1)
            {
                int checks = 0;
                foreach (string line in File.ReadLines(args[1]))
                {
                    string[] parts = line.Split('|');
                    Type type = typeof(H5).Assembly.GetType(parts[0], throwOnError: true);
                    long actual = parts[1] == "$size" ? Marshal.SizeOf(type) : Marshal.OffsetOf(type, parts[1]).ToInt64();
                    if (actual != long.Parse(parts[2]))
                        throw new Exception($"ABI mismatch: {parts[0]}.{parts[1]} managed={actual} native={parts[2]}");
                    checks++;
                }
                Console.WriteLine($"PASS: {checks} native layout checks");
            }
            int missing = 0, imports = 0;
            foreach (var method in typeof(H5).Assembly.GetTypes().SelectMany(t => t.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)))
            {
                var import = method.GetCustomAttribute<DllImportAttribute>();
                if (import == null)
                    continue;
                imports++;
                if (!NativeLibrary.TryGetExport(import.Value == Constants.HLDLLFileName ? hl : handle, import.EntryPoint, out _))
                {
                    Console.WriteLine($"MISSING: {method.DeclaringType.Name}.{method.Name}: {import.EntryPoint}");
                    missing++;
                }
            }
            foreach (var type in new[] { typeof(H5P), typeof(H5T), typeof(H5E) })
            {
                System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(type.TypeHandle);
                foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Static).Where(f => !f.IsLiteral))
                    _ = field.GetValue(null);
            }
            if (missing != 0)
                throw new Exception($"Missing {missing} native entry points");
            var plist = H5P.create(H5P.FILE_ACCESS);
            if (plist < 0 || H5P.close(plist) < 0)
                throw new Exception("Exported-global property-list test failed");
            Console.WriteLine($"PASS: {imports} entry points and exported globals");
            VerifyDatasetRoundTrip();
            if (Environment.CurrentDirectory != originalCwd || Environment.GetEnvironmentVariable("PATH") != originalPath)
                throw new Exception("Native loading changed process-wide state");
            bool rejected = false;
            try
            {
                NativeDependencies.Configure(AppContext.BaseDirectory);
            }
            catch (InvalidOperationException) { rejected = true; }
            if (!rejected)
                throw new Exception("Reconfiguration after loading was accepted");
            return 0;
        }
        catch (Exception e) { Console.Error.WriteLine(e); return 1; }
    }

    static void VerifyDatasetRoundTrip()
    {
        string path = Path.Combine(Path.GetTempPath(), "hdf-package-" + Guid.NewGuid() + ".h5");
        long file = -1, space = -1, properties = -1, dataset = -1;
        GCHandle pinned = default;
        try
        {
            file = H5F.create(path, H5F.ACC_TRUNC);
            space = H5S.create_simple(1, new ulong[] { 0 }, new ulong[] { H5S.UNLIMITED });
            properties = H5P.create(H5P.DATASET_CREATE);
            if (file < 0 || space < 0 || properties < 0 || H5P.set_chunk(properties, 1, new ulong[] { 3 }) < 0 ||
                H5P.set_deflate(properties, 6) < 0)
                throw new Exception("Cannot create compressed dataset properties");
            dataset = H5D.create(file, "données_数据", H5T.NATIVE_INT, space, H5P.DEFAULT, properties);
            int[] values = { 17, -42, 12345 };
            pinned = GCHandle.Alloc(values, GCHandleType.Pinned);
            // The high-level library must share the core library's identifier registry.
            if (dataset < 0 || H5DO.append(dataset, H5P.DEFAULT, 0, (IntPtr)values.Length, H5T.NATIVE_INT, pinned.AddrOfPinnedObject()) < 0)
                throw new Exception("High-level append failed");
            if (H5D.close(dataset) < 0)
                throw new Exception("Dataset close failed");
            dataset = -1;
            if (H5F.close(file) < 0)
                throw new Exception("File close failed");
            file = H5F.open(path, H5F.ACC_RDONLY);
            dataset = H5D.open(file, "données_数据");
            Array.Clear(values);
            if (dataset < 0 || H5D.read(dataset, H5T.NATIVE_INT, H5S.ALL, H5S.ALL, H5P.DEFAULT, pinned.AddrOfPinnedObject()) < 0 ||
                !values.SequenceEqual(new[] { 17, -42, 12345 }))
                throw new Exception("Compressed dataset round-trip failed");
            Console.WriteLine("PASS: core/high-level compressed UTF-8 dataset round-trip");
        }
        finally
        {
            if (pinned.IsAllocated)
                pinned.Free();
            if (dataset >= 0)
                H5D.close(dataset);
            if (properties >= 0)
                H5P.close(properties);
            if (space >= 0)
                H5S.close(space);
            if (file >= 0)
                H5F.close(file);
            File.Delete(path);
        }
    }
}

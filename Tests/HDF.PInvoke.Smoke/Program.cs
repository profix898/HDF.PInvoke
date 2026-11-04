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
}

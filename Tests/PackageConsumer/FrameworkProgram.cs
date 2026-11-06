using System;
using System.IO;
using System.Runtime.InteropServices;
using HDF.PInvoke;

internal static class Program
{
    private static int Main(string[] args)
    {
        try
        {
            if (args.Length == 1 && args[0] == "--configure-first")
            {
                NativeDependencies.Configure(AppDomain.CurrentDomain.BaseDirectory);
            }

            if (args.Length == 1 && args[0] == "--missing-direct")
            {
                string missing = Path.Combine(Path.GetTempPath(), "missing-hdf-" + Guid.NewGuid());
                NativeDependencies.Configure(missing);

                try
                {
                    H5.open();
                }
                catch (Exception error)
                {
                    while (error is TypeInitializationException && error.InnerException != null)
                    {
                        error = error.InnerException;
                    }

                    if (error is DllNotFoundException && error.Message.Contains(missing))
                    {
                        Console.WriteLine("PASS: Framework direct call obeys explicit path without fallback");
                        return 0;
                    }

                    throw;
                }

                throw new Exception("Missing private library unexpectedly loaded.");
            }

            if (args.Length == 1 && args[0] == "--dont-atexit" && H5.dont_atexit() != 0)
            {
                throw new Exception("Early shutdown opt-out failed.");
            }

            uint major = 0, minor = 0, patch = 0;
            Check(H5.get_libversion(ref major, ref minor, ref patch));

            if (major != 2 || minor != 2)
            {
                throw new Exception("Wrong native version.");
            }

            if (typeof(H5).Assembly.GetType("HDF.PInvoke.LegacyNativeLibrary") == null)
            {
                throw new Exception("Framework must select the .NET Standard assembly.");
            }

            RoundTrip();

            if (H5.dont_atexit() >= 0)
            {
                throw new Exception("Late shutdown opt-out succeeded.");
            }

            Console.WriteLine("PASS: Framework selects netstandard2.0; UTF-8, Deflate, high-level append, globals and shutdown");
            return 0;
        }
        catch (Exception error)
        {
            Console.Error.WriteLine(error);
            return 1;
        }
    }

    private static void Check(long status)
    {
        if (status < 0)
        {
            throw new Exception("HDF5 operation failed.");
        }
    }

    private static void RoundTrip()
    {
        string path = Path.Combine(Path.GetTempPath(), "legacy-hdf-" + Guid.NewGuid() + ".h5");
        long file = -1, space = -1, properties = -1, dataset = -1;
        GCHandle buffer = default;

        try
        {
            Check(file = H5F.create(path, H5F.ACC_EXCL));
            Check(space = H5S.create_simple(1, new ulong[] { 0 }, new ulong[] { H5S.UNLIMITED }));
            Check(properties = H5P.create(H5P.DATASET_CREATE));
            Check(H5P.set_chunk(properties, 1, new ulong[] { 3 }));
            Check(H5P.set_deflate(properties, 6));
            Check(dataset = H5D.create(file, "données_数据", H5T.NATIVE_INT, space, H5P.DEFAULT, properties));
            int[] values = { 17, -42, 12345 };
            buffer = GCHandle.Alloc(values, GCHandleType.Pinned);
            Check(H5DO.append(dataset, H5P.DEFAULT, 0, (IntPtr)3, H5T.NATIVE_INT, buffer.AddrOfPinnedObject()));
            Check(H5D.close(dataset));
            dataset = -1;
            Check(H5F.close(file));
            file = -1;
            Check(file = H5F.open(path, H5F.ACC_RDONLY));
            Check(dataset = H5D.open(file, "données_数据"));
            Array.Clear(values, 0, values.Length);
            Check(H5D.read(dataset, H5T.NATIVE_INT, H5S.ALL, H5S.ALL, H5P.DEFAULT, buffer.AddrOfPinnedObject()));

            if (values[0] != 17 || values[1] != -42 || values[2] != 12345)
            {
                throw new Exception("Data mismatch.");
            }
        }
        finally
        {
            if (buffer.IsAllocated)
            {
                buffer.Free();
            }

            if (dataset >= 0)
            {
                H5D.close(dataset);
            }

            if (properties >= 0)
            {
                H5P.close(properties);
            }

            if (space >= 0)
            {
                H5S.close(space);
            }

            if (file >= 0)
            {
                H5F.close(file);
            }

            File.Delete(path);
        }
    }
}

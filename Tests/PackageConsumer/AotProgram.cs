using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using HDF.PInvoke;

/// <summary>
/// Validates the packaged bindings, type forwards, and callback bridges in a reflection-free NativeAOT executable.
/// </summary>
internal static unsafe class Program
{
    private static bool closeCallbackInvoked;

    /// <summary>
    /// Runs the requested NativeAOT scenario and returns a process exit code.
    /// </summary>
    private static int Main(string[] args)
    {
        try
        {
            if (RuntimeFeature.IsDynamicCodeSupported)
            {
                throw new InvalidOperationException("Expected NativeAOT execution without dynamic code generation.");
            }

            if (args.Length == 1 && args[0] == "--configure-first")
            {
                NativeDependencies.Configure(AppContext.BaseDirectory);
            }

            if (args.Length == 1 && args[0] == "--missing-direct")
            {
                var missing = Path.Combine(Path.GetTempPath(), "missing-hdf-" + Guid.NewGuid());
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
                        Console.WriteLine("PASS: NativeAOT direct call obeys explicit path without fallback");
                        return 0;
                    }

                    throw;
                }

                throw new InvalidOperationException("Missing private library unexpectedly loaded.");
            }

            if (args.Length == 1 && args[0] == "--recover-loader")
            {
                NativeDependencies.Configure(Path.Combine(Path.GetTempPath(), "missing-hdf-" + Guid.NewGuid()));
                var rejected = false;

                try
                {
                    H5.open();
                }
                catch (DllNotFoundException)
                {
                    rejected = true;
                }

                if (!rejected)
                {
                    throw new InvalidOperationException("Missing native directory was accepted.");
                }

                NativeDependencies.Configure(AppContext.BaseDirectory);
                Check(H5.open());
                Console.WriteLine("PASS: NativeAOT corrected configuration recovers a failed direct initialization");
                return 0;
            }

            if (args.Length == 1 && args[0] == "--registered-close")
            {
                var linkClass = new H5L.class_t
                {
                    version = H5L.LINK_CLASS_T_VERS,
                    id = (H5L.type_t)204,
                    comment = "aot-close",
                    trav_func = (name, group, data, size, lapl, dxpl) => -1
                };
                Check(H5L.register(ref linkClass));
                Check(H5.atclose(state => closeCallbackInvoked = state == new IntPtr(42), new IntPtr(42)));
                Check(H5.close());

                if (!closeCallbackInvoked)
                {
                    throw new InvalidOperationException("NativeAOT H5.atclose callback did not run on H5.close.");
                }

                Console.WriteLine("PASS: NativeAOT library shutdown invokes close callbacks and clears registrations");
                return 0;
            }

            if (args.Length == 1 && args[0] == "--dont-atexit" && H5.dont_atexit() != 0)
            {
                throw new InvalidOperationException("Early shutdown opt-out failed.");
            }

            var expectedRid = Environment.GetEnvironmentVariable("HDF5_EXPECTED_RID");

            if (expectedRid != null && expectedRid != NativeDependencies.RuntimeIdentifier)
            {
                throw new InvalidOperationException($"Expected {expectedRid}, selected {NativeDependencies.RuntimeIdentifier}");
            }

            uint major = 0, minor = 0, patch = 0;
            Check(H5.get_libversion(ref major, ref minor, ref patch));

            if (major != 2 || minor != 2)
            {
                throw new InvalidOperationException($"Unexpected native HDF5 version {major}.{minor}.{patch}.");
            }

            VerifyRoundTripAndBridges();

            if (H5.dont_atexit() >= 0)
            {
                throw new InvalidOperationException("Late shutdown opt-out unexpectedly succeeded.");
            }

            Console.WriteLine($"PASS: NativeAOT ({NativeDependencies.RuntimeIdentifier}) verified HDF5 {major}.{minor}.{patch}, type forwards, and callback bridges");
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
            throw new InvalidOperationException("HDF5 operation failed.");
        }
    }

    private static void VerifyRoundTripAndBridges()
    {
        const H5Z.filter_t customFilter = (H5Z.filter_t)306;
        const H5L.type_t customLink = (H5L.type_t)205;
        var path = Path.Combine(Path.GetTempPath(), "aot-hdf-" + Guid.NewGuid() + ".h5");
        long file = -1, space = -1, dcpl = -1, customDcpl = -1, fapl = -1, dataset = -1, customDataset = -1, group = -1;
        var filterRegistered = false;
        var linkRegistered = false;

        H5Z.func_t xorFilter = (uint flags, IntPtr cdCount, IntPtr cdValues, IntPtr nbytes, ref IntPtr bufSize, ref IntPtr buf) =>
        {
            var data = (byte*)buf;
            var count = nbytes.ToInt64();

            for (long i = 0; i < count; i++)
            {
                data[i] ^= 0x3C;
            }

            return nbytes;
        };

        H5L.traverse_func_t linkTraverse = (name, grp, data, size, lapl, dxpl) => H5G.open(grp, "target");

        H5FD.udata_copy_t udataCopy = udata => new IntPtr(udata.ToInt64() + 10);
        H5FD.udata_free_t udataFree = udata => 0;

        try
        {
            // 1. Custom filter registration + owned UTF-8 name copy.
            var filterClass = new H5Z.class_t
            {
                version = H5Z.CLASS_T_VERS,
                id = customFilter,
                encoder_present = 1,
                decoder_present = 1,
                name = "aot_xor",
                filter = xorFilter
            };
            Check(H5Z.register(ref filterClass));
            filterRegistered = true;

            // 2. Custom link registration.
            var linkClass = new H5L.class_t
            {
                version = H5L.LINK_CLASS_T_VERS,
                id = customLink,
                comment = "AOT Verknüpfung",
                trav_func = linkTraverse
            };
            Check(H5L.register(ref linkClass));
            linkRegistered = true;

            // 3. File-image callback struct bridge.
            Check(fapl = H5P.create(H5P.FILE_ACCESS));
            var callbacksIn = new H5FD.file_image_callbacks_t
            {
                udata_copy = udataCopy,
                udata_free = udataFree,
                udata = new IntPtr(5)
            };
            Check(H5P.set_file_image_callbacks(fapl, ref callbacksIn));
            var callbacksOut = default(H5FD.file_image_callbacks_t);
            Check(H5P.get_file_image_callbacks(fapl, ref callbacksOut));

            if (callbacksOut.udata != new IntPtr(25) || callbacksOut.udata_copy == null || callbacksOut.udata_free == null)
            {
                throw new InvalidOperationException("File-image callback bridge round-trip failed.");
            }

            // 4. Compressed high-level append + custom-filtered dataset + transitive CLong/CULong check.
            Check(file = H5F.create(path, H5F.ACC_EXCL));
            Check(group = H5G.create(file, "target"));
            Check(H5G.close(group));
            group = -1;
            Check(H5L.create_ud(file, "alias", customLink, IntPtr.Zero, IntPtr.Zero));
            var openedViaLink = H5O.open(file, "alias");
            Check(openedViaLink);
            Check(H5O.close(openedViaLink));

            var transitive = TransitiveBridge.ReadCIntegers(file);
            var directConfig = new H5AC.cache_config_t(H5AC.CURR_CACHE_CONFIG_VERSION);
            Check(H5F.get_mdc_config(file, ref directConfig));
            var directInfo = default(H5O.info_t);
            Check(H5O.get_info(file, ref directInfo));
            Check(H5O.get_info3(file, out var info2));

            if (!transitive.Item1.Equals(directConfig.epoch_length) ||
                !transitive.Item2.Equals(directInfo.fileno) ||
                transitive.Item2.Value.ToUInt64() != info2.fileno)
            {
                throw new InvalidOperationException("Transitive CLong/CULong type forwarding mismatch.");
            }

            Check(space = H5S.create_simple(1, new ulong[] { 0 }, new ulong[] { H5S.UNLIMITED }));
            Check(dcpl = H5P.create(H5P.DATASET_CREATE));
            Check(H5P.set_chunk(dcpl, 1, new ulong[] { 3 }));
            Check(H5P.set_deflate(dcpl, 6));
            Check(dataset = H5D.create(file, "données_数据", H5T.NATIVE_INT, space, H5P.DEFAULT, dcpl));
            int[] values = { 17, -42, 12345 };

            fixed (int* valuesPtr = values)
            {
                Check(H5DO.append(dataset, H5P.DEFAULT, 0, (IntPtr)values.Length, H5T.NATIVE_INT, (IntPtr)valuesPtr));
            }

            Check(H5D.close(dataset));
            dataset = -1;
            Check(H5S.close(space));
            space = -1;

            Check(space = H5S.create_simple(2, new ulong[] { 2, 2 }, null));
            Check(customDcpl = H5P.create(H5P.DATASET_CREATE));
            Check(H5P.set_chunk(customDcpl, 2, new ulong[] { 2, 2 }));
            Check(H5P.set_filter(customDcpl, customFilter, 0, IntPtr.Zero, null));
            Check(customDataset = H5D.create(file, "xor_2d", H5T.NATIVE_INT, space, H5P.DEFAULT, customDcpl));
            int[] grid = { 1, 2, 3, 4 };

            fixed (int* gridPtr = grid)
            {
                // 5. Multidimensional H5D.iterate bridge.
                var coordinateSum = 0ul;
                Check(H5D.iterate((IntPtr)gridPtr, H5T.NATIVE_INT, space, (elem, typeId, ndim, point, opData) =>
                {
                    coordinateSum += point[0] * 10 + point[1];
                    *(int*)elem *= 10;
                    return 0;
                }, IntPtr.Zero));

                if (coordinateSum != 22ul)
                {
                    throw new InvalidOperationException("H5D.iterate coordinate marshalling mismatch.");
                }

                Check(H5D.write(customDataset, H5T.NATIVE_INT, H5S.ALL, H5S.ALL, H5P.DEFAULT, (IntPtr)gridPtr));
            }

            Check(H5D.close(customDataset));
            customDataset = -1;
            Check(H5F.close(file));
            file = -1;

            // Reopen and verify both deflate and custom-filter datasets.
            Check(file = H5F.open(path, H5F.ACC_RDONLY));
            Check(dataset = H5D.open(file, "données_数据"));
            Array.Clear(values, 0, values.Length);

            fixed (int* valuesPtr = values)
            {
                Check(H5D.read(dataset, H5T.NATIVE_INT, H5S.ALL, H5S.ALL, H5P.DEFAULT, (IntPtr)valuesPtr));
            }

            if (values[0] != 17 || values[1] != -42 || values[2] != 12345)
            {
                throw new InvalidOperationException("Deflate dataset round-trip mismatch.");
            }

            Check(customDataset = H5D.open(file, "xor_2d"));
            Array.Clear(grid, 0, grid.Length);

            fixed (int* gridPtr = grid)
            {
                Check(H5D.read(customDataset, H5T.NATIVE_INT, H5S.ALL, H5S.ALL, H5P.DEFAULT, (IntPtr)gridPtr));
            }

            if (grid[0] != 10 || grid[1] != 20 || grid[2] != 30 || grid[3] != 40)
            {
                throw new InvalidOperationException("Custom filter dataset round-trip mismatch.");
            }

            // 6. H5E.walk UTF-8 error callback bridge.
            Check(H5E.clear(H5E.DEFAULT));
            Check(H5E.push(H5E.DEFAULT, "aot.c", "verify", 7, H5E.ERR_CLS, H5E.NONE_MAJOR, H5E.NONE_MINOR, "AOT Fehler 数据"));
            string walked = null;
            Check(H5E.walk(H5E.DEFAULT, H5E.direction_t.H5E_WALK_DOWNWARD, (uint n, ref H5E.error_t err, IntPtr data) =>
            {
                walked = err.desc;
                return 0;
            }, IntPtr.Zero));
            Check(H5E.clear(H5E.DEFAULT));

            if (walked != "AOT Fehler 数据")
            {
                throw new InvalidOperationException($"Unexpected H5E.walk description '{walked}'.");
            }
        }
        finally
        {
            if (customDataset >= 0)
            {
                H5D.close(customDataset);
            }

            if (dataset >= 0)
            {
                H5D.close(dataset);
            }

            if (customDcpl >= 0)
            {
                H5P.close(customDcpl);
            }

            if (dcpl >= 0)
            {
                H5P.close(dcpl);
            }

            if (space >= 0)
            {
                H5S.close(space);
            }

            if (file >= 0)
            {
                H5F.close(file);
            }

            if (fapl >= 0)
            {
                H5P.close(fapl);
            }

            if (linkRegistered)
            {
                H5L.unregister(customLink);
            }

            if (filterRegistered)
            {
                H5Z.unregister(customFilter);
            }

            GC.KeepAlive(xorFilter);
            GC.KeepAlive(linkTraverse);
            GC.KeepAlive(udataCopy);
            GC.KeepAlive(udataFree);

            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}

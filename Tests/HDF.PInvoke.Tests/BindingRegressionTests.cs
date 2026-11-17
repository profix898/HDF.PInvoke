#if HDF_LEGACY && !NETFRAMEWORK
extern alias HdfLegacyBinding;
#endif
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using HDF.PInvoke;
using Microsoft.VisualStudio.TestTools.UnitTesting;
#if HDF_LEGACY && !NETFRAMEWORK
using CLong = HdfLegacyBinding::System.Runtime.InteropServices.CLong;
using CULong = HdfLegacyBinding::System.Runtime.InteropServices.CULong;
#endif

namespace UnitTests
{
    /// <summary>
    /// Groups regression checks for Binding Regression.
    /// </summary>
    [TestClass]
    public unsafe class BindingRegressionTests
    {
        /// <summary>
        /// Verifies loader Rejects Null And Blank Configuration Consistently.
        /// </summary>
        [TestMethod]
        public void LoaderRejectsNullAndBlankConfigurationConsistently()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => NativeDependencies.Configure(null));
            Assert.ThrowsExactly<ArgumentException>(() => NativeDependencies.Configure(" "));
        }

        /// <summary>
        /// Verifies metadata Cache Getter Ignores Unused Input Fields.
        /// </summary>
        [TestMethod]
        public void MetadataCacheGetterIgnoresUnusedInputFields()
        {
            var path = Path.GetTempFileName();
            var file = H5F.create(path, H5F.ACC_TRUNC);
            var fapl = H5P.create(H5P.FILE_ACCESS);
            Assert.IsTrue(file >= 0 && fapl >= 0);

            try
            {
                var config = new H5AC.cache_config_t(H5AC.CURR_CACHE_CONFIG_VERSION);
                config.min_clean_fraction = double.NaN;
                config.epoch_length = new CLong(12345);
                Assert.IsTrue(H5F.get_mdc_config(-1, ref config) < 0);
                Assert.IsTrue(H5P.get_mdc_config(-1, ref config) < 0);
                Assert.IsTrue(double.IsNaN(config.min_clean_fraction));
                Assert.AreEqual(12345L, (long)config.epoch_length.Value);
                Assert.IsTrue(H5F.get_mdc_config(file, ref config) >= 0);
                Assert.IsFalse(double.IsNaN(config.min_clean_fraction));
                Assert.IsTrue((long)config.epoch_length.Value > 0);
                config.min_clean_fraction = 0.25;
                Assert.IsTrue(H5P.set_mdc_config(fapl, ref config) >= 0);
                var roundTrip = new H5AC.cache_config_t(H5AC.CURR_CACHE_CONFIG_VERSION);
                Assert.IsTrue(H5P.get_mdc_config(fapl, ref roundTrip) >= 0);
                Assert.AreEqual(0.25, roundTrip.min_clean_fraction);
                Assert.AreEqual(config.epoch_length, roundTrip.epoch_length);
            }
            finally
            {
                H5P.close(fapl);
                H5F.close(file);
                File.Delete(path);
            }
        }

        /// <summary>
        /// Verifies C long and unsigned long types enforce platform width and ignore trailing Windows padding.
        /// </summary>
        [TestMethod]
        public void CIntegerTypesMatchPlatformWidthAndIgnoreWindowsPadding()
        {
            var signed = new CLong(-42);
            var signedFromPtr = new CLong(new IntPtr(-42));
            Assert.AreEqual(signed, signedFromPtr);
            Assert.IsTrue(signed.Equals((object)signedFromPtr));
            Assert.IsFalse(signed.Equals((object)42));
            Assert.AreEqual(signed.GetHashCode(), signedFromPtr.GetHashCode());
            Assert.AreEqual((-42).ToString(), signed.ToString());

            var unsigned = new CULong(42u);
            var unsignedFromPtr = new CULong(new UIntPtr(42u));
            Assert.AreEqual(unsigned, unsignedFromPtr);
            Assert.IsTrue(unsigned.Equals((object)unsignedFromPtr));
            Assert.IsFalse(unsigned.Equals((object)42u));
            Assert.AreEqual(unsigned.GetHashCode(), unsignedFromPtr.GetHashCode());
            Assert.AreEqual(42u.ToString(), unsigned.ToString());

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Assert.ThrowsExactly<OverflowException>(() => _ = new CLong(new IntPtr(long.MaxValue)));
                Assert.ThrowsExactly<OverflowException>(() => _ = new CULong(new UIntPtr(ulong.MaxValue)));
            }

            var path = Path.GetTempFileName();
            var file = H5F.create(path, H5F.ACC_TRUNC);
            Assert.IsTrue(file >= 0);

            try
            {
                var clean = default(H5O.info_t);
                var dirty = default(H5O.info_t);
                var bytes = (byte*)&dirty;

                for (var i = 0; i < sizeof(H5O.info_t); i++)
                {
                    bytes[i] = 0xFF;
                }

                Assert.IsTrue(H5O.get_info(file, ref clean) >= 0);
                Assert.IsTrue(H5O.get_info(file, ref dirty) >= 0);
                Assert.AreEqual(clean.fileno, dirty.fileno);
                Assert.AreEqual(clean.fileno.GetHashCode(), dirty.fileno.GetHashCode());
                Assert.AreEqual(clean.fileno.ToString(), dirty.fileno.ToString());
            }
            finally
            {
                H5F.close(file);
                File.Delete(path);
            }
        }

        /// <summary>
        /// Verifies multidimensional dataset iteration marshals coordinates and rethrows callback exceptions safely.
        /// </summary>
        [TestMethod]
        public void DatasetIterateBridgesMultidimensionalCoordinatesAndExceptions()
        {
            var space = H5S.create_simple(2, new ulong[] { 2, 3 }, null);
            Assert.IsTrue(space >= 0);
            var elements = new[] { 10, 20, 30, 40, 50, 60 };
            var handle = GCHandle.Alloc(elements, GCHandleType.Pinned);

            try
            {
                var coordinates = new List<(ulong Row, ulong Col)>();
                H5D.operator_t visitor = (IntPtr elem, long typeId, uint ndim, ulong[] point, IntPtr opData) =>
                {
                    Assert.AreEqual(2u, ndim);
                    Assert.AreEqual(2, point.Length);
                    Assert.AreEqual(new IntPtr(0x1234), opData);
                    coordinates.Add((point[0], point[1]));
                    *(int*)elem *= 2;
                    return 0;
                };

                Assert.AreEqual(0, H5D.iterate(handle.AddrOfPinnedObject(), H5T.NATIVE_INT, space, visitor, new IntPtr(0x1234)));
                CollectionAssert.AreEqual(new[] { 20, 40, 60, 80, 100, 120 }, elements);
                CollectionAssert.AreEqual(new[] { (0ul, 0ul), (0ul, 1ul), (0ul, 2ul), (1ul, 0ul), (1ul, 1ul), (1ul, 2ul) }, coordinates);

                var seenBeforeStop = 0;
                Assert.AreEqual(7, H5D.iterate(handle.AddrOfPinnedObject(), H5T.NATIVE_INT, space, (elem, typeId, ndim, point, opData) => ++seenBeforeStop == 2 ? 7 : 0, IntPtr.Zero));
                Assert.AreEqual(2, seenBeforeStop);

                var thrown = Assert.ThrowsExactly<InvalidOperationException>(() =>
                    H5D.iterate(handle.AddrOfPinnedObject(), H5T.NATIVE_INT, space, (elem, typeId, ndim, point, opData) => throw new InvalidOperationException("iterate-boom"), IntPtr.Zero));
                Assert.AreEqual("iterate-boom", thrown.Message);
            }
            finally
            {
                handle.Free();
                H5S.close(space);
            }
        }

        /// <summary>
        /// Verifies error-stack traversal captures and rethrows managed callback exceptions after native frames unwind.
        /// </summary>
        [TestMethod]
        public void ErrorWalkRethrowsManagedCallbackExceptionsCleanly()
        {
            Assert.IsTrue(H5E.clear(H5E.DEFAULT) >= 0);

            try
            {
                Assert.IsTrue(H5E.push(H5E.DEFAULT, "test.c", "check", 42, H5E.ERR_CLS, H5E.NONE_MAJOR, H5E.NONE_MINOR, "boom") >= 0);
                var thrown = Assert.ThrowsExactly<InvalidOperationException>(() =>
                    H5E.walk(H5E.DEFAULT, H5E.direction_t.H5E_WALK_DOWNWARD, (uint n, ref H5E.error_t err, IntPtr data) => throw new InvalidOperationException("walk-boom"), IntPtr.Zero));
                Assert.AreEqual("walk-boom", thrown.Message);
            }
            finally
            {
                H5E.clear(H5E.DEFAULT);
            }
        }

        /// <summary>
        /// Verifies registered filters copy caller name buffers, root callbacks across GC, and transform chunk data.
        /// </summary>
        [TestMethod]
        public void RegisteredFilterSurvivesCollectionCopiesNameAndTransformsChunks()
        {
            const H5Z.filter_t filterId = (H5Z.filter_t)305;
            var path = Path.GetTempFileName();
            long file = -1, space = -1, dcpl = -1, dataset = -1;
            var registered = false;
            WeakReference callbackRef = null;

            try
            {
                callbackRef = RegisterTemporaryXorFilter(filterId);
                registered = true;

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                Assert.IsTrue(callbackRef.IsAlive, "Native HDF5 still owns the filter registration's callback pointer.");

                file = H5F.create(path, H5F.ACC_TRUNC);
                space = H5S.create_simple(1, new ulong[] { 4 }, null);
                dcpl = H5P.create(H5P.DATASET_CREATE);
                Assert.IsTrue(file >= 0 && space >= 0 && dcpl >= 0);
                Assert.IsTrue(H5P.set_chunk(dcpl, 1, new ulong[] { 4 }) >= 0);
                Assert.IsTrue(H5P.set_filter(dcpl, filterId, 0, (IntPtr)1, new[] { 0x5Au }) >= 0);

                uint flags = 0, config = 0;
                var cdCount = (IntPtr)1;
                var cdValues = new uint[1];
                var nameBytes = new byte[64];
                Assert.IsTrue(H5P.get_filter_by_id(dcpl, filterId, ref flags, ref cdCount, cdValues, (IntPtr)nameBytes.Length, nameBytes, ref config) >= 0);
                var terminator = Array.IndexOf<byte>(nameBytes, 0);
                Assert.AreEqual("xor_过滤器", Encoding.UTF8.GetString(nameBytes, 0, terminator));

                dataset = H5D.create(file, "filtered", H5T.NATIVE_INT, space, H5P.DEFAULT, dcpl);
                Assert.IsTrue(dataset >= 0);

                var expected = new[] { 11, -22, 333, -444 };
                fixed (int* writePtr = expected)
                {
                    Assert.IsTrue(H5D.write(dataset, H5T.NATIVE_INT, H5S.ALL, H5S.ALL, H5P.DEFAULT, (IntPtr)writePtr) >= 0);
                }

                Assert.IsTrue(H5D.close(dataset) >= 0);
                dataset = -1;
                Assert.IsTrue(H5F.close(file) >= 0);

                file = H5F.open(path, H5F.ACC_RDONLY);
                dataset = H5D.open(file, "filtered");
                Assert.IsTrue(file >= 0 && dataset >= 0);

                var actual = new int[4];
                fixed (int* readPtr = actual)
                {
                    Assert.IsTrue(H5D.read(dataset, H5T.NATIVE_INT, H5S.ALL, H5S.ALL, H5P.DEFAULT, (IntPtr)readPtr) >= 0);
                }

                CollectionAssert.AreEqual(expected, actual);
                Assert.IsTrue(H5D.close(dataset) >= 0);
                dataset = -1;
                Assert.IsTrue(H5F.close(file) >= 0);
                file = -1;

                Assert.IsTrue(H5Z.unregister(filterId) >= 0);
                registered = false;

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                Assert.IsFalse(callbackRef.IsAlive, "Unregistering a filter must release its managed callback roots.");
            }
            finally
            {
                if (dataset >= 0)
                {
                    H5D.close(dataset);
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

                if (registered)
                {
                    H5Z.unregister(filterId);
                }

                File.Delete(path);
            }
        }

        /// <summary>
        /// Verifies file-image callbacks round-trip through property lists via the blittable callback bridge.
        /// </summary>
        [TestMethod]
        public void FileImageCallbacksRoundTripThroughPropertyList()
        {
            var fapl = H5P.create(H5P.FILE_ACCESS);
            Assert.IsTrue(fapl >= 0);
            var copyCalls = 0;
            var freeCalls = 0;
            var mallocCalls = 0;

            H5FD.image_malloc_t imageMalloc = (size, op, udata) =>
            {
                mallocCalls++;
                return H5.allocate_memory(size, 0);
            };
            H5FD.image_memcpy_t imageMemcpy = (dest, src, size, op, udata) =>
            {
                Buffer.MemoryCopy((void*)src, (void*)dest, size.ToInt64(), size.ToInt64());
                return dest;
            };
            H5FD.image_realloc_t imageRealloc = (ptr, size, op, udata) => H5.resize_memory(ptr, size);
            H5FD.image_free_t imageFree = (ptr, op, udata) => H5.free_memory(ptr);
            H5FD.udata_copy_t udataCopy = udata =>
            {
                copyCalls++;
                return new IntPtr(udata.ToInt64() + 1);
            };
            H5FD.udata_free_t udataFree = udata =>
            {
                freeCalls++;
                return 0;
            };

            try
            {
                var untouched = new H5FD.file_image_callbacks_t { udata = new IntPtr(999) };
                Assert.IsTrue(H5P.get_file_image_callbacks(-1, ref untouched) < 0);
                Assert.AreEqual(new IntPtr(999), untouched.udata);

                var input = new H5FD.file_image_callbacks_t
                {
                    image_malloc = imageMalloc,
                    image_memcpy = imageMemcpy,
                    image_realloc = imageRealloc,
                    image_free = imageFree,
                    udata_copy = udataCopy,
                    udata_free = udataFree,
                    udata = new IntPtr(100)
                };
                Assert.IsTrue(H5P.set_file_image_callbacks(fapl, ref input) >= 0);
                Assert.AreEqual(1, copyCalls);

                var output = default(H5FD.file_image_callbacks_t);
                Assert.IsTrue(H5P.get_file_image_callbacks(fapl, ref output) >= 0);
                Assert.AreEqual(2, copyCalls);
                Assert.AreEqual(new IntPtr(102), output.udata);
                Assert.IsNotNull(output.image_malloc);
                Assert.IsNotNull(output.image_memcpy);
                Assert.IsNotNull(output.image_realloc);
                Assert.IsNotNull(output.image_free);
                Assert.IsNotNull(output.udata_copy);
                Assert.IsNotNull(output.udata_free);

                var block = output.image_malloc((IntPtr)16, H5FD.file_image_op_t.PROPERTY_LIST_GET, output.udata);
                Assert.AreNotEqual(IntPtr.Zero, block);
                Assert.AreEqual(1, mallocCalls);
                Assert.AreEqual(0, output.image_free(block, H5FD.file_image_op_t.PROPERTY_LIST_GET, output.udata));
            }
            finally
            {
                Assert.IsTrue(H5P.close(fapl) >= 0);
                Assert.AreEqual(1, freeCalls);
                GC.KeepAlive(imageMalloc);
                GC.KeepAlive(imageMemcpy);
                GC.KeepAlive(imageRealloc);
                GC.KeepAlive(imageFree);
                GC.KeepAlive(udataCopy);
                GC.KeepAlive(udataFree);
            }
        }

#if NET8_0_OR_GREATER && !HDF_LEGACY
        /// <summary>
        /// Verifies H5.atclose rejects callbacks registered from a collectible AssemblyLoadContext.
        /// </summary>
        [TestMethod]
        public void AtCloseRejectsCollectibleAssemblyLoadContext()
        {
            var context = new System.Runtime.Loader.AssemblyLoadContext("HDF5 collectible atclose test", isCollectible: true);

            try
            {
                var plugin = context.LoadFromAssemblyPath(typeof(H5).Assembly.Location);
                var h5 = plugin.GetType("HDF.PInvoke.H5", throwOnError: true);
                var delegateType = h5.GetNestedType("atclose_t", BindingFlags.Public);
                Assert.IsNotNull(delegateType);
                var noop = Delegate.CreateDelegate(delegateType, typeof(BindingRegressionTests).GetMethod(nameof(IgnoreAtClose), BindingFlags.NonPublic | BindingFlags.Static));
                var error = Assert.ThrowsExactly<TargetInvocationException>(() =>
                    h5.GetMethod("atclose", new[] { delegateType, typeof(IntPtr) }).Invoke(null, new object[] { noop, IntPtr.Zero }));
                Assert.IsInstanceOfType<InvalidOperationException>(error.InnerException);
            }
            finally
            {
                context.Unload();
            }
        }

        private static void IgnoreAtClose(IntPtr state)
        {
        }
#endif

        /// <summary>
        /// Verifies registered Link Callback Survives Garbage Collection.
        /// </summary>
        [TestMethod]
        public void RegisteredLinkCallbackSurvivesGarbageCollection()
        {
            var path = Path.GetTempFileName();
            var file = H5F.create(path, H5F.ACC_TRUNC);
            Assert.IsTrue(file >= 0);
            var registered = false;
            var id = (H5L.type_t)202;

            try
            {
                var group = H5G.create(file, "target");
                Assert.IsTrue(group >= 0);
                Assert.IsTrue(H5G.close(group) >= 0);
                var callback = RegisterTemporaryCallback(id);
                registered = true;

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                Assert.IsTrue(callback.IsAlive, "Native HDF5 still owns the registration's callback pointer.");

                Assert.IsTrue(H5L.create_ud(file, "custom", id, IntPtr.Zero, IntPtr.Zero) >= 0);
                var target = H5O.open(file, "custom");
                Assert.IsTrue(target >= 0);
                Assert.IsTrue(H5O.close(target) >= 0);
                Assert.IsTrue(H5L.delete(file, "custom") >= 0);
            }
            finally
            {
                if (registered)
                {
                    Assert.IsTrue(H5L.unregister(id) >= 0);
                }

                H5F.close(file);
                File.Delete(path);
            }
        }

        /// <summary>
        /// Registers a filter using a transient UTF-8 string and captured state so GC and ownership checks are meaningful.
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static WeakReference RegisterTemporaryXorFilter(H5Z.filter_t id)
        {
            var maskSeed = new object();
            H5Z.can_apply_func_t canApply = (dcpl, typeId, spaceId) => maskSeed != null ? 1 : 0;
            H5Z.set_local_func_t setLocal = (dcpl, typeId, spaceId) => maskSeed != null ? 0 : -1;
            H5Z.func_t filter = (uint flags, IntPtr cdCount, IntPtr cdValues, IntPtr nbytes, ref IntPtr bufSize, ref IntPtr buf) =>
            {
                GC.KeepAlive(maskSeed);
                var mask = cdCount.ToInt64() > 0 ? (byte)((uint*)cdValues)[0] : (byte)0x5A;
                var data = (byte*)buf;
                var length = nbytes.ToInt64();

                for (long i = 0; i < length; i++)
                {
                    data[i] ^= mask;
                }

                return nbytes;
            };

            var info = new H5Z.class_t
            {
                version = H5Z.CLASS_T_VERS,
                id = id,
                encoder_present = 1,
                decoder_present = 1,
                name = new string("xor_过滤器".ToCharArray()),
                can_apply = canApply,
                set_local = setLocal,
                filter = filter
            };
            Assert.IsTrue(H5Z.register(ref info) >= 0);
            return new WeakReference(filter);
        }

        /// <summary>
        /// Registers a callback with captured state so garbage-collection tests can observe its lifetime.
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static WeakReference RegisterTemporaryCallback(H5L.type_t id)
        {
            // Capture a value to avoid the compiler's permanent cache for noncapturing lambdas.
            var target = new string("target".ToCharArray());
            H5L.traverse_func_t callback = (name, group, data, size, lapl, dxpl) => H5G.open(group, target);
            var info = new H5L.class_t { version = H5L.LINK_CLASS_T_VERS, id = id, comment = "Verknüpfung 数据", trav_func = callback };
            Assert.IsTrue(H5L.register(ref info) >= 0);
            return new WeakReference(callback);
        }
    }
}

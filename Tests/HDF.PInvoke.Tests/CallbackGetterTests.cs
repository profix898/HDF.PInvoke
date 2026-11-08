using System;
using System.Runtime.InteropServices;
using HDF.PInvoke;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    /// <summary>Exercises callback output pointers on both managed implementations and Mono.</summary>
    [TestClass]
    public class CallbackGetterTests
    {
        /// <summary>Describes a property-list getter with callback and context outputs.</summary>
        private delegate int Getter<T>(long id, ref T callback, ref IntPtr state) where T : Delegate;

        /// <summary>Checks null output, callback invocation, context identity and failed-getter preservation.</summary>
        private static void RoundTrip<T>(long propertyClass, T callback, Getter<T> get,
            Func<long, T, IntPtr, int> set, Action<T> invoke) where T : Delegate
        {
            var list = H5P.create(propertyClass);
            Assert.IsTrue(list >= 0);

            try
            {
                var actual = callback;
                var state = new IntPtr(123);
                Assert.IsTrue(get(list, ref actual, ref state) >= 0);
                Assert.IsNull(actual);
                Assert.AreEqual(IntPtr.Zero, state);

                Assert.IsTrue(set(list, callback, new IntPtr(456)) >= 0);
                Assert.IsTrue(get(list, ref actual, ref state) >= 0);
                Assert.IsNotNull(actual);
                Assert.AreEqual(new IntPtr(456), state);
                invoke(actual);

                // Native failure must not replace an existing managed callback with a null pointer.
                var retained = actual;
                Assert.IsTrue(get(-1, ref actual, ref state) < 0);
                Assert.AreSame(retained, actual);
            }
            finally
            {
                H5P.close(list);
                GC.KeepAlive(callback);
            }
        }

        /// <summary>Rejects undersized native output buffers while allowing callers to omit boundary output.</summary>
        [TestMethod]
        public void AppendGetterValidatesBoundaryBuffer()
        {
            var list = H5P.create(H5P.DATASET_ACCESS);
            Assert.IsTrue(list >= 0);

            try
            {
                H5D.append_cb_t callback = null;
                var state = IntPtr.Zero;
                Assert.ThrowsExactly<ArgumentException>(() => H5P.get_append_flush(list, 2, new ulong[1], ref callback, ref state));
                Assert.IsTrue(H5P.get_append_flush(list, 2, null, ref callback, ref state) >= 0);
                Assert.IsNull(callback);
            }
            finally
            {
                H5P.close(list);
            }
        }
        /// <summary>Retrieves an error handler, invokes it and observes a cleared native callback.</summary>
        [TestMethod]
        public void ErrorCallbackGetterReturnsAndClearsNativeCallback()
        {
            var stack = H5E.create_stack();
            Assert.IsTrue(stack >= 0);
            H5E.auto_t callback = (id, state) => 29;
            try
            {
                Assert.IsTrue(H5E.set_auto(stack, callback, new IntPtr(41)) >= 0);
                H5E.auto_t actual = null;
                var state = IntPtr.Zero;
                Assert.IsTrue(H5E.get_auto(stack, ref actual, ref state) >= 0);
                Assert.IsNotNull(actual);
                Assert.AreEqual(new IntPtr(41), state);
                Assert.AreEqual(29, actual(stack, state));
                Assert.IsTrue(H5E.set_auto(stack, null, IntPtr.Zero) >= 0);
                Assert.IsTrue(H5E.get_auto(stack, ref actual, ref state) >= 0);
                Assert.IsNull(actual);
            }
            finally
            {
                H5E.close_stack(stack);
                GC.KeepAlive(callback);
            }
        }
        /// <summary>Round-trips an external-link callback with UTF-8 strings and a by-reference flags argument.</summary>
        [TestMethod]
        public void ExternalLinkCallbackGetterPreservesSignature()
        {
            H5L.elink_traverse_t callback = (string parentFile, string parentGroup, string childFile, string childObject,
                ref uint flags, long access, IntPtr state) =>
            {
                Assert.AreEqual("日本語", childObject);
                flags = 17;
                return 19;
            };
            RoundTrip(H5P.LINK_ACCESS, callback, H5P.get_elink_cb, H5P.set_elink_cb, actual =>
            {
                uint flags = 0;
                Assert.AreEqual(19, actual("parent", "group", "child", "日本語", ref flags, 0, IntPtr.Zero));
                Assert.AreEqual(17u, flags);
            });
        }

        /// <summary>Round-trips a committed-datatype search callback and its native enum result.</summary>
        [TestMethod]
        public void DatatypeSearchCallbackGetterPreservesSignature()
        {
            H5O.mcdt_search_cb_t callback = state => H5O.mcdt_search_ret_t.CONT;
            RoundTrip(H5P.OBJECT_COPY, callback, H5P.get_mcdt_search_cb, H5P.set_mcdt_search_cb,
                actual => Assert.AreEqual(H5O.mcdt_search_ret_t.CONT, actual(IntPtr.Zero)));
        }

        /// <summary>Round-trips a conversion-exception callback with pointer parameters.</summary>
        [TestMethod]
        public void ConversionCallbackGetterPreservesSignature()
        {
            H5T.conv_except_func_t callback = (kind, source, destination, input, output, state) => H5T.conv_ret_t.HANDLED;
            RoundTrip(H5P.DATASET_XFER, callback, H5P.get_type_conv_cb, H5P.set_type_conv_cb,
                actual => Assert.AreEqual(H5T.conv_ret_t.HANDLED, actual(default, 0, 0, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero)));
        }

        /// <summary>Round-trips a flush callback and preserves it when the native getter rejects its identifier.</summary>
        [TestMethod]
        public void FlushCallbackGetterPreservesSignature()
        {
            H5F.flush_cb_t callback = (id, state) => 23;
            RoundTrip(H5P.FILE_ACCESS, callback, H5P.get_object_flush_cb, H5P.set_object_flush_cb,
                actual => Assert.AreEqual(23, actual(0, IntPtr.Zero)));
        }

        /// <summary>Round-trips both allocator callbacks and their independent context pointers in one getter.</summary>
        [TestMethod]
        public void VariableLengthMemoryGetterPreservesBothCallbacks()
        {
            var list = H5P.create(H5P.DATASET_XFER);
            Assert.IsTrue(list >= 0);
            H5MM.allocate_t allocate = (size, state) => Marshal.AllocHGlobal(size);
            H5MM.free_t release = (pointer, state) => Marshal.FreeHGlobal(pointer);
            try
            {
                var actualAllocate = allocate;
                var actualRelease = release;
                IntPtr allocationState = IntPtr.Zero, releaseState = IntPtr.Zero;
                Assert.IsTrue(H5P.get_vlen_mem_manager(list, ref actualAllocate, ref allocationState, ref actualRelease, ref releaseState) >= 0);
                Assert.IsNull(actualAllocate);
                Assert.IsNull(actualRelease);

                Assert.IsTrue(H5P.set_vlen_mem_manager(list, allocate, new IntPtr(31), release, new IntPtr(37)) >= 0);
                Assert.IsTrue(H5P.get_vlen_mem_manager(list, ref actualAllocate, ref allocationState, ref actualRelease, ref releaseState) >= 0);
                Assert.AreEqual(new IntPtr(31), allocationState);
                Assert.AreEqual(new IntPtr(37), releaseState);
                var memory = actualAllocate(new IntPtr(16), allocationState);
                Assert.AreNotEqual(IntPtr.Zero, memory);
                actualRelease(memory, releaseState);
            }
            finally
            {
                H5P.close(list);
                GC.KeepAlive(allocate);
                GC.KeepAlive(release);
            }
        }
    }
}

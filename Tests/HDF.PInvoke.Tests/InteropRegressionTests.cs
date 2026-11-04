using System;
using System.IO;
using System.Runtime.InteropServices;
using HDF.PInvoke;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public unsafe class InteropRegressionTests
    {
        [TestMethod]
        public void ChunkCountFailureRemainsNegative()
        {
            ulong chunks = 0;
            Assert.IsTrue(H5D.get_num_chunks(-1, H5S.ALL, ref chunks) < 0);
        }

        [TestMethod]
        public void ErrorDetectionSettingRoundTrips()
        {
            long properties = H5P.create(H5P.DATASET_XFER);
            Assert.IsTrue(properties >= 0);
            try
            {
                Assert.AreEqual(H5Z.EDC_t.ENABLE, H5P.get_edc_check(properties));
                Assert.IsTrue(H5P.set_edc_check(properties, H5Z.EDC_t.DISABLE) >= 0);
                Assert.AreEqual(H5Z.EDC_t.DISABLE, H5P.get_edc_check(properties));
            }
            finally { H5P.close(properties); }
        }

        [TestMethod]
        public void IdentifierTypeReferenceCountRoundTrips()
        {
            var type = H5I.register_type((IntPtr)64, 0, null);
            Assert.IsTrue((int)type >= 0);
            try
            {
                Assert.AreEqual(1, H5I.H5Iget_type_ref(type));
                Assert.AreEqual(2, H5I.inc_type_ref(type));
                Assert.AreEqual(2, H5I.H5Iget_type_ref(type));
            }
            finally { H5I.destroy_type(type); }
        }

        [TestMethod]
        public void ConversionDataIsReturnedAsNativeOwnedPointer()
        {
            var converter = H5T.find(H5T.NATIVE_INT, H5T.NATIVE_DOUBLE, out IntPtr pointer);
            Assert.IsNotNull(converter);
            Assert.AreNotEqual(IntPtr.Zero, pointer);
            var data = Marshal.PtrToStructure<H5T.cdata_t>(pointer);
            Assert.AreEqual(0, (int)data.command);
            Assert.AreEqual(0, (int)data.need_bkg);
            GC.KeepAlive(converter);
        }

        [TestMethod]
        public void PageBufferCountersFillBothElementsAndRejectShortArrays()
        {
            string path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".h5");
            long creation = H5P.create(H5P.FILE_CREATE), access = H5P.create(H5P.FILE_ACCESS), file = -1;
            try
            {
                Assert.IsTrue(H5P.set_file_space_strategy(creation, H5F.fspace_strategy_t.PAGE, 0, 1) >= 0);
                Assert.IsTrue(H5P.set_page_buffer_size(access, (IntPtr)16384, 0, 0) >= 0);
                file = H5F.create(path, H5F.ACC_TRUNC, creation, access);
                Assert.IsTrue(file >= 0);
                uint[][] counts = new uint[5][];
                for (int i = 0; i < counts.Length; i++)
                    counts[i] = new[] { uint.MaxValue, uint.MaxValue };
                Assert.IsTrue(H5F.get_page_buffering_stats(file, counts[0], counts[1], counts[2], counts[3], counts[4]) >= 0);
                foreach (var pair in counts)
                    foreach (uint count in pair)
                        Assert.AreNotEqual(uint.MaxValue, count);
                for (int i = 0; i < counts.Length; i++)
                {
                    var saved = counts[i];
                    counts[i] = new uint[1];
                    Assert.ThrowsExactly<ArgumentException>(() => H5F.get_page_buffering_stats(file, counts[0], counts[1], counts[2], counts[3], counts[4]));
                    counts[i] = null;
                    Assert.ThrowsExactly<ArgumentNullException>(() => H5F.get_page_buffering_stats(file, counts[0], counts[1], counts[2], counts[3], counts[4]));
                    counts[i] = saved;
                }
            }
            finally
            {
                if (file >= 0)
                    H5F.close(file);
                H5P.close(access);
                H5P.close(creation);
                File.Delete(path);
            }
        }

        [TestMethod]
        public void AppendCallbackReadsAllDimensions()
        {
            ulong[] observed = new ulong[2];
            H5D.append_cb_t callback = (id, dimensions, state) =>
            {
                observed[0] = ((ulong*)dimensions)[0];
                observed[1] = ((ulong*)dimensions)[1];
                return 0;
            };
            ulong* dimensions = stackalloc ulong[] { 4, 7 };
            // Call the unmanaged thunk, so the test exercises reverse P/Invoke marshalling.
            var invoke = (delegate* unmanaged[Cdecl]<long, ulong*, IntPtr, int>)Marshal.GetFunctionPointerForDelegate(callback);
            Assert.AreEqual(0, invoke(0, dimensions, IntPtr.Zero));
            CollectionAssert.AreEqual(new ulong[] { 4, 7 }, observed);
            GC.KeepAlive(callback);
        }

        [TestMethod]
        public void FilterCallbackReadsAllClientData()
        {
            uint[] observed = new uint[3];
            H5Z.func_t callback = (uint flags, IntPtr count, IntPtr values, IntPtr bytes, ref IntPtr capacity, ref IntPtr buffer) =>
            {
                for (int i = 0; i < (int)count; i++)
                    observed[i] = ((uint*)values)[i];
                return bytes;
            };
            uint* values = stackalloc uint[] { 10, 20, 30 };
            var invoke = (delegate* unmanaged[Cdecl]<uint, IntPtr, uint*, IntPtr, IntPtr*, IntPtr*, IntPtr>)Marshal.GetFunctionPointerForDelegate(callback);
            IntPtr capacity = 4, buffer = IntPtr.Zero;
            Assert.AreEqual((IntPtr)4, invoke(0, 3, values, 4, &capacity, &buffer));
            CollectionAssert.AreEqual(new uint[] { 10, 20, 30 }, observed);
            GC.KeepAlive(callback);
        }

        [TestMethod]
        public void ExternalLinkCallbackDecodesCompleteUtf8Names()
        {
            string[] observed = null;
            H5L.elink_traverse_t callback = (string parent, string group, string child, string name, ref uint flags, long properties, IntPtr state) =>
            {
                observed = new[] { parent, group, child, name };
                flags = 23;
                return 0;
            };
            IntPtr text = Marshal.StringToCoTaskMemUTF8("/grüppe/数据");
            try
            {
                var invoke = (delegate* unmanaged[Cdecl]<IntPtr, IntPtr, IntPtr, IntPtr, uint*, long, IntPtr, int>)Marshal.GetFunctionPointerForDelegate(callback);
                uint flags = 0;
                Assert.AreEqual(0, invoke(text, text, text, text, &flags, 0, IntPtr.Zero));
                CollectionAssert.AreEqual(new[] { "/grüppe/数据", "/grüppe/数据", "/grüppe/数据", "/grüppe/数据" }, observed);
                Assert.AreEqual(23u, flags);
            }
            finally { Marshal.FreeCoTaskMem(text); GC.KeepAlive(callback); }
        }

        [TestMethod]
        public void FreeCallbackHasVoidReturnAndPreservesArguments()
        {
            IntPtr observedMemory = IntPtr.Zero, observedContext = IntPtr.Zero;
            H5MM.free_t callback = (memory, context) => { observedMemory = memory; observedContext = context; };
            var invoke = (delegate* unmanaged[Cdecl]<IntPtr, IntPtr, void>)Marshal.GetFunctionPointerForDelegate(callback);
            invoke((IntPtr)123, (IntPtr)456);
            Assert.AreEqual(typeof(void), typeof(H5MM.free_t).GetMethod("Invoke").ReturnType);
            Assert.AreEqual((IntPtr)123, observedMemory);
            Assert.AreEqual((IntPtr)456, observedContext);
            GC.KeepAlive(callback);
        }
    }
}

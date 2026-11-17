#if NET8_0_OR_GREATER && !HDF_LEGACY
using System;
using System.IO;
using HDF.PInvoke;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    /// <summary>Checks that span boundaries are enforced before native code can access backing memory.</summary>
    [TestClass]
    public class SpanBoundaryTests
    {
        /// <summary>Rejects short coordinate spans even when a larger backing array hides the native overrun.</summary>
        [TestMethod]
        public void DimensionOutputsRespectSpanLength()
        {
            var space = H5S.create_simple(new ulong[] { 2, 3 }.AsSpan());
            try
            {
                var dimensions = new ulong[] { 123, 456 };
                Assert.ThrowsExactly<ArgumentException>(() => H5S.get_simple_extent_dims(space, dimensions.AsSpan(0, 1)));
                Assert.AreEqual(123ul, dimensions[0]);
                Assert.AreEqual(456ul, dimensions[1]);
            }
            finally
            {
                H5S.close(space);
            }
        }

        /// <summary>Rejects a sliced destination that is smaller than the selected dataset data.</summary>
        [TestMethod]
        public void DatasetReadRespectsSpanLength()
        {
            var path = Path.Combine(Path.GetTempPath(), "span-boundary-" + Guid.NewGuid() + ".h5");
            var file = H5F.create(path, H5F.ACC_TRUNC);
            var space = H5S.create_simple(new ulong[] { 2 }.AsSpan());
            var dataset = H5D.create(file, "data", H5T.NATIVE_INT, space);
            try
            {
                var values = new int[] { 11, 22 };
                Assert.IsTrue(H5D.write<int>(dataset, H5T.NATIVE_INT, H5S.ALL, H5S.ALL, H5P.DEFAULT, values.AsSpan()) >= 0);
                var output = new int[] { 123, 456 };
                Assert.ThrowsExactly<ArgumentException>(() => H5D.read(dataset, H5T.NATIVE_INT, H5S.ALL, H5S.ALL, H5P.DEFAULT, output.AsSpan(0, 1)));
                Assert.AreEqual(123, output[0]);
                Assert.AreEqual(456, output[1]);
            }
            finally
            {
                H5D.close(dataset);
                H5S.close(space);
                H5F.close(file);
                File.Delete(path);
            }
        }
        /// <summary>Validates byte widths for scalar and packed generic overloads before native memory access.</summary>
        [TestMethod]
        public void GenericBuffersRespectNativeDatatypeSize()
        {
            var list = H5P.create(H5P.DATASET_CREATE);
            try
            {
                Assert.ThrowsExactly<ArgumentException>(() => H5P.get_fill_value<byte>(list, H5T.NATIVE_DOUBLE, out _));
                Assert.ThrowsExactly<ArgumentException>(() => H5P.set_fill_value(list, H5T.NATIVE_DOUBLE, (byte)1));
                Assert.ThrowsExactly<ArgumentException>(() => H5LT.make_dataset<byte>(-1, "unused", new ulong[] { 1 }, H5T.NATIVE_DOUBLE, new byte[1]));
            }
            finally
            {
                H5P.close(list);
            }
        }

        /// <summary>Checks the highest selected address rather than just the number of selected elements.</summary>
        [TestMethod]
        public void SparseSelectionsRequireReachableBufferAddresses()
        {
            var space = H5S.create_simple(new ulong[] { 8 }.AsSpan());
            try
            {
                Assert.IsTrue(H5S.select_hyperslab(space, H5S.seloper_t.SET, new ulong[] { 6 }.AsSpan(), default, new ulong[] { 1 }.AsSpan()) >= 0);
                var values = new int[8];
                Assert.ThrowsExactly<ArgumentException>(() => H5D.fill(7, H5T.NATIVE_INT, values.AsSpan(0, 1), H5T.NATIVE_INT, space));
                Assert.IsTrue(H5D.fill(7, H5T.NATIVE_INT, values.AsSpan(0, 7), H5T.NATIVE_INT, space) >= 0);
                Assert.AreEqual(7, values[6]);
                Assert.AreEqual(0, values[7]);
                Assert.IsTrue(H5S.select_none(space) >= 0);
                Assert.AreEqual(0, H5D.iterate<int>(values.AsSpan(0, 1), H5T.NATIVE_INT, space, static (ref int value, long type, ReadOnlySpan<ulong> point) => 0));
            }
            finally
            {
                H5S.close(space);
            }
        }

        /// <summary>Rejects short point coordinates and optional coordinate arrays when supplied.</summary>
        [TestMethod]
        public void CoordinateInputsRespectNativeRank()
        {
            var space = H5S.create_simple(new ulong[] { 2, 3 }.AsSpan());
            try
            {
                Assert.ThrowsExactly<ArgumentException>(() => H5S.select_hyperslab(space, H5S.seloper_t.SET, new ulong[] { 0 }.AsSpan(), default, new ulong[] { 1, 1 }.AsSpan()));
                Assert.ThrowsExactly<ArgumentException>(() => H5S.select_elements(space, H5S.seloper_t.SET, (IntPtr)2, new ulong[3].AsSpan()));
                Assert.ThrowsExactly<ArgumentException>(() => H5P.set_dataset_io_hyperslab_selection(-1, 2, H5S.seloper_t.SET, new ulong[1], default, new ulong[2]));
                Assert.AreEqual(2, H5S.get_simple_extent_dims(space, Span<ulong>.Empty));
            }
            finally
            {
                H5S.close(space);
            }
        }
    }
}
#endif


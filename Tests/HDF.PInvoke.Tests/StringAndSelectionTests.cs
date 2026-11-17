using System;
using System.IO;
using HDF.PInvoke;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    /// <summary>Exercises variable-length string ownership and special dataspace selections.</summary>
    [TestClass]
    public class StringAndSelectionTests
    {
        /// <summary>Checks complete transfers, invalid strings and the capacity required by each selection mode.</summary>
        [TestMethod]
        public unsafe void VariableLengthStringsRoundTripAndValidateInputs()
        {
            var path = Path.GetTempFileName();
            var file = H5F.create(path, H5F.ACC_TRUNC);
            var space = H5S.create_simple(1, new ulong[] { 4 }, null);
            var type = H5T.copy(H5T.C_S1);
            long dataset = -1, attribute = -1;
            try
            {
                Assert.IsTrue(H5T.set_size(type, H5T.VARIABLE) >= 0);
                Assert.IsTrue(H5T.set_cset(type, H5T.cset_t.UTF8) >= 0);
                dataset = H5D.create(file, "strings", type, space);
                attribute = H5A.create(dataset, "labels", type, space);
                var expected = new[] { "α🌍", "", null, "Größe" };
                Assert.IsTrue(H5D.write_utf8_strings(dataset, expected) >= 0);
                Assert.IsTrue(H5A.write_utf8_strings(attribute, expected) >= 0);
                Assert.IsTrue(H5D.read_utf8_strings(dataset, out var data) >= 0);
                Assert.IsTrue(H5A.read_utf8_strings(attribute, out var labels) >= 0);
                CollectionAssert.AreEqual(expected, data);
                CollectionAssert.AreEqual(expected, labels);
                Assert.ThrowsExactly<ArgumentException>(() => H5A.write_utf8_strings(attribute, new string[1]));
                Assert.ThrowsExactly<ArgumentException>(() => H5D.write_utf8_strings(dataset, new[] { "ok", "a\0b", "", "" }));
                // Store invalid UTF-8 through the raw API to exercise cleanup during decoding.
                byte* invalid = stackalloc byte[] { 0xff, 0 };
                IntPtr* slots = stackalloc IntPtr[] { (IntPtr)invalid, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero };
                Assert.IsTrue(H5D.write(dataset, type, H5S.ALL, H5S.ALL, H5P.DEFAULT, (IntPtr)slots) >= 0);
                Assert.IsTrue(H5A.write(attribute, type, (IntPtr)slots) >= 0);
                Assert.ThrowsExactly<System.Text.DecoderFallbackException>(() => H5D.read_utf8_strings(dataset, out _));
                Assert.ThrowsExactly<System.Text.DecoderFallbackException>(() => H5A.read_utf8_strings(attribute, out _));

#if NET8_0_OR_GREATER && !HDF_LEGACY
                var numbers = H5D.create(file, "numbers", H5T.NATIVE_INT, space);
                var transfer = H5P.create(H5P.DATASET_XFER);
                try
                {
                    ulong[] start = { 3 }, count = { 1 };
                    Assert.IsTrue(H5S.select_hyperslab(space, H5S.seloper_t.SET, start.AsSpan(), default, count.AsSpan()) >= 0);
                    Assert.IsTrue(H5D.write<int>(numbers, H5T.NATIVE_INT, H5S.BLOCK, space, H5P.DEFAULT, new int[] { 17 }) >= 0);
                    Assert.IsTrue(H5P.set_dataset_io_hyperslab_selection(transfer, 1, H5S.seloper_t.SET, start, default, count) >= 0);
                    var buffer = new int[4];
                    Assert.ThrowsExactly<ArgumentException>(() => H5D.read(numbers, H5T.NATIVE_INT, H5S.BLOCK, space, H5P.DEFAULT, Span<int>.Empty));
                    Assert.IsTrue(H5D.read(numbers, H5T.NATIVE_INT, H5S.PLIST, space, H5P.DEFAULT, buffer.AsSpan()) < 0);
                    Assert.IsTrue(H5D.read(numbers, H5T.NATIVE_INT, space, H5S.BLOCK, H5P.DEFAULT, buffer.AsSpan()) < 0);
                    Assert.ThrowsExactly<ArgumentException>(() => H5D.read(numbers, H5T.NATIVE_INT, H5S.ALL, H5S.PLIST, transfer, buffer.AsSpan(0, 1)));
                    Assert.IsTrue(H5D.read(numbers, H5T.NATIVE_INT, H5S.ALL, H5S.PLIST, transfer, buffer.AsSpan()) >= 0);
                    Assert.AreEqual(17, buffer[3]);
                    Assert.IsTrue(H5D.read(numbers, H5T.NATIVE_INT, H5S.BLOCK, space, H5P.DEFAULT, buffer.AsSpan(0, 1)) >= 0);
                    Assert.AreEqual(17, buffer[0]);
                }
                finally
                {
                    H5P.close(transfer);
                    H5D.close(numbers);
                }
#endif
            }
            finally
            {
                if (attribute >= 0) H5A.close(attribute);
                if (dataset >= 0) H5D.close(dataset);
                H5T.close(type);
                H5S.close(space);
                H5F.close(file);
                File.Delete(path);
            }
        }
    }
}

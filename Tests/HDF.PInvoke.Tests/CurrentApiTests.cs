using System;
using System.IO;
using System.Runtime.InteropServices;
using HDF.PInvoke;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    /// <summary>Exercises current serial HDF5 I/O and property APIs on both binding implementations.</summary>
    [TestClass]
    public unsafe class CurrentApiTests
    {
        /// <summary>Round-trips parallel dataset buffers and verifies bounded raw chunk reads.</summary>
        [TestMethod]
        public void MultiDatasetAndBoundedChunkRoundTrip()
        {
            var path = Path.Combine(Path.GetTempPath(), "hdf-多-" + Guid.NewGuid() + ".h5");
            var file = H5F.create(path, H5F.ACC_TRUNC);
            var space = H5S.create_simple(1, new ulong[] { 4 }, null);
            var creation = H5P.create(H5P.DATASET_CREATE);
            var transfer = H5P.create(H5P.DATASET_XFER);
            long first = -1, second = -1;

            try
            {
                Assert.IsTrue(file >= 0 && space >= 0);
                Assert.IsTrue(H5P.set_chunk(creation, 1, new ulong[] { 4 }) >= 0);
                first = H5D.create(file, "a", H5T.NATIVE_INT, space, H5P.DEFAULT, creation);
                second = H5D.create(file, "b", H5T.NATIVE_INT, space, H5P.DEFAULT, creation);
                Assert.IsTrue(first >= 0 && second >= 0);
                var ids = new[] { first, second };
                var types = new[] { H5T.NATIVE_INT, H5T.NATIVE_INT };
                var spaces = new long[] { H5S.ALL, H5S.ALL };
                var a = new[] { 1, 2, 3, 4 };
                var b = new[] { 5, 6, 7, 8 };
                var x = new int[4];
                var y = new int[4];
                fixed (int* pa = a, pb = b, px = x, py = y)
                {
                    Assert.IsTrue(H5D.write_multi(ids, types, spaces, spaces, transfer, new[] { (IntPtr)pa, (IntPtr)pb }) >= 0);
                    Assert.IsTrue(H5D.read_multi(ids, types, spaces, spaces, transfer, new[] { (IntPtr)px, (IntPtr)py }) >= 0);
                    CollectionAssert.AreEqual(a, x);
                    CollectionAssert.AreEqual(b, y);
                    ulong offset = 0;
                    var size = UIntPtr.Zero;
                    Assert.IsTrue(H5D.read_chunk2(first, H5P.DEFAULT, (IntPtr)(&offset), out var filters, IntPtr.Zero, ref size) >= 0);
                    Assert.AreEqual(16UL, size.ToUInt64());
                    Assert.AreEqual(0u, filters);
                    // An undersized buffer must remain untouched while the required capacity is reported.
                    x[0] = -123;
                    size = new UIntPtr(1u);
                    Assert.IsTrue(H5D.read_chunk2(first, H5P.DEFAULT, (IntPtr)(&offset), out filters, (IntPtr)px, ref size) >= 0);
                    Assert.AreEqual(-123, x[0]);
                    Assert.AreEqual(16UL, size.ToUInt64());
                    Assert.IsTrue(H5D.read_chunk2(first, H5P.DEFAULT, (IntPtr)(&offset), out filters, (IntPtr)px, ref size) >= 0);
                    CollectionAssert.AreEqual(a, x);
                }
                Assert.IsTrue(H5P.get_actual_selection_io_mode(transfer, out _) >= 0);
                var chunks = 0;
                H5D.chunk_iter_t chunkCallback = (offset, filters, address, size, state) =>
                {
                    if (Marshal.ReadInt64(offset) != 0 || size != 16 || state != new IntPtr(42))
                    {
                        return -1;
                    }

                    chunks++;
                    return 0;
                };
                Assert.IsTrue(H5D.chunk_iter(first, H5P.DEFAULT, chunkCallback, new IntPtr(42)) >= 0);
                Assert.AreEqual(1, chunks);
                Assert.IsTrue(H5D.debug(first) >= 0);
                GC.KeepAlive(chunkCallback);
                Assert.IsTrue(H5P.get_no_selection_io_cause(transfer, out _) >= 0);
                Assert.ThrowsExactly<ArgumentException>(() => H5D.read_multi(ids, new long[1], spaces, spaces, transfer, new IntPtr[2]));
                Assert.ThrowsExactly<ArgumentNullException>(() => H5D.write_multi(null, types, spaces, spaces, transfer, new IntPtr[2]));
                Assert.IsTrue(H5F.set_libver_bounds(file, H5F.libver_t.V110, H5F.libver_t.LATEST) >= 0);
                Assert.IsTrue(H5F.get_eoa(file, out var before) >= 0);
                Assert.IsTrue(H5F.increment_filesize(file, 1024) >= 0);
                Assert.IsTrue(H5F.get_eoa(file, out var after) >= 0);
                Assert.IsTrue(after >= before + 1024);
            }
            finally
            {
                if (second >= 0)
                {
                    H5D.close(second);
                }

                if (first >= 0)
                {
                    H5D.close(first);
                }

                H5P.close(transfer);
                H5P.close(creation);
                H5S.close(space);
                H5F.close(file);
            }

            try
            {
                Assert.IsTrue(H5F.is_accessible(path) > 0);
                Assert.IsTrue(H5F.delete(path) >= 0);
                Assert.IsFalse(File.Exists(path));
            }
            finally
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }

        /// <summary>Checks byte-sized boolean output and selection-I/O enum round-trips.</summary>
        [TestMethod]
        public void CurrentPropertiesRoundTrip()
        {
            var access = H5P.create(H5P.FILE_ACCESS);
            var transfer = H5P.create(H5P.DATASET_XFER);

            try
            {
                Assert.IsTrue(H5P.set_file_locking(access, 0, 1) >= 0);
                Assert.IsTrue(H5P.get_file_locking(access, out var locking, out var ignore) >= 0);
                Assert.AreEqual((byte)0, locking);
                Assert.AreEqual((byte)1, ignore);
                Assert.IsTrue(H5P.set_evict_on_close(access, 1) >= 0);
                Assert.IsTrue(H5P.get_evict_on_close(access, out var evict) >= 0);
                Assert.AreEqual((byte)1, evict);
                Assert.IsTrue(H5P.set_selection_io(transfer, H5D.selection_io_mode_t.ON) >= 0);
                Assert.IsTrue(H5P.get_selection_io(transfer, out var mode) >= 0);
                Assert.AreEqual(H5D.selection_io_mode_t.ON, mode);
                Assert.IsTrue(H5P.set_modify_write_buf(transfer, 1) >= 0);
                Assert.IsTrue(H5P.get_modify_write_buf(transfer, out var modify) >= 0);
                Assert.AreEqual((byte)1, modify);
            }
            finally
            {
                H5P.close(transfer);
                H5P.close(access);
            }
        }

        /// <summary>Checks complex datatype encoding and bounds-aware decoding.</summary>
        [TestMethod]
        public void ComplexDatatypeEncodingRoundTrip()
        {
            var type = H5T.complex_create(H5T.IEEE_F64LE);
            Assert.IsTrue(type >= 0);
            long decoded = -1;
            var space = H5S.create(H5S.class_t.SCALAR);

            try
            {
                Assert.AreEqual(H5T.class_t.COMPLEX, H5T.get_class(type));
                Assert.AreEqual(16L, H5T.get_size(type).ToInt64());
                var size = IntPtr.Zero;
                Assert.IsTrue(H5T.encode(type, null, ref size) >= 0);
                var bytes = new byte[size.ToInt32()];
                Assert.IsTrue(H5T.encode(type, bytes, ref size) >= 0);
                fixed (byte* pointer = bytes)
                {
                    decoded = H5T.decode2((IntPtr)pointer, new UIntPtr((uint)bytes.Length));
                    Assert.IsTrue(decoded >= 0);
                    Assert.IsTrue(H5T.equal(type, decoded) > 0);
                    Assert.IsTrue(H5T.decode2((IntPtr)pointer, new UIntPtr(1u)) < 0);
                }
                double* value = stackalloc double[2];
                Assert.IsTrue(H5T.reclaim(type, space, H5P.DEFAULT, (IntPtr)value) >= 0);
            }
            finally
            {
                if (decoded >= 0)
                {
                    H5T.close(decoded);
                }

                H5S.close(space);
                H5T.close(type);
            }
        }
    }
}


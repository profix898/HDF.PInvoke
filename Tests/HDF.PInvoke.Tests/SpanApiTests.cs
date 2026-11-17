#if NET8_0_OR_GREATER && !HDF_LEGACY
using System;
using System.IO;
using System.Text;
using HDF.PInvoke;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    /// <summary>
    /// Validates modern Span/ReadOnlySpan overloads and zero-allocation hot paths on .NET 8+.
    /// </summary>
    [TestClass]
    public unsafe class SpanApiTests
    {
        /// <summary>
        /// Verifies dataset, dataspace, chunk, hyperslab, and zero-allocation iteration/string-builder hot paths.
        /// </summary>
        [TestMethod]
        public void DatasetHyperslabIterateAndZeroAllocationHotPaths()
        {
            var path = Path.Combine(Path.GetTempPath(), "span-dset-" + Guid.NewGuid() + ".h5");
            long file = -1, space = -1, memSpace = -1, dcpl = -1, dset = -1;

            try
            {
                file = H5F.create(path, H5F.ACC_TRUNC);
                space = H5S.create_simple([2, 3], [H5S.UNLIMITED, 3]);
                dcpl = H5P.create(H5P.DATASET_CREATE);
                Assert.IsTrue(file >= 0 && space >= 0 && dcpl >= 0);

                Assert.IsTrue(H5P.set_chunk(dcpl, [2, 3]) >= 0);
                Span<ulong> chunkDims = stackalloc ulong[2];
                Assert.AreEqual(2, H5P.get_chunk(dcpl, chunkDims));
                Assert.AreEqual(2ul, chunkDims[0]);
                Assert.AreEqual(3ul, chunkDims[1]);

                Assert.IsTrue(H5P.set_fill_value(dcpl, H5T.NATIVE_INT, -7) >= 0);
                Assert.IsTrue(H5P.get_fill_value<int>(dcpl, H5T.NATIVE_INT, out var fillRead) >= 0);
                Assert.AreEqual(-7, fillRead);

                dset = H5D.create(file, "matrix_数据", H5T.NATIVE_INT, space, H5P.DEFAULT, dcpl);
                Assert.IsTrue(dset >= 0);

                Span<int> elements = stackalloc int[6];
                Assert.IsTrue(H5D.fill(10, H5T.NATIVE_INT, elements, H5T.NATIVE_INT, space) >= 0);

                // Warm up JIT before measuring thread allocations.
                H5D.span_operator_t<int> transform = static (ref int value, long typeId, ReadOnlySpan<ulong> coords) =>
                {
                    value += (int)(coords[0] * 100 + coords[1]);
                    return 0;
                };
                Assert.AreEqual(0, H5D.iterate(elements, H5T.NATIVE_INT, space, transform));

                var sb = new StringBuilder(64);
                _ = H5I.get_name(dset, sb, (IntPtr)64);

                var beforeBytes = GC.GetAllocatedBytesForCurrentThread();

                for (var iter = 0; iter < 20; iter++)
                {
                    Assert.AreEqual(0, H5D.iterate(elements, H5T.NATIVE_INT, space, transform));
                    Assert.IsTrue(H5I.get_name(dset, sb, (IntPtr)64).ToInt64() > 0);
                }

                var allocated = GC.GetAllocatedBytesForCurrentThread() - beforeBytes;
                Assert.AreEqual(0L, allocated, $"Expected zero managed heap allocations across 20 dataset iterations and StringBuilder queries, observed {allocated} bytes.");
                Assert.AreEqual("/matrix_数据", sb.ToString());

                Assert.IsTrue(H5D.write<int>(dset, H5T.NATIVE_INT, H5S.ALL, H5S.ALL, H5P.DEFAULT, [1, 2, 3, 4, 5, 6]) >= 0);
                Assert.IsTrue(H5DS.set_label(dset, 0, "rows_行") >= 0);
                var labelLen = (int)H5DS.get_label(dset, 0, Span<byte>.Empty);
                Span<byte> labelUtf8 = stackalloc byte[labelLen + 1];
                Assert.AreEqual((IntPtr)labelLen, H5DS.get_label(dset, 0, labelUtf8));
                Assert.AreEqual("rows_行", Encoding.UTF8.GetString(labelUtf8.Slice(0, labelLen)));

                Assert.IsTrue(H5D.set_extent(dset, [3, 3]) >= 0);
                Assert.IsTrue(H5DO.append<int>(dset, H5P.DEFAULT, 0, (IntPtr)1, H5T.NATIVE_INT, [7, 8, 9]) >= 0);

                Span<ulong> ldDims = stackalloc ulong[2];
                Assert.IsTrue(H5LD.get_dset_dims(dset, ldDims) >= 0);
                Assert.AreEqual(4ul, ldDims[0]);
                Span<int> appendedRow = stackalloc int[3];
                Assert.IsTrue(H5LD.get_dset_elmts(dset, [3, 3], [4, 3], null, appendedRow) >= 0);
                Assert.AreEqual(7, appendedRow[0]);
                Assert.AreEqual(9, appendedRow[2]);

                Assert.IsTrue(H5S.close(space) >= 0);
                space = H5D.get_space(dset);
                Span<ulong> dims = stackalloc ulong[2];
                Span<ulong> maxDims = stackalloc ulong[2];
                Assert.AreEqual(2, H5S.get_simple_extent_dims(space, dims, maxDims));
                Assert.AreEqual(4ul, dims[0]);
                Assert.AreEqual(3ul, dims[1]);
                Assert.AreEqual(H5S.UNLIMITED, maxDims[0]);

                // Read fill-initialized row [2] and H5DO.append row [3] via hyperslab into stackalloc span.
                memSpace = H5S.create_simple([3]);
                Span<int> row = stackalloc int[3];
                Assert.IsTrue(H5S.select_hyperslab(space, H5S.seloper_t.SET, [2, 0], default, [1, 3]) >= 0);
                Assert.IsTrue(H5D.read(dset, H5T.NATIVE_INT, memSpace, space, H5P.DEFAULT, row) >= 0);
                Assert.AreEqual(-7, row[0]);
                Assert.AreEqual(-7, row[1]);
                Assert.AreEqual(-7, row[2]);

                Assert.IsTrue(H5S.select_hyperslab(space, H5S.seloper_t.SET, [3, 0], default, [1, 3]) >= 0);
                Assert.IsTrue(H5D.read(dset, H5T.NATIVE_INT, memSpace, space, H5P.DEFAULT, row) >= 0);
                Assert.AreEqual(7, row[0]);
                Assert.AreEqual(8, row[1]);
                Assert.AreEqual(9, row[2]);

                // Direct chunk inspection via Span overloads.
                uint filterMask = 0;
                ulong chunkAddr = 0, chunkSize = 0;
                Span<ulong> chunkOffset = stackalloc ulong[2];
                Assert.IsTrue(H5D.get_chunk_info(dset, H5S.ALL, 0, chunkOffset, ref filterMask, ref chunkAddr, ref chunkSize) >= 0);
                ulong queriedBytes = 0;
                Assert.IsTrue(H5D.get_chunk_storage_size(dset, [0, 0], ref queriedBytes) >= 0);
                Assert.AreEqual(chunkSize, queriedBytes);

                Span<byte> rawChunk = stackalloc byte[(int)queriedBytes];
                Assert.IsTrue(H5D.read_chunk2(dset, H5P.DEFAULT, [0, 0], out var chunkFilters, rawChunk, out var actualChunkSize) >= 0);
                Assert.AreEqual(queriedBytes, actualChunkSize.ToUInt64());
                Assert.AreEqual(0u, chunkFilters);
            }
            finally
            {
                if (memSpace >= 0)
                {
                    H5S.close(memSpace);
                }

                if (dset >= 0)
                {
                    H5D.close(dset);
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

                File.Delete(path);
            }
        }

        /// <summary>
        /// Verifies attribute, datatype, reference, property-encoding, and high-level Span overloads.
        /// </summary>
        [TestMethod]
        public void MetadataHighLevelAndInMemoryImageSpanOverloads()
        {
            var path = Path.Combine(Path.GetTempPath(), "span-meta-" + Guid.NewGuid() + ".h5");
            long file = -1, attrSpace = -1, attr = -1, arrayType = -1, enumType = -1, fapl = -1, decodedFapl = -1, table = -1, memFile = -1;
            H5R.ref_t objRef = default;
            var refCreated = false;

            try
            {
                file = H5F.create(path, H5F.ACC_TRUNC);
                Assert.IsTrue(file >= 0);

                // Attribute Span read/write & UTF-8 Span<byte> name query.
                attrSpace = H5S.create_simple([3]);
                attr = H5A.create(file, "weights_属性", H5T.NATIVE_DOUBLE, attrSpace);
                Assert.IsTrue(attr >= 0);
                Assert.IsTrue(H5A.write<double>(attr, H5T.NATIVE_DOUBLE, [1.25, 2.5, 3.75]) >= 0);
                Span<double> weights = stackalloc double[3];
                Assert.IsTrue(H5A.read(attr, H5T.NATIVE_DOUBLE, weights) >= 0);
                Assert.AreEqual(2.5, weights[1]);

                var attrNameLen = (int)H5A.get_name(attr, Span<byte>.Empty);
                Assert.IsTrue(attrNameLen > 0);
                Span<byte> attrNameUtf8 = stackalloc byte[attrNameLen + 1];
                Assert.AreEqual((IntPtr)attrNameLen, H5A.get_name(attr, attrNameUtf8));
                Assert.AreEqual("weights_属性", Encoding.UTF8.GetString(attrNameUtf8.Slice(0, attrNameLen)));

                // Dataspace encode/encode2/decode via Span<byte>.
                Assert.IsTrue(H5S.encode(attrSpace, Span<byte>.Empty, out var encodedSpaceSize) >= 0);
                Span<byte> encodedSpace = stackalloc byte[(int)encodedSpaceSize];
                Assert.IsTrue(H5S.encode(attrSpace, encodedSpace, out _) >= 0);
                var decodedSpace = H5S.decode(encodedSpace);
                Assert.IsTrue(decodedSpace >= 0);
                Assert.IsTrue(H5S.extent_equal(attrSpace, decodedSpace) > 0);
                H5S.close(decodedSpace);

                fapl = H5P.create(H5P.FILE_ACCESS);
                Assert.IsTrue(H5S.encode2(attrSpace, Span<byte>.Empty, out var encodedSpace2Size, fapl) >= 0);
                Span<byte> encodedSpace2 = stackalloc byte[(int)encodedSpace2Size.ToUInt64()];
                Assert.IsTrue(H5S.encode2(attrSpace, encodedSpace2, out _, fapl) >= 0);
                var decodedSpace2 = H5S.decode(encodedSpace2);
                Assert.IsTrue(decodedSpace2 >= 0);
                Assert.IsTrue(H5S.extent_equal(attrSpace, decodedSpace2) > 0);
                H5S.close(decodedSpace2);

                // Property list encode2/decode via Span<byte>.
                Assert.IsTrue(H5P.encode2(fapl, Span<byte>.Empty, out var encodedPlistSize) >= 0);
                Span<byte> encodedPlist = stackalloc byte[(int)encodedPlistSize.ToUInt64()];
                Assert.IsTrue(H5P.encode2(fapl, encodedPlist, out _) >= 0);
                decodedFapl = H5P.decode(encodedPlist);
                Assert.IsTrue(decodedFapl >= 0);
                Assert.IsTrue(H5P.equal(fapl, decodedFapl) > 0);

                // Array, Enum, and serialized datatype Span overloads.
                arrayType = H5T.array_create(H5T.NATIVE_INT, [2, 4]);
                Assert.IsTrue(arrayType >= 0);
                Span<ulong> arrayDims = stackalloc ulong[2];
                Assert.AreEqual(2, H5T.get_array_dims(arrayType, arrayDims));
                Assert.AreEqual(2ul, arrayDims[0]);
                Assert.AreEqual(4ul, arrayDims[1]);

                Assert.IsTrue(H5T.encode(arrayType, Span<byte>.Empty, out var encodedTypeSize) >= 0);
                Span<byte> encodedType = stackalloc byte[(int)encodedTypeSize];
                Assert.IsTrue(H5T.encode(arrayType, encodedType, out _) >= 0);
                var decodedType = H5T.decode2(encodedType);
                Assert.IsTrue(decodedType >= 0);
                Assert.IsTrue(H5T.equal(arrayType, decodedType) > 0);
                H5T.close(decodedType);

                enumType = H5T.enum_create(H5T.NATIVE_INT);
                Assert.IsTrue(H5T.enum_insert(enumType, "READY", 42) >= 0);
                Assert.IsTrue(H5T.enum_valueof<int>(enumType, "READY", out var enumVal) >= 0);
                Assert.AreEqual(42, enumVal);
                Span<byte> enumName = stackalloc byte[16];
                Assert.IsTrue(H5T.enum_nameof(enumType, 42, enumName) >= 0);
                Assert.AreEqual("READY", Encoding.UTF8.GetString(enumName.Slice(0, enumName.IndexOf((byte)0))));

                // High-level H5LT, H5IM, and H5PT Span overloads.
                Assert.IsTrue(H5LT.make_dataset<float>(file, "lt_vec", [4], H5T.NATIVE_FLOAT, [10f, 20f, 30f, 40f]) >= 0);
                Span<ulong> ltDims = stackalloc ulong[1];
                Assert.IsTrue(H5LT.get_dataset_info(file, "lt_vec", ltDims, out var ltClass, out _) >= 0);
                Assert.AreEqual(4ul, ltDims[0]);
                Assert.AreEqual(H5T.class_t.FLOAT, ltClass);
                Span<float> ltValues = stackalloc float[4];
                Assert.IsTrue(H5LT.read_dataset(file, "lt_vec", H5T.NATIVE_FLOAT, ltValues) >= 0);
                Assert.AreEqual(30f, ltValues[2]);

                Assert.IsTrue(H5IM.make_image_8bit(file, "img8", 2, 2, [1, 2, 3, 4]) >= 0);
                Assert.IsTrue(H5IM.make_palette(file, "pal8", [2, 3], [10, 20, 30, 40, 50, 60]) >= 0);
                Assert.IsTrue(H5IM.link_palette(file, "img8", "pal8") >= 0);
                Span<ulong> palDims = stackalloc ulong[2];
                Assert.IsTrue(H5IM.get_palette_info(file, "img8", 0, palDims) >= 0);
                Assert.AreEqual(2ul, palDims[0]);
                Assert.AreEqual(3ul, palDims[1]);
                Span<byte> palRead = stackalloc byte[6];
                Assert.IsTrue(H5IM.get_palette(file, "img8", 0, palRead) >= 0);
                Assert.AreEqual((byte)60, palRead[5]);

                table = H5PT.create_fl(file, "packets", H5T.NATIVE_INT64, 4, 0);
                Assert.IsTrue(table >= 0);
                Assert.IsTrue(H5PT.append<long>(table, [100L, 200L, 300L]) >= 0);
                Span<long> packets = stackalloc long[2];
                Assert.IsTrue(H5PT.read_packets(table, 1, packets) >= 0);
                Assert.AreEqual(200L, packets[0]);
                Assert.AreEqual(300L, packets[1]);
                Assert.IsTrue(H5PT.close(table) >= 0);
                table = -1;

                // Modern reference and VOL connector UTF-8 Span<byte> queries.
                var volLen = (int)H5VL.get_connector_name(file, Span<byte>.Empty);
                Assert.IsTrue(volLen > 0);
                Span<byte> volName = stackalloc byte[volLen + 1];
                Assert.AreEqual((IntPtr)volLen, H5VL.get_connector_name(file, volName));
                Assert.AreEqual("native", Encoding.UTF8.GetString(volName.Slice(0, volLen)));

                Assert.IsTrue(H5R.create_object(file, "lt_vec", H5P.DEFAULT, out objRef) >= 0);
                refCreated = true;
                var objNameLen = (int)H5R.get_obj_name(ref objRef, H5P.DEFAULT, Span<byte>.Empty);
                Span<byte> objNameBytes = stackalloc byte[objNameLen + 1];
                Assert.AreEqual((IntPtr)objNameLen, H5R.get_obj_name(ref objRef, H5P.DEFAULT, objNameBytes));
                Assert.AreEqual("/lt_vec", Encoding.UTF8.GetString(objNameBytes.Slice(0, objNameLen)));

                // Flush & round-trip the entire HDF5 file via Span<byte> in-memory image.
                Assert.IsTrue(H5F.flush(file, H5F.scope_t.GLOBAL) >= 0);
                var imgSize = (int)H5F.get_file_image(file, Span<byte>.Empty);
                Assert.IsTrue(imgSize > 0);
                var imgBytes = new byte[imgSize];
                Assert.AreEqual((IntPtr)imgSize, H5F.get_file_image(file, imgBytes.AsSpan()));

                memFile = H5LT.open_file_image(imgBytes);
                Assert.IsTrue(memFile >= 0);
                Span<float> memValues = stackalloc float[4];
                Assert.IsTrue(H5LT.read_dataset(memFile, "lt_vec", H5T.NATIVE_FLOAT, memValues) >= 0);
                Assert.AreEqual(40f, memValues[3]);
            }
            finally
            {
                if (memFile >= 0)
                {
                    H5F.close(memFile);
                }

                if (refCreated)
                {
                    H5R.destroy(ref objRef);
                }

                if (table >= 0)
                {
                    H5PT.close(table);
                }

                if (enumType >= 0)
                {
                    H5T.close(enumType);
                }

                if (arrayType >= 0)
                {
                    H5T.close(arrayType);
                }

                if (decodedFapl >= 0)
                {
                    H5P.close(decodedFapl);
                }

                if (fapl >= 0)
                {
                    H5P.close(fapl);
                }

                if (attr >= 0)
                {
                    H5A.close(attr);
                }

                if (attrSpace >= 0)
                {
                    H5S.close(attrSpace);
                }

                if (file >= 0)
                {
                    H5F.close(file);
                }

                File.Delete(path);
            }
        }

        /// <summary>
        /// Verifies truncation-free string getters, H5LT span bounds validation, and zero-cleared pooled StringBuilder buffers.
        /// </summary>
        [TestMethod]
        public void TruncationFreeStringGettersAndZeroClearedPoolBuffers()
        {
            var path = Path.Combine(Path.GetTempPath(), "span-strings-" + Guid.NewGuid() + ".h5");
            long file = -1, space = -1, dset = -1, attr = -1, dapl = -1, fapl = -1;
            H5R.ref_t objRef = default;
            var refCreated = false;

            try
            {
                file = H5F.create(path, H5F.ACC_TRUNC);
                space = H5S.create_simple([2, 2]);
                dset = H5D.create(file, "matrix_数据", H5T.NATIVE_INT, space);
                attr = H5A.create(dset, "unit_单位", H5T.NATIVE_INT, space);
                dapl = H5P.create(H5P.DATASET_ACCESS);
                fapl = H5P.create(H5P.FILE_ACCESS);
                Assert.IsTrue(file >= 0 && space >= 0 && dset >= 0 && attr >= 0 && dapl >= 0 && fapl >= 0);

                // Contaminate the thread-local ArrayPool<byte> bucket via H5I.get_name, then check unset property getters.
                var sb = new StringBuilder(64);
                Assert.IsTrue(H5I.get_name(dset, sb, (IntPtr)64).ToInt64() > 0);
                Assert.AreEqual("/matrix_数据", sb.ToString());

                Assert.AreEqual(IntPtr.Zero, H5P.get_virtual_prefix(dapl, sb, (IntPtr)64));
                Assert.AreEqual(string.Empty, sb.ToString());

                sb.Append("stale");
                byte isEnabled = 0, startOnAccess = 0;
                var locSize = (IntPtr)64;
                Assert.IsTrue(H5P.get_mdc_log_options(fapl, ref isEnabled, sb, ref locSize, ref startOnAccess) >= 0);
                Assert.AreEqual(string.Empty, sb.ToString());

                // H5LT.make_dataset<T> rejects spans shorter than the product of dimensions.
                Assert.ThrowsExactly<ArgumentException>(() => H5LT.make_dataset<float>(file, "bad", [2, 2], H5T.NATIVE_FLOAT, [1f, 2f, 3f]));

                // Short (< 256 byte) stack-fast-path string getters.
                Assert.AreEqual("/matrix_数据", H5I.get_name(dset));
                StringAssert.EndsWith(H5F.get_name(file), Path.GetFileName(path));
                Assert.AreEqual("unit_单位", H5A.get_name(attr));
                Assert.AreEqual("unit_单位", H5A.get_name_by_idx(file, "matrix_数据", H5.index_t.NAME, H5.iter_order_t.INC, 0));
                Assert.AreEqual("matrix_数据", H5L.get_name_by_idx(file, ".", H5.index_t.NAME, H5.iter_order_t.INC, 0));
                Assert.AreEqual("native", H5VL.get_connector_name(file));
                Assert.AreEqual("HDF5", H5E.get_class_name(H5E.ERR_CLS));

                Assert.IsTrue(H5DS.set_label(dset, 0, "time_时间") >= 0);
                Assert.AreEqual("time_时间", H5DS.get_label(dset, 0));
                Assert.AreEqual(string.Empty, H5DS.get_label(dset, 1));

                Assert.IsTrue(H5R.create_object(file, "matrix_数据", H5P.DEFAULT, out objRef) >= 0);
                refCreated = true;
                Assert.AreEqual("/matrix_数据", H5R.get_obj_name(ref objRef));
                StringAssert.EndsWith(H5R.get_file_name(ref objRef), Path.GetFileName(path));

                // Unset vs long (> 256 UTF-8 bytes) pooled-retry path without truncation.
                Assert.AreEqual(string.Empty, H5O.get_comment(dset));
                var longComment = new string('数', 200) + "_end";
                Assert.IsTrue(Encoding.UTF8.GetByteCount(longComment) > 512);
                Assert.IsTrue(H5O.set_comment(dset, longComment) >= 0);
                Assert.AreEqual(longComment, H5O.get_comment(dset));
                Assert.AreEqual(longComment, H5O.get_comment_by_name(file, "matrix_数据"));

                // Invalid identifiers return null rather than throwing or returning empty.
                Assert.IsNull(H5I.get_name(-1));
                Assert.IsNull(H5O.get_comment(-1));
                Assert.IsNull(H5F.get_name(-1));
                Assert.IsNull(H5A.get_name(-1));
            }
            finally
            {
                if (refCreated)
                {
                    H5R.destroy(ref objRef);
                }

                if (fapl >= 0)
                {
                    H5P.close(fapl);
                }

                if (dapl >= 0)
                {
                    H5P.close(dapl);
                }

                if (attr >= 0)
                {
                    H5A.close(attr);
                }

                if (dset >= 0)
                {
                    H5D.close(dset);
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
}
#endif

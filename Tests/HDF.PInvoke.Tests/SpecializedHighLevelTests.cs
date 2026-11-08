using System;
using System.IO;
using System.Runtime.InteropServices;
using HDF.PInvoke;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    /// <summary>Checks packet tables, image conventions, table records and growing datasets.</summary>
    [TestClass]
    public unsafe class SpecializedHighLevelTests
    {
        /// <summary>Matches the native variable-length sequence memory representation.</summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct VariablePacket
        {
            /// <summary>Number of sequence elements.</summary>
            public UIntPtr length;

            /// <summary>Address of sequence data.</summary>
            public IntPtr data;
        }

        /// <summary>Checks packet reads, iteration indexes, growing-dataset reads and variable-length reclamation.</summary>
        [TestMethod]
        public void PacketTablesAndGrowingDatasets()
        {
            var path = Path.Combine(Path.GetTempPath(), "packets-" + Guid.NewGuid() + ".h5");
            var file = H5F.create(path, H5F.ACC_TRUNC);
            var table = H5PT.create(file, "packets", H5T.NATIVE_INT, 4, H5P.DEFAULT);
            long variableType = -1, variableTable = -1;

            try
            {
                Assert.IsTrue(table >= 0);
                Assert.IsTrue(H5PT.is_valid(table) >= 0);
                Assert.AreEqual(0, H5PT.is_varlen(table));
                int* input = stackalloc int[2] { 10, 20 };
                int* output = stackalloc int[2];
                Assert.IsTrue(H5PT.append(table, new UIntPtr(2u), (IntPtr)input) >= 0);
                Assert.IsTrue(H5PT.get_num_packets(table, out var count) >= 0);
                Assert.AreEqual(2UL, count);
                Assert.IsTrue(H5PT.read_packets(table, 0, new UIntPtr(2u), (IntPtr)output) >= 0);
                Assert.AreEqual(20, output[1]);
                Assert.IsTrue(H5PT.create_index(table) >= 0);
                Assert.IsTrue(H5PT.get_next(table, new UIntPtr(1u), (IntPtr)output) >= 0);
                Assert.AreEqual(10, output[0]);
                Assert.IsTrue(H5PT.set_index(table, 1) >= 0);
                Assert.IsTrue(H5PT.get_index(table, out var index) >= 0);
                Assert.AreEqual(1UL, index);
                Assert.IsTrue(H5PT.get_next(table, new UIntPtr(1u), (IntPtr)output) >= 0);
                Assert.AreEqual(20, output[0]);
                var dataset = H5PT.get_dataset(table);
                Assert.IsTrue(dataset >= 0 && H5PT.get_type(table) >= 0);
                ulong previous = 0, current = 0;
                Assert.IsTrue(H5LD.get_dset_dims(dataset, (IntPtr)(&previous)) >= 0);
                Assert.AreEqual(2UL, previous);
                Assert.AreEqual(4UL, H5LD.get_dset_type_size(dataset, null).ToUInt64());
                input[0] = 30;
                input[1] = 40;
                Assert.IsTrue(H5PT.append(table, new UIntPtr(2u), (IntPtr)input) >= 0);
                Assert.IsTrue(H5LD.get_dset_dims(dataset, (IntPtr)(&current)) >= 0);
                Assert.AreEqual(4UL, current);
                Assert.IsTrue(H5LD.get_dset_elmts(dataset, (IntPtr)(&previous), (IntPtr)(&current), null, (IntPtr)output) >= 0);
                Assert.AreEqual(30, output[0]);
                Assert.AreEqual(40, output[1]);
                var opened = H5PT.open(file, "packets");
                Assert.IsTrue(opened >= 0);
                H5PT.close(opened);
                var fixedTable = H5PT.create_fl(file, "fixed", H5T.NATIVE_INT, 4, -1);
                Assert.IsTrue(fixedTable >= 0);
                H5PT.close(fixedTable);
                variableType = H5T.vlen_create(H5T.NATIVE_INT);
                variableTable = H5PT.create(file, "variable", variableType, 4, H5P.DEFAULT);
                Assert.IsTrue(variableTable >= 0);
                Assert.IsTrue(H5PT.is_varlen(variableTable) > 0);
                var packet = new VariablePacket { length = new UIntPtr(2u), data = (IntPtr)input };
                Assert.IsTrue(H5PT.append(variableTable, new UIntPtr(1u), (IntPtr)(&packet)) >= 0);
                VariablePacket read = default;
                Assert.IsTrue(H5PT.read_packets(variableTable, 0, new UIntPtr(1u), (IntPtr)(&read)) >= 0);

                try
                {
                    Assert.AreEqual(2UL, read.length.ToUInt64());
                    Assert.AreEqual(40, Marshal.ReadInt32(read.data, 4));
                }
                finally
                {
                    Assert.IsTrue(H5PT.free_vlen_buff(variableTable, new UIntPtr(1u), (IntPtr)(&read)) >= 0);
                }
            }
            finally
            {
                if (variableTable >= 0)
                {
                    H5PT.close(variableTable);
                }

                if (variableType >= 0)
                {
                    H5T.close(variableType);
                }

                H5PT.close(table);
                H5F.close(file);
                File.Delete(path);
            }
        }

        /// <summary>Checks indexed/true-color images and palette attachment and contents.</summary>
        [TestMethod]
        public void ImagesAndPalettes()
        {
            var path = Path.Combine(Path.GetTempPath(), "images-" + Guid.NewGuid() + ".h5");
            var file = H5F.create(path, H5F.ACC_TRUNC);

            try
            {
                byte* indexed = stackalloc byte[4] { 0, 1, 1, 0 };
                byte* color = stackalloc byte[12] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
                byte* output = stackalloc byte[64];
                byte* palette = stackalloc byte[6] { 0, 0, 0, 255, 255, 255 };
                ulong* dimensions = stackalloc ulong[2] { 2, 3 };
                Assert.IsTrue(H5IM.make_image_8bit(file, "indexed", 2, 2, (IntPtr)indexed) >= 0);
                Assert.IsTrue(H5IM.make_image_24bit(file, "color", 2, 2, "INTERLACE_PIXEL", (IntPtr)color) >= 0);
                Assert.IsTrue(H5IM.is_image(file, "color") > 0);
                Assert.IsTrue(H5IM.get_image_info(file, "color", out var width, out var height, out var planes, (IntPtr)output, out var palettes) >= 0);
                Assert.AreEqual(2UL, width);
                Assert.AreEqual(2UL, height);
                Assert.AreEqual(3UL, planes);
                Assert.AreEqual(0L, palettes);
                Assert.AreEqual("INTERLACE_PIXEL", Marshal.PtrToStringAnsi((IntPtr)output));
                Assert.IsTrue(H5IM.read_image(file, "color", (IntPtr)output) >= 0);

                for (var i = 0; i < 12; i++)
                {
                    Assert.AreEqual(color[i], output[i]);
                }

                Assert.IsTrue(H5IM.make_palette(file, "palette", (IntPtr)dimensions, (IntPtr)palette) >= 0);
                Assert.IsTrue(H5IM.is_palette(file, "palette") > 0);
                Assert.IsTrue(H5IM.link_palette(file, "indexed", "palette") >= 0);
                Assert.IsTrue(H5IM.get_npalettes(file, "indexed", out palettes) >= 0);
                Assert.AreEqual(1L, palettes);
                Assert.IsTrue(H5IM.get_palette_info(file, "indexed", 0, (IntPtr)dimensions) >= 0);
                Assert.AreEqual(2UL, dimensions[0]);
                Assert.AreEqual(3UL, dimensions[1]);
                Assert.IsTrue(H5IM.get_palette(file, "indexed", 0, (IntPtr)output) >= 0);

                for (var i = 0; i < 6; i++)
                {
                    Assert.AreEqual(palette[i], output[i]);
                }

                Assert.IsTrue(H5IM.unlink_palette(file, "indexed", "palette") >= 0);
                Assert.IsTrue(H5IM.get_npalettes(file, "indexed", out palettes) >= 0);
                Assert.AreEqual(0L, palettes);
            }
            finally
            {
                H5F.close(file);
                File.Delete(path);
            }
        }

        /// <summary>Checks all table record, field, metadata and combination operations on a known native record layout.</summary>
        [TestMethod]
        public void TableRecordsAndFields()
        {
            var path = Path.Combine(Path.GetTempPath(), "tables-" + Guid.NewGuid() + ".h5");
            var file = H5F.create(path, H5F.ACC_TRUNC);
            var fieldName = Marshal.StringToHGlobalAnsi("value");
            var outputName = Marshal.AllocHGlobal(64);

            try
            {
                UIntPtr offset = UIntPtr.Zero, width = new UIntPtr(4u);
                long type = H5T.NATIVE_INT;
                int* input = stackalloc int[2] { 1, 2 };
                int* output = stackalloc int[8];
                var fill = -1;
                Assert.IsTrue(H5TB.make_table("title", file, "first", 1, 2, width, (IntPtr)(&fieldName), (IntPtr)(&offset), (IntPtr)(&type), 2, (IntPtr)(&fill), 0, (IntPtr)input) >= 0);
                Assert.IsTrue(H5TB.get_table_info(file, "first", out var fields, out var records) >= 0);
                Assert.AreEqual(1UL, fields);
                Assert.AreEqual(2UL, records);
                UIntPtr fieldWidth = UIntPtr.Zero, fieldOffset = UIntPtr.Zero;
                Assert.IsTrue(H5TB.get_field_info(file, "first", (IntPtr)(&outputName), (IntPtr)(&fieldWidth), (IntPtr)(&fieldOffset), out var recordWidth) >= 0);
                Assert.AreEqual("value", Marshal.PtrToStringAnsi(outputName));
                Assert.AreEqual(4UL, fieldWidth.ToUInt64());
                Assert.AreEqual(0UL, fieldOffset.ToUInt64());
                Assert.AreEqual(4UL, recordWidth.ToUInt64());
                var dataset = H5D.open(file, "first");

                try
                {
                    Assert.IsTrue(H5TB.Aget_title(dataset, outputName) >= 0);
                    Assert.AreEqual("title", Marshal.PtrToStringAnsi(outputName));
                    Assert.IsTrue(H5TB.Aget_fill(file, "first", dataset, (IntPtr)output) >= 0);
                    Assert.AreEqual(-1, output[0]);
                }
                finally
                {
                    H5D.close(dataset);
                }

                input[0] = 3;
                input[1] = 4;
                Assert.IsTrue(H5TB.append_records(file, "first", 2, width, (IntPtr)(&offset), (IntPtr)(&width), (IntPtr)input) >= 0);
                input[0] = 5;
                Assert.IsTrue(H5TB.write_records(file, "first", 0, 1, width, (IntPtr)(&offset), (IntPtr)(&width), (IntPtr)input) >= 0);
                input[0] = 6;
                Assert.IsTrue(H5TB.write_fields_name(file, "first", "value", 1, 1, width, (IntPtr)(&offset), (IntPtr)(&width), (IntPtr)input) >= 0);
                var index = 0;
                input[0] = 7;
                Assert.IsTrue(H5TB.write_fields_index(file, "first", 1, (IntPtr)(&index), 2, 1, width, (IntPtr)(&offset), (IntPtr)(&width), (IntPtr)input) >= 0);
                Assert.IsTrue(H5TB.read_table(file, "first", width, (IntPtr)(&offset), (IntPtr)(&width), (IntPtr)output) >= 0);

                for (var i = 0; i < 4; i++)
                {
                    Assert.AreEqual(new[] { 5, 6, 7, 4 }[i], output[i]);
                }

                Assert.IsTrue(H5TB.read_fields_name(file, "first", "value", 2, 1, width, (IntPtr)(&offset), (IntPtr)(&width), (IntPtr)output) >= 0);
                Assert.AreEqual(7, output[0]);
                Assert.IsTrue(H5TB.read_fields_index(file, "first", 1, (IntPtr)(&index), 1, 1, width, (IntPtr)(&offset), (IntPtr)(&width), (IntPtr)output) >= 0);
                Assert.AreEqual(6, output[0]);
                Assert.IsTrue(H5TB.read_records(file, "first", 3, 1, width, (IntPtr)(&offset), (IntPtr)(&width), (IntPtr)output) >= 0);
                Assert.AreEqual(4, output[0]);
                Assert.IsTrue(H5TB.delete_record(file, "first", 3, 1) >= 0);
                input[0] = 8;
                Assert.IsTrue(H5TB.insert_record(file, "first", 1, 1, width, (IntPtr)(&offset), (IntPtr)(&width), (IntPtr)input) >= 0);
                input[0] = 9;
                Assert.IsTrue(H5TB.make_table("second", file, "second", 1, 1, width, (IntPtr)(&fieldName), (IntPtr)(&offset), (IntPtr)(&type), 2, IntPtr.Zero, 0, (IntPtr)input) >= 0);
                Assert.IsTrue(H5TB.add_records_from(file, "first", 0, 1, "second", 1) >= 0);
                Assert.IsTrue(H5TB.combine_tables(file, "first", file, "second", "combined") >= 0);
                Assert.IsTrue(H5TB.get_table_info(file, "combined", out fields, out records) >= 0);
                Assert.AreEqual(6UL, records);
                Assert.IsTrue(H5TB.insert_field(file, "first", "extra", H5T.NATIVE_INT, 1, (IntPtr)(&fill), IntPtr.Zero) >= 0);
                Assert.IsTrue(H5TB.delete_field(file, "first", "extra") >= 0);
                Assert.IsTrue(H5TB.get_table_info(file, "first", out fields, out records) >= 0);
                Assert.AreEqual(1UL, fields);
                Assert.AreEqual(4UL, records);
            }
            finally
            {
                Marshal.FreeHGlobal(outputName);
                Marshal.FreeHGlobal(fieldName);
                H5F.close(file);
                File.Delete(path);
            }
        }
    }
}


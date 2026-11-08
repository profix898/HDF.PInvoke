using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using HDF.PInvoke;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    /// <summary>Exercises high-level dataset, attribute and dimension-scale APIs with native buffers.</summary>
    [TestClass]
    public unsafe class HighLevelApiTests
    {
        /// <summary>Checks every native numeric convenience pair, including platform-sized C long values.</summary>
        /// <param name="suffix">The native numeric API suffix.</param>
        /// <param name="width">Native element width; zero selects C long width.</param>
        [TestMethod]
        [DataRow("char", 1)]
        [DataRow("uchar", 1)]
        [DataRow("short", 2)]
        [DataRow("ushort", 2)]
        [DataRow("int", 4)]
        [DataRow("uint", 4)]
        [DataRow("long", 0)]
        [DataRow("ulong", 0)]
        [DataRow("long_long", 8)]
        [DataRow("ullong", 8)]
        [DataRow("float", 4)]
        [DataRow("double", 8)]
        public void NumericConvenienceRoundTrip(string suffix, int width)
        {
            if (width == 0)
            {
                width = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? 4 : 8;
            }

            var path = Path.Combine(Path.GetTempPath(), "lite-" + Guid.NewGuid() + ".h5");
            var file = H5F.create(path, H5F.ACC_TRUNC);

            try
            {
                var input = new byte[width * 2];
                input[0] = 1;
                input[width] = 2;
                var output = new byte[input.Length];
                ulong dimensions = 2;
                fixed (byte* source = input, destination = output)
                {
                    var setter = typeof(H5LT).GetMethod("set_attribute_" + suffix);
                    var getter = typeof(H5LT).GetMethod("get_attribute_" + suffix);
                    Assert.IsTrue((int)setter.Invoke(null, new object[] { file, "/", "值", (IntPtr)source, new UIntPtr(2u) }) >= 0);
                    Assert.IsTrue((int)getter.Invoke(null, new object[] { file, "/", "值", (IntPtr)destination }) >= 0);
                    CollectionAssert.AreEqual(input, output);
                    var make = typeof(H5LT).GetMethod("make_dataset_" + suffix);

                    if (make != null)
                    {
                        Array.Clear(output, 0, output.Length);
                        Assert.IsTrue((int)make.Invoke(null, new object[] { file, "数据", 1, (IntPtr)(&dimensions), (IntPtr)source }) >= 0);
                        Assert.IsTrue((int)typeof(H5LT).GetMethod("read_dataset_" + suffix).Invoke(null, new object[] { file, "数据", (IntPtr)destination }) >= 0);
                        CollectionAssert.AreEqual(input, output);
                    }
                }
            }
            finally
            {
                H5F.close(file);
                File.Delete(path);
            }
        }

        /// <summary>Checks generic data access, metadata, strings, datatype text and copied file images.</summary>
        [TestMethod]
        public void LiteMetadataStringsAndFileImage()
        {
            var path = Path.Combine(Path.GetTempPath(), "lite-metadata-" + Guid.NewGuid() + ".h5");
            var file = H5F.create(path, H5F.ACC_TRUNC);

            try
            {
                int value = 42, actual = 0;
                ulong dimensions = 1;
                Assert.IsTrue(H5LT.make_dataset(file, "data", 1, (IntPtr)(&dimensions), H5T.NATIVE_INT, (IntPtr)(&value)) >= 0);
                Assert.IsTrue(H5LT.read_dataset(file, "data", H5T.NATIVE_INT, (IntPtr)(&actual)) >= 0);
                Assert.AreEqual(value, actual);
                Assert.IsTrue(H5LT.find_dataset(file, "data") > 0);
                Assert.IsTrue(H5LT.path_valid(file, "data", 1) > 0);
                Assert.IsTrue(H5LT.get_dataset_ndims(file, "data", out var rank) >= 0);
                Assert.AreEqual(1, rank);
                Assert.IsTrue(H5LT.get_dataset_info(file, "data", (IntPtr)(&dimensions), out var kind, out var size) >= 0);
                Assert.AreEqual(H5T.class_t.INTEGER, kind);
                Assert.AreEqual(4UL, size.ToUInt64());
                Assert.AreEqual(1UL, dimensions);
                Assert.IsTrue(H5LT.set_attribute_int(file, "data", "number", (IntPtr)(&value), new UIntPtr(1u)) >= 0);
                Assert.IsTrue(H5LT.get_attribute(file, "data", "number", H5T.NATIVE_INT, (IntPtr)(&actual)) >= 0);
                Assert.AreEqual(42, actual);
                Assert.IsTrue(H5LT.get_attribute_ndims(file, "data", "number", out rank) >= 0);
                Assert.AreEqual(1, rank);
                Assert.IsTrue(H5LT.get_attribute_info(file, "data", "number", (IntPtr)(&dimensions), out kind, out size) >= 0);
                Assert.AreEqual(4UL, size.ToUInt64());
                var data = H5D.open(file, "data");
                Assert.IsTrue(H5LT.find_attribute(data, "number") > 0);
                H5D.close(data);
                Assert.IsTrue(H5LT.make_dataset_string(file, "text", "Grüße") >= 0);
                Assert.IsTrue(H5LT.set_attribute_string(file, "data", "label", "标签") >= 0);
                var bytes = new byte[512];
                fixed (byte* p = bytes)
                {
                    Assert.IsTrue(H5LT.read_dataset_string(file, "text", (IntPtr)p) >= 0);
                    Assert.AreEqual("Grüße", Decode(bytes));
                    Assert.IsTrue(H5LT.get_attribute_string(file, "data", "label", (IntPtr)p) >= 0);
                    Assert.AreEqual("标签", Decode(bytes));
                    size = UIntPtr.Zero;
                    Assert.IsTrue(H5LT.dtype_to_text(H5T.IEEE_F64LE, IntPtr.Zero, H5LT.lang_t.DDL, ref size) >= 0);
                    Assert.IsTrue(size.ToUInt64() < (ulong)bytes.Length);
                    size = new UIntPtr((uint)bytes.Length);
                    Assert.IsTrue(H5LT.dtype_to_text(H5T.IEEE_F64LE, (IntPtr)p, H5LT.lang_t.DDL, ref size) >= 0);
                    var type = H5LT.text_to_dtype(Decode(bytes), H5LT.lang_t.DDL);
                    Assert.IsTrue(type >= 0);

                    try
                    {
                        Assert.IsTrue(H5T.equal(type, H5T.IEEE_F64LE) > 0);
                    }
                    finally
                    {
                        H5T.close(type);
                    }
                }
                Assert.IsTrue(H5F.flush(file, H5F.scope_t.GLOBAL) >= 0);
                var length = H5F.get_file_image(file, IntPtr.Zero, IntPtr.Zero);
                Assert.IsTrue(length.ToInt64() > 0);
                var image = new byte[length.ToInt32()];
                fixed (byte* p = image)
                {
                    Assert.AreEqual(length, H5F.get_file_image(file, (IntPtr)p, length));
                    var copy = H5LT.open_file_image((IntPtr)p, new UIntPtr((uint)image.Length), 0);
                    Assert.IsTrue(copy >= 0);

                    try
                    {
                        Assert.IsTrue(H5LT.find_dataset(copy, "data") > 0);
                    }
                    finally
                    {
                        H5F.close(copy);
                    }
                }
            }
            finally
            {
                H5F.close(file);
                File.Delete(path);
            }
        }

        /// <summary>Checks attachment, labels, scale iteration and reference-format queries.</summary>
        [TestMethod]
        public void DimensionScalesRoundTrip()
        {
            var path = Path.Combine(Path.GetTempPath(), "scales-" + Guid.NewGuid() + ".h5");
            var file = H5F.create(path, H5F.ACC_TRUNC);
            var space = H5S.create_simple(1, new ulong[] { 2 }, null);
            var data = H5D.create(file, "data", H5T.NATIVE_INT, space);
            var scale = H5D.create(file, "scale", H5T.NATIVE_DOUBLE, space);

            try
            {
                Assert.IsTrue(H5DS.with_new_ref(file, out var modern) >= 0);
                Assert.IsTrue(modern <= 1);
                Assert.IsTrue(H5DS.set_scale(scale, "时间") >= 0);
                Assert.IsTrue(H5DS.is_scale(scale) > 0);
                Assert.IsTrue(H5DS.attach_scale(data, scale, 0) >= 0);
                Assert.IsTrue(H5DS.is_attached(data, scale, 0) > 0);
                Assert.AreEqual(1, H5DS.get_num_scales(data, 0));
                Assert.IsTrue(H5DS.set_label(data, 0, "Time") >= 0);
                var bytes = new byte[64];
                fixed (byte* p = bytes)
                {
                    Assert.AreEqual(4L, H5DS.get_label(data, 0, (IntPtr)p, new UIntPtr(64u)).ToInt64());
                    Assert.AreEqual("Time", Decode(bytes));
                    Assert.IsTrue(H5DS.get_scale_name(scale, (IntPtr)p, new UIntPtr(64u)).ToInt64() > 0);
                    Assert.AreEqual("时间", Decode(bytes));
                }
                var visits = 0;
                H5DS.iterate_t callback = (dataset, dimension, attached, state) =>
                {
                    if (dimension != 0 || H5DS.is_scale(attached) <= 0 || state != new IntPtr(42))
                    {
                        return -1;
                    }

                    visits++;
                    return 0;
                };
                var index = 0;
                Assert.IsTrue(H5DS.iterate_scales(data, 0, ref index, callback, new IntPtr(42)) >= 0);
                Assert.AreEqual(1, visits);
                Assert.IsTrue(H5DS.detach_scale(data, scale, 0) >= 0);
                Assert.AreEqual(0, H5DS.is_attached(data, scale, 0));
                GC.KeepAlive(callback);
            }
            finally
            {
                H5D.close(scale);
                H5D.close(data);
                H5S.close(space);
                H5F.close(file);
                File.Delete(path);
            }
        }

        /// <summary>Decodes a null-terminated UTF-8 test output without relying on runtime-specific helpers.</summary>
        private static string Decode(byte[] bytes) => Encoding.UTF8.GetString(bytes, 0, Array.IndexOf(bytes, (byte)0));
    }
}


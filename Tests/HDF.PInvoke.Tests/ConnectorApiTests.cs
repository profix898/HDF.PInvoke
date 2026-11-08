using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using HDF.PInvoke;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    /// <summary>Validates current predefined types and connector configuration against the packaged native connector.</summary>
    [TestClass]
    public unsafe class ConnectorApiTests
    {
        /// <summary>Checks reduced-precision and complex predefined types and optional native types.</summary>
        [TestMethod]
        public void CurrentDatatypeGlobals()
        {
            foreach (var name in new[] { "IEEE_F16BE", "IEEE_F16LE", "FLOAT_BFLOAT16BE", "FLOAT_BFLOAT16LE", "FLOAT_F8E4M3", "FLOAT_F8E5M2", "FLOAT_F6E2M3", "FLOAT_F6E3M2", "FLOAT_F4E2M1", "COMPLEX_IEEE_F16BE", "COMPLEX_IEEE_F16LE", "COMPLEX_IEEE_F32BE", "COMPLEX_IEEE_F32LE", "COMPLEX_IEEE_F64BE", "COMPLEX_IEEE_F64LE" })
            {
                var id = (long)typeof(H5T).GetProperty(name).GetValue(null);
                Assert.IsTrue(id >= 0, name);
                Assert.AreEqual(name.StartsWith("COMPLEX") ? H5T.class_t.COMPLEX : H5T.class_t.FLOAT, H5T.get_class(id), name);
                Assert.IsTrue(H5T.get_size(id).ToInt64() > 0);
            }
            foreach (var id in new[] { H5T.NATIVE_FLOAT16, H5T.NATIVE_FLOAT_COMPLEX, H5T.NATIVE_DOUBLE_COMPLEX, H5T.NATIVE_LDOUBLE_COMPLEX })
            {
                // HDF5 deliberately exports negative identifiers for optional types absent from the C compiler.
                if (id >= 0) Assert.IsTrue(H5T.get_size(id).ToInt64() > 0);
            }
            foreach (var cls in new[] { H5P.ATTRIBUTE_ACCESS, H5P.REFERENCE_ACCESS, H5P.VOL_INITIALIZE })
            {
                var list = H5P.create(cls);
                Assert.IsTrue(list >= 0);
                H5P.close(list);
            }
            Assert.IsTrue(H5P.ATTRIBUTE_ACCESS_DEFAULT >= 0);
            Assert.IsTrue(H5P.REFERENCE_ACCESS_DEFAULT >= 0);
            Assert.IsTrue(H5P.VOL_INITIALIZE_DEFAULT >= 0);
            _ = H5P.MAP_CREATE_DEFAULT;
            _ = H5P.MAP_ACCESS_DEFAULT;
            _ = H5P.MAP_CREATE;
            var mapAccess = H5P.create(H5P.MAP_ACCESS);
            Assert.IsTrue(mapAccess >= 0);
            H5P.close(mapAccess);
        }

        /// <summary>Checks connector discovery, identifiers, capability flags and configuration ownership.</summary>
        [TestMethod]
        public void NativeConnectorConfiguration()
        {
            var first = H5VL.register_connector_by_name("native");
            var second = H5VL.register_connector_by_value(0);
            var named = H5VL.get_connector_id_by_name("native");
            var valued = H5VL.get_connector_id_by_value(0);
            var access = H5P.create(H5P.FILE_ACCESS);
            var path = Path.Combine(Path.GetTempPath(), "vol-" + Guid.NewGuid() + ".h5");
            long file = -1;
            try
            {
                Assert.IsTrue(first >= 0 && second >= 0 && named >= 0 && valued >= 0);
                Assert.IsTrue(H5VL.is_connector_registered_by_name("native") > 0);
                Assert.IsTrue(H5VL.is_connector_registered_by_value(0) > 0);
                Assert.IsTrue(H5P.set_vol(access, first, IntPtr.Zero) >= 0);
                Assert.IsTrue(H5P.get_vol_id(access, out var selected) >= 0);
                H5VL.close(selected);
                Assert.IsTrue(H5P.get_vol_info(access, out var info) >= 0);
                Assert.IsTrue(H5VL.free_connector_info(first, info) >= 0);
                Assert.IsTrue(H5P.get_vol_cap_flags(access, out var flags) >= 0);
                Assert.IsTrue((flags & H5VL.CAP_FLAG_NATIVE_FILES) != 0);
                file = H5F.create(path, H5F.ACC_TRUNC, H5P.DEFAULT, access);
                Assert.IsTrue(file >= 0);
                Assert.IsTrue(H5VL.object_is_native(file, out var native) >= 0);
                Assert.AreEqual((byte)1, native);
                var objectConnector = H5VL.get_connector_id(file);
                Assert.IsTrue(objectConnector >= 0);
                H5VL.close(objectConnector);
                var bytes = new byte[64];
                fixed (byte* p = bytes)
                {
                    var length = H5VL.get_connector_name(file, (IntPtr)p, new UIntPtr(64u)).ToInt32();
                    Assert.AreEqual("native", Encoding.UTF8.GetString(bytes, 0, length));
                }
                // Native dataset optional operation zero is format conversion; query it on a dataset object.
                var space = H5S.create(H5S.class_t.SCALAR);
                var dataset = H5D.create(file, "data", H5T.NATIVE_INT, space);
                try { Assert.IsTrue(H5VL.query_optional(dataset, H5VL.subclass_t.DATASET, 0, out flags) >= 0); }
                finally { H5D.close(dataset); H5S.close(space); }
                // HDF5 explicitly forbids unregistering its native connector.
                Assert.IsTrue(H5VL.unregister_connector(first) < 0);
            }
            finally
            {
                if (file >= 0) H5F.close(file);
                H5P.close(access);
                foreach (var id in new[] { valued, named, second, first }) if (id >= 0) H5VL.close(id);
                if (File.Exists(path)) File.Delete(path);
            }
        }
    }
}


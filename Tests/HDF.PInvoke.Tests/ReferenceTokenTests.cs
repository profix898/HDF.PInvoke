using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using HDF.PInvoke;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    /// <summary>Checks reference ownership and current token-based traversal across native platforms.</summary>
    [TestClass]
    public unsafe class ReferenceTokenTests
    {
        /// <summary>Round-trips object, region and attribute references, including persisted modern reference values.</summary>
        [TestMethod]
        public void ModernReferenceOwnershipAndPersistence()
        {
            var path = Path.Combine(Path.GetTempPath(), "references-" + Guid.NewGuid() + ".h5");
            var file = H5F.create(path, H5F.ACC_TRUNC);
            var space = H5S.create_simple(1, new ulong[] { 4 }, null);
            var scalar = H5S.create(H5S.class_t.SCALAR);
            var dataset = H5D.create(file, "数据", H5T.NATIVE_INT, space);
            var attribute = H5A.create(dataset, "标签", H5T.NATIVE_INT, scalar);
            var references = new List<H5R.ref_t>();

            try
            {
                Assert.IsTrue(H5R.create_object(file, "数据", H5P.DEFAULT, out var obj) >= 0);
                references.Add(obj);
                Assert.AreEqual(H5R.type_t.OBJECT2, H5R.get_type(ref obj));
                Assert.IsTrue(H5R.copy(ref obj, out var copy) >= 0);
                references.Add(copy);
                Assert.IsTrue(H5R.equal(ref obj, ref copy) > 0);
                Assert.IsTrue(H5R.get_obj_type3(ref obj, H5P.DEFAULT, out var kind) >= 0);
                Assert.AreEqual(H5O.type_t.DATASET, kind);
                var opened = H5R.open_object(ref copy);
                Assert.IsTrue(opened >= 0);
                H5O.close(opened);
                Assert.IsTrue(H5S.select_hyperslab(space, H5S.seloper_t.SET, new ulong[] { 1 }, null, new ulong[] { 2 }, null) >= 0);
                Assert.IsTrue(H5R.create_region(file, "数据", space, H5P.DEFAULT, out var region) >= 0);
                references.Add(region);
                Assert.AreEqual(H5R.type_t.DATASET_REGION2, H5R.get_type(ref region));
                opened = H5R.open_region(ref region);
                Assert.IsTrue(opened >= 0);
                Assert.AreEqual(2L, H5S.get_select_npoints(opened));
                H5S.close(opened);
                Assert.IsTrue(H5R.create_attr(file, "数据", "标签", H5P.DEFAULT, out var attr) >= 0);
                references.Add(attr);
                Assert.AreEqual(H5R.type_t.ATTR, H5R.get_type(ref attr));
                opened = H5R.open_attr(ref attr);
                Assert.IsTrue(opened >= 0);
                H5A.close(opened);
                var bytes = new byte[1024];
                fixed (byte* p = bytes)
                {
                    var length = H5R.get_obj_name(ref obj, H5P.DEFAULT, (IntPtr)p, new UIntPtr((uint)bytes.Length)).ToInt32();
                    Assert.AreEqual("/数据", Encoding.UTF8.GetString(bytes, 0, length));
                    length = H5R.get_attr_name(ref attr, (IntPtr)p, new UIntPtr((uint)bytes.Length)).ToInt32();
                    Assert.AreEqual("标签", Encoding.UTF8.GetString(bytes, 0, length));
                    length = H5R.get_file_name(ref obj, (IntPtr)p, new UIntPtr((uint)bytes.Length)).ToInt32();
                    Assert.AreEqual(path, Encoding.UTF8.GetString(bytes, 0, length));
                }
                var stored = H5D.create(file, "references", H5T.STD_REF, scalar);
                Assert.IsTrue(stored >= 0);
                H5R.ref_t read = default;

                try
                {
                    Assert.IsTrue(H5D.write(stored, H5T.STD_REF, H5S.ALL, H5S.ALL, H5P.DEFAULT, (IntPtr)(&obj)) >= 0);
                    Assert.IsTrue(H5D.read(stored, H5T.STD_REF, H5S.ALL, H5S.ALL, H5P.DEFAULT, (IntPtr)(&read)) >= 0);
                    Assert.IsTrue(H5R.equal(ref obj, ref read) > 0);
                    // Reclamation destroys reference resources but leaves caller-owned storage intact.
                    Assert.IsTrue(H5T.reclaim(H5T.STD_REF, scalar, H5P.DEFAULT, (IntPtr)(&read)) >= 0);
                }
                finally
                {
                    H5D.close(stored);
                }
            }
            finally
            {
                // The list is the sole owner; temporary struct copies above are borrowed views.
                for (var i = 0; i < references.Count; i++)
                {
                    var value = references[i];
                    H5R.destroy(ref value);
                }

                H5A.close(attribute);
                H5D.close(dataset);
                H5S.close(scalar);
                H5S.close(space);
                H5F.close(file);
                File.Delete(path);
            }
        }

        /// <summary>Checks token serialization, native metadata and both current traversal callback ABIs.</summary>
        [TestMethod]
        public void TokenInformationAndTraversal()
        {
            var path = Path.Combine(Path.GetTempPath(), "tokens-" + Guid.NewGuid() + ".h5");
            var file = H5F.create(path, H5F.ACC_TRUNC);
            var group = H5G.create(file, "组");

            try
            {
                Assert.IsTrue(H5O.get_info3(group, out var direct) >= 0);
                Assert.IsTrue(H5O.get_info_by_name3(file, "组", out var named) >= 0);
                Assert.IsTrue(H5O.get_info_by_idx3(file, ".", H5.index_t.NAME, H5.iter_order_t.INC, 0, out var indexed) >= 0);
                Assert.AreEqual(H5O.type_t.GROUP, direct.type);
                Assert.AreEqual(direct.fileno, named.fileno);
                Assert.IsTrue(H5F.get_fileno(file, out var fileNumber) >= 0);
                Assert.AreEqual(direct.fileno, fileNumber);
                Assert.IsTrue(H5O.token_cmp(file, ref direct.token, ref indexed.token, out var comparison) >= 0);
                Assert.AreEqual(0, comparison);
                Assert.IsTrue(H5O.token_to_str(file, ref direct.token, out var text) >= 0);
                H5O.token_t decoded;

                try
                {
                    var length = 0;

                    while (Marshal.ReadByte(text, length) != 0)
                    {
                        length++;
                    }

                    var bytes = new byte[length];
                    Marshal.Copy(text, bytes, 0, length);
                    Assert.IsTrue(H5O.token_from_str(file, Encoding.UTF8.GetString(bytes), out decoded) >= 0);
                }
                finally
                {
                    H5.free_memory(text);
                }

                var opened = H5O.open_by_token(file, decoded);
                Assert.IsTrue(opened >= 0);
                H5O.close(opened);
                Assert.IsTrue(H5O.get_native_info(group, out var native, H5O.NATIVE_INFO_ALL) >= 0);
                Assert.IsTrue(H5O.get_native_info_by_name(file, "组", out var nativeNamed, H5O.NATIVE_INFO_ALL) >= 0);
                Assert.IsTrue(H5O.get_native_info_by_idx(file, ".", H5.index_t.NAME, H5.iter_order_t.INC, 0, out var nativeIndex, H5O.NATIVE_INFO_ALL) >= 0);
                Assert.AreEqual(native.hdr.version, nativeNamed.hdr.version);
                Assert.AreEqual(native.hdr.version, nativeIndex.hdr.version);
                Assert.IsTrue(H5L.get_info2(file, "组", out var link) >= 0);
                Assert.IsTrue(H5L.get_info_by_idx2(file, ".", H5.index_t.NAME, H5.iter_order_t.INC, 0, out var linkIndex) >= 0);
                Assert.IsTrue(H5O.token_cmp(file, ref direct.token, ref link.u.token, out comparison) >= 0);
                Assert.AreEqual(0, comparison);
                var links = 0;
                H5L.iterate2_t linkCallback = (long id, string name, ref H5L.info2_t info, IntPtr state) =>
                {
                    if (name != "组" || info.type != H5L.type_t.HARD || state != new IntPtr(42))
                    {
                        return -1;
                    }

                    links++;
                    return 0;
                };
                ulong position = 0;
                Assert.IsTrue(H5L.iterate2(file, H5.index_t.NAME, H5.iter_order_t.INC, ref position, linkCallback, new IntPtr(42)) >= 0);
                position = 0;
                Assert.IsTrue(H5L.iterate_by_name2(file, ".", H5.index_t.NAME, H5.iter_order_t.INC, ref position, linkCallback, new IntPtr(42)) >= 0);
                Assert.IsTrue(H5L.visit2(file, H5.index_t.NAME, H5.iter_order_t.INC, linkCallback, new IntPtr(42)) >= 0);
                Assert.IsTrue(H5L.visit_by_name2(file, ".", H5.index_t.NAME, H5.iter_order_t.INC, linkCallback, new IntPtr(42)) >= 0);
                Assert.AreEqual(4, links);
                var objects = 0;
                H5O.iterate2_t objectCallback = (long id, string name, ref H5O.info2_t info, IntPtr state) =>
                {
                    if (info.type != H5O.type_t.GROUP || info.fileno != direct.fileno || state != new IntPtr(42))
                    {
                        return -1;
                    }

                    objects++;
                    return 0;
                };
                Assert.IsTrue(H5O.visit3(file, H5.index_t.NAME, H5.iter_order_t.INC, objectCallback, new IntPtr(42)) >= 0);
                Assert.IsTrue(H5O.visit_by_name3(file, ".", H5.index_t.NAME, H5.iter_order_t.INC, objectCallback, new IntPtr(42)) >= 0);
                Assert.AreEqual(4, objects);
                GC.KeepAlive(linkCallback);
                GC.KeepAlive(objectCallback);
            }
            finally
            {
                H5G.close(group);
                H5F.close(file);
                File.Delete(path);
            }
        }
    }
}


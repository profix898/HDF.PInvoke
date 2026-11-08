/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * Copyright by The HDF Group.                                               *
 * Copyright by the Board of Trustees of the University of Illinois.         *
 * All rights reserved.                                                      *
 *                                                                           *
 * This file is part of HDF.PInvoke. Copyright notices and redistribution
 * terms for the wrapper and its native dependencies are in LICENSE.txt
 * at the repository and package root.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System;
using System.Runtime.InteropServices;
using System.Text;
using HDF.PInvoke;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using hid_t = System.Int64;
using hsize_t = System.UInt64;
using ssize_t = System.IntPtr;

namespace UnitTests
{
    /// <summary>
    /// Groups regression checks for the H5R API.
    /// </summary>
    public partial class H5RTest
    {
        /// <summary>
        /// Exercises the native <c>H5Rget_obj_type</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5Rget_obj_typeTest1()
        {
            var path =
                                        Encoding.UTF8.GetBytes(String.Join("/", m_utf8strings));
            // make room for the trailling \0
            var name = new byte[path.Length + 1];
            Array.Copy(path, name, path.Length);

            Assert.IsTrue(
                H5G.close(H5G.create(m_v0_test_file, name, m_lcpl_utf8)) >= 0);

            var refer = new byte[H5R.OBJ_REF_BUF_SIZE];
            var hnd = GCHandle.Alloc(refer, GCHandleType.Pinned);

            Assert.IsTrue(
                H5R.create(hnd.AddrOfPinnedObject(), m_v0_test_file, name,
                H5R.type_t.OBJECT, -1) >= 0);

            var obj_type = H5O.type_t.UNKNOWN;
            Assert.IsTrue(
                H5R.get_obj_type(m_v0_test_file, H5R.type_t.OBJECT,
                hnd.AddrOfPinnedObject(), ref obj_type) >= 0);

            hnd.Free();

            Assert.IsTrue(obj_type == H5O.type_t.GROUP);
        }

        /// <summary>
        /// Exercises the native <c>H5Rget_obj_type</c> operation in scenario 2.
        /// </summary>
        [TestMethod]
        public void H5Rget_obj_typeTest2()
        {
            var path =
                                        Encoding.UTF8.GetBytes(String.Join("/", m_utf8strings));
            // make room for the trailling \0
            var name = new byte[path.Length + 1];
            Array.Copy(path, name, path.Length);

            Assert.IsTrue(
                H5G.close(H5G.create(m_v2_test_file, name, m_lcpl_utf8)) >= 0);

            var refer = new byte[H5R.OBJ_REF_BUF_SIZE];
            var hnd = GCHandle.Alloc(refer, GCHandleType.Pinned);

            Assert.IsTrue(
                H5R.create(hnd.AddrOfPinnedObject(), m_v2_test_file, name,
                H5R.type_t.OBJECT, -1) >= 0);

            var obj_type = H5O.type_t.UNKNOWN;
            Assert.IsTrue(
                H5R.get_obj_type(m_v2_test_file, H5R.type_t.OBJECT,
                hnd.AddrOfPinnedObject(), ref obj_type) >= 0);

            hnd.Free();

            Assert.IsTrue(obj_type == H5O.type_t.GROUP);
        }

        /// <summary>
        /// Exercises the native <c>H5Rget_obj_type</c> operation in scenario 3.
        /// </summary>
        [TestMethod]
        public void H5Rget_obj_typeTest3()
        {
            var path =
                                           Encoding.UTF8.GetBytes(String.Join("/", m_utf8strings));
            // make room for the trailling \0
            var name = new byte[path.Length + 1];
            Array.Copy(path, name, path.Length);

            var dims = new hsize_t[] { 10, 20 };
            var space = H5S.create_simple(2, dims, null);
            Assert.IsTrue(space >= 0);
            var dset = H5D.create(m_v0_test_file, name, H5T.STD_I32LE, space,
                m_lcpl_utf8);
            Assert.IsTrue(dset >= 0);
            hsize_t[] start = { 5, 10 };
            hsize_t[] count = { 1, 1 };
            hsize_t[] block = { 2, 4 };
            Assert.IsTrue(
                H5S.select_hyperslab(space, H5S.seloper_t.SET, start, null,
                count, block) >= 0);

            var refer = new byte[H5R.DSET_REG_REF_BUF_SIZE];
            var hnd = GCHandle.Alloc(refer, GCHandleType.Pinned);

            Assert.IsTrue(
                H5R.create(hnd.AddrOfPinnedObject(), m_v0_test_file, name,
                H5R.type_t.DATASET_REGION, space) >= 0);

            var obj_type = H5O.type_t.UNKNOWN;
            Assert.IsTrue(
                H5R.get_obj_type(m_v0_test_file, H5R.type_t.DATASET_REGION,
                hnd.AddrOfPinnedObject(), ref obj_type) >= 0);

            hnd.Free();

            Assert.IsTrue(obj_type == H5O.type_t.DATASET);

            Assert.IsTrue(H5D.close(dset) >= 0);
            Assert.IsTrue(H5S.close(space) >= 0);
        }

        /// <summary>
        /// Exercises the native <c>H5Rget_obj_type</c> operation in scenario 4.
        /// </summary>
        [TestMethod]
        public void H5Rget_obj_typeTest4()
        {
            var path =
                                           Encoding.UTF8.GetBytes(String.Join("/", m_utf8strings));
            // make room for the trailling \0
            var name = new byte[path.Length + 1];
            Array.Copy(path, name, path.Length);

            var dims = new hsize_t[] { 10, 20 };
            var space = H5S.create_simple(2, dims, null);
            Assert.IsTrue(space >= 0);
            var dset = H5D.create(m_v2_test_file, name, H5T.STD_I32LE, space,
                m_lcpl_utf8);
            Assert.IsTrue(dset >= 0);
            hsize_t[] start = { 5, 10 };
            hsize_t[] count = { 1, 1 };
            hsize_t[] block = { 2, 4 };
            Assert.IsTrue(
                H5S.select_hyperslab(space, H5S.seloper_t.SET, start, null,
                count, block) >= 0);

            var refer = new byte[H5R.DSET_REG_REF_BUF_SIZE];
            var hnd = GCHandle.Alloc(refer, GCHandleType.Pinned);

            Assert.IsTrue(
                H5R.create(hnd.AddrOfPinnedObject(), m_v2_test_file, name,
                H5R.type_t.DATASET_REGION, space) >= 0);

            var obj_type = H5O.type_t.UNKNOWN;
            Assert.IsTrue(
                H5R.get_obj_type(m_v2_test_file, H5R.type_t.DATASET_REGION,
                hnd.AddrOfPinnedObject(), ref obj_type) >= 0);

            hnd.Free();

            Assert.IsTrue(obj_type == H5O.type_t.DATASET);

            Assert.IsTrue(H5D.close(dset) >= 0);
            Assert.IsTrue(H5S.close(space) >= 0);
        }
    }
}

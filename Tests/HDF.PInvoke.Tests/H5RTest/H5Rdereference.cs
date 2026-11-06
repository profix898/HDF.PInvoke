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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HDF.PInvoke;

using haddr_t = System.UInt64;
using hsize_t = System.UInt64;
using ssize_t = System.IntPtr;

using hid_t = System.Int64;

namespace UnitTests
{
    /// <summary>
    /// Groups regression checks for the H5R API.
    /// </summary>
    public partial class H5RTest
    {
        /// <summary>
        /// Exercises the native <c>H5Rdereference</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5RdereferenceTest1()
        {
            byte[] path =
                                        Encoding.UTF8.GetBytes(String.Join("/", m_utf8strings));
            // make room for the trailling \0
            byte[] name = new byte[path.Length + 1];
            Array.Copy(path, name, path.Length);

            hid_t gid = H5G.create(m_v0_test_file, path, m_lcpl_utf8);
            Assert.IsTrue(gid >= 0);
            H5O.info_t info = new H5O.info_t();
            Assert.IsTrue(H5O.get_info(gid, ref info) >= 0);
            haddr_t address = info.addr;
            Assert.IsTrue(H5G.close(gid) >= 0);

            byte[] refer = new byte[H5R.OBJ_REF_BUF_SIZE];
            GCHandle hnd = GCHandle.Alloc(refer, GCHandleType.Pinned);

            Assert.IsTrue(
                H5R.create(hnd.AddrOfPinnedObject(), m_v0_test_file, name,
                H5R.type_t.OBJECT, -1) >= 0);

            gid = H5R.dereference(m_v0_test_file, H5P.DEFAULT,
                H5R.type_t.OBJECT, hnd.AddrOfPinnedObject());
            Assert.IsTrue(gid >= 0);

            hnd.Free();

            Assert.IsTrue(H5O.get_info(gid, ref info) >= 0);
            Assert.IsTrue(address == info.addr);
            Assert.IsTrue(H5G.close(gid) >= 0);
        }

        /// <summary>
        /// Exercises the native <c>H5Rdereference</c> operation in scenario 2.
        /// </summary>
        [TestMethod]
        public void H5RdereferenceTest2()
        {
            byte[] path =
                                        Encoding.UTF8.GetBytes(String.Join("/", m_utf8strings));
            // make room for the trailling \0
            byte[] name = new byte[path.Length + 1];
            Array.Copy(path, name, path.Length);

            hid_t gid = H5G.create(m_v2_test_file, path, m_lcpl_utf8);
            Assert.IsTrue(gid >= 0);
            H5O.info_t info = new H5O.info_t();
            Assert.IsTrue(H5O.get_info(gid, ref info) >= 0);
            haddr_t address = info.addr;
            Assert.IsTrue(H5G.close(gid) >= 0);

            byte[] refer = new byte[H5R.OBJ_REF_BUF_SIZE];
            GCHandle hnd = GCHandle.Alloc(refer, GCHandleType.Pinned);

            Assert.IsTrue(
                H5R.create(hnd.AddrOfPinnedObject(), m_v2_test_file, name,
                H5R.type_t.OBJECT, -1) >= 0);

            gid = H5R.dereference(m_v2_test_file, H5P.DEFAULT,
                H5R.type_t.OBJECT, hnd.AddrOfPinnedObject());
            Assert.IsTrue(gid >= 0);

            hnd.Free();

            Assert.IsTrue(H5O.get_info(gid, ref info) >= 0);
            Assert.IsTrue(address == info.addr);
            Assert.IsTrue(H5G.close(gid) >= 0);
        }

        /// <summary>
        /// Exercises the native <c>H5Rdereference</c> operation in scenario 3.
        /// </summary>
        [TestMethod]
        public void H5RdereferenceTest3()
        {
            byte[] path =
                                           Encoding.UTF8.GetBytes(String.Join("/", m_utf8strings));
            // make room for the trailling \0
            byte[] name = new byte[path.Length + 1];
            Array.Copy(path, name, path.Length);

            hsize_t[] dims = new hsize_t[] { 10, 20 };
            hid_t space = H5S.create_simple(2, dims, null);
            Assert.IsTrue(space >= 0);
            hid_t dset = H5D.create(m_v0_test_file, name, H5T.STD_I32LE, space,
                m_lcpl_utf8);

            H5O.info_t info = new H5O.info_t();
            Assert.IsTrue(H5O.get_info(dset, ref info) >= 0);
            haddr_t address = info.addr;
            Assert.IsTrue(H5D.close(dset) >= 0);

            Assert.IsTrue(dset >= 0);
            hsize_t[] start = { 5, 10 };
            hsize_t[] count = { 1, 1 };
            hsize_t[] block = { 2, 4 };
            Assert.IsTrue(
                H5S.select_hyperslab(space, H5S.seloper_t.SET, start, null,
                count, block) >= 0);

            byte[] refer = new byte[H5R.DSET_REG_REF_BUF_SIZE];
            GCHandle hnd = GCHandle.Alloc(refer, GCHandleType.Pinned);

            Assert.IsTrue(
                H5R.create(hnd.AddrOfPinnedObject(), m_v0_test_file, name,
                H5R.type_t.DATASET_REGION, space) >= 0);

            dset = H5R.dereference(m_v0_test_file, H5P.DEFAULT,
                H5R.type_t.DATASET_REGION, hnd.AddrOfPinnedObject());
            Assert.IsTrue(dset >= 0);

            hnd.Free();

            Assert.IsTrue(H5O.get_info(dset, ref info) >= 0);
            Assert.IsTrue(address == info.addr);
            Assert.IsTrue(H5D.close(dset) >= 0);

            Assert.IsTrue(H5S.close(space) >= 0);
        }

        /// <summary>
        /// Exercises the native <c>H5Rdereference</c> operation in scenario 4.
        /// </summary>
        [TestMethod]
        public void H5RdereferenceTest4()
        {
            byte[] path =
                                           Encoding.UTF8.GetBytes(String.Join("/", m_utf8strings));
            // make room for the trailling \0
            byte[] name = new byte[path.Length + 1];
            Array.Copy(path, name, path.Length);

            hsize_t[] dims = new hsize_t[] { 10, 20 };
            hid_t space = H5S.create_simple(2, dims, null);
            Assert.IsTrue(space >= 0);
            hid_t dset = H5D.create(m_v2_test_file, name, H5T.STD_I32LE, space,
                m_lcpl_utf8);

            H5O.info_t info = new H5O.info_t();
            Assert.IsTrue(H5O.get_info(dset, ref info) >= 0);
            haddr_t address = info.addr;
            Assert.IsTrue(H5D.close(dset) >= 0);

            Assert.IsTrue(dset >= 0);
            hsize_t[] start = { 5, 10 };
            hsize_t[] count = { 1, 1 };
            hsize_t[] block = { 2, 4 };
            Assert.IsTrue(
                H5S.select_hyperslab(space, H5S.seloper_t.SET, start, null,
                count, block) >= 0);

            byte[] refer = new byte[H5R.DSET_REG_REF_BUF_SIZE];
            GCHandle hnd = GCHandle.Alloc(refer, GCHandleType.Pinned);

            Assert.IsTrue(
                H5R.create(hnd.AddrOfPinnedObject(), m_v2_test_file, name,
                H5R.type_t.DATASET_REGION, space) >= 0);

            dset = H5R.dereference(m_v2_test_file, H5P.DEFAULT,
                H5R.type_t.DATASET_REGION, hnd.AddrOfPinnedObject());
            Assert.IsTrue(dset >= 0);

            hnd.Free();

            Assert.IsTrue(H5O.get_info(dset, ref info) >= 0);
            Assert.IsTrue(address == info.addr);
            Assert.IsTrue(H5D.close(dset) >= 0);

            Assert.IsTrue(H5S.close(space) >= 0);
        }
    }
}

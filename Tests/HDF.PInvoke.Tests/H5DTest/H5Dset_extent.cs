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
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HDF.PInvoke;

using herr_t = System.Int32;
using hsize_t = System.UInt64;

using hid_t = System.Int64;

namespace UnitTests
{
    public partial class H5DTest
    {
        [TestMethod]
        public void H5Dset_extentTest1()
        {
            hsize_t[] dims = { 10, 10, 10 };
            hsize_t[] max_dims = { H5S.UNLIMITED, H5S.UNLIMITED, H5S.UNLIMITED };
            hsize_t[] new_dims = { 10, 20, 30 };
            hid_t space = H5S.create_simple(3, dims, max_dims);

            hid_t dcpl = H5P.create(H5P.DATASET_CREATE);
            Assert.IsTrue(dcpl >= 0);
            hsize_t[] chunk = { 64, 64, 64 };
            Assert.IsTrue(H5P.set_chunk(dcpl, 3, chunk) >= 0);

            hid_t dset = H5D.create_anon(m_v0_test_file, H5T.IEEE_F32BE,
                space, dcpl);
            Assert.IsTrue(dset >= 0);
            Assert.IsTrue(H5D.set_extent(dset, new_dims) >= 0);
            Assert.IsTrue(H5D.close(dset) >= 0);

            dset = H5D.create_anon(m_v2_test_file, H5T.IEEE_F32BE,
                space, dcpl);
            Assert.IsTrue(dset >= 0);
            Assert.IsTrue(H5D.set_extent(dset, new_dims) >= 0);
            Assert.IsTrue(H5D.close(dset) >= 0);

            Assert.IsTrue(H5P.close(dcpl) >= 0);
            Assert.IsTrue(H5S.close(space) >= 0);
        }

        [TestMethod]
        public void H5Dset_extentTest2()
        {
            hsize_t[] dims = { 10, 10, 10 };
            hsize_t[] max_dims = { H5S.UNLIMITED, H5S.UNLIMITED, H5S.UNLIMITED };
            hid_t space = H5S.create_simple(3, dims, max_dims);

            hid_t dcpl = H5P.create(H5P.DATASET_CREATE);
            Assert.IsTrue(dcpl >= 0);
            hsize_t[] chunk = { 64, 64, 64 };
            Assert.IsTrue(H5P.set_chunk(dcpl, 3, chunk) >= 0);

            hid_t dset = H5D.create_anon(m_v0_test_file, H5T.IEEE_F32BE,
                space, dcpl);
            Assert.IsTrue(dset >= 0);
            Assert.IsFalse(H5D.set_extent(dset, (ulong[])null) >= 0);
            Assert.IsTrue(H5D.close(dset) >= 0);

            dset = H5D.create_anon(m_v2_test_file, H5T.IEEE_F32BE,
                space, dcpl);
            Assert.IsTrue(dset >= 0);
            Assert.IsFalse(H5D.set_extent(dset, (ulong[])null) >= 0);
            Assert.IsTrue(H5D.close(dset) >= 0);

            Assert.IsTrue(H5P.close(dcpl) >= 0);
            Assert.IsTrue(H5S.close(space) >= 0);
        }
    }
}

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
using uint32_t = System.UInt32;
using haddr_t = System.UInt64;
using hsize_t = System.UInt64;

using hid_t = System.Int64;

namespace UnitTests
{
    /// <summary>
    /// Groups regression checks for the H5D API.
    /// </summary>
    public partial class H5DTest
    {
        /// <summary>
        /// Exercises the native <c>H5Dget_chunk_info_by_coord</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5Dget_chunk_info_by_coordTest1()
        {
            hsize_t[] dims = { 10, 10 };
            hsize_t[] max_dims = { H5S.UNLIMITED, H5S.UNLIMITED };
            hid_t space = H5S.create_simple(2, dims, max_dims);

            hid_t dcpl = H5P.create(H5P.DATASET_CREATE);
            Assert.IsTrue(dcpl >= 0);
            hsize_t[] chunk = { 4, 4 };
            Assert.IsTrue(H5P.set_chunk(dcpl, 2, chunk) >= 0);
            Assert.IsTrue(H5P.set_alloc_time(dcpl, H5D.alloc_time_t.EARLY) >= 0);
            Assert.IsTrue(H5P.set_fill_time(dcpl, H5D.fill_time_t.ALLOC) >= 0);

            hid_t dset = H5D.create(m_v0_test_file, "Early Bird2", H5T.IEEE_F32BE,
                space, H5P.DEFAULT, dcpl);
            Assert.IsTrue(dset >= 0);

            hsize_t size = 0;
            hsize_t[] offset = { 1, 2 };
            uint32_t filter_mask = 0;
            haddr_t addr = 0;
            Assert.IsTrue(H5D.get_chunk_info_by_coord(dset, offset, ref filter_mask, ref addr, ref size) >= 0);
            Assert.IsTrue(filter_mask == 0 && size > 0 && addr > 0);

            Assert.IsTrue(H5D.close(dset) >= 0);

            dset = H5D.create(m_v2_test_file, "Early Bird2", H5T.IEEE_F32BE,
                space, H5P.DEFAULT, dcpl);
            Assert.IsTrue(dset >= 0);

            Assert.IsTrue(H5D.get_chunk_info_by_coord(dset, offset, ref filter_mask, ref addr, ref size) >= 0);
            Assert.IsTrue(filter_mask == 0 && size > 0 && addr > 0);

            Assert.IsTrue(H5D.close(dset) >= 0);
        }

        /// <summary>
        /// Exercises the native <c>H5Dget_chunk_info_by_coord</c> operation in scenario 2.
        /// </summary>
        [TestMethod]
        public void H5Dget_chunk_info_by_coordTest2()
        {
            hsize_t[] dims = { 10, 10 };
            hsize_t[] max_dims = { H5S.UNLIMITED, H5S.UNLIMITED };
            hid_t space = H5S.create_simple(2, dims, max_dims);

            hid_t dcpl = H5P.create(H5P.DATASET_CREATE);
            Assert.IsTrue(dcpl >= 0);
            hsize_t[] chunk = { 4, 4 };
            Assert.IsTrue(H5P.set_chunk(dcpl, 2, chunk) >= 0);

            hid_t dset = H5D.create(m_v0_test_file, "Early Bird3", H5T.IEEE_F32BE,
                space, H5P.DEFAULT, dcpl);
            Assert.IsTrue(dset >= 0);

            hsize_t size = 100;
            hsize_t[] offset = { 1, 2 };
            uint32_t filter_mask = 0;
            haddr_t addr = 0;
            Assert.IsTrue(H5D.get_chunk_info_by_coord(dset, offset, ref filter_mask, ref addr, ref size) >= 0);
            Assert.IsTrue(filter_mask == 0 && size == 0 && addr == H5.HADDR_UNDEF);

            Assert.IsTrue(H5D.close(dset) >= 0);

            dset = H5D.create(m_v2_test_file, "Early Bird3", H5T.IEEE_F32BE,
                space, H5P.DEFAULT, dcpl);
            Assert.IsTrue(dset >= 0);

            size = 100;
            addr = 0;
            Assert.IsTrue(H5D.get_chunk_info_by_coord(dset, offset, ref filter_mask, ref addr, ref size) >= 0);
            Assert.IsTrue(filter_mask == 0 && size == 0 && addr == H5.HADDR_UNDEF);

            Assert.IsTrue(H5D.close(dset) >= 0);
        }

    }
}

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
    public partial class H5DTest
    {

        [TestMethod]
        public void H5Dget_chunk_infoTest1()
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

            hid_t dset = H5D.create(m_v0_test_file, "Early Bird1", H5T.IEEE_F32BE,
                space, H5P.DEFAULT, dcpl);
            Assert.IsTrue(dset >= 0);

            // This should work but doesn't:
            // Assert.IsTrue(H5D.get_num_chunks(dset, H5S.ALL, ref nchunks) >= 0);
            hid_t fspace = H5D.get_space(dset);
            Assert.IsTrue(fspace >= 0);
            Assert.IsTrue(H5S.select_all(fspace) >= 0);

            hsize_t index = 8, size = 0;
            hsize_t[] offset = { 4711, 4712 };
            uint32_t filter_mask = 0;
            haddr_t addr = 0;
            Assert.IsTrue(H5D.get_chunk_info(dset, fspace, index, offset, ref filter_mask, ref addr, ref size) >= 0);
            Assert.IsTrue(offset[0] > 0);
            Assert.IsTrue(filter_mask == 0 && size > 0 && addr > 0);

            Assert.IsTrue(H5D.close(dset) >= 0);

            dset = H5D.create(m_v2_test_file, "Early Bird1", H5T.IEEE_F32BE,
                space, H5P.DEFAULT, dcpl);
            Assert.IsTrue(dset >= 0);

            // This should work but doesn't:
            // Assert.IsTrue(H5D.get_num_chunks(dset, H5S.ALL, ref nchunks) >= 0);
            fspace = H5D.get_space(dset);
            Assert.IsTrue(fspace >= 0);
            Assert.IsTrue(H5S.select_all(fspace) >= 0);

            Assert.IsTrue(H5D.get_chunk_info(dset, fspace, index, offset, ref filter_mask, ref addr, ref size) >= 0);
            Assert.IsTrue(offset[0] > 0);
            Assert.IsTrue(filter_mask == 0 && size > 0 && addr > 0);

            Assert.IsTrue(H5D.close(dset) >= 0);
        }

    }
}

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
using HDF.PInvoke;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using herr_t = System.Int32;
using hid_t = System.Int64;
using hsize_t = System.UInt64;

namespace UnitTests
{
    /// <summary>
    /// Groups regression checks for the H5D API.
    /// </summary>
    public partial class H5DTest
    {
        /// <summary>
        /// Exercises the native <c>H5Dget_num_chunks</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5Dget_num_chunksTest1()
        {
            hsize_t[] dims = { 10, 10 };
            hsize_t[] max_dims = { H5S.UNLIMITED, H5S.UNLIMITED };
            var space = H5S.create_simple(2, dims, max_dims);

            var dcpl = H5P.create(H5P.DATASET_CREATE);
            Assert.IsTrue(dcpl >= 0);
            hsize_t[] chunk = { 4, 4 };
            Assert.IsTrue(H5P.set_chunk(dcpl, 2, chunk) >= 0);
            Assert.IsTrue(H5P.set_alloc_time(dcpl, H5D.alloc_time_t.EARLY) >= 0);
            Assert.IsTrue(H5P.set_fill_time(dcpl, H5D.fill_time_t.ALLOC) >= 0);

            var dset = H5D.create(m_v0_test_file, "Early Bird", H5T.IEEE_F32BE,
                space, H5P.DEFAULT, dcpl);
            Assert.IsTrue(dset >= 0);

            // This should work but doesn't:
            // Assert.IsTrue(H5D.get_num_chunks(dset, H5S.ALL, ref nchunks) >= 0);
            var fspace = H5D.get_space(dset);
            Assert.IsTrue(fspace >= 0);
            Assert.IsTrue(H5S.select_all(fspace) >= 0);

            hsize_t nchunks = 0;
            Assert.IsTrue(H5D.get_num_chunks(dset, fspace, ref nchunks) >= 0);
            Assert.IsTrue(nchunks == 9);

            Assert.IsTrue(H5D.close(dset) >= 0);

            dset = H5D.create(m_v2_test_file, "Early Bird", H5T.IEEE_F32BE,
                space, H5P.DEFAULT, dcpl);
            Assert.IsTrue(dset >= 0);

            // This should work but doesn't:
            // Assert.IsTrue(H5D.get_num_chunks(dset, H5S.ALL, ref nchunks) >= 0);
            fspace = H5D.get_space(dset);
            Assert.IsTrue(fspace >= 0);
            Assert.IsTrue(H5S.select_all(fspace) >= 0);

            nchunks = 0;
            Assert.IsTrue(H5D.get_num_chunks(dset, fspace, ref nchunks) >= 0);
            Assert.IsTrue(nchunks == 9);

            Assert.IsTrue(H5D.close(dset) >= 0);
        }

    }
}

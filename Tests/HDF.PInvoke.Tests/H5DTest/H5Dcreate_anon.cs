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
        /// Exercises the native <c>H5Dcreate_anon</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5Dcreate_anonTest1()
        {
            hsize_t[] dims = { 1024, 2048 };
            var space = H5S.create_simple(3, dims, null);

            var dset = H5D.create_anon(m_v0_test_file, H5T.STD_I16LE, space);
            Assert.IsTrue(dset >= 0);
            Assert.IsTrue(H5D.close(dset) >= 0);
            dset = H5D.create_anon(m_v2_test_file, H5T.STD_I16LE, space);
            Assert.IsTrue(dset >= 0);
            Assert.IsTrue(H5D.close(dset) >= 0);

            Assert.IsTrue(H5S.close(space) >= 0);
        }

        /// <summary>
        /// Exercises the native <c>H5Dcreate_anon</c> operation in scenario 2.
        /// </summary>
        [TestMethod]
        public void H5Dcreate_anonTest2()
        {
            hsize_t[] dims = { 10, 10, 10 };
            hsize_t[] max_dims = { H5S.UNLIMITED, H5S.UNLIMITED, H5S.UNLIMITED };
            var space = H5S.create_simple(3, dims, max_dims);

            var dcpl = H5P.create(H5P.DATASET_CREATE);
            Assert.IsTrue(dcpl >= 0);
            hsize_t[] chunk = { 64, 64, 64 };
            Assert.IsTrue(H5P.set_chunk(dcpl, 3, chunk) >= 0);
            Assert.IsTrue(H5P.set_deflate(dcpl, 9) >= 0);

            var dset = H5D.create_anon(m_v0_test_file, H5T.IEEE_F32BE,
                space, dcpl);
            Assert.IsTrue(dset >= 0);
            Assert.IsTrue(H5D.close(dset) >= 0);

            dset = H5D.create_anon(m_v2_test_file, H5T.IEEE_F32BE,
                space, dcpl);
            Assert.IsTrue(dset >= 0);
            Assert.IsTrue(H5D.close(dset) >= 0);

            Assert.IsTrue(H5P.close(dcpl) >= 0);
            Assert.IsTrue(H5S.close(space) >= 0);
        }
    }
}

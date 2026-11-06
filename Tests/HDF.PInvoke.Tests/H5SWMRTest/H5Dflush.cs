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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HDF.PInvoke;

using herr_t = System.Int32;
using hsize_t = System.UInt64;

using hid_t = System.Int64;

namespace UnitTests
{
    /// <summary>
    /// Groups regression checks for the H5SWMR API.
    /// </summary>
    public partial class H5SWMRTest
    {
        /// <summary>
        /// Exercises the native <c>H5Dflush</c> operation in scenario SWMR1.
        /// </summary>
        [TestMethod]
        public void H5DflushTestSWMR1()
        {
            hsize_t[] dims = { 6, 6 };
            hsize_t[] maxdims = { 6, H5S.UNLIMITED };
            hsize_t[] chunk_dims = { 2, 5 };
            int[] cbuf = new int[36];

            hid_t dsp = H5S.create_simple(2, dims, maxdims);
            Assert.IsTrue(dsp >= 0);

            hid_t dcpl = H5P.create(H5P.DATASET_CREATE);
            Assert.IsTrue(dcpl >= 0);
            Assert.IsTrue(H5P.set_chunk(dcpl, 2, chunk_dims) >= 0);

            hid_t dst = H5D.create(m_v3_test_file_no_swmr, "dset",
                H5T.NATIVE_INT, dsp, H5P.DEFAULT, dcpl);
            Assert.IsTrue(dst >= 0);

            GCHandle hnd = GCHandle.Alloc(cbuf, GCHandleType.Pinned);

            Assert.IsTrue(H5D.write(dst, H5T.NATIVE_INT, H5S.ALL, H5S.ALL,
                H5P.DEFAULT, hnd.AddrOfPinnedObject()) >= 0);

            hnd.Free();

            Assert.IsTrue(H5D.flush(dst) >= 0);

            Assert.IsTrue(H5D.close(dst) >= 0);
            Assert.IsTrue(H5P.close(dcpl) >= 0);
            Assert.IsTrue(H5S.close(dsp) >= 0);
        }

        /// <summary>
        /// Exercises the native <c>H5Dflush</c> operation in scenario SWMR2.
        /// </summary>
        [TestMethod]
        public void H5DflushTestSWMR2()
        {
            hsize_t[] dims = { 6, 6 };
            hsize_t[] maxdims = { 6, H5S.UNLIMITED };
            hsize_t[] chunk_dims = { 2, 5 };
            int[] cbuf = new int[36];

            hid_t dsp = H5S.create_simple(2, dims, maxdims);
            Assert.IsTrue(dsp >= 0);

            hid_t dcpl = H5P.create(H5P.DATASET_CREATE);
            Assert.IsTrue(dcpl >= 0);
            Assert.IsTrue(H5P.set_chunk(dcpl, 2, chunk_dims) >= 0);

            hid_t dst = H5D.create(m_v3_test_file_swmr, "dset",
                H5T.NATIVE_INT, dsp, H5P.DEFAULT, dcpl);
            Assert.IsTrue(dst >= 0);

            GCHandle hnd = GCHandle.Alloc(cbuf, GCHandleType.Pinned);

            Assert.IsTrue(H5D.write(dst, H5T.NATIVE_INT, H5S.ALL, H5S.ALL,
                H5P.DEFAULT, hnd.AddrOfPinnedObject()) >= 0);

            hnd.Free();

            Assert.IsTrue(H5D.flush(dst) >= 0);

            Assert.IsTrue(H5D.close(dst) >= 0);
            Assert.IsTrue(H5P.close(dcpl) >= 0);
            Assert.IsTrue(H5S.close(dsp) >= 0);
        }
    }
}

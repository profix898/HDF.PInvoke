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
        /// Exercises the native <c>H5Pset_object_flush_cb</c> operation in scenario SWMR1.
        /// </summary>
        [TestMethod]
        public void H5Pset_object_flush_cbTestSWMR1()
        {
            hid_t fapl = H5P.create(H5P.FILE_ACCESS);
            Assert.IsTrue(fapl >= 0);

            H5F.flush_cb_t cb = flush_func;

            Assert.IsTrue(
                H5P.set_object_flush_cb(fapl, cb, IntPtr.Zero) >= 0);

            Assert.IsTrue(H5P.close(fapl) >= 0);
        }

        /// <summary>
        /// Exercises the native <c>H5Pset_object_flush_cb</c> operation in scenario SWMR2.
        /// </summary>
        [TestMethod]
        public void H5Pset_object_flush_cbTestSWMR2()
        {
            hid_t fapl = H5P.create(H5P.FILE_ACCESS);
            Assert.IsTrue(fapl >= 0);

            H5F.flush_cb_t cb = flush_func;

            Assert.IsTrue(
                H5P.set_object_flush_cb(fapl, cb, IntPtr.Zero) >= 0);

            H5F.flush_cb_t check_cb = null;

            IntPtr check_ptr = new IntPtr(4711);

            Assert.IsTrue(
                H5P.get_object_flush_cb(fapl, ref check_cb,
                ref check_ptr) >= 0);

            Assert.IsTrue(check_cb == cb);

            Assert.IsTrue(check_ptr == IntPtr.Zero);

            Assert.IsTrue(H5P.close(fapl) >= 0);
        }
    }
}

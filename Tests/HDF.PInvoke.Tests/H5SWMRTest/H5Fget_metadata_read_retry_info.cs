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
using HDF.PInvoke;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using herr_t = System.Int32;
using hid_t = System.Int64;

namespace UnitTests
{
    /// <summary>
    /// Groups regression checks for the H5SWMR API.
    /// </summary>
    public partial class H5SWMRTest
    {
        /// <summary>
        /// Exercises the native <c>H5Fget_metadata_read_retry_info</c> operation in scenario SWMR1.
        /// </summary>
        [TestMethod]
        public void H5Fget_metadata_read_retry_infoTestSWMR1()
        {
            var info = new H5F.retry_info_t();

            info.retries0 = new IntPtr(10);
            info.retries6 = new IntPtr(60);
            info.retries14 = new IntPtr(140);
            info.retries20 = new IntPtr(200);

            Assert.IsTrue(
                H5F.get_metadata_read_retry_info(m_v3_test_file_swmr,
                ref info) >= 0);

            Assert.IsTrue(info.nbins == 2);

            Assert.IsTrue(info.retries0 == IntPtr.Zero);
            Assert.IsTrue(info.retries6 == IntPtr.Zero);
            Assert.IsTrue(info.retries14 == IntPtr.Zero);
            Assert.IsTrue(info.retries20 == IntPtr.Zero);
        }
    }
}

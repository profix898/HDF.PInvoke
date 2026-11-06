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

using hid_t = System.Int64;

namespace UnitTests
{
    public partial class H5SWMRTest
    {
        [TestMethod]
        public void H5Fstart_swmr_writeTestSWMR1()
        {
            Assert.IsFalse(H5F.start_swmr_write(m_v3_test_file_swmr) >= 0);
        }

        [TestMethod]
        public void H5Fstart_swmr_writeTestSWMR2()
        {
            Assert.IsTrue(H5F.start_swmr_write(m_v3_test_file_no_swmr) >= 0);
            uint intent = 0;
            Assert.IsTrue(
                H5F.get_intent(m_v3_test_file_no_swmr, ref intent) >= 0);
            Assert.IsTrue((intent & H5F.ACC_SWMR_WRITE) > 0);
        }
    }
}

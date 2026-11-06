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
        public void H5Fget_metadata_read_attemptsTestSWMR1()
        {
            hid_t fapl = H5F.get_access_plist(m_v3_test_file_swmr);
            Assert.IsTrue(fapl >= 0);

            uint attempts = 0;
            Assert.IsTrue(
                H5P.get_metadata_read_attempts(fapl, ref attempts) >= 0);
            Assert.IsTrue(attempts > 0);

            Assert.IsTrue(H5P.close(fapl) >= 0);
        }

        [TestMethod]
        public void H5Fget_metadata_read_attemptsTestSWMR2()
        {
            hid_t fapl = H5F.get_access_plist(m_v3_test_file_no_swmr);
            Assert.IsTrue(fapl >= 0);

            uint attempts = 0;
            Assert.IsTrue(
                H5P.get_metadata_read_attempts(fapl, ref attempts) >= 0);
            Assert.IsTrue(attempts > 0);

            Assert.IsTrue(H5P.close(fapl) >= 0);
        }
    }
}

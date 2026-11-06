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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HDF.PInvoke;

using hid_t = System.Int64;

namespace UnitTests
{
    public partial class H5GTest
    {
        [TestMethod]
        public void H5Fclear_elink_cacheTest1()
        {
            Assert.IsTrue(H5F.clear_elink_file_cache(m_v0_class_file) >= 0);
            Assert.IsTrue(H5F.clear_elink_file_cache(m_v2_class_file) >= 0);
            Assert.IsTrue(H5F.clear_elink_file_cache(m_v0_test_file) >= 0);
            Assert.IsTrue(H5F.clear_elink_file_cache(m_v2_test_file) >= 0);
        }

        [TestMethod]
        public void H5Fclear_elink_cacheTest2()
        {
            Assert.IsFalse(
                                        H5F.clear_elink_file_cache(Utilities.RandomInvalidHandle()) >= 0);
        }
    }
}

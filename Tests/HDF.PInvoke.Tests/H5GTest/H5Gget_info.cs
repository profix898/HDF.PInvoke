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
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HDF.PInvoke;

using hid_t = System.Int64;

namespace UnitTests
{
    public partial class H5GTest
    {
        [TestMethod]
        public void H5Gget_infoTest1()
        {
            H5G.info_t info = new H5G.info_t();
            Assert.IsTrue(H5G.get_info(m_v0_class_file, ref info) >= 0);
            Assert.IsTrue(H5G.get_info(m_v0_test_file, ref info) >= 0);
            Assert.IsTrue(H5G.get_info(m_v2_class_file, ref info) >= 0);
            Assert.IsTrue(H5G.get_info(m_v2_test_file, ref info) >= 0);
        }

        [TestMethod]
        public void H5Gget_infoTest2()
        {
            H5G.info_t info = new H5G.info_t();
            hid_t gid = Utilities.RandomInvalidHandle();
            Assert.IsTrue(H5G.get_info(gid, ref info) < 0);
        }
    }
}

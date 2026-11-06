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
        public void H5Gget_create_plistTest1()
        {
            hid_t gid = H5G.create(m_v0_test_file, "A");
            Assert.IsTrue(gid > 0);

            hid_t gcpl = H5G.get_create_plist(gid);
            Assert.IsTrue(gcpl > 0);

            hid_t gid1 = H5G.create_anon(gid);
            Assert.IsTrue(gid1 > 0);

            hid_t gcpl1 = H5G.get_create_plist(gid1);
            Assert.IsTrue(gcpl1 > 0);

            Assert.IsTrue(H5P.close(gcpl1) >= 0);
            Assert.IsTrue(H5P.close(gcpl) >= 0);
            Assert.IsTrue(H5G.close(gid1) >= 0);
            Assert.IsTrue(H5G.close(gid) >= 0);

            gid = H5G.create(m_v2_test_file, "A");
            Assert.IsTrue(gid > 0);

            gcpl = H5G.get_create_plist(gid);
            Assert.IsTrue(gcpl > 0);

            gid1 = H5G.create_anon(gid);
            Assert.IsTrue(gid1 > 0);

            gcpl1 = H5G.get_create_plist(gid1);
            Assert.IsTrue(gcpl1 > 0);

            Assert.IsTrue(H5P.close(gcpl1) >= 0);
            Assert.IsTrue(H5P.close(gcpl) >= 0);
            Assert.IsTrue(H5G.close(gid1) >= 0);
            Assert.IsTrue(H5G.close(gid) >= 0);
        }

        [TestMethod]
        public void H5Gget_create_plistTest2()
        {
            hid_t group = Utilities.RandomInvalidHandle();
            hid_t gcpl = H5G.get_create_plist(group);
            Assert.IsTrue(gcpl < 0);
        }
    }
}

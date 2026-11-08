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
using HDF.PInvoke;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using hid_t = System.Int64;

namespace UnitTests
{
    /// <summary>
    /// Groups regression checks for the H5G API.
    /// </summary>
    public partial class H5GTest
    {
        /// <summary>
        /// Exercises the native <c>H5Gget_create_plist</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5Gget_create_plistTest1()
        {
            var gid = H5G.create(m_v0_test_file, "A");
            Assert.IsTrue(gid > 0);

            var gcpl = H5G.get_create_plist(gid);
            Assert.IsTrue(gcpl > 0);

            var gid1 = H5G.create_anon(gid);
            Assert.IsTrue(gid1 > 0);

            var gcpl1 = H5G.get_create_plist(gid1);
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

        /// <summary>
        /// Exercises the native <c>H5Gget_create_plist</c> operation in scenario 2.
        /// </summary>
        [TestMethod]
        public void H5Gget_create_plistTest2()
        {
            var group = Utilities.RandomInvalidHandle();
            var gcpl = H5G.get_create_plist(group);
            Assert.IsTrue(gcpl < 0);
        }
    }
}

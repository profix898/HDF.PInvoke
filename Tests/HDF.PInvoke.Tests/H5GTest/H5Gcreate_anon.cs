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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HDF.PInvoke;

using hid_t = System.Int64;

namespace UnitTests
{
    /// <summary>
    /// Groups regression checks for the H5G API.
    /// </summary>
    public partial class H5GTest
    {
        /// <summary>
        /// Exercises the native <c>H5Gcreate_anon</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5Gcreate_anonTest1()
        {
            hid_t gid = H5G.create_anon(m_v0_test_file);
            Assert.IsTrue(gid > 0);

            hid_t gid1 = H5G.create_anon(gid);
            Assert.IsTrue(gid1 > 0);

            Assert.IsTrue(H5G.close(gid1) >= 0);
            Assert.IsTrue(H5G.close(gid) >= 0);

            gid = H5G.create_anon(m_v2_test_file);
            Assert.IsTrue(gid > 0);

            gid1 = H5G.create_anon(gid);
            Assert.IsTrue(gid1 > 0);

            Assert.IsTrue(H5G.close(gid1) >= 0);
            Assert.IsTrue(H5G.close(gid) >= 0);
        }

        /// <summary>
        /// Exercises the native <c>H5Gcreate_anon</c> operation in scenario 2.
        /// </summary>
        [TestMethod]
        public void H5Gcreate_anonTest2()
        {
            hid_t file = Utilities.RandomInvalidHandle();
            hid_t gid = H5G.create_anon(file);
            Assert.IsTrue(gid < 0);
        }
    }
}

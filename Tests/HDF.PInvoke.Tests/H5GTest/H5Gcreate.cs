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
        /// Exercises the native <c>H5Gcreate</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5GcreateTest1()
        {
            hid_t gid = H5G.create(m_v0_test_file, "A");
            Assert.IsTrue(gid > 0);

            hid_t gid1 = H5G.create(gid, "B");
            Assert.IsTrue(gid1 > 0);

            Assert.IsTrue(H5G.close(gid1) >= 0);
            Assert.IsTrue(H5G.close(gid) >= 0);

            gid = H5G.create(m_v2_test_file, "A");
            Assert.IsTrue(gid > 0);

            gid1 = H5G.create(gid, "B");
            Assert.IsTrue(gid1 > 0);

            Assert.IsTrue(H5G.close(gid1) >= 0);
            Assert.IsTrue(H5G.close(gid) >= 0);
        }

        /// <summary>
        /// Exercises the native <c>H5Gcreate</c> operation in scenario 2.
        /// </summary>
        [TestMethod]
        public void H5GcreateTest2()
        {
            hid_t file = Utilities.RandomInvalidHandle();
            hid_t gid = H5G.create(file, "A");
            Assert.IsTrue(gid < 0);
        }

        /// <summary>
        /// Exercises the native <c>H5Gcreate</c> operation in scenario 3.
        /// </summary>
        [TestMethod]
        public void H5GcreateTest3()
        {
            hid_t lcpl = H5P.create(H5P.LINK_CREATE);
            Assert.IsTrue(lcpl >= 0);
            Assert.IsTrue(H5P.set_create_intermediate_group(lcpl, 1) >= 0);
            hid_t gid = H5G.create(m_v0_test_file, "A/B/C/D/E/F/G/H", lcpl);
            Assert.IsTrue(gid > 0);
            Assert.IsTrue(H5G.close(gid) >= 0);
            hid_t gid1 = H5G.create(m_v2_test_file, "A/B/C/D/E/F/G/H", lcpl);
            Assert.IsTrue(gid1 > 0);
            Assert.IsTrue(H5G.close(gid1) >= 0);
            Assert.IsTrue(H5P.close(lcpl) >= 0);
        }
    }
}

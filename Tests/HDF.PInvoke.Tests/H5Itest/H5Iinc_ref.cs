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
using HDF.PInvoke;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using hid_t = System.Int64;

namespace UnitTests
{
    /// <summary>
    /// Groups regression checks for the H5I API.
    /// </summary>
    public partial class H5ITest
    {
        /// <summary>
        /// Exercises the native <c>H5Iinc_ref</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5Iinc_refTest1()
        {
            var gid = H5G.create(m_v0_test_file, "A");
            Assert.IsTrue(gid > 0);
            Assert.IsTrue(H5I.inc_ref(gid) >= 0);
            Assert.IsTrue(H5G.close(gid) >= 0);

            gid = H5G.create(m_v2_test_file, "A");
            Assert.IsTrue(gid > 0);
            Assert.IsTrue(H5I.inc_ref(gid) >= 0);
            Assert.IsTrue(H5G.close(gid) >= 0);
        }

        /// <summary>
        /// Exercises the native <c>H5Iinc_ref</c> operation in scenario 2.
        /// </summary>
        [TestMethod]
        public void H5Iinc_refTest2()
        {
            var gid = H5G.create(m_v0_test_file, "A");
            Assert.IsTrue(gid > 0);
            Assert.IsTrue(H5I.inc_ref(gid) >= 0);
            Assert.IsTrue(H5I.dec_ref(gid) >= 0);
            Assert.IsTrue(H5G.close(gid) >= 0);

            gid = H5G.create(m_v2_test_file, "A");
            Assert.IsTrue(gid > 0);
            Assert.IsTrue(H5I.inc_ref(gid) >= 0);
            Assert.IsTrue(H5I.dec_ref(gid) >= 0);
            Assert.IsTrue(H5G.close(gid) >= 0);
        }

        /// <summary>
        /// Exercises the native <c>H5Iinc_ref</c> operation in scenario 3.
        /// </summary>
        [TestMethod]
        public void H5Iinc_refTest3()
        {
            Assert.IsFalse(H5I.inc_ref(Utilities.RandomInvalidHandle()) >= 0);
        }
    }
}

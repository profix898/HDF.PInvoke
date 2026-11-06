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
    public partial class H5ITest
    {
        [TestMethod]
        public void H5Idec_refTest1()
        {
            hid_t gid = H5G.create(m_v0_test_file, "A");
            Assert.IsTrue(gid > 0);
            Assert.IsTrue(H5I.dec_ref(gid) >= 0);
            Assert.IsFalse(H5G.close(gid) >= 0);

            gid = H5G.create(m_v2_test_file, "A");
            Assert.IsTrue(gid > 0);
            Assert.IsTrue(H5I.dec_ref(gid) >= 0);
            Assert.IsFalse(H5G.close(gid) >= 0);
        }

        [TestMethod]
        public void H5Idec_refTest2()
        {
            hid_t gid = H5G.create(m_v0_test_file, "A");
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

        [TestMethod]
        public void H5Idec_refTest3()
        {
            Assert.IsFalse(H5I.dec_ref(Utilities.RandomInvalidHandle()) >= 0);
        }
    }
}

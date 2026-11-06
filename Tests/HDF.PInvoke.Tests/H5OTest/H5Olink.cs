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
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HDF.PInvoke;

using hid_t = System.Int64;

namespace UnitTests
{
    public partial class H5OTest
    {
        [TestMethod]
        public void H5OlinkTest1()
        {
            hid_t gid = H5G.create_anon(m_v0_test_file);
            Assert.IsTrue(gid >= 0);

            Assert.IsTrue(H5O.link(gid, m_v0_test_file, "A/B/C", m_lcpl) >= 0);

            Assert.IsTrue(H5G.close(gid) >= 0);

            gid = H5G.create_anon(m_v2_test_file);
            Assert.IsTrue(gid >= 0);

            Assert.IsTrue(H5O.link(gid, m_v2_test_file, "A/B/C", m_lcpl) >= 0);

            Assert.IsTrue(H5G.close(gid) >= 0);
        }

        [TestMethod]
        public void H5OlinkTest2()
        {
            Assert.IsFalse(
                                        H5O.link(Utilities.RandomInvalidHandle(), m_v0_test_file,
                                        "A/B/C", m_lcpl) >= 0);
        }
    }
}

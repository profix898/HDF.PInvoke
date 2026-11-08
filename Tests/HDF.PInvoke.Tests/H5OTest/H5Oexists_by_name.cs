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
using HDF.PInvoke;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using hid_t = System.Int64;

namespace UnitTests
{
    /// <summary>
    /// Groups regression checks for the H5O API.
    /// </summary>
    public partial class H5OTest
    {
        /// <summary>
        /// Exercises the native <c>H5Oexists_by_name</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5Oexists_by_nameTest1()
        {
            Assert.IsTrue(H5L.create_soft("/oh my",
                                        m_v0_test_file, "AA") >= 0);

            var gid = H5G.create(m_v0_test_file, "A/B/C", m_lcpl);
            Assert.IsTrue(gid >= 0);

            Assert.IsTrue(H5O.exists_by_name(m_v0_test_file, "A/B") > 0);

            Assert.IsTrue(H5O.exists_by_name(m_v0_test_file, "AA") == 0);

            Assert.IsTrue(
                H5O.exists_by_name(m_v0_test_file, "A/B/Caesar") == 0);

            Assert.IsTrue(H5G.close(gid) >= 0);

            Assert.IsTrue(H5L.create_soft("/oh my",
                m_v2_test_file, "AA") >= 0);

            gid = H5G.create(m_v2_test_file, "A/B/C", m_lcpl);
            Assert.IsTrue(gid >= 0);

            Assert.IsTrue(H5O.exists_by_name(m_v2_test_file, "A/B") > 0);

            Assert.IsTrue(H5O.exists_by_name(m_v2_test_file, "AA") == 0);

            Assert.IsTrue(
                H5O.exists_by_name(m_v2_test_file, "A/B/Caesar") == 0);

            Assert.IsTrue(H5G.close(gid) >= 0);
        }

        /// <summary>
        /// Exercises the native <c>H5Oexists_by_name</c> operation in scenario 2.
        /// </summary>
        [TestMethod]
        public void H5Oexists_by_nameTest2()
        {
            Assert.IsFalse(
                                        H5O.exists_by_name(Utilities.RandomInvalidHandle(),
                                        (string)null) >= 0);
        }
    }
}

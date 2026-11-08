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
    /// Groups regression checks for the H5L API.
    /// </summary>
    public partial class H5LTest
    {
        /// <summary>
        /// Exercises the native <c>H5Lcopy</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5LcopyTest1()
        {
            var gid = H5G.create(m_v0_test_file, "A/B/C", m_lcpl);
            Assert.IsTrue(gid >= 0);

            Assert.IsTrue(H5L.copy(m_v0_test_file, "A", gid, "A_copy") >= 0);
            Assert.IsTrue(H5L.exists(m_v0_test_file, "A/B/C/A_copy/B") > 0);

            Assert.IsTrue(H5G.close(gid) >= 0);

            gid = H5G.create(m_v2_test_file, "A/B/C", m_lcpl);
            Assert.IsTrue(gid >= 0);

            Assert.IsTrue(H5L.copy(m_v2_test_file, "A", gid, "A_copy") >= 0);
            Assert.IsTrue(H5L.exists(m_v2_test_file, "A/B/C/A_copy/B") > 0);

            Assert.IsTrue(H5G.close(gid) >= 0);
        }

        /// <summary>
        /// Exercises the native <c>H5Lcopy</c> operation in scenario 2.
        /// </summary>
        [TestMethod]
        public void H5LcopyTest2()
        {
            Assert.IsTrue(H5L.create_soft("/my/fantastic/path",
                                                                m_v0_test_file, "A") >= 0);

            // copy symlink to the other test file

            Assert.IsTrue(H5L.copy(m_v0_test_file, "A", m_v2_test_file,
                "C/B/A_copy", m_lcpl) >= 0);
        }

        /// <summary>
        /// Exercises the native <c>H5Lcopy</c> operation in scenario 3.
        /// </summary>
        [TestMethod]
        public void H5LcopyTest3()
        {
            var gid = H5G.create(m_v0_test_file, "A/B/C", m_lcpl);
            Assert.IsTrue(gid >= 0);

            for (var i = 0; i < m_utf8strings.Length; ++i)
            {
                Assert.IsTrue(
                                                                                    H5L.copy(m_v0_test_file, Encoding.ASCII.GetBytes("A" + "\0"), gid,
                                                                                    Encoding.UTF8.GetBytes(m_utf8strings[i] + "\0"),
                                                                                    m_lcpl_utf8) >= 0);

                var path = "A/B/C/" + m_utf8strings[i] + "/B";
                Assert.IsTrue(
                    H5L.exists(m_v0_test_file, Encoding.UTF8.GetBytes(path + "\0")) > 0);
            }

            Assert.IsTrue(H5G.close(gid) >= 0);

            gid = H5G.create(m_v2_test_file, "A/B/C", m_lcpl);
            Assert.IsTrue(gid >= 0);

            for (var i = 0; i < m_utf8strings.Length; ++i)
            {
                Assert.IsTrue(
                                                                                    H5L.copy(m_v2_test_file, Encoding.ASCII.GetBytes("A" + "\0"), gid,
                                                                                    Encoding.UTF8.GetBytes(m_utf8strings[i] + "\0"),
                                                                                    m_lcpl_utf8) >= 0);

                var path = "A/B/C/" + m_utf8strings[i] + "/B";
                Assert.IsTrue(
                    H5L.exists(m_v2_test_file, Encoding.UTF8.GetBytes(path + "\0")) > 0);
            }

            Assert.IsTrue(H5G.close(gid) >= 0);
        }
    }
}

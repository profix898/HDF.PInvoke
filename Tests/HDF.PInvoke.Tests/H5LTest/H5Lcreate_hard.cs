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
        /// Exercises the native <c>H5Lcreate_hard</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5Lcreate_hardTest1()
        {
            Assert.IsTrue(H5G.create(m_v0_test_file, "A/B/C/D", m_lcpl) >= 0);
            Assert.IsTrue(
                H5L.create_hard(m_v0_test_file, "A/B/C/D", m_v0_test_file,
                "shortcut") >= 0);

            Assert.IsTrue(H5G.create(m_v2_test_file, "A/B/C/D", m_lcpl) >= 0);
            Assert.IsTrue(
                H5L.create_hard(m_v2_test_file, "A/B/C/D", m_v2_test_file,
                "shortcut") >= 0);
        }

        /// <summary>
        /// Exercises the native <c>H5Lcreate_hard</c> operation in scenario 2.
        /// </summary>
        [TestMethod]
        public void H5Lcreate_hardTest2()
        {
            Assert.IsTrue(
                                                                H5G.create(m_v0_test_file, "A/B/C/D", m_lcpl_utf8) >= 0);

            for (var i = 0; i < m_utf8strings.Length; ++i)
            {
                Assert.IsTrue(
                                                                                    H5L.create_hard(m_v0_test_file,
                                                                                    Encoding.ASCII.GetBytes("A/B/C/D" + "\0"), m_v0_test_file,
                                                                                    Encoding.UTF8.GetBytes(m_utf8strings[i] + "\0")) >= 0);
            }

            Assert.IsTrue(
                H5G.create(m_v2_test_file, "A/B/C/D", m_lcpl_utf8) >= 0);

            for (var i = 0; i < m_utf8strings.Length; ++i)
            {
                Assert.IsTrue(
                                                                                    H5L.create_hard(m_v2_test_file,
                                                                                    Encoding.ASCII.GetBytes("A/B/C/D" + "\0"), m_v2_test_file,
                                                                                    Encoding.UTF8.GetBytes(m_utf8strings[i] + "\0")) >= 0);
            }
        }
    }
}

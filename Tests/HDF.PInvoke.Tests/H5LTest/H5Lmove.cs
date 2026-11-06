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
    /// <summary>
    /// Groups regression checks for the H5L API.
    /// </summary>
    public partial class H5LTest
    {
        /// <summary>
        /// Exercises the native <c>H5Lmove</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5LmoveTest1()
        {
            Assert.IsTrue(
                                        H5G.close(H5G.create(m_v0_test_file, "A/B/C/D", m_lcpl)) >= 0);
            Assert.IsTrue(
                H5L.create_hard(m_v0_test_file, "A/B/C/D", m_v0_test_file,
                "shortcut") >= 0);
            Assert.IsTrue(
                H5L.move(m_v0_test_file, "shortcut", m_v0_test_file,
                "A/B/C/D/E") >= 0);
            Assert.IsTrue(
                H5L.exists(m_v0_test_file, "A/B/C/D/E") > 0);
            Assert.IsTrue(
                H5L.exists(m_v0_test_file, "A/B/C/D/shortcut") == 0);

            Assert.IsTrue(
                H5G.close(H5G.create(m_v2_test_file, "A/B/C/D", m_lcpl)) >= 0);
            Assert.IsTrue(
                H5L.create_hard(m_v2_test_file, "A/B/C/D", m_v2_test_file,
                "shortcut") >= 0);
            Assert.IsTrue(
                H5L.move(m_v2_test_file, "shortcut", m_v2_test_file,
                "A/B/C/D/E") >= 0);
            Assert.IsTrue(
                H5L.exists(m_v0_test_file, "A/B/C/D/E") > 0);
            Assert.IsTrue(
                H5L.exists(m_v0_test_file, "A/B/C/D/shortcut") == 0);
        }

        /// <summary>
        /// Exercises the native <c>H5Lmove</c> operation in scenario 2.
        /// </summary>
        [TestMethod]
        public void H5LmoveTest2()
        {
            int size = Encoding.UTF8.GetBytes(m_utf8strings[0]).Length;
            byte[] target = new byte[size + 1];
            Array.Copy(Encoding.UTF8.GetBytes(m_utf8strings[0]), target, size);

            Assert.IsTrue(
                H5G.close(H5G.create(m_v0_test_file, "A/B/C/D", m_lcpl)) >= 0);
            Assert.IsTrue(
                H5L.create_hard(m_v0_test_file, "A/B/C/D", m_v0_test_file,
                "shortcut") >= 0);
            Assert.IsTrue(
                H5L.move(m_v0_test_file,
                Encoding.ASCII.GetBytes("shortcut"), m_v0_test_file,
                target, m_lcpl_utf8) >= 0);

            Assert.IsTrue(H5L.exists(m_v0_test_file, target) > 0);
        }

    }
}

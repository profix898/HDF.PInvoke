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
        /// Exercises the native <c>H5Lcreate_soft</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5Lcreate_softTest1()
        {
            Assert.IsTrue(H5L.create_soft("/A/B/C/D", m_v0_test_file,
                                        "this/is/a/soft/link", m_lcpl) >= 0);

            Assert.IsTrue(H5L.create_soft("/A/B/C/D", m_v2_test_file,
                "this/is/a/soft/link", m_lcpl) >= 0);
        }

        /// <summary>
        /// Exercises the native <c>H5Lcreate_soft</c> operation in scenario 2.
        /// </summary>
        [TestMethod]
        public void H5Lcreate_softTest2()
        {
            string sym_path = String.Join("/", m_utf8strings);

            Assert.IsTrue(
                H5L.create_soft(Encoding.ASCII.GetBytes("/A/B/C/D"),
                m_v0_test_file, Encoding.UTF8.GetBytes(sym_path),
                m_lcpl_utf8) >= 0);

            Assert.IsTrue(
                H5L.create_soft(Encoding.ASCII.GetBytes("/A/B/C/D"),
                m_v2_test_file, Encoding.UTF8.GetBytes(sym_path),
                m_lcpl_utf8) >= 0);
        }
    }
}

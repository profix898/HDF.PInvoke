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
using HDF.PInvoke;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using hid_t = System.Int64;
using hsize_t = System.UInt64;

namespace UnitTests
{
    /// <summary>
    /// Groups regression checks for the H5O API.
    /// </summary>
    public partial class H5OTest
    {
        /// <summary>
        /// Exercises the native <c>H5Oopen</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5OopenTest1()
        {
            Assert.IsTrue(
                                        H5G.close(H5G.create(m_v0_test_file, "A/B/C", m_lcpl)) >= 0);
            var obj = H5O.open(m_v0_test_file, "A/B");
            Assert.IsTrue(obj >= 0);
            Assert.IsTrue(H5O.close(obj) >= 0);

            Assert.IsTrue(
                H5G.close(H5G.create(m_v2_test_file, "A/B/C", m_lcpl)) >= 0);
            obj = H5O.open(m_v2_test_file, "A/B");
            Assert.IsTrue(obj >= 0);
            Assert.IsTrue(H5O.close(obj) >= 0);
        }

        /// <summary>
        /// Exercises the native <c>H5Oopen</c> operation in scenario 2.
        /// </summary>
        [TestMethod]
        public void H5OopenTest2()
        {
            Assert.IsFalse(
                                        H5O.open(Utilities.RandomInvalidHandle(), ".") >= 0);
        }
    }
}

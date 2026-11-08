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

namespace UnitTests
{
    /// <summary>
    /// Groups regression checks for the H5O API.
    /// </summary>
    public partial class H5OTest
    {
        /// <summary>
        /// Exercises the native <c>H5Oget_info_by_name</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5Oget_info_by_nameTest1()
        {
            Assert.IsTrue(
                                        H5G.close(H5G.create(m_v0_test_file, "A/B/C", m_lcpl)) >= 0);

            var info = new H5O.info_t();
            Assert.IsTrue(
                H5O.get_info_by_name(m_v0_test_file, "A/B", ref info) >= 0);
            Assert.IsTrue(info.type == H5O.type_t.GROUP);

            Assert.IsTrue(
                H5G.close(H5G.create(m_v2_test_file, "A/B/C", m_lcpl)) >= 0);

            info = new H5O.info_t();
            Assert.IsTrue(
                H5O.get_info_by_name(m_v2_test_file, "A/B", ref info) >= 0);
            Assert.IsTrue(info.type == H5O.type_t.GROUP);
        }

        /// <summary>
        /// Exercises the native <c>H5Oget_info_by_name</c> operation in scenario 2.
        /// </summary>
        [TestMethod]
        public void H5Oget_info_by_nameTest2()
        {
            var info = new H5O.info_t();
            Assert.IsFalse(
                H5O.get_info_by_name(Utilities.RandomInvalidHandle(), ".",
                ref info) >= 0);
        }
    }
}

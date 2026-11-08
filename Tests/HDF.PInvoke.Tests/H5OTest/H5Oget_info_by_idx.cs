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
        /// Exercises the native <c>H5Oget_info_by_idx</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5Oget_info_by_idxTest1()
        {
            Assert.IsTrue(
                                                                H5G.close(H5G.create(m_v0_test_file, "A")) >= 0);
            Assert.IsTrue(
                H5G.close(H5G.create(m_v0_test_file, "AA")) >= 0);
            Assert.IsTrue(
                H5G.close(H5G.create(m_v0_test_file, "AAA")) >= 0);
            Assert.IsTrue(
                H5G.close(H5G.create(m_v0_test_file, "AAAA")) >= 0);

            var info = new H5O.info_t();
            Assert.IsTrue(H5O.get_info_by_idx(m_v0_test_file, ".",
                H5.index_t.NAME, H5.iter_order_t.NATIVE, 2, ref info) >= 0);
            Assert.IsTrue(info.type == H5O.type_t.GROUP);

            Assert.IsTrue(
                H5G.close(H5G.create(m_v2_test_file, "A")) >= 0);
            Assert.IsTrue(
                H5G.close(H5G.create(m_v2_test_file, "AA")) >= 0);
            Assert.IsTrue(
                H5G.close(H5G.create(m_v2_test_file, "AAA")) >= 0);
            Assert.IsTrue(
                H5G.close(H5G.create(m_v2_test_file, "AAAA")) >= 0);

            info = new H5O.info_t();
            Assert.IsTrue(H5O.get_info_by_idx(m_v2_test_file, ".",
                H5.index_t.NAME, H5.iter_order_t.NATIVE, 2, ref info) >= 0);
            Assert.IsTrue(info.type == H5O.type_t.GROUP);
        }

        /// <summary>
        /// Exercises the native <c>H5Oget_info_by_idx</c> operation in scenario 2.
        /// </summary>
        [TestMethod]
        public void H5Oget_info_by_idxTest2()
        {
            var info = new H5O.info_t();
            Assert.IsFalse(
                H5O.get_info_by_idx(Utilities.RandomInvalidHandle(), "A",
                H5.index_t.NAME, H5.iter_order_t.NATIVE, 44, ref info) >= 0);
        }
    }
}

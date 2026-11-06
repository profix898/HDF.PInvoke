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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HDF.PInvoke;

using hid_t = System.Int64;

namespace UnitTests
{
    /// <summary>
    /// Groups regression checks for the H5O API.
    /// </summary>
    public partial class H5OTest
    {
        /// <summary>
        /// Exercises the native <c>H5Oget_info</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5Oget_infoTest1()
        {
            hid_t gid = H5G.create(m_v0_test_file, "A/B/C", m_lcpl);
            Assert.IsTrue(gid >= 0);

            H5O.info_t info = new H5O.info_t();
            Assert.IsTrue(H5O.get_info(gid, ref info) >= 0);
            Assert.IsTrue(info.type == H5O.type_t.GROUP);

            Assert.IsTrue(H5G.close(gid) >= 0);

            gid = H5G.create(m_v2_test_file, "A/B/C", m_lcpl);
            Assert.IsTrue(gid >= 0);

            info = new H5O.info_t();
            Assert.IsTrue(H5O.get_info(gid, ref info) >= 0);
            Assert.IsTrue(info.type == H5O.type_t.GROUP);

            Assert.IsTrue(H5G.close(gid) >= 0);
        }

        /// <summary>
        /// Exercises the native <c>H5Oget_info</c> operation in scenario 2.
        /// </summary>
        [TestMethod]
        public void H5Oget_infoTest2()
        {
            H5O.info_t info = new H5O.info_t();
            Assert.IsFalse(
                H5O.get_info(Utilities.RandomInvalidHandle(), ref info) >= 0);
        }
    }
}

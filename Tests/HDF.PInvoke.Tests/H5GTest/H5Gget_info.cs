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
using System.IO;
using System.Runtime.InteropServices;
using HDF.PInvoke;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using hid_t = System.Int64;

namespace UnitTests
{
    /// <summary>
    /// Groups regression checks for the H5G API.
    /// </summary>
    public partial class H5GTest
    {
        /// <summary>
        /// Exercises the native <c>H5Gget_info</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5Gget_infoTest1()
        {
            var info = new H5G.info_t();
            Assert.IsTrue(H5G.get_info(m_v0_class_file, ref info) >= 0);
            Assert.IsTrue(H5G.get_info(m_v0_test_file, ref info) >= 0);
            Assert.IsTrue(H5G.get_info(m_v2_class_file, ref info) >= 0);
            Assert.IsTrue(H5G.get_info(m_v2_test_file, ref info) >= 0);
        }

        /// <summary>
        /// Exercises the native <c>H5Gget_info</c> operation in scenario 2.
        /// </summary>
        [TestMethod]
        public void H5Gget_infoTest2()
        {
            var info = new H5G.info_t();
            var gid = Utilities.RandomInvalidHandle();
            Assert.IsTrue(H5G.get_info(gid, ref info) < 0);
        }
    }
}

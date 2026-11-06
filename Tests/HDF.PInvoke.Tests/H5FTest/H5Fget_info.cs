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
    /// Groups regression checks for the H5F API.
    /// </summary>
    public partial class H5FTest
    {
        /// <summary>
        /// Exercises the native <c>H5Fget_info</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5Fget_infoTest1()
        {
            H5F.info_t info = new H5F.info_t();
            Assert.IsTrue(H5F.get_info(m_v0_class_file, ref info) >= 0);
            Assert.IsTrue(H5F.get_info(m_v2_class_file, ref info) >= 0);
        }

        /// <summary>
        /// Exercises the native <c>H5Fget_info</c> operation in scenario 2.
        /// </summary>
        [TestMethod]
        public void H5Fget_infoTest2()
        {
            H5F.info_t info = new H5F.info_t();
            Assert.IsFalse(
                H5F.get_info(Utilities.RandomInvalidHandle(), ref info) >= 0);
        }
    }
}

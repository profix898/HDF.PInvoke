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
using System.Runtime.InteropServices;
using HDF.PInvoke;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using herr_t = System.Int32;
using hid_t = System.Int64;

namespace UnitTests
{
    /// <summary>
    /// Groups regression checks for the H5SWMR API.
    /// </summary>
    public partial class H5SWMRTest
    {
        /// <summary>
        /// Exercises the native <c>H5Fget_intent</c> operation in scenario SWMR1.
        /// </summary>
        [TestMethod]
        public void H5Fget_intentTestSWMR1()
        {
            uint intent = 0;
            Assert.IsTrue(H5F.get_intent(m_v3_class_file, ref intent) >= 0);
            Assert.IsTrue((intent & H5F.ACC_SWMR_WRITE) > 0);

            Assert.IsTrue(
                H5F.get_intent(m_v3_test_file_no_swmr, ref intent) >= 0);
            Assert.IsTrue((intent & H5F.ACC_SWMR_WRITE) == 0);
        }
    }
}

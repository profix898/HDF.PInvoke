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
        /// Exercises the native <c>H5Freset_mdc_hit_rate_stats</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5Freset_mdc_hit_rate_statsTest1()
        {
            Assert.IsTrue(
                                        H5F.reset_mdc_hit_rate_stats(m_v0_class_file) >= 0);
            Assert.IsTrue(
                H5F.reset_mdc_hit_rate_stats(m_v2_class_file) >= 0);
        }

        /// <summary>
        /// Exercises the native <c>H5Freset_mdc_hit_rate_stats</c> operation in scenario 2.
        /// </summary>
        [TestMethod]
        public void H5Freset_mdc_hit_rate_statsTest2()
        {
            Assert.IsFalse(
                                        H5F.reset_mdc_hit_rate_stats(Utilities.RandomInvalidHandle())
                                        >= 0);
        }
    }
}

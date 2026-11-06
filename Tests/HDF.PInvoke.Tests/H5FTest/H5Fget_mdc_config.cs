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
        /// Exercises the native <c>H5Fget_mdc_config</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5Fget_mdc_configTest1()
        {
            H5AC.cache_config_t conf = new H5AC.cache_config_t(
                                        H5AC.CURR_CACHE_CONFIG_VERSION);
            Assert.IsTrue(H5F.get_mdc_config(m_v0_class_file, ref conf) >= 0);
            Assert.IsTrue(H5F.get_mdc_config(m_v2_class_file, ref conf) >= 0);
        }

        /// <summary>
        /// Exercises the native <c>H5Fget_mdc_config</c> operation in scenario 2.
        /// </summary>
        [TestMethod]
        public void H5Fget_mdc_configTest2()
        {
            H5AC.cache_config_t conf = new H5AC.cache_config_t();
            Assert.IsFalse(
                H5F.get_mdc_config(Utilities.RandomInvalidHandle(),
                ref conf) >= 0);
        }
    }
}

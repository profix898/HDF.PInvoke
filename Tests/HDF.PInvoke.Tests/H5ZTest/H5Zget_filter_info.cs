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
using herr_t = System.Int32;

namespace UnitTests
{
    /// <summary>
    /// Groups regression checks for the H5Z API.
    /// </summary>
    public partial class H5ZTest
    {
        /// <summary>
        /// Exercises the native <c>H5Zget_filter_info</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5Zget_filter_infoTest1()
        {
            uint filter_config = 0;
            Assert.IsTrue(H5Z.get_filter_info(H5Z.filter_t.DEFLATE,
                ref filter_config) >= 0);
            Assert.IsTrue((filter_config & H5Z.CONFIG_ENCODE_ENABLED) == 1);
            Assert.IsTrue((filter_config & H5Z.CONFIG_DECODE_ENABLED) == 2);
        }
    }
}

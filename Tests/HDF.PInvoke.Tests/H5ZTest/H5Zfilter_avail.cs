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
    /// Groups regression checks for the H5Z API.
    /// </summary>
    public partial class H5ZTest
    {
        /// <summary>
        /// Exercises the native <c>H5Zfilter_avail</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5Zfilter_availTest1()
        {
            Assert.IsTrue(H5Z.filter_avail(H5Z.filter_t.DEFLATE) >= 0);
            Assert.IsFalse(H5Z.filter_avail(H5Z.filter_t.ERROR) >= 0);
        }
    }
}

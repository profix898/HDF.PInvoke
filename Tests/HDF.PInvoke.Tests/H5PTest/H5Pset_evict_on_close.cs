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
using HDF.PInvoke;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using hid_t = System.Int64;
using hsize_t = System.UInt64;

namespace UnitTests
{
    /// <summary>
    /// Groups regression checks for the H5P API.
    /// </summary>
    public partial class H5PTest
    {
        /// <summary>
        /// Exercises the native <c>H5Pset_evict_on_close</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5Pset_evict_on_closeTest1()
        {
            var fapl = H5P.create(H5P.FILE_ACCESS);
            Assert.IsTrue(fapl >= 0);
            Assert.IsTrue(H5P.set_evict_on_close(fapl, 1) >= 0);
            Assert.IsTrue(H5P.close(fapl) >= 0);
        }

        /// <summary>
        /// Exercises the native <c>H5Pset_evict_on_close</c> operation in scenario 2.
        /// </summary>
        [TestMethod]
        public void H5Pset_evict_on_closeTest2()
        {
            var fcpl = H5P.create(H5P.FILE_CREATE);
            Assert.IsTrue(fcpl >= 0);
            Assert.IsFalse(H5P.set_evict_on_close(fcpl, 1) >= 0);
            Assert.IsTrue(H5P.close(fcpl) >= 0);
        }
    }
}

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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HDF.PInvoke;

using hsize_t = System.UInt64;

using hid_t = System.Int64;

namespace UnitTests
{
    /// <summary>
    /// Groups regression checks for the H5P API.
    /// </summary>
    public partial class H5PTest
    {
        /// <summary>
        /// Exercises the native <c>H5Pset_userblock</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5Pset_userblockTest1()
        {
            hid_t fcpl = H5P.create(H5P.FILE_CREATE);
            Assert.IsTrue(fcpl >= 0);
            Assert.IsTrue(H5P.set_userblock(fcpl, 1024) >= 0);
            Assert.IsTrue(H5P.close(fcpl) >= 0);
        }

        /// <summary>
        /// Exercises the native <c>H5Pset_userblock</c> operation in scenario 2.
        /// </summary>
        [TestMethod]
        public void H5Pset_userblockTest2()
        {
            hid_t fcpl = H5P.create(H5P.FILE_CREATE);
            Assert.IsTrue(fcpl >= 0);
            Assert.IsTrue(H5P.set_userblock(fcpl, 1024) >= 0);

            string fname = Path.GetTempFileName();
            hid_t file = H5F.create(fname, H5F.ACC_TRUNC, fcpl);
            Assert.IsTrue(file >= 0);
            Assert.IsTrue(H5F.close(file) >= 0);

            hsize_t size = 0;
            Assert.IsTrue(H5P.get_userblock(fcpl, ref size) >= 0);
            Assert.IsTrue(size == 1024);

            File.Delete(fname);
            Assert.IsTrue(H5P.close(fcpl) >= 0);
        }

        /// <summary>
        /// Exercises the native <c>H5Pset_userblock</c> operation in scenario 3.
        /// </summary>
        [TestMethod]
        public void H5Pset_userblockTest3()
        {
            hid_t fcpl = H5P.create(H5P.FILE_CREATE);
            Assert.IsTrue(fcpl >= 0);
            Assert.IsTrue(H5P.set_userblock(fcpl, 0) >= 0);
            Assert.IsFalse(H5P.set_userblock(fcpl, 111) >= 0);
            Assert.IsTrue(H5P.set_userblock(fcpl, 512) >= 0);
            Assert.IsFalse(H5P.set_userblock(fcpl, 513) >= 0);
            Assert.IsTrue(H5P.close(fcpl) >= 0);
        }
    }
}

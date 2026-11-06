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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HDF.PInvoke;

using hsize_t = System.UInt64;

using hid_t = System.Int64;

namespace UnitTests
{
    /// <summary>
    /// Groups regression checks for the H5S API.
    /// </summary>
    public partial class H5STest
    {
        /// <summary>
        /// Exercises the native <c>H5Sget_simple_extent_npoints</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5Sget_simple_extent_npointsTest1()
        {
            hsize_t[] dims = { 1, 2, 3 };
            hid_t space = H5S.create_simple(dims.Length, dims, dims);
            Assert.IsTrue(space >= 0);
            Assert.IsTrue(H5S.get_simple_extent_npoints(space) == 6);
            Assert.IsTrue(H5S.close(space) >= 0);
        }

        /// <summary>
        /// Exercises the native <c>H5Sget_simple_extent_npoints</c> operation in scenario 2.
        /// </summary>
        [TestMethod]
        public void H5Sget_simple_extent_npointsTest2()
        {
            hid_t space = H5S.create(H5S.class_t.NULL);
            Assert.IsTrue(space >= 0);
            Assert.IsTrue(H5S.get_simple_extent_npoints(space) == 0);
            Assert.IsTrue(H5S.close(space) >= 0);
        }

        /// <summary>
        /// Exercises the native <c>H5Sget_simple_extent_npoints</c> operation in scenario 3.
        /// </summary>
        [TestMethod]
        public void H5Sget_simple_extent_npointsTest3()
        {
            hid_t space = H5S.create(H5S.class_t.SCALAR);
            Assert.IsTrue(space >= 0);
            Assert.IsTrue(H5S.get_simple_extent_npoints(space) == 1);
            Assert.IsTrue(H5S.close(space) >= 0);
        }

        /// <summary>
        /// Exercises the native <c>H5Sget_simple_extent_npoints</c> operation in scenario 4.
        /// </summary>
        [TestMethod]
        public void H5Sget_simple_extent_npointsTest4()
        {
            Assert.IsFalse(
                                        H5S.get_simple_extent_npoints(Utilities.RandomInvalidHandle())
                                        >= 0);
        }
    }
}

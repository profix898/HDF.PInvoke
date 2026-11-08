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
using hsize_t = System.UInt64;

namespace UnitTests
{
    /// <summary>
    /// Groups regression checks for the H5S API.
    /// </summary>
    public partial class H5STest
    {
        /// <summary>
        /// Exercises the native <c>H5Sget_simple_extent_ndims</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5Sget_simple_extent_ndimsTest1()
        {
            hsize_t[] dims = { 1, 2, 3 };
            var space = H5S.create_simple(dims.Length, dims, dims);
            Assert.IsTrue(space >= 0);
            Assert.IsTrue(H5S.get_simple_extent_ndims(space) == 3);
            Assert.IsTrue(H5S.close(space) >= 0);
        }

        /// <summary>
        /// Exercises the native <c>H5Sget_simple_extent_ndims</c> operation in scenario 2.
        /// </summary>
        [TestMethod]
        public void H5Sget_simple_extent_ndimsTest2()
        {
            var space = H5S.create(H5S.class_t.NULL);
            Assert.IsTrue(space >= 0);
            Assert.IsTrue(H5S.get_simple_extent_ndims(space) == 0);
            Assert.IsTrue(H5S.close(space) >= 0);
        }

        /// <summary>
        /// Exercises the native <c>H5Sget_simple_extent_ndims</c> operation in scenario 3.
        /// </summary>
        [TestMethod]
        public void H5Sget_simple_extent_ndimsTest3()
        {
            var space = H5S.create(H5S.class_t.SCALAR);
            Assert.IsTrue(space >= 0);
            Assert.IsTrue(H5S.get_simple_extent_ndims(space) == 0);
            Assert.IsTrue(H5S.close(space) >= 0);
        }

        /// <summary>
        /// Exercises the native <c>H5Sget_simple_extent_ndims</c> operation in scenario 4.
        /// </summary>
        [TestMethod]
        public void H5Sget_simple_extent_ndimsTest4()
        {
            Assert.IsFalse(
                                                                H5S.get_simple_extent_ndims(Utilities.RandomInvalidHandle())
                                                                >= 0);
        }
    }
}

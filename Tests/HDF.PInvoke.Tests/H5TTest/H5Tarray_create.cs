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

using hsize_t = System.UInt64;

using hid_t = System.Int64;

namespace UnitTests
{
    /// <summary>
    /// Groups regression checks for the H5T API.
    /// </summary>
    public partial class H5TTest
    {
        /// <summary>
        /// Exercises the native <c>H5Tarray_create</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5Tarray_createTest1()
        {
            hsize_t[] dims = new hsize_t[] { 3, 3 };
            hid_t dtype = H5T.array_create(H5T.IEEE_F64LE, (uint)dims.Length,
                dims);
            Assert.IsTrue(dtype >= 0);
            Assert.IsTrue(H5T.close(dtype) >= 0);
        }

        /// <summary>
        /// Exercises the native <c>H5Tarray_create</c> operation in scenario 2.
        /// </summary>
        [TestMethod]
        public void H5Tarray_createTest2()
        {
            hsize_t[] dims = new hsize_t[] { 3, 3 };
            hid_t dtype = H5T.array_create(H5T.IEEE_F64LE, 0, dims);
            Assert.IsFalse(dtype >= 0);
        }

        /// <summary>
        /// Exercises the native <c>H5Tarray_create</c> operation in scenario 3.
        /// </summary>
        [TestMethod]
        public void H5Tarray_createTest3()
        {
            hsize_t[] dims = new hsize_t[] { 3, 3 };
            hid_t dtype = H5T.array_create(Utilities.RandomInvalidHandle(),
                (uint)dims.Length, dims);
            Assert.IsFalse(dtype >= 0);
        }
    }
}

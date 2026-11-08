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
using herr_t = System.Int32;
using hid_t = System.Int64;
using hsize_t = System.UInt64;
using hssize_t = System.Int64;

namespace UnitTests
{
    /// <summary>
    /// Groups regression checks for the H5S API.
    /// </summary>
    public partial class H5STest
    {
        /// <summary>
        /// Exercises the native <c>H5Sset_extent_none</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5Sset_extent_noneTest1()
        {
            hsize_t[] dims = { 10, 20, 30 };
            var space = H5S.create_simple(dims.Length, dims, dims);
            Assert.IsTrue(space > 0);
            Assert.IsTrue(H5S.set_extent_none(space) >= 0);
            Assert.IsTrue(
                H5S.get_simple_extent_type(space) == H5S.class_t.NULL);

            Assert.IsTrue(H5S.close(space) >= 0);
        }

        /// <summary>
        /// Exercises the native <c>H5Sset_extent_none</c> operation in scenario 2.
        /// </summary>
        [TestMethod]
        public void H5Sset_extent_noneTest2()
        {
            Assert.IsFalse(
                                        H5S.set_extent_none(Utilities.RandomInvalidHandle()) >= 0);
        }
    }
}

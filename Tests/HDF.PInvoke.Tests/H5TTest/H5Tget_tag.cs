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
using System.Runtime.InteropServices;
using HDF.PInvoke;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using herr_t = System.Int32;
using hid_t = System.Int64;

namespace UnitTests
{
    /// <summary>
    /// Groups regression checks for the H5T API.
    /// </summary>
    public partial class H5TTest
    {
        /// <summary>
        /// Exercises the native <c>H5Tget_tag</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5Tget_tagTest1()
        {
            var dtype = H5T.create(H5T.class_t.OPAQUE, new IntPtr(1024));
            Assert.IsTrue(dtype >= 0);

            Assert.IsTrue(
                H5T.set_tag(dtype, "Mary had a little lamb...") >= 0);

            var tag = H5T.get_tag(dtype);
            Assert.IsTrue(tag.ToInt64() >= 0);

            Assert.IsTrue(Marshal.PtrToStringAnsi(tag)
                == "Mary had a little lamb...");

            Assert.IsTrue(H5.free_memory(tag) >= 0);

            Assert.IsTrue(H5T.close(dtype) >= 0);
        }

        /// <summary>
        /// Exercises the native <c>H5Tget_tag</c> operation in scenario 2.
        /// </summary>
        [TestMethod]
        public void H5Tget_tagTest2()
        {
            var tag = H5T.get_tag(Utilities.RandomInvalidHandle());
            Assert.IsTrue(tag == IntPtr.Zero);
        }
    }
}

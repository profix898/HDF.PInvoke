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
using size_t = System.IntPtr;

using hid_t = System.Int64;

namespace UnitTests
{
    /// <summary>
    /// Groups regression checks for the H5S API.
    /// </summary>
    public partial class H5STest
    {
        /// <summary>
        /// Exercises the native <c>H5Sdecode</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5SdecodeTest1()
        {
            hsize_t[] dims = { 1, 2, 3 };
            hid_t space = H5S.create_simple(dims.Length, dims, dims);
            Assert.IsTrue(space > 0);

            size_t nalloc = new IntPtr();
            Assert.IsTrue(H5S.encode(space, null, ref nalloc) >= 0);

            byte[] buf = new byte[nalloc.ToInt32()];
            Assert.IsTrue(H5S.encode(space, buf, ref nalloc) >= 0);

            Assert.IsTrue(H5S.close(space) >= 0);

            space = H5S.decode(buf);
            Assert.IsTrue(space >= 0);

            Assert.IsTrue(H5S.get_simple_extent_ndims(space) == dims.Length);
            hsize_t[] tdims = new hsize_t[dims.Length];
            Assert.IsTrue(
                H5S.get_simple_extent_dims(space, tdims, null) == dims.Length);

            for (int i = 0; i < dims.Length; ++i)
            {
                Assert.IsTrue(tdims[i] == dims[i]);
            }

            Assert.IsTrue(H5S.close(space) >= 0);
        }

        /// <summary>
        /// Exercises the native <c>H5Sdecode</c> operation in scenario 2.
        /// </summary>
        [TestMethod]
        public void H5SdecodeTest2()
        {
            Assert.IsFalse(H5S.decode(null) >= 0);
        }
    }
}

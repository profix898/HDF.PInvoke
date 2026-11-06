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
    public partial class H5STest
    {
        [TestMethod]
        public void H5SencodeTest1()
        {
            hsize_t[] dims = { 1, 2, 3 };
            hid_t space = H5S.create_simple(dims.Length, dims, dims);
            Assert.IsTrue(space > 0);

            size_t nalloc = new IntPtr();
            Assert.IsTrue(H5S.encode(space, null, ref nalloc) >= 0);

            byte[] buf = new byte[nalloc.ToInt32()];
            Assert.IsTrue(H5S.encode(space, buf, ref nalloc) >= 0);

            Assert.IsTrue(H5S.close(space) >= 0);
        }

        [TestMethod]
        public void H5SencodeTest2()
        {
            size_t nalloc = IntPtr.Zero;
            Assert.IsFalse(H5S.encode(Utilities.RandomInvalidHandle(),
                null, ref nalloc) >= 0);
        }
    }
}

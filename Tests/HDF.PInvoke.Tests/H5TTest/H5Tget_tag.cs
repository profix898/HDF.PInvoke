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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HDF.PInvoke;

using herr_t = System.Int32;

using hid_t = System.Int64;

namespace UnitTests
{
    public partial class H5TTest
    {
        [TestMethod]
        public void H5Tget_tagTest1()
        {
            hid_t dtype = H5T.create(H5T.class_t.OPAQUE, new IntPtr(1024));
            Assert.IsTrue(dtype >= 0);

            Assert.IsTrue(
                H5T.set_tag(dtype, "Mary had a little lamb...") >= 0);

            IntPtr tag = H5T.get_tag(dtype);
            Assert.IsTrue(tag.ToInt64() >= 0);

            Assert.IsTrue(Marshal.PtrToStringAnsi(tag)
                == "Mary had a little lamb...");

            Assert.IsTrue(H5.free_memory(tag) >= 0);

            Assert.IsTrue(H5T.close(dtype) >= 0);
        }

        [TestMethod]
        public void H5Tget_tagTest2()
        {
            IntPtr tag = H5T.get_tag(Utilities.RandomInvalidHandle());
            Assert.IsTrue(tag == IntPtr.Zero);
        }
    }
}

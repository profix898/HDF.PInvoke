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

using herr_t = System.Int32;
using hsize_t = System.UInt64;
using hssize_t = System.Int64;

using hid_t = System.Int64;

namespace UnitTests
{
    public partial class H5STest
    {
        [TestMethod]
        public void H5Sset_extent_simpleTest1()
        {
            hid_t space = H5S.create(H5S.class_t.NULL);
            Assert.IsTrue(space > 0);

            hsize_t[] dims = { 10, 20, 30 };
            Assert.IsTrue(
                H5S.set_extent_simple(space, 3, dims, null) >= 0);

            Assert.IsTrue(H5S.close(space) >= 0);
        }

        [TestMethod]
        public void H5Sset_extent_simpleTest2()
        {
            Assert.IsFalse(
                                        H5S.set_extent_simple(Utilities.RandomInvalidHandle(), 3, null,
                                        null) >= 0);
        }
    }
}

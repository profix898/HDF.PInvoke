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
    public partial class H5STest
    {
        [TestMethod]
        public void H5Sget_simple_extent_dimsTest1()
        {
            hsize_t[] dims = { 1, 2, 3 };
            hid_t space = -1;
            hsize_t[] dims_out = new hsize_t[3];

            space = H5S.create_simple(dims.Length, dims, dims);
            Assert.IsTrue(
                H5S.get_simple_extent_dims(space, (ulong[])null, null) == 3);

            Assert.IsTrue(
                H5S.get_simple_extent_dims(space, dims_out, null) == 3);

            Assert.IsTrue(dims_out[2] == 3);

            Assert.IsTrue(
                H5S.get_simple_extent_dims(space, null, dims_out) == 3);

            Assert.IsTrue(dims_out[0] == 1);

            Assert.IsTrue(space > 0);
            Assert.IsTrue(H5S.close(space) >= 0);
        }

        [TestMethod]
        public void H5Sget_simple_extent_dimsTest2()
        {
            hsize_t[] dims = { 1, 2, 3 };
            hsize_t[] max_dims = { H5S.UNLIMITED, H5S.UNLIMITED, H5S.UNLIMITED };
            hid_t space = -1;
            hsize_t[] dims_out = new hsize_t[3];

            space = H5S.create_simple(dims.Length, dims, max_dims);
            Assert.IsTrue(
            H5S.get_simple_extent_dims(space, (ulong[])null, null) == 3);

            Assert.IsTrue(
                H5S.get_simple_extent_dims(space, dims_out, null) == 3);
            Assert.IsTrue(dims_out[0] == 1);

            Assert.IsTrue(
                H5S.get_simple_extent_dims(space, null, dims_out) == 3);
            Assert.IsTrue(dims_out[0] == H5S.UNLIMITED);

            Assert.IsTrue(space > 0);
            Assert.IsTrue(H5S.close(space) >= 0);
        }
    }
}

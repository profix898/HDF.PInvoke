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
        public void H5Sget_select_hyper_blocklistTest1()
        {
            hsize_t[] dims = { 1, 2, 3 };
            hid_t space = H5S.create_simple(dims.Length, dims, dims);
            Assert.IsTrue(space > 0);
            hsize_t[] start = { 0, 0, 0 };
            hsize_t[] count = { 1, 1, 1 };
            hsize_t[] block = { 1, 2, 3 };
            Assert.IsTrue(
                H5S.select_hyperslab(space, H5S.seloper_t.SET, start, null,
                count, block) >= 0);
            Assert.IsTrue(H5S.get_select_hyper_nblocks(space) == 1);

            hsize_t[] buf = new hsize_t[2 * dims.Length];

            Assert.IsTrue(
                H5S.get_select_hyper_blocklist(space, 0, 1, buf) >= 0);

            for (int i = 0; i < dims.Length; ++i)
            {
                buf[i] = 0;
                buf[i + 3] = dims[i] - 1;
            }

            Assert.IsTrue(H5S.close(space) >= 0);
        }

        [TestMethod]
        public void H5Sget_select_hyper_blocklistTest2()
        {
            Assert.IsFalse(
                                        H5S.get_select_hyper_blocklist(Utilities.RandomInvalidHandle(),
                                        0, 0, null) >= 0);
        }
    }
}

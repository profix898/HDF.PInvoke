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
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HDF.PInvoke;

using herr_t = System.Int32;
using hsize_t = System.UInt64;

using hid_t = System.Int64;

namespace UnitTests
{
    public partial class H5DTest
    {
        [TestMethod]
        public void H5DfillTest1()
        {
            hsize_t[] dims = { 10 };
            hid_t space = H5S.create_simple(1, dims, null);
            Assert.IsTrue(H5S.select_all(space) >= 0);

            double[] v = new double[10];
            double fill = 1.0;

            GCHandle v_hnd = GCHandle.Alloc(v, GCHandleType.Pinned);
            GCHandle fill_hnd = GCHandle.Alloc(fill, GCHandleType.Pinned);
            Assert.IsTrue(
                H5D.fill(fill_hnd.AddrOfPinnedObject(), H5T.NATIVE_DOUBLE,
                v_hnd.AddrOfPinnedObject(), H5T.NATIVE_DOUBLE, space) >= 0);
            fill_hnd.Free();
            v_hnd.Free();

            for (int i = 0; i < v.Length; ++i)
            {
                Assert.IsTrue(v[i] == 1.0);
            }

            Assert.IsTrue(H5S.close(space) >= 0);
        }

        [TestMethod]
        public void H5DfillTest2()
        {
            hsize_t[] dims = { 5 };
            hid_t space = H5S.create_simple(1, dims, null);
            Assert.IsTrue(H5S.select_all(space) >= 0);

            double[] v = new double[5] { 0.0, 1.0, 2.0, 3.0, 4.0 };
            GCHandle v_hnd = GCHandle.Alloc(v, GCHandleType.Pinned);
            Assert.IsTrue(
                H5D.fill(IntPtr.Zero, H5T.NATIVE_DOUBLE,
                v_hnd.AddrOfPinnedObject(), H5T.NATIVE_DOUBLE, space) >= 0);
            v_hnd.Free();

            for (int i = 0; i < v.Length; ++i)
            {
                Assert.IsTrue(v[i] == 0.0);
            }

            Assert.IsTrue(H5S.close(space) >= 0);
        }
    }
}

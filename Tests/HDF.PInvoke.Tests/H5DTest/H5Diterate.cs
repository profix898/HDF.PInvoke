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
        public void H5DiterateTest1()
        {
            int[] buf = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            IntPtr count_ptr = Marshal.AllocHGlobal(sizeof(int));
            Marshal.WriteInt32(count_ptr, 0);

            hsize_t[] dims = { 10 };
            hid_t space = H5S.create_simple(1, dims, null);
            Assert.IsTrue(space >= 0);
            Assert.IsTrue(H5S.select_all(space) >= 0);

            GCHandle buf_hnd = GCHandle.Alloc(buf, GCHandleType.Pinned);

            H5D.operator_t cb = DelegateMethod;
            Assert.IsTrue(
                H5D.iterate(buf_hnd.AddrOfPinnedObject(), H5T.NATIVE_INT,
                space, cb, count_ptr) >= 0);

            buf_hnd.Free();

            int count = Marshal.ReadInt32(count_ptr);
            // expect the sum of the buffer elements
            Assert.IsTrue(count == 45);

            Assert.IsTrue(H5S.close(space) >= 0);
            Marshal.FreeHGlobal(count_ptr);
        }
    }
}

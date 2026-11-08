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
using HDF.PInvoke;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using herr_t = System.Int32;
using hid_t = System.Int64;
using hsize_t = System.UInt64;

namespace UnitTests
{
    /// <summary>
    /// Groups regression checks for the H5D API.
    /// </summary>
    public partial class H5DTest
    {
        /// <summary>
        /// Exercises the native <c>H5Diterate</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5DiterateTest1()
        {
            int[] buf = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            var count_ptr = Marshal.AllocHGlobal(sizeof(int));
            Marshal.WriteInt32(count_ptr, 0);

            hsize_t[] dims = { 10 };
            var space = H5S.create_simple(1, dims, null);
            Assert.IsTrue(space >= 0);
            Assert.IsTrue(H5S.select_all(space) >= 0);

            var buf_hnd = GCHandle.Alloc(buf, GCHandleType.Pinned);

            H5D.operator_t cb = DelegateMethod;
            Assert.IsTrue(
                H5D.iterate(buf_hnd.AddrOfPinnedObject(), H5T.NATIVE_INT,
                space, cb, count_ptr) >= 0);

            buf_hnd.Free();

            var count = Marshal.ReadInt32(count_ptr);
            // expect the sum of the buffer elements
            Assert.IsTrue(count == 45);

            Assert.IsTrue(H5S.close(space) >= 0);
            Marshal.FreeHGlobal(count_ptr);
        }
    }
}

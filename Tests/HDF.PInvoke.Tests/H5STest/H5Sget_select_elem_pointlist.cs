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
using HDF.PInvoke;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using herr_t = System.Int32;
using hid_t = System.Int64;
using hsize_t = System.UInt64;
using hssize_t = System.Int64;

namespace UnitTests
{
    /// <summary>
    /// Groups regression checks for the H5S API.
    /// </summary>
    public partial class H5STest
    {
        /// <summary>
        /// Exercises the native <c>H5Sget_select_elem_pointlist</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5Sget_select_elem_pointlistTest1()
        {
            hsize_t[] dims = { 1, 2, 3 };
            var space = H5S.create_simple(dims.Length, dims, dims);
            Assert.IsTrue(space > 0);
            hsize_t[] sel = { 0, 1, 2, 0, 2, 2 };
            Assert.IsTrue(
                H5S.select_elements(space, H5S.seloper_t.SET, new IntPtr(2),
                sel) >= 0);
            var buf = new hsize_t[sel.Length];
            Assert.IsTrue(H5S.get_select_elem_pointlist(space, 0, 2, buf) >= 0);

            for (var i = 0; i < buf.Length; ++i)
            {
                Assert.IsTrue(sel[i] == buf[i]);
            }

            Assert.IsTrue(H5S.close(space) >= 0);
        }

        /// <summary>
        /// Exercises the native <c>H5Sget_select_elem_pointlist</c> operation in scenario 2.
        /// </summary>
        [TestMethod]
        public void H5Sget_select_elem_pointlistTest2()
        {
            Assert.IsFalse(
                                                                H5S.get_select_elem_pointlist(Utilities.RandomInvalidHandle(),
                                                                0, 0, null) >= 0);
        }
    }
}

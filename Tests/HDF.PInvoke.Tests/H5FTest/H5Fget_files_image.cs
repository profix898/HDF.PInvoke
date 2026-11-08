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
using ssize_t = System.IntPtr;

namespace UnitTests
{
    /// <summary>
    /// Groups regression checks for the H5F API.
    /// </summary>
    public partial class H5FTest
    {
        /// <summary>
        /// Exercises the native <c>H5Fget_files_image</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5Fget_files_imageTest1()
        {
            var fname = Path.GetTempFileName();
            var file = H5F.create(fname, H5F.ACC_TRUNC);
            Assert.IsTrue(file >= 0);

            var buf_len = new IntPtr();
            var size = H5F.get_file_image(file, IntPtr.Zero, buf_len);
            Assert.IsTrue(size.ToInt32() > 0);

            var buf = H5.allocate_memory(new IntPtr(size.ToInt32()), 1);
            Assert.IsTrue(buf != IntPtr.Zero);

            Assert.IsTrue(H5F.get_file_image(file, IntPtr.Zero,
                buf_len).ToInt32() > 0);

            Assert.IsTrue(H5.free_memory(buf) >= 0);

            Assert.IsTrue(H5F.close(file) >= 0);
            File.Delete(fname);
        }

        /// <summary>
        /// Exercises the native <c>H5Fget_files_image</c> operation in scenario 2.
        /// </summary>
        [TestMethod]
        public void H5Fget_files_imageTest2()
        {
            var fname = Path.GetTempFileName();
            var file = H5F.create(fname, H5F.ACC_TRUNC);
            Assert.IsTrue(file >= 0);

            var buf_len = new IntPtr();
            var size = H5F.get_file_image(file, IntPtr.Zero, buf_len);
            Assert.IsTrue(size.ToInt32() > 0);

            var buf = Marshal.AllocHGlobal((int)size);
            Assert.IsTrue(buf != IntPtr.Zero);

            Assert.IsTrue(H5F.get_file_image(file, IntPtr.Zero,
                buf_len).ToInt32() > 0);

            Marshal.FreeHGlobal(buf);

            Assert.IsTrue(H5F.close(file) >= 0);
            File.Delete(fname);
        }
    }
}

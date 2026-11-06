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
using ssize_t = System.IntPtr;

using hid_t = System.Int64;

namespace UnitTests
{
    public partial class H5FTest
    {
        [TestMethod]
        public void H5Fget_files_imageTest1()
        {
            string fname = Path.GetTempFileName();
            hid_t file = H5F.create(fname, H5F.ACC_TRUNC);
            Assert.IsTrue(file >= 0);

            IntPtr buf_len = new IntPtr();
            ssize_t size = H5F.get_file_image(file, IntPtr.Zero, buf_len);
            Assert.IsTrue(size.ToInt32() > 0);

            IntPtr buf = H5.allocate_memory(new IntPtr(size.ToInt32()), 1);
            Assert.IsTrue(buf != IntPtr.Zero);

            Assert.IsTrue(H5F.get_file_image(file, IntPtr.Zero,
                buf_len).ToInt32() > 0);

            Assert.IsTrue(H5.free_memory(buf) >= 0);

            Assert.IsTrue(H5F.close(file) >= 0);
            File.Delete(fname);
        }

        [TestMethod]
        public void H5Fget_files_imageTest2()
        {
            string fname = Path.GetTempFileName();
            hid_t file = H5F.create(fname, H5F.ACC_TRUNC);
            Assert.IsTrue(file >= 0);

            IntPtr buf_len = new IntPtr();
            ssize_t size = H5F.get_file_image(file, IntPtr.Zero, buf_len);
            Assert.IsTrue(size.ToInt32() > 0);

            IntPtr buf = Marshal.AllocHGlobal((int)size);
            Assert.IsTrue(buf != IntPtr.Zero);

            Assert.IsTrue(H5F.get_file_image(file, IntPtr.Zero,
                buf_len).ToInt32() > 0);

            Marshal.FreeHGlobal(buf);

            Assert.IsTrue(H5F.close(file) >= 0);
            File.Delete(fname);
        }
    }
}

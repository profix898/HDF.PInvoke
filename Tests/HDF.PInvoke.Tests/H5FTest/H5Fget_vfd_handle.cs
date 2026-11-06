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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HDF.PInvoke;

using hid_t = System.Int64;

namespace UnitTests
{
    public partial class H5FTest
    {
        [TestMethod]
        public void H5Fget_vfd_handleTest1()
        {
            IntPtr hnd = IntPtr.Zero;
            Assert.IsTrue(
                H5F.get_vfd_handle(m_v0_class_file, H5P.DEFAULT, ref hnd) >= 0);
            Assert.IsTrue(hnd != IntPtr.Zero);
            Assert.IsTrue(
                H5F.get_vfd_handle(m_v2_class_file, H5P.DEFAULT, ref hnd) >= 0);
            Assert.IsTrue(hnd != IntPtr.Zero);
        }

        [TestMethod]
        public void H5Fget_vfd_handleTest2()
        {
            IntPtr hnd = new IntPtr();
            Assert.IsFalse(
                H5F.get_vfd_handle(Utilities.RandomInvalidHandle(),
                H5P.DEFAULT, ref hnd) >= 0);
        }
    }
}

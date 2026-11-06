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

using ssize_t = System.IntPtr;

using hid_t = System.Int64;

namespace UnitTests
{
    public partial class H5FTest
    {
        [TestMethod]
        public void H5Fget_obj_idsTest1()
        {
            IntPtr buf = H5.allocate_memory(new IntPtr(10 * sizeof(hid_t)), 0);

            Assert.IsTrue(
                H5F.get_obj_ids(m_v0_class_file, H5F.OBJ_ALL, new IntPtr(10),
                buf).ToInt32() > 0);
            Assert.IsTrue(
                H5F.get_obj_ids(m_v2_class_file, H5F.OBJ_ALL, new IntPtr(10),
                buf).ToInt32() > 0);

            Assert.IsTrue(H5.free_memory(buf) >= 0);
        }

        [TestMethod]
        public void H5Fget_obj_idsTest2()
        {
            IntPtr buf = H5.allocate_memory(new IntPtr(10 * sizeof(hid_t)), 0);
            Assert.IsFalse(
                H5F.get_obj_ids(Utilities.RandomInvalidHandle(),
                H5F.OBJ_ALL, new IntPtr(10), buf).ToInt32() > 0);
            Assert.IsTrue(H5.free_memory(buf) >= 0);
        }
    }
}

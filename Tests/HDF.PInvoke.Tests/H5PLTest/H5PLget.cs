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
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HDF.PInvoke;

using uint32_t = System.UInt32;

using hid_t = System.Int64;

namespace UnitTests
{
    public partial class H5PLTest
    {
        [TestMethod]
        public void H5PLgetTest1()
        {
            Assert.IsTrue(H5PL.append("foo") >= 0);
            uint32_t listsize = 0;
            Assert.IsTrue(H5PL.size(ref listsize) >= 0);
            Assert.IsTrue(listsize >= 0);
            StringBuilder sb = new StringBuilder();
            IntPtr size = new IntPtr(4);
            Assert.IsFalse(H5PL.get(0, sb, size) == IntPtr.Zero);
        }

        [TestMethod]
        public void H5PLgetTest2()
        {
            uint32_t listsize = 0;
            Assert.IsTrue(H5PL.size(ref listsize) >= 0);
            Assert.IsTrue(listsize >= 0);
            StringBuilder sb = new StringBuilder();
            IntPtr size = new IntPtr(4);
            Assert.IsFalse(H5PL.get(0, sb, size) == IntPtr.Zero);
        }
    }
}

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
using HDF.PInvoke;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using hid_t = System.Int64;
using uint32_t = System.UInt32;

namespace UnitTests
{
    /// <summary>
    /// Groups regression checks for the H5PL API.
    /// </summary>
    public partial class H5PLTest
    {
        /// <summary>
        /// Exercises the native <c>H5PLget</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5PLgetTest1()
        {
            Assert.IsTrue(H5PL.append("foo") >= 0);
            uint32_t listsize = 0;
            Assert.IsTrue(H5PL.size(ref listsize) >= 0);
            Assert.IsTrue(listsize >= 0);
            var sb = new StringBuilder();
            var size = new IntPtr(4);
            Assert.IsFalse(H5PL.get(0, sb, size) == IntPtr.Zero);
        }

        /// <summary>
        /// Exercises the native <c>H5PLget</c> operation in scenario 2.
        /// </summary>
        [TestMethod]
        public void H5PLgetTest2()
        {
            uint32_t listsize = 0;
            Assert.IsTrue(H5PL.size(ref listsize) >= 0);
            Assert.IsTrue(listsize >= 0);
            var sb = new StringBuilder();
            var size = new IntPtr(4);
            Assert.IsFalse(H5PL.get(0, sb, size) == IntPtr.Zero);
        }
    }
}

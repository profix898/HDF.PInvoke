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
using HDF.PInvoke;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using hid_t = System.Int64;

namespace UnitTests
{
    /// <summary>
    /// Groups regression checks for the H5E API.
    /// </summary>
    public partial class H5ETest
    {
        /// <summary>
        /// Exercises the native <c>H5Epop</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5EpopTest1()
        {
            Assert.IsTrue(
                                        H5E.push(H5E.DEFAULT, "hello.c", "sqrt", 77, H5E.ERR_CLS,
                                        H5E.NONE_MAJOR, H5E.NONE_MINOR, "Hello, World!") >= 0);
            Assert.IsTrue(
                H5E.push(H5E.DEFAULT, "hello.c", "sqr", 78, H5E.ERR_CLS,
                H5E.NONE_MAJOR, H5E.NONE_MINOR, "Hello, World!") >= 0);
            Assert.IsTrue(H5E.pop(H5E.DEFAULT, new IntPtr(2)) >= 0);
        }

        /// <summary>
        /// Exercises the native <c>H5Epop</c> operation in scenario 2.
        /// </summary>
        [TestMethod]
        public void H5EpopTest2()
        {
            Assert.IsFalse(
                                        H5E.pop(Utilities.RandomInvalidHandle(), IntPtr.Zero) >= 0);
        }
    }
}

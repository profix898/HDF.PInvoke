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
    /// <summary>
    /// Groups regression checks for the H5E API.
    /// </summary>
    public partial class H5ETest
    {
        /// <summary>
        /// Exercises the native <c>H5Eget_num</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5Eget_numTest1()
        {
            hid_t est = H5E.get_current_stack();
            Assert.IsTrue(est >= 0);
            ssize_t size = H5E.get_num(est);
            Assert.IsTrue(size.ToInt32() >= 0);
            Assert.IsTrue(H5E.close_stack(est) >= 0);
        }

        /// <summary>
        /// Exercises the native <c>H5Eget_num</c> operation in scenario 2.
        /// </summary>
        [TestMethod]
        public void H5Eget_numTest2()
        {
            Assert.IsFalse(
                                        H5E.get_num(Utilities.RandomInvalidHandle()).ToInt32() >= 0);
        }
    }
}

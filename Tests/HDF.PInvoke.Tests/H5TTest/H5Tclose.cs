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

using hsize_t = System.UInt64;

using hid_t = System.Int64;

namespace UnitTests
{
    /// <summary>
    /// Groups regression checks for the H5T API.
    /// </summary>
    public partial class H5TTest
    {
        /// <summary>
        /// Exercises the native <c>H5Tclose</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5TcloseTest1()
        {
            hid_t dtype = H5T.copy(H5T.IEEE_F64LE);
            Assert.IsTrue(dtype >= 0);
            Assert.IsTrue(H5T.close(dtype) >= 0);
        }

        /// <summary>
        /// Exercises the native <c>H5Tclose</c> operation in scenario 2.
        /// </summary>
        [TestMethod]
        public void H5TcloseTest2()
        {
            Assert.IsFalse(H5T.close(H5T.IEEE_F64LE) >= 0);
        }

        /// <summary>
        /// Exercises the native <c>H5Tclose</c> operation in scenario 3.
        /// </summary>
        [TestMethod]
        public void H5TcloseTest3()
        {
            Assert.IsFalse(H5T.close(Utilities.RandomInvalidHandle()) >= 0);
        }
    }
}

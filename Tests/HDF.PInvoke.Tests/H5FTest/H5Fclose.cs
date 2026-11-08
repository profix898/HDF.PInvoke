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
using HDF.PInvoke;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using hid_t = System.Int64;

namespace UnitTests
{
    /// <summary>
    /// Groups regression checks for the H5F API.
    /// </summary>
    public partial class H5FTest
    {
        /// <summary>
        /// Exercises the native <c>H5Fclose</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5FcloseTest1()
        {
            var fname = Path.GetTempFileName();
            var file = H5F.create(fname, H5F.ACC_TRUNC);
            Assert.IsTrue(file >= 0);
            Assert.IsTrue(H5F.close(file) >= 0);
            file = H5F.open(fname, H5F.ACC_RDONLY);
            Assert.IsTrue(file >= 0);
            Assert.IsTrue(H5F.close(file) >= 0);
            File.Delete(fname);
        }

        /// <summary>
        /// Exercises the native <c>H5Fclose</c> operation in scenario 2.
        /// </summary>
        [TestMethod]
        public void H5FcloseTest2()
        {
            Assert.IsFalse(H5F.close(Utilities.RandomInvalidHandle()) >= 0);
        }
    }
}

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
        /// Exercises the native <c>H5Freopen</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5FreopenTest1()
        {
            var file = H5F.reopen(m_v0_class_file);
            Assert.IsTrue(file >= 0);

            var file1 = H5F.reopen(m_v2_class_file);
            Assert.IsTrue(file1 >= 0);

            Assert.IsTrue(H5F.close(file) >= 0);
            Assert.IsTrue(H5F.close(file1) >= 0);
        }

        /// <summary>
        /// Exercises the native <c>H5Freopen</c> operation in scenario 2.
        /// </summary>
        [TestMethod]
        public void H5FreopenTest2()
        {
            Assert.IsFalse(
                                                                H5F.reopen(Utilities.RandomInvalidHandle()) >= 0);
        }
    }
}

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
using HDF.PInvoke;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using hid_t = System.Int64;

namespace UnitTests
{
    /// <summary>
    /// Groups regression checks for the H5G API.
    /// </summary>
    public partial class H5GTest
    {
        /// <summary>
        /// Exercises the native <c>H5Gopen</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5GopenTest1()
        {
            var group = H5G.open(m_v0_class_file, ".");
            Assert.IsTrue(group >= 0);
            Assert.IsTrue(H5G.close(group) >= 0);

            group = H5G.open(m_v2_class_file, ".");
            Assert.IsTrue(group >= 0);
            Assert.IsTrue(H5G.close(group) >= 0);
        }

        /// <summary>
        /// Exercises the native <c>H5Gopen</c> operation in scenario 2.
        /// </summary>
        [TestMethod]
        public void H5GopenTest2()
        {
            var group = H5G.open(Utilities.RandomInvalidHandle(), ".");
            Assert.IsTrue(group < 0);

            group = H5G.open(Utilities.RandomInvalidHandle(), "A");
            Assert.IsTrue(group < 0);
        }
    }
}

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
using System.Text;
using HDF.PInvoke;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using hid_t = System.Int64;
using hsize_t = System.UInt64;

namespace UnitTests
{
    /// <summary>
    /// Groups regression checks for the H5P API.
    /// </summary>
    public partial class H5PTest
    {
        /// <summary>
        /// Exercises the native <c>H5Pset_virtual_prefix</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5Pset_virtual_prefixTest1()
        {
            var dapl = H5P.create(H5P.DATASET_ACCESS);
            Assert.IsTrue(dapl >= 0);
            var prefix = "foo";
            Assert.IsTrue(H5P.set_virtual_prefix(dapl, prefix) >= 0);

            var sb = new StringBuilder(4);
            var size = new IntPtr(4);
            Assert.IsTrue(H5P.get_virtual_prefix(dapl, sb, size).ToInt32() == 3);

            Assert.IsTrue(H5P.close(dapl) >= 0);
        }
    }
}

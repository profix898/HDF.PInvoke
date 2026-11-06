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
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HDF.PInvoke;

using hsize_t = System.UInt64;

using hid_t = System.Int64;

namespace UnitTests
{
    public partial class H5PTest
    {
        [TestMethod]
        public void H5Pset_virtual_prefixTest1()
        {
            hid_t dapl = H5P.create(H5P.DATASET_ACCESS);
            Assert.IsTrue(dapl >= 0);
            string prefix = "foo";
            Assert.IsTrue(H5P.set_virtual_prefix(dapl, prefix) >= 0);

            StringBuilder sb = new StringBuilder(4);
            IntPtr size = new IntPtr(4);
            Assert.IsTrue(H5P.get_virtual_prefix(dapl, sb, size).ToInt32() == 3);

            Assert.IsTrue(H5P.close(dapl) >= 0);
        }
    }
}

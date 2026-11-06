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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HDF.PInvoke;

using hsize_t = System.UInt64;

using hid_t = System.Int64;

namespace UnitTests
{
    public partial class H5PTest
    {
        [TestMethod]
        public void H5Pset_evict_on_closeTest1()
        {
            hid_t fapl = H5P.create(H5P.FILE_ACCESS);
            Assert.IsTrue(fapl >= 0);
            Assert.IsTrue(H5P.set_evict_on_close(fapl, 1) >= 0);
            Assert.IsTrue(H5P.close(fapl) >= 0);
        }

        [TestMethod]
        public void H5Pset_evict_on_closeTest2()
        {
            hid_t fcpl = H5P.create(H5P.FILE_CREATE);
            Assert.IsTrue(fcpl >= 0);
            Assert.IsFalse(H5P.set_evict_on_close(fcpl, 1) >= 0);
            Assert.IsTrue(H5P.close(fcpl) >= 0);
        }
    }
}

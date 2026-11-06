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

using hid_t = System.Int64;

namespace UnitTests
{
    public partial class H5FTest
    {
        [TestMethod]
        public void H5Fis_hdf5Test1()
        {
            string fname = Path.GetTempFileName();
            hid_t file = H5F.create(fname, H5F.ACC_TRUNC);
            Assert.IsTrue(file >= 0);
            Assert.IsTrue(H5F.close(file) >= 0);
            Assert.IsTrue(H5F.is_hdf5(fname) > 0);
            File.Delete(fname);
        }

        [TestMethod]
        public void H5Fis_hdf5Test2()
        {
            Assert.IsTrue(H5F.is_hdf5("") < 0);
            string fname = Path.GetTempFileName();
            Assert.IsTrue(H5F.is_hdf5(fname) == 0);
            File.Delete(fname);
        }
    }
}

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
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HDF.PInvoke;

using herr_t = System.Int32;
using hsize_t = System.UInt64;

using hid_t = System.Int64;

namespace UnitTests
{
    public partial class H5DTest
    {
        [TestMethod]
        public void H5Dget_access_plistTest1()
        {
            hid_t dset = H5D.create(m_v0_test_file, "dset", H5T.IEEE_F64BE,
                                        m_space_null);
            Assert.IsTrue(dset >= 0);

            hid_t dapl = H5D.get_access_plist(dset);
            Assert.IsTrue(dapl >= 0);

            Assert.IsTrue(H5P.close(dapl) >= 0);
            Assert.IsTrue(H5D.close(dset) >= 0);
        }

        [TestMethod]
        public void H5Dget_access_plistTest2()
        {
            Assert.IsFalse(
                                        H5D.get_access_plist(Utilities.RandomInvalidHandle()) >= 0);
        }
    }
}

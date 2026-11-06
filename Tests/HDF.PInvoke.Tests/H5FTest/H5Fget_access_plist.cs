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

using hid_t = System.Int64;

namespace UnitTests
{
    public partial class H5FTest
    {
        [TestMethod]
        public void H5Fget_access_plistTest1()
        {
            hid_t fapl = H5F.get_access_plist(m_v0_class_file);
            Assert.IsTrue(fapl >= 0);
            Assert.IsTrue(H5P.close(fapl) >= 0);
            fapl = H5F.get_access_plist(m_v2_class_file);
            Assert.IsTrue(fapl >= 0);
            Assert.IsTrue(H5P.close(fapl) >= 0);
        }

        [TestMethod]
        public void H5Fget_access_plistTest2()
        {
            Assert.IsFalse(
                                        H5F.get_access_plist(Utilities.RandomInvalidHandle()) >= 0);
        }
    }
}

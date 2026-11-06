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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HDF.PInvoke;

using hid_t = System.Int64;

namespace UnitTests
{
    public partial class H5GTest
    {
        [TestMethod]
        public void H5GopenTest1()
        {
            hid_t group = H5G.open(m_v0_class_file, ".");
            Assert.IsTrue(group >= 0);
            Assert.IsTrue(H5G.close(group) >= 0);

            group = H5G.open(m_v2_class_file, ".");
            Assert.IsTrue(group >= 0);
            Assert.IsTrue(H5G.close(group) >= 0);
        }

        [TestMethod]
        public void H5GopenTest2()
        {
            hid_t group = H5G.open(Utilities.RandomInvalidHandle(), ".");
            Assert.IsTrue(group < 0);

            group = H5G.open(Utilities.RandomInvalidHandle(), "A");
            Assert.IsTrue(group < 0);
        }
    }
}

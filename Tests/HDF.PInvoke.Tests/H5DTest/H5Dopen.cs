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
using HDF.PInvoke;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using haddr_t = System.UInt64;
using herr_t = System.Int32;
using hid_t = System.Int64;

namespace UnitTests
{
    /// <summary>
    /// Groups regression checks for the H5D API.
    /// </summary>
    public partial class H5DTest
    {
        /// <summary>
        /// Exercises the native <c>H5Dopen</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5DopenTest1()
        {
            Assert.IsTrue(
                                        H5D.close(H5D.create(m_v0_test_file, "dset", H5T.IEEE_F64BE,
                                        m_space_null)) >= 0);
            var dset = H5D.open(m_v0_test_file, "dset");
            Assert.IsTrue(dset >= 0);
            Assert.IsTrue(H5D.close(dset) >= 0);

            Assert.IsTrue(
                H5D.close(H5D.create(m_v2_test_file, "dset", H5T.IEEE_F64BE,
                m_space_null)) >= 0);
            dset = H5D.open(m_v0_test_file, "dset");
            Assert.IsTrue(dset >= 0);
            Assert.IsTrue(H5D.close(dset) >= 0);
        }

        /// <summary>
        /// Exercises the native <c>H5Dopen</c> operation in scenario 2.
        /// </summary>
        [TestMethod]
        public void H5DopenTest2()
        {
            Assert.IsFalse(
                                        H5D.open(Utilities.RandomInvalidHandle(), "dset") >= 0);
        }
    }
}

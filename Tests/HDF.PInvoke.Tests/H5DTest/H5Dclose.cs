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
using herr_t = System.Int32;
using hid_t = System.Int64;
using hsize_t = System.UInt64;

namespace UnitTests
{
    /// <summary>
    /// Groups regression checks for the H5D API.
    /// </summary>
    public partial class H5DTest
    {
        /// <summary>
        /// Exercises the native <c>H5Dclose</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5DcloseTest1()
        {
            var dset = H5D.create_anon(m_v0_test_file, H5T.STD_I16LE,
                                                                m_space_null);
            Assert.IsTrue(dset >= 0);
            Assert.IsTrue(H5D.close(dset) >= 0);

            dset = H5D.create_anon(m_v2_test_file, H5T.STD_I16LE, m_space_null);
            Assert.IsTrue(dset >= 0);
            Assert.IsTrue(H5D.close(dset) >= 0);
        }

        /// <summary>
        /// Exercises the native <c>H5Dclose</c> operation in scenario 2.
        /// </summary>
        [TestMethod]
        public void H5DcloseTest2()
        {
            Assert.IsFalse(H5D.close(Utilities.RandomInvalidHandle()) >= 0);
        }
    }
}

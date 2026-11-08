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
        /// Exercises the native <c>H5Dget_space_status</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5Dget_space_statusTest1()
        {
            hsize_t[] dims = { 1024, 2048 };
            var space = H5S.create_simple(3, dims, null);

            var dset = H5D.create(m_v0_test_file, "dset", H5T.STD_I16LE,
                space);
            Assert.IsTrue(dset >= 0);
            var status = H5D.space_status_t.ERROR;
            Assert.IsTrue(H5D.get_space_status(dset, ref status) >= 0);
            Assert.IsTrue(status == H5D.space_status_t.NOT_ALLOCATED);
            Assert.IsTrue(H5D.close(dset) >= 0);

            dset = H5D.create(m_v2_test_file, "dset", H5T.STD_I16LE,
                space);
            Assert.IsTrue(dset >= 0);
            status = H5D.space_status_t.ERROR;
            Assert.IsTrue(H5D.get_space_status(dset, ref status) >= 0);
            Assert.IsTrue(status == H5D.space_status_t.NOT_ALLOCATED);
            Assert.IsTrue(H5D.close(dset) >= 0);

            Assert.IsTrue(H5S.close(space) >= 0);
        }

        /// <summary>
        /// Exercises the native <c>H5Dget_space_status</c> operation in scenario 2.
        /// </summary>
        [TestMethod]
        public void H5Dget_space_statusTest2()
        {
            var dset = H5D.create(m_v0_test_file, "dset", H5T.STD_I16LE,
                                                                m_space_null);
            Assert.IsTrue(dset >= 0);
            var status = H5D.space_status_t.ERROR;
            Assert.IsTrue(H5D.get_space_status(dset, ref status) >= 0);
            Assert.IsTrue(status == H5D.space_status_t.NOT_ALLOCATED);
            Assert.IsTrue(H5D.close(dset) >= 0);

            dset = H5D.create(m_v2_test_file, "dset", H5T.STD_I16LE,
                m_space_null);
            Assert.IsTrue(dset >= 0);
            status = H5D.space_status_t.ERROR;
            Assert.IsTrue(H5D.get_space_status(dset, ref status) >= 0);
            Assert.IsTrue(status == H5D.space_status_t.NOT_ALLOCATED);
            Assert.IsTrue(H5D.close(dset) >= 0);
        }

        /// <summary>
        /// Exercises the native <c>H5Dget_space_status</c> operation in scenario 3.
        /// </summary>
        [TestMethod]
        public void H5Dget_space_statusTest3()
        {
            var status = H5D.space_status_t.ERROR;
            Assert.IsFalse(H5D.get_space_status(Utilities.RandomInvalidHandle(),
                ref status) >= 0);
        }
    }
}

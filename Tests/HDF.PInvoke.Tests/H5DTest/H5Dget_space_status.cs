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
        public void H5Dget_space_statusTest1()
        {
            hsize_t[] dims = { 1024, 2048 };
            hid_t space = H5S.create_simple(3, dims, null);

            hid_t dset = H5D.create(m_v0_test_file, "dset", H5T.STD_I16LE,
                space);
            Assert.IsTrue(dset >= 0);
            H5D.space_status_t status = H5D.space_status_t.ERROR;
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

        [TestMethod]
        public void H5Dget_space_statusTest2()
        {
            hid_t dset = H5D.create(m_v0_test_file, "dset", H5T.STD_I16LE,
                                        m_space_null);
            Assert.IsTrue(dset >= 0);
            H5D.space_status_t status = H5D.space_status_t.ERROR;
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

        [TestMethod]
        public void H5Dget_space_statusTest3()
        {
            H5D.space_status_t status = H5D.space_status_t.ERROR;
            Assert.IsFalse(H5D.get_space_status(Utilities.RandomInvalidHandle(),
                ref status) >= 0);
        }
    }
}

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

using hsize_t = System.UInt64;

using hid_t = System.Int64;

namespace UnitTests
{
    public partial class H5ATest
    {
        [TestMethod]
        public void H5Aget_storage_sizeTest1()
        {
            hid_t att = H5A.create(m_v2_test_file, "A", H5T.IEEE_F64LE,
                                        m_space_scalar);
            Assert.IsTrue(att >= 0);
            hsize_t size = H5A.get_storage_size(att);
            Assert.IsTrue(size == 8);
            Assert.IsTrue(H5A.close(att) >= 0);

            att = H5A.create(m_v0_test_file, "A", H5T.IEEE_F64LE,
                m_space_scalar);
            Assert.IsTrue(att >= 0);
            size = H5A.get_storage_size(att);
            Assert.IsTrue(size == 8);
            Assert.IsTrue(H5A.close(att) >= 0);
        }

        [TestMethod]
        public void H5Aget_storage_sizeTest2()
        {
            Assert.IsFalse(
                                        H5A.get_storage_size(Utilities.RandomInvalidHandle()) > 0);
        }
    }
}

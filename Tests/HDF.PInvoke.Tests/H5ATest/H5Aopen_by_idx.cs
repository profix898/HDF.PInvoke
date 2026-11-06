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
using htri_t = System.Int32;

using hid_t = System.Int64;

namespace UnitTests
{
    /// <summary>
    /// Groups regression checks for the H5A API.
    /// </summary>
    public partial class H5ATest
    {
        /// <summary>
        /// Exercises the native <c>H5Aopen_by_idx</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5Aopen_by_idxTest1()
        {
            hid_t att = H5A.create(m_v2_test_file, "A", H5T.IEEE_F64LE,
                                        m_space_scalar);
            Assert.IsTrue(att >= 0);
            Assert.IsTrue(H5A.close(att) >= 0);
            att = H5A.create(m_v2_test_file, "AA", H5T.IEEE_F64LE,
                m_space_scalar);
            Assert.IsTrue(att >= 0);
            Assert.IsTrue(H5A.close(att) >= 0);

            att = H5A.open_by_idx(m_v2_test_file, ".", H5.index_t.NAME,
                H5.iter_order_t.NATIVE, 0);
            Assert.IsTrue(att >= 0);
            Assert.IsTrue(H5A.close(att) >= 0);

            att = H5A.open_by_idx(m_v2_test_file, ".", H5.index_t.NAME,
                H5.iter_order_t.NATIVE, 1);
            Assert.IsTrue(att >= 0);
            Assert.IsTrue(H5A.close(att) >= 0);

            att = H5A.create(m_v0_test_file, "A", H5T.IEEE_F64LE,
                m_space_scalar);
            Assert.IsTrue(att >= 0);
            Assert.IsTrue(H5A.close(att) >= 0);

            att = H5A.open_by_idx(m_v0_test_file, ".", H5.index_t.NAME,
                H5.iter_order_t.NATIVE, 0);
            Assert.IsTrue(att >= 0);
            Assert.IsTrue(H5A.close(att) >= 0);
        }

        /// <summary>
        /// Exercises the native <c>H5Aopen_by_idx</c> operation in scenario 2.
        /// </summary>
        [TestMethod]
        public void H5Aopen_by_idxTest2()
        {
            Assert.IsFalse(
                                        H5A.open_by_idx(Utilities.RandomInvalidHandle(), ".",
                                        H5.index_t.NAME, H5.iter_order_t.NATIVE,
                                        44) >= 0);
            Assert.IsFalse(
                H5A.open_by_idx(m_v2_class_file, ".",
                H5.index_t.NAME, H5.iter_order_t.NATIVE,
                hsize_t.MaxValue) >= 0);
        }
    }
}

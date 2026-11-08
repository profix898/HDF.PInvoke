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
using HDF.PInvoke;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using hid_t = System.Int64;

namespace UnitTests
{
    /// <summary>
    /// Groups regression checks for the H5A API.
    /// </summary>
    public partial class H5ATest
    {
        /// <summary>
        /// Exercises the native <c>H5Adelete_by_idx</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5Adelete_by_idxTest1()
        {
            // use test-local files, because we don't know what's out there
            // at the class level, create two attributes in each file

            var att = H5A.create(m_v0_test_file, "DNA", H5T.IEEE_F32BE,
                m_space_null);
            Assert.IsTrue(att >= 0);
            Assert.IsTrue(H5A.close(att) >= 0);

            att = H5A.create(m_v2_test_file, "DNA", H5T.IEEE_F32BE,
               m_space_null);
            Assert.IsTrue(att >= 0);
            Assert.IsTrue(H5A.close(att) >= 0);

            att = H5A.create(m_v0_test_file, "DSA", H5T.IEEE_F32BE,
                m_space_scalar);
            Assert.IsTrue(att >= 0);
            Assert.IsTrue(H5A.close(att) >= 0);

            att = H5A.create(m_v2_test_file, "DSA", H5T.IEEE_F32BE,
               m_space_scalar);
            Assert.IsTrue(att >= 0);
            Assert.IsTrue(H5A.close(att) >= 0);

            // we have two attributes, delete the one in first position twice
            Assert.IsTrue(H5A.delete_by_idx(m_v0_test_file, ".",
                H5.index_t.NAME,
                H5.iter_order_t.NATIVE, 0) >= 0);
            Assert.IsTrue(H5A.delete_by_idx(m_v0_test_file, ".",
                H5.index_t.NAME,
                H5.iter_order_t.NATIVE, 0) >= 0);

            // we have two attributes, first delete the one in second position
            // then the one in first position
            Assert.IsTrue(H5A.delete_by_idx(m_v2_test_file, ".",
                H5.index_t.NAME,
                H5.iter_order_t.NATIVE, 1) >= 0);
            Assert.IsTrue(H5A.delete_by_idx(m_v2_test_file, ".",
                H5.index_t.NAME,
                H5.iter_order_t.NATIVE, 0) >= 0);
        }

        /// <summary>
        /// Exercises the native <c>H5Adelete_by_idx</c> operation in scenario 2.
        /// </summary>
        [TestMethod]
        public void H5Adelete_by_idxTest2()
        {
            Assert.IsFalse(
                                        H5A.delete_by_idx(Utilities.RandomInvalidHandle(), ".",
                                        H5.index_t.NAME, H5.iter_order_t.NATIVE, 10)
                                        >= 0);
            Assert.IsFalse(
                H5A.delete_by_idx(m_v0_class_file, ".",
                H5.index_t.NAME, H5.iter_order_t.NATIVE, 1024)
                >= 0);
        }
    }
}

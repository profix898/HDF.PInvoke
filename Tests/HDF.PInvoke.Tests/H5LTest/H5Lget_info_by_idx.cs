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

using size_t = System.IntPtr;

using hid_t = System.Int64;

namespace UnitTests
{
    /// <summary>
    /// Groups regression checks for the H5L API.
    /// </summary>
    public partial class H5LTest
    {
        /// <summary>
        /// Exercises the native <c>H5Lget_info_by_idx</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5Lget_info_by_idxTest1()
        {
            Assert.IsTrue(
                                        H5L.create_external(m_v0_class_file_name, "/", m_v0_test_file,
                                        "A", m_lcpl) >= 0);
            Assert.IsTrue(
                H5L.create_external(m_v0_class_file_name, "/", m_v0_test_file,
                "AB", m_lcpl) >= 0);
            Assert.IsTrue(
                H5L.create_external(m_v0_class_file_name, "/", m_v0_test_file,
                "ABC", m_lcpl) >= 0);

            H5L.info_t info = new H5L.info_t();
            Assert.IsTrue(H5L.get_info_by_idx(m_v0_test_file, ".",
                H5.index_t.NAME, H5.iter_order_t.NATIVE, 1, ref info) >= 0);
            Assert.IsTrue(info.type == H5L.type_t.EXTERNAL);
            Assert.IsTrue(info.corder_valid == 0);
            Assert.IsTrue(info.cset == H5T.cset_t.ASCII);
            Assert.IsTrue(info.u.val_size.ToInt64() > 0);

            Assert.IsTrue(
                H5L.create_external(m_v2_class_file_name, "/", m_v2_test_file,
                "A", m_lcpl) >= 0);
            Assert.IsTrue(
                H5L.create_external(m_v2_class_file_name, "/", m_v2_test_file,
                "AB", m_lcpl) >= 0);
            Assert.IsTrue(
                H5L.create_external(m_v2_class_file_name, "/", m_v2_test_file,
                "ABC", m_lcpl) >= 0);

            Assert.IsTrue(H5L.get_info_by_idx(m_v2_test_file, ".",
                H5.index_t.NAME, H5.iter_order_t.NATIVE, 1, ref info) >= 0);
            Assert.IsTrue(info.type == H5L.type_t.EXTERNAL);
            Assert.IsTrue(info.corder_valid == 0);
            Assert.IsTrue(info.cset == H5T.cset_t.ASCII);
            Assert.IsTrue(info.u.val_size.ToInt64() > 0);
        }
    }
}

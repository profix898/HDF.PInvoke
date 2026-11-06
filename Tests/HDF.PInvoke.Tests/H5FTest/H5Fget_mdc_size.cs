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
    public partial class H5FTest
    {
        [TestMethod]
        public void H5Fget_mdc_sizeTest1()
        {
            size_t max_size = IntPtr.Zero, min_clean_size = IntPtr.Zero,
                                        cur_size = IntPtr.Zero;
            int cur_num_entries = 0;
            Assert.IsTrue(H5F.get_mdc_size(m_v0_class_file, ref max_size,
                ref min_clean_size, ref cur_size, ref cur_num_entries) >= 0);
            Assert.IsTrue(H5F.get_mdc_size(m_v2_class_file, ref max_size,
                ref min_clean_size, ref cur_size, ref cur_num_entries) >= 0);
            Assert.IsTrue(H5F.get_mdc_size(m_v0_test_file, ref max_size,
                ref min_clean_size, ref cur_size, ref cur_num_entries) >= 0);
            Assert.IsTrue(H5F.get_mdc_size(m_v2_test_file, ref max_size,
                ref min_clean_size, ref cur_size, ref cur_num_entries) >= 0);
        }

        [TestMethod]
        public void H5Fget_mdc_sizeTest2()
        {
            size_t max_size = IntPtr.Zero, min_clean_size = IntPtr.Zero,
                                        cur_size = IntPtr.Zero;
            int cur_num_entries = 0;
            Assert.IsFalse(
                H5F.get_mdc_size(Utilities.RandomInvalidHandle(), ref max_size,
                ref min_clean_size, ref cur_size, ref cur_num_entries) >= 0);
        }
    }
}

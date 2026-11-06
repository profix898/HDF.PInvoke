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

using hid_t = System.Int64;

namespace UnitTests
{
    public partial class H5FTest
    {
        [TestMethod]
        public void H5Fget_mdc_hit_rateTest1()
        {
            double rate = 0.0;
            Assert.IsTrue(H5F.get_mdc_hit_rate(m_v0_class_file, ref rate) >= 0);
            Assert.IsTrue(H5F.get_mdc_hit_rate(m_v2_class_file, ref rate) >= 0);
            Assert.IsTrue(H5F.get_mdc_hit_rate(m_v0_test_file, ref rate) >= 0);
            Assert.IsTrue(H5F.get_mdc_hit_rate(m_v2_test_file, ref rate) >= 0);
        }

        [TestMethod]
        public void H5Fget_mdc_hit_rateTest2()
        {
            double rate = 0.0;
            Assert.IsFalse(
                H5F.get_mdc_hit_rate(Utilities.RandomInvalidHandle(),
                ref rate) >= 0);
        }
    }
}

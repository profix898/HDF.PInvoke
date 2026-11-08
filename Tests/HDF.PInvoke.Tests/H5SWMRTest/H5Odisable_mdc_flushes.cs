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
using System.Runtime.InteropServices;
using HDF.PInvoke;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using hbool_t = System.Byte;
using herr_t = System.Int32;
using hid_t = System.Int64;
using hsize_t = System.UInt64;

namespace UnitTests
{
    /// <summary>
    /// Groups regression checks for the H5SWMR API.
    /// </summary>
    public partial class H5SWMRTest
    {
        /// <summary>
        /// Exercises the native <c>H5Odisable_mdc_flushes</c> operation in scenario SWMR1.
        /// </summary>
        [TestMethod]
        public void H5Odisable_mdc_flushesTestSWMR1()
        {
            var grp = H5G.create(m_v3_test_file_swmr, "/A/B/C", m_lcpl);
            Assert.IsTrue(grp >= 0);
            Assert.IsTrue(H5O.disable_mdc_flushes(grp) >= 0);
            Assert.IsTrue(H5G.flush(grp) >= 0);
            Assert.IsTrue(H5G.close(grp) >= 0);
        }

        /// <summary>
        /// Exercises the native <c>H5Odisable_mdc_flushes</c> operation in scenario SWMR2.
        /// </summary>
        [TestMethod]
        public void H5Odisable_mdc_flushesTestSWMR2()
        {
            var grp = H5G.create(m_v3_test_file_no_swmr, "/A/B/C", m_lcpl);
            Assert.IsTrue(grp >= 0);
            Assert.IsTrue(H5O.disable_mdc_flushes(grp) >= 0);
            Assert.IsTrue(H5G.flush(grp) >= 0);
            Assert.IsTrue(H5G.close(grp) >= 0);
        }

        /// <summary>
        /// Exercises the native <c>H5Odisable_mdc_flushes</c> operation in scenario SWMR3.
        /// </summary>
        [TestMethod]
        public void H5Odisable_mdc_flushesTestSWMR3()
        {
            var grp = H5G.create(m_v3_test_file_swmr, "/A/B/C", m_lcpl);
            Assert.IsTrue(grp >= 0);

            hbool_t flag = 11;
            Assert.IsTrue(H5O.are_mdc_flushes_disabled(grp, ref flag) >= 0);
            Console.WriteLine(flag);
            Assert.IsTrue(flag == 0);

            Assert.IsTrue(H5O.disable_mdc_flushes(grp) >= 0);

            Assert.IsTrue(H5O.are_mdc_flushes_disabled(grp, ref flag) >= 0);
            Assert.IsTrue(flag > 0);

            Assert.IsTrue(H5G.flush(grp) >= 0);

            Assert.IsTrue(H5G.close(grp) >= 0);
        }
    }
}

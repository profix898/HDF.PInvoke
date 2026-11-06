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
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HDF.PInvoke;

using hbool_t = System.Byte;
using herr_t = System.Int32;

using hid_t = System.Int64;

namespace UnitTests
{
    /// <summary>
    /// Groups regression checks for the H5SWMR API.
    /// </summary>
    public partial class H5SWMRTest
    {
        /// <summary>
        /// Exercises the native <c>H5Fstart_mdc_logging</c> operation in scenario SWMR1.
        /// </summary>
        [TestMethod]
        public void H5Fstart_mdc_loggingTestSWMR1()
        {
            hid_t fapl = H5P.create(H5P.FILE_ACCESS);
            Assert.IsTrue(fapl >= 0);
            Assert.IsTrue(
                H5P.set_libver_bounds(fapl, H5F.libver_t.LATEST) >= 0);

            hbool_t is_enabled = 1;
            string location = "mdc.log";
            hbool_t start_on_access = 0;

            Assert.IsTrue(
                H5P.set_mdc_log_options(fapl, is_enabled, location,
                start_on_access) >= 0);

            string fileName = Path.GetTempFileName();
            hid_t file = H5F.create(fileName, H5F.ACC_TRUNC, H5P.DEFAULT, fapl);
            Assert.IsTrue(file >= 0);

            Assert.IsTrue(H5F.start_mdc_logging(file) >= 0);

            hid_t group = H5G.create(file, "/A/B/C", m_lcpl);
            Assert.IsTrue(group >= 0);
            Assert.IsTrue(H5G.close(group) >= 0);

            Assert.IsTrue(H5F.stop_mdc_logging(file) >= 0);

            Assert.IsTrue(H5F.close(file) >= 0);

            Assert.IsTrue(H5P.close(fapl) >= 0);

            File.Delete("mdc.log");
        }
    }
}

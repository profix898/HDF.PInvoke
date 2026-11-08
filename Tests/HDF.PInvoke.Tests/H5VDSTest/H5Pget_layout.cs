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
using herr_t = System.Int32;
using hid_t = System.Int64;

namespace UnitTests
{
    /// <summary>
    /// Groups regression checks for the H5VDS API.
    /// </summary>
    public partial class H5VDSTest
    {
        /// <summary>
        /// Exercises the native <c>H5Pget_layout</c> operation in scenario VDS1.
        /// </summary>
        [TestMethod]
        public void H5Pget_layoutTestVDS1()
        {
            var vds = H5D.open(m_vds_class_file, "VDS");
            Assert.IsTrue(vds >= 0);

            var dcpl = H5D.get_create_plist(vds);
            Assert.IsTrue(dcpl >= 0);

            var layout = H5P.get_layout(dcpl);
            Assert.IsTrue(layout == H5D.layout_t.VIRTUAL);

            Assert.IsTrue(H5P.close(dcpl) >= 0);
            Assert.IsTrue(H5D.close(vds) >= 0);
        }
    }
}

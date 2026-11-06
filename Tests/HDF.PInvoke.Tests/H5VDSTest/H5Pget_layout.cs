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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HDF.PInvoke;

using herr_t = System.Int32;

using hid_t = System.Int64;

namespace UnitTests
{
    public partial class H5VDSTest
    {
        [TestMethod]
        public void H5Pget_layoutTestVDS1()
        {
            hid_t vds = H5D.open(m_vds_class_file, "VDS");
            Assert.IsTrue(vds >= 0);

            hid_t dcpl = H5D.get_create_plist(vds);
            Assert.IsTrue(dcpl >= 0);

            H5D.layout_t layout = H5P.get_layout(dcpl);
            Assert.IsTrue(layout == H5D.layout_t.VIRTUAL);

            Assert.IsTrue(H5P.close(dcpl) >= 0);
            Assert.IsTrue(H5D.close(vds) >= 0);
        }
    }
}

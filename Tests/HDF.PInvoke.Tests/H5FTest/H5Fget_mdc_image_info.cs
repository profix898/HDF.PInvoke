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

using haddr_t = System.UInt64;
using hsize_t = System.UInt64;

namespace UnitTests
{
    public partial class H5FTest
    {
        [TestMethod]
        public void H5Fget_mdc_image_infoTest1()
        {
            haddr_t image_addr = 111;
            hsize_t image_size = 222;

            Assert.IsTrue(H5F.get_mdc_image_info(m_v2_class_file,
                ref image_addr,
                ref image_size) >= 0);
            Assert.IsTrue(image_addr == H5.HADDR_UNDEF);
            Assert.IsTrue(image_size == 0);
        }
    }
}

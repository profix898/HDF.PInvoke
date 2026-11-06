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
using size_t = System.IntPtr;

using hid_t = System.Int64;

namespace UnitTests
{
    /// <summary>
    /// Groups regression checks for the H5VDS API.
    /// </summary>
    public partial class H5VDSTest
    {
        /// <summary>
        /// Exercises the native <c>H5Pget_virtual_srcspace</c> operation in scenario VDS1.
        /// </summary>
        [TestMethod]
        public void H5Pget_virtual_srcspaceTestVDS1()
        {
            hid_t vds = H5D.open(m_vds_class_file, "VDS");
            Assert.IsTrue(vds >= 0);

            hid_t dcpl = H5D.get_create_plist(vds);
            Assert.IsTrue(dcpl >= 0);

            IntPtr count = IntPtr.Zero;
            Assert.IsTrue(H5P.get_virtual_count(dcpl, ref count) >= 0);
            Assert.IsTrue(3 == count.ToInt32());

            for (int i = 0; i < count.ToInt32(); ++i)
            {
                size_t index = new size_t(i);
                hid_t srcspace = H5P.get_virtual_srcspace(dcpl, index);
                Assert.IsTrue(srcspace >= 0);

                Assert.IsTrue(H5S.get_select_type(srcspace) ==
                    H5S.sel_type.ALL);

                Assert.IsTrue(H5S.close(srcspace) >= 0);
            }

            Assert.IsTrue(H5P.close(dcpl) >= 0);
            Assert.IsTrue(H5D.close(vds) >= 0);
        }
    }
}

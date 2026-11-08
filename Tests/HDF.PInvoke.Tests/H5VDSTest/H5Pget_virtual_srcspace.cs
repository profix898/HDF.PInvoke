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
using size_t = System.IntPtr;

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
            var vds = H5D.open(m_vds_class_file, "VDS");
            Assert.IsTrue(vds >= 0);

            var dcpl = H5D.get_create_plist(vds);
            Assert.IsTrue(dcpl >= 0);

            var count = IntPtr.Zero;
            Assert.IsTrue(H5P.get_virtual_count(dcpl, ref count) >= 0);
            Assert.IsTrue(3 == count.ToInt32());

            for (var i = 0; i < count.ToInt32(); ++i)
            {
                var index = new size_t(i);
                var srcspace = H5P.get_virtual_srcspace(dcpl, index);
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

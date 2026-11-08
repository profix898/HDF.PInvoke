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
using System.Text;
using HDF.PInvoke;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using herr_t = System.Int32;
using hid_t = System.Int64;
using size_t = System.IntPtr;
using ssize_t = System.IntPtr;

namespace UnitTests
{
    /// <summary>
    /// Groups regression checks for the H5VDS API.
    /// </summary>
    public partial class H5VDSTest
    {
        /// <summary>
        /// Exercises the native <c>H5Pget_virtual_dsetname</c> operation in scenario VDS1.
        /// </summary>
        [TestMethod]
        public void H5Pget_virtual_dsetnameTestVDS1()
        {
            var vds = H5D.open(m_vds_class_file, "VDS");
            Assert.IsTrue(vds >= 0);

            var dcpl = H5D.get_create_plist(vds);
            Assert.IsTrue(dcpl >= 0);

            var count = IntPtr.Zero;
            Assert.IsTrue(H5P.get_virtual_count(dcpl, ref count) >= 0);
            Assert.IsTrue(3 == count.ToInt32());

            string[] names = { "A", "B", "C" };

            for (var i = 0; i < count.ToInt32(); ++i)
            {
                var index = new ssize_t(i);
                var len = H5P.get_virtual_filename(dcpl, index, null,
                    IntPtr.Zero);
                Assert.IsTrue(len.ToInt32() > 0);
                var name = new StringBuilder(len.ToInt32() + 1);
                len = H5P.get_virtual_dsetname(dcpl, index, name, len + 1);
                Assert.IsTrue(len.ToInt32() > 0);
                Assert.IsTrue(name.ToString() == names[i]);
            }

            Assert.IsTrue(H5P.close(dcpl) >= 0);
            Assert.IsTrue(H5D.close(vds) >= 0);
        }
    }
}

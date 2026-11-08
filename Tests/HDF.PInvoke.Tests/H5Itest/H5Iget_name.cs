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
using System.Text;
using HDF.PInvoke;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using hid_t = System.Int64;
using ssize_t = System.IntPtr;

namespace UnitTests
{
    /// <summary>
    /// Groups regression checks for the H5I API.
    /// </summary>
    public partial class H5ITest
    {
        /// <summary>
        /// Exercises the native <c>H5Iget_name</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5Iget_nameTest1()
        {
            var gid = H5G.create(m_v0_test_file, "AAAAAAAAAAAAAAAAAAAAA");
            Assert.IsTrue(gid > 0);

            var buf_size = H5I.get_name(gid, (StringBuilder)null,
                IntPtr.Zero) + 1;
            Assert.IsTrue(buf_size.ToInt32() > 1);
            var nameBuilder = new StringBuilder(buf_size.ToInt32());
            var size = H5I.get_name(gid, nameBuilder, buf_size);
            Assert.IsTrue(size.ToInt32() > 0);
            Assert.IsTrue(nameBuilder.ToString() == "/AAAAAAAAAAAAAAAAAAAAA");

            Assert.IsTrue(H5G.close(gid) >= 0);

            gid = H5G.create(m_v2_test_file, "AAAAAAAAAAAAAAAAAAAAA");
            Assert.IsTrue(gid > 0);

            buf_size = H5I.get_name(gid, (StringBuilder)null, IntPtr.Zero) + 1;
            Assert.IsTrue(buf_size.ToInt32() > 1);
            nameBuilder = new StringBuilder(buf_size.ToInt32());
            size = H5I.get_name(gid, nameBuilder, buf_size);
            Assert.IsTrue(size.ToInt32() > 0);
            Assert.IsTrue(nameBuilder.ToString() == "/AAAAAAAAAAAAAAAAAAAAA");

            Assert.IsTrue(H5G.close(gid) >= 0);
        }

        /// <summary>
        /// Exercises the native <c>H5Iget_name</c> operation in scenario 2.
        /// </summary>
        [TestMethod]
        public void H5Iget_nameTest2()
        {
            var size = H5I.get_name(Utilities.RandomInvalidHandle(), null,
                                        IntPtr.Zero);
            Assert.IsFalse(size.ToInt32() >= 0);
        }
    }
}

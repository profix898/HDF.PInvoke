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
using HDF.PInvoke;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using hid_t = System.Int64;

namespace UnitTests
{
    /// <summary>
    /// Groups regression checks for the H5F API.
    /// </summary>
    public partial class H5FTest
    {
        /// <summary>
        /// Exercises the native <c>H5Funmount</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5FunmountTest1()
        {
            var mount_point = H5G.create(m_v0_class_file, "UA");
            Assert.IsTrue(mount_point >= 0);
            Assert.IsTrue(H5G.close(mount_point) >= 0);
            Assert.IsTrue(H5F.mount(m_v0_class_file, "UA", m_v0_test_file) >= 0);

            mount_point = H5G.create(m_v0_class_file, "UB");
            Assert.IsTrue(mount_point >= 0);
            Assert.IsTrue(H5G.close(mount_point) >= 0);
            Assert.IsTrue(H5F.mount(m_v0_class_file, "UB", m_v2_test_file) >= 0);

            mount_point = H5G.create(m_v2_class_file, "UC");
            Assert.IsTrue(mount_point >= 0);
            Assert.IsTrue(H5G.close(mount_point) >= 0);
            Assert.IsTrue(H5F.mount(m_v2_class_file, "UC", m_v0_class_file) >= 0);

            Assert.IsTrue(H5F.unmount(m_v2_class_file, "UC") >= 0);
            Assert.IsTrue(H5F.unmount(m_v0_class_file, "UB") >= 0);
            Assert.IsTrue(H5F.unmount(m_v0_class_file, "UA") >= 0);
        }

        /// <summary>
        /// Exercises the native <c>H5Funmount</c> operation in scenario 2.
        /// </summary>
        [TestMethod]
        public void H5FunmountTest2()
        {
            Assert.IsFalse(
                                                                H5F.unmount(Utilities.RandomInvalidHandle(), "AA") >= 0);

            Assert.IsFalse(H5F.unmount(m_v0_class_file, "") >= 0);
        }
    }
}

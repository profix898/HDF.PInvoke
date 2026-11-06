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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HDF.PInvoke;

using hid_t = System.Int64;

namespace UnitTests
{
    public partial class H5FTest
    {
        [TestMethod]
        public void H5FmountTest1()
        {
            hid_t mount_point = H5G.create(m_v0_class_file, "A");
            Assert.IsTrue(mount_point >= 0);
            Assert.IsTrue(H5G.close(mount_point) >= 0);
            Assert.IsTrue(H5F.mount(m_v0_class_file, "A", m_v0_test_file) >= 0);

            mount_point = H5G.create(m_v0_class_file, "B");
            Assert.IsTrue(mount_point >= 0);
            Assert.IsTrue(H5G.close(mount_point) >= 0);
            Assert.IsTrue(H5F.mount(m_v0_class_file, "B", m_v2_test_file) >= 0);

            mount_point = H5G.create(m_v2_class_file, "C");
            Assert.IsTrue(mount_point >= 0);
            Assert.IsTrue(H5G.close(mount_point) >= 0);
            Assert.IsTrue(H5F.mount(m_v2_class_file, "C", m_v0_class_file) >= 0);

            Assert.IsTrue(H5F.unmount(m_v2_class_file, "C") >= 0);
            Assert.IsTrue(H5F.unmount(m_v0_class_file, "B") >= 0);
            Assert.IsTrue(H5F.unmount(m_v0_class_file, "A") >= 0);
        }

        [TestMethod]
        public void H5FmountTest2()
        {
            hid_t mount_point = H5G.create(m_v0_class_file, "AA");
            Assert.IsTrue(mount_point >= 0);
            Assert.IsFalse(H5F.mount(mount_point, "AA",
                Utilities.RandomInvalidHandle()) >= 0);
            Assert.IsTrue(H5G.close(mount_point) >= 0);

            // can't mount a file onto itself
            mount_point = H5G.create(m_v0_class_file, "BB");
            Assert.IsTrue(mount_point >= 0);
            Assert.IsTrue(H5G.close(mount_point) >= 0);
            Assert.IsFalse(
                H5F.mount(m_v0_class_file, "BB", m_v0_class_file) >= 0);
        }
    }
}

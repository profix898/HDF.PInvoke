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
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;
using HDF.PInvoke;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using hid_t = System.Int64;
using size_t = System.IntPtr;

namespace UnitTests
{
    /// <summary>
    /// Groups regression checks for the H5L API.
    /// </summary>
    public partial class H5LTest
    {
        /// <summary>
        /// Exercises the native <c>H5Lget_val</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5Lget_valTest1()
        {
            var sym_path = String.Join("/", m_utf8strings);
            var bytes = Encoding.UTF8.GetBytes(sym_path);

            Assert.IsTrue(
                H5L.create_soft(bytes, m_v0_test_file,
                Encoding.ASCII.GetBytes("/A/B/C/D" + "\0"), m_lcpl) >= 0);

            var info = new H5L.info_t();
            Assert.IsTrue(
                H5L.get_info(m_v0_test_file, "/A/B/C/D", ref info) >= 0);
            Assert.IsTrue(info.type == H5L.type_t.SOFT);
            Assert.IsTrue(info.corder_valid == 0);
            Assert.IsTrue(info.cset == H5T.cset_t.ASCII);
            var size = info.u.val_size.ToInt32();
            Assert.IsTrue(size == 70);

            // the library appends a null terminator (weired!)
            Assert.IsTrue(size == bytes.Length + 1);

            var buf = new byte[size];

            var hnd = GCHandle.Alloc(buf, GCHandleType.Pinned);
            Assert.IsTrue(H5L.get_val(m_v0_test_file, "/A/B/C/D",
                hnd.AddrOfPinnedObject(), new IntPtr(buf.Length)) >= 0);
            hnd.Free();

            for (var i = 0; i < buf.Length - 1; ++i)
            {
                Assert.IsTrue(buf[i] == bytes[i]);
            }

            Assert.IsTrue(
                H5L.create_soft(bytes, m_v2_test_file,
                Encoding.ASCII.GetBytes("/A/B/C/D" + "\0"), m_lcpl) >= 0);

            info = new H5L.info_t();
            Assert.IsTrue(
                H5L.get_info(m_v2_test_file, "/A/B/C/D", ref info) >= 0);
            Assert.IsTrue(info.type == H5L.type_t.SOFT);
            Assert.IsTrue(info.corder_valid == 0);
            Assert.IsTrue(info.cset == H5T.cset_t.ASCII);
            size = info.u.val_size.ToInt32();
            Assert.IsTrue(size == 70);

            // the library appends a null terminator
            Assert.IsTrue(size == bytes.Length + 1);

            buf = new byte[size - 1];

            hnd = GCHandle.Alloc(buf, GCHandleType.Pinned);
            Assert.IsTrue(H5L.get_val(m_v2_test_file, "/A/B/C/D",
                hnd.AddrOfPinnedObject(), new IntPtr(buf.Length)) >= 0);
            hnd.Free();

            for (var i = 0; i < buf.Length - 1; ++i)
            {
                Assert.IsTrue(buf[i] == bytes[i]);
            }
        }
    }
}

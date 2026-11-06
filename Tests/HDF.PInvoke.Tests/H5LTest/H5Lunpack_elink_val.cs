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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HDF.PInvoke;

using herr_t = System.Int32;
using size_t = System.IntPtr;

using hid_t = System.Int64;

namespace UnitTests
{
    /// <summary>
    /// Groups regression checks for the H5L API.
    /// </summary>
    public partial class H5LTest
    {
        /// <summary>
        /// Exercises the native <c>H5Lunpack_elink_val</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5Lunpack_elink_valTest1()
        {
            // v0 file format

            Assert.IsTrue(
                H5L.create_external(m_v0_class_file_name, "/", m_v0_test_file,
                "A/B/C", m_lcpl) >= 0);

            H5L.info_t info = new H5L.info_t();
            Assert.IsTrue(
                H5L.get_info(m_v0_test_file, "A/B/C", ref info) >= 0);

            Assert.IsTrue(info.type == H5L.type_t.EXTERNAL);
            IntPtr size = new IntPtr(info.u.val_size.ToInt32());
            Assert.IsTrue(size.ToInt32() > 0);

            IntPtr buf = Marshal.AllocHGlobal(size.ToInt32());
            Assert.IsTrue(buf != IntPtr.Zero);
            Assert.IsTrue(
                H5L.get_val(m_v0_test_file, "A/B/C", buf, size) >= 0);

            uint flags = 0;
            IntPtr filename = new IntPtr();
            IntPtr obj_path = new IntPtr();
            Assert.IsTrue(
                H5L.unpack_elink_val(buf, size, ref flags, ref filename,
                ref obj_path) >= 0);

            Assert.IsTrue(
                Marshal.PtrToStringAnsi(filename) == m_v0_class_file_name);
            Assert.IsTrue(Marshal.PtrToStringAnsi(obj_path) == "/");

            Marshal.FreeHGlobal(buf);

            // v2 file format

            Assert.IsTrue(
                H5L.create_external(m_v2_class_file_name, "/", m_v2_test_file,
                "A/B/C", m_lcpl) >= 0);

            info = new H5L.info_t();
            Assert.IsTrue(
                H5L.get_info(m_v2_test_file, "A/B/C", ref info) >= 0);

            Assert.IsTrue(info.type == H5L.type_t.EXTERNAL);
            size = new IntPtr(info.u.val_size.ToInt32());
            Assert.IsTrue(size.ToInt32() > 0);

            buf = Marshal.AllocHGlobal(size.ToInt32());
            Assert.IsTrue(buf != IntPtr.Zero);
            Assert.IsTrue(
                H5L.get_val(m_v2_test_file, "A/B/C", buf, size) >= 0);

            flags = 0;
            filename = new IntPtr();
            obj_path = new IntPtr();
            Assert.IsTrue(
                H5L.unpack_elink_val(buf, size, ref flags, ref filename,
                ref obj_path) >= 0);

            Assert.IsTrue(
                Marshal.PtrToStringAnsi(filename) == m_v2_class_file_name);
            Assert.IsTrue(Marshal.PtrToStringAnsi(obj_path) == "/");

            Marshal.FreeHGlobal(buf);
        }

        /// <summary>
        /// Exercises the native <c>H5Lunpack_elink_val</c> operation in scenario 2.
        /// </summary>
        [TestMethod]
        public void H5Lunpack_elink_valTest2()
        {
            // v0 file format

            byte[] bytes = Encoding.UTF8.GetBytes(m_utf8strings[0]);

            Assert.IsTrue(
                H5L.create_external(m_v0_class_file_name,
                Encoding.ASCII.GetBytes("/"), m_v0_test_file,
                bytes, m_lcpl_utf8) >= 0);

            H5L.info_t info = new H5L.info_t();
            Assert.IsTrue(
                H5L.get_info(m_v0_test_file, bytes, ref info) >= 0);

            Assert.IsTrue(info.type == H5L.type_t.EXTERNAL);
            IntPtr size = new IntPtr(info.u.val_size.ToInt32());
            Assert.IsTrue(size.ToInt32() > 0);

            IntPtr buf = Marshal.AllocHGlobal(size.ToInt32());
            Assert.IsTrue(buf != IntPtr.Zero);
            Assert.IsTrue(
                H5L.get_val(m_v0_test_file, bytes, buf, size) >= 0);

            uint flags = 0;
            IntPtr filename = new IntPtr();
            IntPtr obj_path = new IntPtr();
            Assert.IsTrue(
                H5L.unpack_elink_val(buf, size, ref flags, ref filename,
                ref obj_path) >= 0);

            Assert.IsTrue(
                Marshal.PtrToStringAnsi(filename) == m_v0_class_file_name);
            Assert.IsTrue(Marshal.PtrToStringAnsi(obj_path) == "/");

            Marshal.FreeHGlobal(buf);

            // v2 file format

            Assert.IsTrue(
                H5L.create_external(m_v2_class_file_name,
                Encoding.ASCII.GetBytes("/"), m_v2_test_file,
                bytes, m_lcpl_utf8) >= 0);

            info = new H5L.info_t();
            Assert.IsTrue(
                H5L.get_info(m_v2_test_file, bytes, ref info) >= 0);

            Assert.IsTrue(info.type == H5L.type_t.EXTERNAL);
            size = new IntPtr(info.u.val_size.ToInt32());
            Assert.IsTrue(size.ToInt32() > 0);

            buf = Marshal.AllocHGlobal(size.ToInt32());
            Assert.IsTrue(buf != IntPtr.Zero);
            Assert.IsTrue(
                H5L.get_val(m_v2_test_file, bytes, buf, size) >= 0);

            flags = 0;
            filename = new IntPtr();
            obj_path = new IntPtr();
            Assert.IsTrue(
                H5L.unpack_elink_val(buf, size, ref flags, ref filename,
                ref obj_path) >= 0);

            Assert.IsTrue(
                Marshal.PtrToStringAnsi(filename) == m_v2_class_file_name);
            Assert.IsTrue(Marshal.PtrToStringAnsi(obj_path) == "/");

            Marshal.FreeHGlobal(buf);
        }

        /// <summary>
        /// Exercises the native <c>H5Lunpack_elink_val</c> operation in scenario 3.
        /// </summary>
        [TestMethod]
        public void H5Lunpack_elink_valTest3()
        {
            // v0 file format

            byte[] bytes = Encoding.UTF8.GetBytes((m_utf8strings[0] + "_unpack"));

            Assert.IsTrue(
                H5G.close(H5G.create(m_v0_class_file, bytes, m_lcpl_utf8)) >= 0);

            Assert.IsTrue(
                H5L.create_external(m_v0_class_file_name,
                bytes, m_v0_test_file, bytes, m_lcpl_utf8) >= 0);

            H5L.info_t info = new H5L.info_t();
            Assert.IsTrue(
                H5L.get_info(m_v0_test_file, bytes, ref info) >= 0);

            Assert.IsTrue(info.type == H5L.type_t.EXTERNAL);
            IntPtr size = new IntPtr(info.u.val_size.ToInt32());
            Assert.IsTrue(size.ToInt32() > 0);

            IntPtr buf = Marshal.AllocHGlobal(size.ToInt32());
            Assert.IsTrue(buf != IntPtr.Zero);
            Assert.IsTrue(
                H5L.get_val(m_v0_test_file, bytes, buf, size) >= 0);

            uint flags = 0;
            IntPtr filename = new IntPtr();
            IntPtr obj_path = new IntPtr();
            Assert.IsTrue(
                H5L.unpack_elink_val(buf, size, ref flags, ref filename,
                ref obj_path) >= 0);

            Assert.IsTrue(
                Marshal.PtrToStringAnsi(filename) == m_v0_class_file_name);

            // the elink value is packed like this:
            // <file name>\0<object path>\0
            // the whole thing is of info.u.val_size

            int count = size.ToInt32() -
                (int)(obj_path.ToInt64() + 1 - filename.ToInt64()) - 1;
            byte[] obj_path_buf = new byte[count];
            Marshal.Copy(obj_path, obj_path_buf, 0, count);

            Assert.IsTrue(
                Encoding.UTF8.GetString(obj_path_buf) == (m_utf8strings[0] + "_unpack"),
                String.Format("{0}", Encoding.UTF8.GetString(obj_path_buf)));

            Marshal.FreeHGlobal(buf);

            // v2 file format

            Assert.IsTrue(
                H5G.close(H5G.create(m_v2_class_file, bytes, m_lcpl_utf8)) >= 0);

            Assert.IsTrue(
                H5L.create_external(m_v2_class_file_name,
                bytes, m_v2_test_file, bytes, m_lcpl_utf8) >= 0);

            info = new H5L.info_t();
            Assert.IsTrue(
                H5L.get_info(m_v2_test_file, bytes, ref info) >= 0);

            Assert.IsTrue(info.type == H5L.type_t.EXTERNAL);
            size = new IntPtr(info.u.val_size.ToInt32());
            Assert.IsTrue(size.ToInt32() > 0);

            buf = Marshal.AllocHGlobal(size.ToInt32());
            Assert.IsTrue(buf != IntPtr.Zero);
            Assert.IsTrue(
                H5L.get_val(m_v2_test_file, bytes, buf, size) >= 0);

            flags = 0;
            filename = new IntPtr();
            obj_path = new IntPtr();
            Assert.IsTrue(
                H5L.unpack_elink_val(buf, size, ref flags, ref filename,
                ref obj_path) >= 0);

            Assert.IsTrue(
                Marshal.PtrToStringAnsi(filename) == m_v2_class_file_name);

            // the elink value is packed like this:
            // <file name>\0<object path>\0
            // the whole thing is of info.u.val_size

            count = size.ToInt32() -
                (int)(obj_path.ToInt64() + 1 - filename.ToInt64()) - 1;
            obj_path_buf = new byte[count];
            Marshal.Copy(obj_path, obj_path_buf, 0, count);

            Assert.IsTrue(
                Encoding.UTF8.GetString(obj_path_buf) == (m_utf8strings[0] + "_unpack"),
                String.Format("{0}", Encoding.UTF8.GetString(obj_path_buf)));

            Marshal.FreeHGlobal(buf);
        }
    }
}

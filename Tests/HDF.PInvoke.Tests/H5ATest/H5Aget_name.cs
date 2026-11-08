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
using hid_t = System.Int64;
using size_t = System.IntPtr;
using ssize_t = System.IntPtr;

namespace UnitTests
{
    /// <summary>
    /// Groups regression checks for the H5A API.
    /// </summary>
    public partial class H5ATest
    {
        /// <summary>
        /// Exercises the native <c>H5Aget_name</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5Aget_nameTest1()
        {
            var buf_size = IntPtr.Zero;
            var size = IntPtr.Zero;
            var att = H5A.create(m_v2_test_file, "H5Aget_name",
                H5T.IEEE_F64LE, m_space_scalar);
            Assert.IsTrue(att >= 0);

            // pretend we don't know the size
            size = H5A.get_name(att, buf_size, (StringBuilder)null);
            Assert.IsTrue(size.ToInt32() == 11);
            buf_size = new IntPtr(size.ToInt32() + 1);
            var nameBuilder = new StringBuilder(buf_size.ToInt32());
            size = H5A.get_name(att, buf_size, nameBuilder);
            Assert.IsTrue(size.ToInt32() == 11);
            var name = nameBuilder.ToString();
            // names should match
            Assert.AreEqual("H5Aget_name", name);

            // read a truncated version
            buf_size = new IntPtr(3);
            nameBuilder = new StringBuilder(3);
            size = H5A.get_name(att, buf_size, nameBuilder);
            Assert.IsTrue(size.ToInt32() == 11);
            name = nameBuilder.ToString();
            // names won't match
            Assert.AreNotEqual("H5Aget_name", name);
            Assert.AreEqual("H5", name);

            Assert.IsTrue(H5A.close(att) >= 0);
        }

        /// <summary>
        /// Exercises the native <c>H5Aget_name</c> operation in scenario 2.
        /// </summary>
        [TestMethod]
        public void H5Aget_nameTest2()
        {
            Assert.IsFalse(H5A.get_name(Utilities.RandomInvalidHandle(),
                                                                IntPtr.Zero, (StringBuilder)null).ToInt32() >= 0);

            Assert.IsFalse(H5A.get_name(Utilities.RandomInvalidHandle(),
                IntPtr.Zero, (byte[])null).ToInt32() >= 0);
        }

        /// <summary>
        /// Exercises the native <c>H5Aget_name</c> operation in scenario 3.
        /// </summary>
        [TestMethod]
        public void H5Aget_nameTest3()
        {
            var name = Encoding.UTF8.GetBytes(
                                                                String.Join(":", m_utf8strings));
            var name_buf = new byte[name.Length + 1];
            Array.Copy(name, name_buf, name.Length);
            var att = H5A.create(m_v0_test_file, name_buf, H5T.IEEE_F64BE,
                m_space_scalar, m_acpl);
            Assert.IsTrue(att >= 0);

            var buf_size = H5A.get_name(att, IntPtr.Zero, (byte[])null) + 1;
            Assert.IsTrue(buf_size.ToInt32() > 1);
            var buf = new byte[buf_size.ToInt32()];
            Assert.IsTrue(H5A.get_name(att, buf_size, buf).ToInt32() >= 0);

            for (var i = 0; i < buf.Length; ++i)
            {
                Assert.IsTrue(name_buf[i] == buf[i]);
            }

            Assert.IsTrue(H5A.close(att) >= 0);
        }

        /// <summary>
        /// Exercises the native <c>H5Aget_name</c> operation in scenario 4.
        /// </summary>
        [TestMethod]
        public void H5Aget_nameTest4()
        {
            var name = Encoding.UTF8.GetBytes(
                                                                String.Join(":", m_utf8strings));
            var name_buf = new byte[name.Length + 1];
            Array.Copy(name, name_buf, name.Length);
            var att = H5A.create(m_v2_test_file, name_buf, H5T.IEEE_F64BE,
                m_space_scalar, m_acpl);
            Assert.IsTrue(att >= 0);

            var buf_size = H5A.get_name(att, IntPtr.Zero, (byte[])null) + 1;
            Assert.IsTrue(buf_size.ToInt32() > 1);
            var buf = new byte[buf_size.ToInt32()];
            Assert.IsTrue(H5A.get_name(att, buf_size, buf).ToInt32() >= 0);

            for (var i = 0; i < buf.Length; ++i)
            {
                Assert.IsTrue(name_buf[i] == buf[i]);
            }

            Assert.IsTrue(H5A.close(att) >= 0);
        }
    }
}

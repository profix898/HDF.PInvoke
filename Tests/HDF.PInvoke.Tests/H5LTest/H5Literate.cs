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
using herr_t = System.Int32;
using hid_t = System.Int64;
using hsize_t = System.UInt64;

namespace UnitTests
{
    /// <summary>
    /// Groups regression checks for the H5L API.
    /// </summary>
    public partial class H5LTest
    {
        /// <summary>
        /// Exercises the native <c>H5Literate</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5LiterateTest1()
        {
            Assert.IsTrue(H5L.create_soft("this/is/a/soft/link",
                                        m_v0_test_file, "A", m_lcpl) >= 0);
            Assert.IsTrue(H5L.create_soft("this/is/a/soft/link",
                m_v0_test_file, "B", m_lcpl) >= 0);
            Assert.IsTrue(H5L.create_soft("this/is/a/soft/link",
                m_v0_test_file, "C", m_lcpl) >= 0);

            var al = new ArrayList();
            var hnd = GCHandle.Alloc(al);
            var op_data = (IntPtr)hnd;
            hsize_t n = 0;
            // the callback is defined in H5LTest.cs
            H5L.iterate_t cb = DelegateMethod;
            Assert.IsTrue(H5L.iterate(m_v0_test_file, H5.index_t.NAME,
                H5.iter_order_t.NATIVE, ref n, cb, op_data) >= 0);
            // we should have 3 elements in the array list
            Assert.IsTrue(al.Count == 3);

            Assert.IsTrue(H5L.create_soft("this/is/a/soft/link",
                m_v2_test_file, "A", m_lcpl) >= 0);
            Assert.IsTrue(H5L.create_soft("this/is/a/soft/link",
                m_v2_test_file, "B", m_lcpl) >= 0);
            Assert.IsTrue(H5L.create_soft("this/is/a/soft/link",
                m_v2_test_file, "C", m_lcpl) >= 0);

            n = 0;
            Assert.IsTrue(H5L.iterate(m_v2_test_file, H5.index_t.NAME,
                H5.iter_order_t.NATIVE, ref n, cb, op_data) >= 0);
            // we should have 6 (3 + 3) elements in the array list
            Assert.IsTrue(al.Count == 6);

            hnd.Free();
        }

        /// <summary>
        /// Exercises the native <c>H5Literate</c> operation in scenario 2.
        /// </summary>
        [TestMethod]
        public void H5LiterateTest2()
        {
            var al = new ArrayList();
            var hnd = GCHandle.Alloc(al);
            var op_data = (IntPtr)hnd;
            hsize_t n = 0;
            // the callback is defined in H5ATest.cs
            H5L.iterate_t cb = DelegateMethod;

            Assert.IsFalse(
                H5L.iterate(Utilities.RandomInvalidHandle(),
                H5.index_t.NAME, H5.iter_order_t.NATIVE, ref n,
                cb, op_data) >= 0);

            hnd.Free();
        }

        /// <summary>
        /// Exercises the native <c>H5Literate</c> operation in scenario 3.
        /// </summary>
        [TestMethod]
        public void H5LiterateTest3()
        {
            for (var i = 0; i < m_utf8strings.Length; ++i)
            {
                Assert.IsTrue(H5L.create_soft(
                                                    Encoding.ASCII.GetBytes("this/is/a/soft/link" + "\0"),
                                                    m_v0_test_file, Encoding.UTF8.GetBytes(m_utf8strings[i] + "\0"),
                                                    m_lcpl_utf8) >= 0);

                Assert.IsTrue(H5L.create_soft(
                    Encoding.ASCII.GetBytes("this/is/a/soft/link" + "\0"),
                    m_v2_test_file, Encoding.UTF8.GetBytes(m_utf8strings[i] + "\0"),
                    m_lcpl_utf8) >= 0);
            }

            var al = new ArrayList();
            var hnd = GCHandle.Alloc(al);
            var op_data = (IntPtr)hnd;
            hsize_t n = 0;
            // the callback is defined in H5LTest.cs
            H5L.iterate_t cb = DelegateMethod;
            Assert.IsTrue(H5L.iterate(m_v0_test_file, H5.index_t.NAME,
                H5.iter_order_t.NATIVE, ref n, cb, op_data) >= 0);
            Assert.IsTrue(al.Count == m_utf8strings.Length);

            n = 0;
            Assert.IsTrue(H5L.iterate(m_v2_test_file, H5.index_t.NAME,
                H5.iter_order_t.NATIVE, ref n, cb, op_data) >= 0);
            Assert.IsTrue(al.Count == 2 * m_utf8strings.Length);

            hnd.Free();
        }
    }
}

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
        /// Exercises the native <c>H5Lvisit_by_name</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5Lvisit_by_nameTest1()
        {
            Assert.IsTrue(H5G.create(m_v0_test_file, "A/B/C/D", m_lcpl) >= 0);
            Assert.IsTrue(
                H5L.create_hard(m_v0_test_file, "A/B/C/D", m_v0_test_file,
                "shortcut") >= 0);

            Assert.IsTrue(H5G.create(m_v2_test_file, "A/B/C/D", m_lcpl) >= 0);
            Assert.IsTrue(
                H5L.create_hard(m_v2_test_file, "A/B/C/D", m_v2_test_file,
                "shortcut") >= 0);

            var al = new ArrayList();
            var hnd = GCHandle.Alloc(al);
            var op_data = (IntPtr)hnd;
            // the callback is defined in H5LTest.cs
            H5L.iterate_t cb = DelegateMethod;

            Assert.IsTrue(H5L.visit_by_name(m_v0_test_file, "A",
                H5.index_t.NAME, H5.iter_order_t.NATIVE, cb, op_data) >= 0);
            // we should have 3 elements in the array list
            Assert.IsTrue(al.Count == 3);

            Assert.IsTrue(H5L.visit_by_name(m_v2_test_file, "A",
                H5.index_t.NAME, H5.iter_order_t.NATIVE, cb, op_data) >= 0);
            // we should have 6 (3 + 3) elements in the array list
            Assert.IsTrue(al.Count == 6);

            hnd.Free();
        }

        /// <summary>
        /// Exercises the native <c>H5Lvisit_by_name</c> operation in scenario 2.
        /// </summary>
        [TestMethod]
        public void H5Lvisit_by_nameTest2()
        {
            var path = String.Join("/", m_utf8strings);
            Assert.IsTrue(H5G.create(m_v0_test_file,
                Encoding.UTF8.GetBytes(path + "\0"), m_lcpl_utf8) >= 0);
            Assert.IsTrue(H5G.create(m_v2_test_file,
                Encoding.UTF8.GetBytes(path + "\0"), m_lcpl_utf8) >= 0);

            var al = new ArrayList();
            var hnd = GCHandle.Alloc(al);
            var op_data = (IntPtr)hnd;
            // the callback is defined in H5LTest.cs
            H5L.iterate_t cb = DelegateMethod;

            Assert.IsTrue(H5L.visit_by_name(m_v0_test_file,
                Encoding.UTF8.GetBytes(m_utf8strings[0] + "\0"), H5.index_t.NAME,
                H5.iter_order_t.NATIVE, cb, op_data) >= 0);
            // we should have 4 elements in the array list
            Assert.IsTrue(al.Count == 4);

            Assert.IsTrue(H5L.visit_by_name(m_v2_test_file,
                Encoding.UTF8.GetBytes(m_utf8strings[0] + "\0"), H5.index_t.NAME,
                H5.iter_order_t.NATIVE, cb, op_data) >= 0);
            // we should have 8 (4 + 4) elements in the array list
            Assert.IsTrue(al.Count == 8);

            hnd.Free();
        }
    }
}

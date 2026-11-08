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
using System.Linq;
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
    /// Groups regression checks for the H5A API.
    /// </summary>
    public partial class H5ATest
    {
        /// <summary>
        /// Exercises the native <c>H5Aiterate_by_name</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5Aiterate_by_nameTest1()
        {
            var att = H5A.create(m_v2_test_file, "IEEE_F32BE",
                                                                H5T.IEEE_F32BE, m_space_scalar);
            Assert.IsTrue(att >= 0);
            Assert.IsTrue(H5A.close(att) >= 0);

            att = H5A.create(m_v2_test_file, "IEEE_F64BE", H5T.IEEE_F64BE,
               m_space_scalar);
            Assert.IsTrue(att >= 0);
            Assert.IsTrue(H5A.close(att) >= 0);

            att = H5A.create(m_v2_test_file, "NATIVE_B8", H5T.NATIVE_B8,
               m_space_scalar);
            Assert.IsTrue(att >= 0);
            Assert.IsTrue(H5A.close(att) >= 0);
            var al = new ArrayList();
            hsize_t n = 0;

            // if we define the callback as lambda we can simple capture the array list
            H5A.operator_t cb = (hid_t location_id, IntPtr attr_name, ref H5A.info_t ainfo, IntPtr op_data
            ) =>
            {
                Assert.IsTrue(op_data.ToInt64() == -99);
                var len = 0;

                while (Marshal.ReadByte(attr_name, len) != 0)
                {
                    ++len;
                }

                var buf = new byte[len];
                Marshal.Copy(attr_name, buf, 0, len);
                al.Add(Encoding.UTF8.GetString(buf));
                return 0;
            };

            Assert.IsTrue(H5A.iterate_by_name(m_v2_test_file, ".",
                H5.index_t.NAME, H5.iter_order_t.NATIVE, ref n, cb,
                // the op_data argument is simply tested as a constant
                new IntPtr(-99)) >= 0);
            // we should have 3 elements in the array list [now obsolete]
            Assert.IsTrue(al.Count == 3);
            Assert.IsFalse(al.ToArray().Any(s => s == null || String.IsNullOrEmpty(s.ToString())));
            // let's be specific
            Assert.IsTrue(al[0].ToString() == "IEEE_F32BE");
            Assert.IsTrue(al[1].ToString() == "IEEE_F64BE");
            Assert.IsTrue(al[2].ToString() == "NATIVE_B8");

            att = H5A.create(m_v0_test_file, "IEEE_F32BE",
                H5T.IEEE_F32BE, m_space_scalar);
            Assert.IsTrue(att >= 0);
            Assert.IsTrue(H5A.close(att) >= 0);

            att = H5A.create(m_v0_test_file, "IEEE_F64BE", H5T.IEEE_F64BE,
               m_space_scalar);
            Assert.IsTrue(att >= 0);
            Assert.IsTrue(H5A.close(att) >= 0);

            att = H5A.create(m_v0_test_file, "NATIVE_B8", H5T.NATIVE_B8,
               m_space_scalar);
            Assert.IsTrue(att >= 0);
            Assert.IsTrue(H5A.close(att) >= 0);

            al.Clear();
            n = 0;
            Assert.IsTrue(H5A.iterate_by_name(m_v0_test_file, ".",
                H5.index_t.NAME, H5.iter_order_t.NATIVE, ref n, cb,
                new IntPtr(-99)) >= 0);
            // we should have 3 elements in the array list [now obsolete]
            Assert.IsTrue(al.Count == 3);
            Assert.IsFalse(al.ToArray().Any(s => s == null || String.IsNullOrEmpty(s.ToString())));
            // let's be specific
            Assert.IsTrue(al[0].ToString() == "IEEE_F32BE");
            Assert.IsTrue(al[1].ToString() == "IEEE_F64BE");
            Assert.IsTrue(al[2].ToString() == "NATIVE_B8");
        }

        /// <summary>
        /// Exercises the native <c>H5Aiterate_by_name</c> operation in scenario 2.
        /// </summary>
        [TestMethod]
        public void H5Aiterate_by_nameTest2()
        {
            var al = new ArrayList();
            var hnd = GCHandle.Alloc(al);
            var op_data = (IntPtr)hnd;
            hsize_t n = 0;
            // the callback is defined in H5ATest.cs
            H5A.operator_t cb = DelegateMethod;

            Assert.IsFalse(
                H5A.iterate_by_name(Utilities.RandomInvalidHandle(), ".",
                H5.index_t.NAME, H5.iter_order_t.NATIVE, ref n,
                cb, op_data) >= 0);

            hnd.Free();
        }
    }
}

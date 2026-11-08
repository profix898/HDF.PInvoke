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
using ssize_t = System.IntPtr;

namespace UnitTests
{
    /// <summary>
    /// Groups regression checks for the H5R API.
    /// </summary>
    public partial class H5RTest
    {
        /// <summary>
        /// Exercises the native <c>H5Rget_name</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5Rget_nameTest1()
        {
            var path =
                                                                Encoding.UTF8.GetBytes(String.Join("/", m_utf8strings));
            // make room for the trailling \0
            var name = new byte[path.Length + 1];
            Array.Copy(path, name, path.Length);

            Assert.IsTrue(
                H5G.close(H5G.create(m_v0_test_file, name, m_lcpl_utf8)) >= 0);

            var refer = new byte[H5R.OBJ_REF_BUF_SIZE];
            var hnd = GCHandle.Alloc(refer, GCHandleType.Pinned);

            Assert.IsTrue(
                H5R.create(hnd.AddrOfPinnedObject(), m_v0_test_file, name,
                H5R.type_t.OBJECT, -1) >= 0);

            var size = H5R.get_name(m_v0_test_file, H5R.type_t.OBJECT,
                hnd.AddrOfPinnedObject(), (byte[])null, IntPtr.Zero);
            Assert.IsTrue(size.ToInt32() == name.Length);

            // size does not include the trailling \0
            var buf = new byte[size.ToInt32() + 1];
            size = H5R.get_name(m_v0_test_file, H5R.type_t.OBJECT,
                hnd.AddrOfPinnedObject(), buf, new IntPtr(buf.Length));
            Assert.IsTrue(size.ToInt32() == name.Length);

            hnd.Free();

            // we need to account for the leading "/", which was not included
            // in path
            for (var i = 0; i < name.Length; ++i)
            {
                Assert.IsTrue(name[i] == buf[i + 1]);
            }
        }
    }
}

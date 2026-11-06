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
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HDF.PInvoke;

using hsize_t = System.UInt64;

using hid_t = System.Int64;

namespace UnitTests
{
    /// <summary>
    /// Groups regression checks for the H5D API.
    /// </summary>
    public partial class H5DTest
    {
        /// <summary>
        /// Exercises the native <c>H5Dwrite</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5DwriteTest1()
        {
            string utf8string
                                        = "Γαζέες καὶ μυρτιὲς δὲν θὰ βρῶ πιὰ στὸ χρυσαφὶ ξέφωτο";
            byte[] wdata = Encoding.UTF8.GetBytes(utf8string);

            hid_t dtype = H5T.create(H5T.class_t.STRING,
                new IntPtr(wdata.Length));
            Assert.IsTrue(H5T.set_cset(dtype, H5T.cset_t.UTF8) >= 0);
            Assert.IsTrue(H5T.set_strpad(dtype, H5T.str_t.SPACEPAD) >= 0);

            hid_t dset_v0 = H5D.create(m_v0_test_file, "dset", dtype,
                m_space_scalar);
            Assert.IsTrue(dset_v0 >= 0);

            hid_t dset_v2 = H5D.create(m_v2_test_file, "dset", dtype,
                m_space_scalar);
            Assert.IsTrue(dset_v2 >= 0);

            GCHandle hnd = GCHandle.Alloc(wdata, GCHandleType.Pinned);
            Assert.IsTrue(H5D.write(dset_v0, dtype, H5S.ALL,
                H5S.ALL, H5P.DEFAULT, hnd.AddrOfPinnedObject()) >= 0);
            Assert.IsTrue(H5D.write(dset_v2, dtype, H5S.ALL,
                H5S.ALL, H5P.DEFAULT, hnd.AddrOfPinnedObject()) >= 0);
            hnd.Free();

            Assert.IsTrue(H5T.close(dtype) >= 0);
            Assert.IsTrue(H5D.close(dset_v2) >= 0);
            Assert.IsTrue(H5D.close(dset_v0) >= 0);
        }

        /// <summary>
        /// Exercises the native <c>H5Dwrite</c> operation in scenario 2.
        /// </summary>
        [TestMethod]
        public void H5DwriteTest2()
        {
            ArrayList utf8strings = new ArrayList()
            { "Ελληνικά", "日本語", "العربية" };

            hid_t dtype = H5T.create(H5T.class_t.STRING, H5T.VARIABLE);
            Assert.IsTrue(H5T.set_cset(dtype, H5T.cset_t.UTF8) >= 0);
            Assert.IsTrue(H5T.set_strpad(dtype, H5T.str_t.SPACEPAD) >= 0);

            hid_t dspace = H5S.create_simple(1,
                new hsize_t[] { (hsize_t)utf8strings.Count }, null);

            hid_t dset = H5D.create(m_v0_test_file, "dset", dtype, dspace);
            Assert.IsTrue(dset >= 0);

            GCHandle[] hnds = new GCHandle[utf8strings.Count];
            IntPtr[] wdata = new IntPtr[utf8strings.Count];

            for (int i = 0; i < utf8strings.Count; ++i)
            {
                hnds[i] = GCHandle.Alloc(
                                                    Encoding.UTF8.GetBytes((string)utf8strings[i]),
                                                    GCHandleType.Pinned);
                wdata[i] = hnds[i].AddrOfPinnedObject();
            }

            GCHandle hnd = GCHandle.Alloc(wdata, GCHandleType.Pinned);
            Assert.IsTrue(H5D.write(dset, dtype, H5S.ALL, H5S.ALL, H5P.DEFAULT,
                hnd.AddrOfPinnedObject()) >= 0);
            hnd.Free();

            for (int i = 0; i < utf8strings.Count; ++i)
            {
                hnds[i].Free();
            }

            Assert.IsTrue(H5D.close(dset) >= 0);
            Assert.IsTrue(H5S.close(dspace) >= 0);
            Assert.IsTrue(H5T.close(dtype) >= 0);
        }
    }
}

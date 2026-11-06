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

using herr_t = System.Int32;
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
        /// Exercises the native <c>H5Dvlen_reclaim</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5Dvlen_reclaimTest1()
        {
            // write a VLEN dataset

            hid_t vlen = H5T.vlen_create(H5T.NATIVE_INT);
            Assert.IsTrue(vlen >= 0);

            hsize_t[] dims = { 10 };
            hid_t space = H5S.create_simple(1, dims, null);
            Assert.IsTrue(space >= 0);

            hid_t dset = H5D.create(m_v0_test_file, "vlen", vlen, space);
            Assert.IsTrue(space >= 0);
            hid_t dset1 = H5D.create(m_v2_test_file, "vlen", vlen, space);
            Assert.IsTrue(space >= 0);

            H5T.hvl_t[] wdata = new H5T.hvl_t[dims[0]];
            GCHandle[] whndl = new GCHandle[wdata.Length];
            int[][] jagged = new int[wdata.Length][];

            for (int i = 0; i < wdata.Length; ++i)
            {
                jagged[i] = new int[i + 1];
                whndl[i] = GCHandle.Alloc(jagged[i], GCHandleType.Pinned);
                wdata[i].len = new IntPtr(i + 1);
                wdata[i].p = whndl[i].AddrOfPinnedObject();
            }

            GCHandle wdata_hndl = GCHandle.Alloc(wdata, GCHandleType.Pinned);
            Assert.IsTrue(H5D.write(dset, vlen, H5S.ALL, H5S.ALL, H5P.DEFAULT,
                wdata_hndl.AddrOfPinnedObject()) >= 0);
            Assert.IsTrue(H5D.write(dset1, vlen, H5S.ALL, H5S.ALL, H5P.DEFAULT,
                wdata_hndl.AddrOfPinnedObject()) >= 0);
            wdata_hndl.Free();

            for (int i = 0; i < wdata.Length; ++i)
            {
                whndl[i].Free();
            }

            // read it back

            H5T.hvl_t[] rdata = new H5T.hvl_t[dims[0]];
            GCHandle rdata_hndl = GCHandle.Alloc(rdata, GCHandleType.Pinned);

            Assert.IsTrue(H5D.read(dset, vlen, H5S.ALL, H5S.ALL, H5P.DEFAULT,
                rdata_hndl.AddrOfPinnedObject()) >= 0);

            for (int i = 0; i < rdata.Length; ++i)
            {
                Assert.IsTrue(rdata[i].len.ToInt32() == i + 1);
            }

            Assert.IsTrue(H5D.vlen_reclaim(vlen, space, H5P.DEFAULT,
                rdata_hndl.AddrOfPinnedObject()) >= 0);

            Assert.IsTrue(H5D.read(dset1, vlen, H5S.ALL, H5S.ALL, H5P.DEFAULT,
                rdata_hndl.AddrOfPinnedObject()) >= 0);

            for (int i = 0; i < rdata.Length; ++i)
            {
                Assert.IsTrue(rdata[i].len.ToInt32() == i + 1);
            }

            // reclaim the space
            Assert.IsTrue(H5D.vlen_reclaim(vlen, space, H5P.DEFAULT,
                rdata_hndl.AddrOfPinnedObject()) >= 0);

            rdata_hndl.Free();

            Assert.IsTrue(H5D.close(dset1) >= 0);
            Assert.IsTrue(H5D.close(dset) >= 0);
            Assert.IsTrue(H5T.close(vlen) >= 0);
            Assert.IsTrue(H5S.close(space) >= 0);
        }
    }
}

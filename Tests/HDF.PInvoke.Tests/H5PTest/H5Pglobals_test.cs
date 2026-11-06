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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HDF.PInvoke;

using hsize_t = System.UInt64;

using hid_t = System.Int64;

namespace UnitTests
{
    public partial class H5PTest
    {
        [TestMethod]
        public void H5PglobalsTest1()
        {
            hsize_t[] dims = { 10, 10, 10 };
            hsize_t[] max_dims = { H5S.UNLIMITED, H5S.UNLIMITED, H5S.UNLIMITED };
            hid_t space = H5S.create_simple(3, dims, max_dims);

            hid_t lcpl = H5P.create(H5P.LINK_CREATE);
            Assert.IsTrue(H5P.set_create_intermediate_group(lcpl, 1) >= 0);

            hid_t dcpl = H5P.create(H5P.DATASET_CREATE);
            Assert.IsTrue(dcpl >= 0);
            hsize_t[] chunk = { 64, 64, 64 };
            Assert.IsTrue(H5P.set_chunk(dcpl, 3, chunk) >= 0);
            Assert.IsTrue(H5P.set_deflate(dcpl, 9) >= 0);

            hid_t dset = H5D.create(m_v0_test_file, "A/B/C", H5T.IEEE_F32BE,
                space, lcpl, dcpl);
            Assert.IsTrue(dset >= 0);

            hid_t plist = H5D.get_create_plist(dset);
            Assert.IsTrue(H5D.layout_t.CHUNKED == H5P.get_layout(plist));
            Assert.IsTrue(H5P.close(plist) >= 0);

            Assert.IsTrue(H5D.close(dset) >= 0);

            dset = H5D.create(m_v2_test_file, "A/B/C", H5T.IEEE_F32BE,
                space, lcpl, dcpl);
            Assert.IsTrue(dset >= 0);

            plist = H5D.get_create_plist(dset);
            Assert.IsTrue(H5D.layout_t.CHUNKED == H5P.get_layout(plist));
            Assert.IsTrue(H5P.close(plist) >= 0);

            Assert.IsTrue(H5D.close(dset) >= 0);

            Assert.IsTrue(H5P.close(dcpl) >= 0);
            Assert.IsTrue(H5P.close(lcpl) >= 0);
            Assert.IsTrue(H5S.close(space) >= 0);
        }
    }
}

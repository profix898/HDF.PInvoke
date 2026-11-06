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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HDF.PInvoke;

using hbool_t = System.Byte;
using herr_t = System.Int32;
using hsize_t = System.UInt64;

using hid_t = System.Int64;

namespace UnitTests
{
    public partial class H5SWMRTest
    {
        [TestMethod]
        public void H5Pset_chunk_optsTestSWMR1()
        {
            hid_t dcpl = H5P.create(H5P.DATASET_CREATE);
            Assert.IsTrue(dcpl >= 0);

            // without chunking, H5Pset_chunk_opts will throw an error
            hsize_t[] dims = { 4711 };
            Assert.IsTrue(H5P.set_chunk(dcpl, 1, dims) >= 0);

            uint opts = H5D.DONT_FILTER_PARTIAL_CHUNKS;
            Assert.IsTrue(H5P.set_chunk_opts(dcpl, opts) >= 0);

            Assert.IsTrue(H5P.close(dcpl) >= 0);
        }
    }
}

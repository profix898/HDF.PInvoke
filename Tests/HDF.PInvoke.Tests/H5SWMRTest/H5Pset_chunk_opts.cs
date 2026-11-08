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
using HDF.PInvoke;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using hbool_t = System.Byte;
using herr_t = System.Int32;
using hid_t = System.Int64;
using hsize_t = System.UInt64;

namespace UnitTests
{
    /// <summary>
    /// Groups regression checks for the H5SWMR API.
    /// </summary>
    public partial class H5SWMRTest
    {
        /// <summary>
        /// Exercises the native <c>H5Pset_chunk_opts</c> operation in scenario SWMR1.
        /// </summary>
        [TestMethod]
        public void H5Pset_chunk_optsTestSWMR1()
        {
            var dcpl = H5P.create(H5P.DATASET_CREATE);
            Assert.IsTrue(dcpl >= 0);

            // without chunking, H5Pset_chunk_opts will throw an error
            hsize_t[] dims = { 4711 };
            Assert.IsTrue(H5P.set_chunk(dcpl, 1, dims) >= 0);

            var opts = H5D.DONT_FILTER_PARTIAL_CHUNKS;
            Assert.IsTrue(H5P.set_chunk_opts(dcpl, opts) >= 0);

            Assert.IsTrue(H5P.close(dcpl) >= 0);
        }
    }
}

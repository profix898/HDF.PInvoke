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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HDF.PInvoke;

using herr_t = System.Int32;
using hsize_t = System.UInt64;

using hid_t = System.Int64;

namespace UnitTests
{
    /// <summary>
    /// Groups regression checks for the H5SWMR API.
    /// </summary>
    [TestClass]
    public partial class H5SWMRTest
    {
        /// <summary>
        /// Creates the native resources shared by this test fixture.
        /// </summary>
        [ClassInitialize()]
        public static void ClassInit(TestContext testContext)
        {
            // create test files which persists across file tests
            m_v3_class_file = Utilities.H5TempFileSWMR(ref m_v3_class_file_name);
            Assert.IsTrue(m_v3_class_file >= 0);

            m_lcpl = H5P.create(H5P.LINK_CREATE);
            Assert.IsTrue(H5P.set_create_intermediate_group(m_lcpl, 1) >= 0);

            m_lcpl_utf8 = H5P.copy(m_lcpl);
            Assert.IsTrue(
                H5P.set_char_encoding(m_lcpl_utf8, H5T.cset_t.UTF8) >= 0);

            // create a sample dataset

            hsize_t[] dims = { 6, 6 };
            hsize_t[] maxdims = { 6, H5S.UNLIMITED };
            hsize_t[] chunk_dims = { 2, 5 };
            int[] cbuf = new int[36];

            hid_t dsp = H5S.create_simple(2, dims, maxdims);
            Assert.IsTrue(dsp >= 0);

            hid_t dcpl = H5P.create(H5P.DATASET_CREATE);
            Assert.IsTrue(dcpl >= 0);
            Assert.IsTrue(H5P.set_chunk(dcpl, 2, chunk_dims) >= 0);

            hid_t dst = H5D.create(m_v3_class_file, "int6x6",
                H5T.NATIVE_INT, dsp, H5P.DEFAULT, dcpl);
            Assert.IsTrue(dst >= 0);

            GCHandle hnd = GCHandle.Alloc(cbuf, GCHandleType.Pinned);

            Assert.IsTrue(H5D.write(dst, H5T.NATIVE_INT, H5S.ALL, H5S.ALL,
                H5P.DEFAULT, hnd.AddrOfPinnedObject()) >= 0);

            hnd.Free();

            Assert.IsTrue(H5D.flush(dst) >= 0);

            Assert.IsTrue(H5D.close(dst) >= 0);
            Assert.IsTrue(H5P.close(dcpl) >= 0);
            Assert.IsTrue(H5S.close(dsp) >= 0);
        }

        /// <summary>
        /// Creates fresh native resources for the current test.
        /// </summary>
        [TestInitialize()]
        public void Init()
        {
            Utilities.DisableErrorPrinting();

            m_v3_test_file_no_swmr =
                Utilities.H5TempFileNoSWMR(ref m_v3_test_file_name_no_swmr);
            Assert.IsTrue(m_v3_test_file_no_swmr >= 0);

            m_v3_test_file_swmr =
                Utilities.H5TempFileSWMR(ref m_v3_test_file_name_swmr);
            Assert.IsTrue(m_v3_test_file_swmr >= 0);
        }

        /// <summary>
        /// Closes and removes the native resources created for the current test.
        /// </summary>
        [TestCleanup()]
        public void Cleanup()
        {
            // close the test-local files
            Assert.IsTrue(H5F.close(m_v3_test_file_no_swmr) >= 0);
            File.Delete(m_v3_test_file_name_no_swmr);
            Assert.IsTrue(H5F.close(m_v3_test_file_swmr) >= 0);
            File.Delete(m_v3_test_file_name_swmr);
        }

        /// <summary>
        /// Closes and removes the native resources shared by this test fixture.
        /// </summary>
        [ClassCleanup()]
        public static void ClassCleanup()
        {
            Assert.IsTrue(H5P.close(m_lcpl) >= 0);
            Assert.IsTrue(H5P.close(m_lcpl_utf8) >= 0);

            // close the global test files
            Assert.IsTrue(H5F.close(m_v3_class_file) >= 0);
            File.Delete(m_v3_class_file_name);
        }

        /// <summary>
        /// Retains the fixture's v3 class file identifier.
        /// </summary>
        private static hid_t m_v3_class_file = -1;

        /// <summary>
        /// Retains the fixture's v3 class file name path.
        /// </summary>
        private static string m_v3_class_file_name;

        /// <summary>
        /// Retains the link-creation property list used for ASCII link names.
        /// </summary>
        private static hid_t m_lcpl;

        /// <summary>
        /// Retains the link-creation property list configured for UTF-8 link names.
        /// </summary>
        private static hid_t m_lcpl_utf8;

        /// <summary>
        /// Retains the fixture's v3 test file no swmr identifier.
        /// </summary>
        private hid_t m_v3_test_file_no_swmr = -1;

        /// <summary>
        /// Retains the fixture's v3 test file name no swmr path.
        /// </summary>
        private string m_v3_test_file_name_no_swmr;

        /// <summary>
        /// Retains the fixture's v3 test file swmr identifier.
        /// </summary>
        private hid_t m_v3_test_file_swmr = -1;

        /// <summary>
        /// Retains the fixture's v3 test file name swmr path.
        /// </summary>
        private string m_v3_test_file_name_swmr;

        // Two simple callbacks for H5Pset_append_flush and
        // H5Pset_object_flush_cb. We assume that a pointer to a counter in
        // unmanaged memory is passed as user data (op_data).

        /// <summary>
        /// Provides the append callback used by the SWMR property-list tests.
        /// </summary>
        public herr_t append_func
            (
            hid_t dset_id,
            IntPtr cur_dims,
            IntPtr op_data
            )
        {
            int append_ct = Marshal.ReadInt32(op_data);
            Marshal.WriteInt32(op_data, ++append_ct);
            return 0;
        }

        /// <summary>
        /// Provides the flush callback used by the SWMR property-list tests.
        /// </summary>
        public herr_t flush_func
            (
            hid_t obj_id,
            IntPtr op_data
            )
        {
            int flush_ct = Marshal.ReadInt32(op_data);
            Marshal.WriteInt32(op_data, ++flush_ct);
            return 0;
        }
    }
}

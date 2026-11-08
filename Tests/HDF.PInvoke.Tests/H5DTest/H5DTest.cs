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
using HDF.PInvoke;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using herr_t = System.Int32;
using hid_t = System.Int64;
using hsize_t = System.UInt64;

namespace UnitTests
{
    /// <summary>
    /// Groups regression checks for the H5D API.
    /// </summary>
    [TestClass]
    public partial class H5DTest
    {
        /// <summary>
        /// Creates the native resources shared by this test fixture.
        /// </summary>
        [ClassInitialize()]
        public static void ClassInit(TestContext testContext)
        {
            // create test files which persists across file tests
            m_v0_class_file = Utilities.H5TempFile(ref m_v0_class_file_name,
                H5F.libver_t.EARLIEST);
            Assert.IsTrue(m_v0_class_file >= 0);
            m_v2_class_file = Utilities.H5TempFile(ref m_v2_class_file_name);
            Assert.IsTrue(m_v2_class_file >= 0);

            m_space_null = H5S.create(H5S.class_t.NULL);
            Assert.IsTrue(m_space_null >= 0);
            m_space_scalar = H5S.create(H5S.class_t.SCALAR);
            Assert.IsTrue(m_space_scalar >= 0);

            // create two datasets of the extended ASCII character set
            // store as H5T.FORTRAN_S1 -> space padding

            hsize_t[] dims = { 256 };

            var space = H5S.create_simple(1, dims, null);
            m_v0_ascii_dset = H5D.create(m_v0_class_file, "ASCII",
                H5T.FORTRAN_S1, space);
            m_v2_ascii_dset = H5D.create(m_v2_class_file, "ASCII",
                H5T.FORTRAN_S1, space);
            Assert.IsTrue(H5S.close(space) >= 0);

            // we write from C and must provide null-terminated strings

            var wdata = new byte[512];

            for (var i = 0; i < 256; ++i)
            {
                wdata[2 * i] = (byte)i;
            }

            var mem_type = H5T.copy(H5T.C_S1);
            Assert.IsTrue(H5T.set_size(mem_type, new IntPtr(2)) >= 0);

            var hnd = GCHandle.Alloc(wdata, GCHandleType.Pinned);
            Assert.IsTrue(H5D.write(m_v0_ascii_dset, mem_type, H5S.ALL,
                H5S.ALL, H5P.DEFAULT, hnd.AddrOfPinnedObject()) >= 0);
            Assert.IsTrue(H5D.write(m_v2_ascii_dset, mem_type, H5S.ALL,
                H5S.ALL, H5P.DEFAULT, hnd.AddrOfPinnedObject()) >= 0);
            hnd.Free();

            Assert.IsTrue(H5T.close(mem_type) >= 0);

            // create UTF-8 encoded test datasets

            var dtype = H5T.create(H5T.class_t.STRING, H5T.VARIABLE);
            Assert.IsTrue(H5T.set_cset(dtype, H5T.cset_t.UTF8) >= 0);
            Assert.IsTrue(H5T.set_strpad(dtype, H5T.str_t.SPACEPAD) >= 0);

            var dspace = H5S.create_simple(1,
                new hsize_t[] { (hsize_t)m_utf8strings.Count }, null);

            m_v0_utf8_dset = H5D.create(m_v0_class_file, "UTF-8", dtype, dspace);
            Assert.IsTrue(m_v0_utf8_dset >= 0);
            m_v2_utf8_dset = H5D.create(m_v2_class_file, "UTF-8", dtype, dspace);
            Assert.IsTrue(m_v2_utf8_dset >= 0);

            var hnds = new GCHandle[m_utf8strings.Count];
            var wdata1 = new IntPtr[m_utf8strings.Count];

            for (var i = 0; i < m_utf8strings.Count; ++i)
            {
                hnds[i] = GCHandle.Alloc(
                                                    Encoding.UTF8.GetBytes((string)m_utf8strings[i] + "\0"),
                                                    GCHandleType.Pinned);
                wdata1[i] = hnds[i].AddrOfPinnedObject();
            }

            hnd = GCHandle.Alloc(wdata1, GCHandleType.Pinned);
            Assert.IsTrue(H5D.write(m_v0_utf8_dset, dtype, H5S.ALL, H5S.ALL,
                H5P.DEFAULT, hnd.AddrOfPinnedObject()) >= 0);
            Assert.IsTrue(H5D.write(m_v2_utf8_dset, dtype, H5S.ALL, H5S.ALL,
                H5P.DEFAULT, hnd.AddrOfPinnedObject()) >= 0);
            hnd.Free();

            for (var i = 0; i < m_utf8strings.Count; ++i)
            {
                hnds[i].Free();
            }

            Assert.IsTrue(H5S.close(dspace) >= 0);
            Assert.IsTrue(H5T.close(dtype) >= 0);
        }

        /// <summary>
        /// Creates fresh native resources for the current test.
        /// </summary>
        [TestInitialize()]
        public void Init()
        {
            Utilities.DisableErrorPrinting();

            // create test-local files
            m_v0_test_file = Utilities.H5TempFile(ref m_v0_test_file_name,
                H5F.libver_t.EARLIEST);
            Assert.IsTrue(m_v0_test_file >= 0);

            m_v2_test_file = Utilities.H5TempFile(ref m_v2_test_file_name);
            Assert.IsTrue(m_v2_test_file >= 0);
        }

        /// <summary>
        /// Closes and removes the native resources created for the current test.
        /// </summary>
        [TestCleanup()]
        public void Cleanup()
        {
            // close the test-local files
            Assert.IsTrue(H5F.close(m_v0_test_file) >= 0);
            Assert.IsTrue(H5F.close(m_v2_test_file) >= 0);
            File.Delete(m_v0_test_file_name);
            File.Delete(m_v2_test_file_name);
        }

        /// <summary>
        /// Closes and removes the native resources shared by this test fixture.
        /// </summary>
        [ClassCleanup()]
        public static void ClassCleanup()
        {
            // close the sample datasets
            Assert.IsTrue(H5D.close(m_v2_ascii_dset) >= 0);
            Assert.IsTrue(H5D.close(m_v0_ascii_dset) >= 0);
            Assert.IsTrue(H5D.close(m_v2_utf8_dset) >= 0);
            Assert.IsTrue(H5D.close(m_v0_utf8_dset) >= 0);

            // close the global test files
            Assert.IsTrue(H5F.close(m_v0_class_file) >= 0);
            Assert.IsTrue(H5F.close(m_v2_class_file) >= 0);
            Assert.IsTrue(H5S.close(m_space_null) >= 0);
            Assert.IsTrue(H5S.close(m_space_scalar) >= 0);

            File.Delete(m_v0_class_file_name);
            File.Delete(m_v2_class_file_name);
        }

        /// <summary>
        /// Retains the fixture's v0 class file identifier.
        /// </summary>
        private static hid_t m_v0_class_file = -1;

        /// <summary>
        /// Retains the fixture's v0 class file name path.
        /// </summary>
        private static string m_v0_class_file_name;

        /// <summary>
        /// Retains the fixture's v2 class file identifier.
        /// </summary>
        private static hid_t m_v2_class_file = -1;

        /// <summary>
        /// Retains the fixture's v2 class file name path.
        /// </summary>
        private static string m_v2_class_file_name;

        /// <summary>
        /// Retains the fixture's v0 test file identifier.
        /// </summary>
        private hid_t m_v0_test_file = -1;

        /// <summary>
        /// Retains the fixture's v0 test file name path.
        /// </summary>
        private string m_v0_test_file_name;

        /// <summary>
        /// Retains the fixture's v2 test file identifier.
        /// </summary>
        private hid_t m_v2_test_file = -1;

        /// <summary>
        /// Retains the fixture's v2 test file name path.
        /// </summary>
        private string m_v2_test_file_name;

        /// <summary>
        /// Retains the null dataspace shared by the fixture.
        /// </summary>
        private static hid_t m_space_null = -1;

        /// <summary>
        /// Retains the scalar dataspace shared by the fixture.
        /// </summary>
        private static hid_t m_space_scalar = -1;

        /// <summary>
        /// Retains the fixture's v0 ascii dataset identifier.
        /// </summary>
        private static hid_t m_v0_ascii_dset = -1;

        /// <summary>
        /// Retains the fixture's v2 ascii dataset identifier.
        /// </summary>
        private static hid_t m_v2_ascii_dset = -1;

        /// <summary>
        /// Provides multilingual strings that exercise UTF-8 encoding beyond ASCII.
        /// </summary>
        private static ArrayList m_utf8strings = new ArrayList() { "Ελληνικά", "日本語", "العربية", "экземпляр", "סקרן" };

        /// <summary>
        /// Retains the fixture's v0 utf8 dataset identifier.
        /// </summary>
        private static hid_t m_v0_utf8_dset = -1;

        /// <summary>
        /// Retains the fixture's v2 utf8 dataset identifier.
        /// </summary>
        private static hid_t m_v2_utf8_dset = -1;

        // Callback for H5D.iterate
        // op_data is a pointer to a counter and we keep adding the elements
        /// <summary>
        /// Provides the dataset-iteration callback used to verify native callback dispatch.
        /// </summary>
        private herr_t DelegateMethod
            (
            IntPtr elem,
            hid_t type_id,
            uint ndim,
            hsize_t[] point,
            IntPtr op_data
            )
        {
            var count = Marshal.ReadInt32(op_data) + Marshal.ReadInt32(elem);
            Marshal.WriteInt32(op_data, count);
            return 0;
        }
    }
}

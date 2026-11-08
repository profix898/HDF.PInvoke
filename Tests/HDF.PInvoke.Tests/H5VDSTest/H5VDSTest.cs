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
using HDF.PInvoke;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using herr_t = System.Int32;
using hid_t = System.Int64;
using hsize_t = System.UInt64;

namespace UnitTests
{
    /// <summary>
    /// Groups regression checks for the H5VDS API.
    /// </summary>
    [TestClass]
    public partial class H5VDSTest
    {
        /// <summary>
        /// Creates the native resources shared by this test fixture.
        /// </summary>
        [ClassInitialize()]
        public static void ClassInit(TestContext testContext)
        {
            createVDS();
        }

        /// <summary>
        /// Creates fresh native resources for the current test.
        /// </summary>
        [TestInitialize()]
        public void Init()
        {
            Utilities.DisableErrorPrinting();
        }

        /// <summary>
        /// Closes and removes the native resources created for the current test.
        /// </summary>
        [TestCleanup()]
        public void Cleanup()
        {
        }

        /// <summary>
        /// Closes and removes the native resources shared by this test fixture.
        /// </summary>
        [ClassCleanup()]
        public static void ClassCleanup()
        {
            cleanupVDS();
        }

        /// <summary>
        /// Retains the fixture's vds class file identifier.
        /// </summary>
        private static hid_t m_vds_class_file = 1;

        /// <summary>
        /// Retains the fixture's vds class file name path.
        /// </summary>
        private static string m_vds_class_file_name;

        /// <summary>
        /// Retains the fixture's a class file identifier.
        /// </summary>
        private static hid_t m_a_class_file = 1;

        /// <summary>
        /// Retains the fixture's a class file name path.
        /// </summary>
        private static string m_a_class_file_name;

        /// <summary>
        /// Retains the fixture's b class file identifier.
        /// </summary>
        private static hid_t m_b_class_file = 1;

        /// <summary>
        /// Retains the fixture's b class file name path.
        /// </summary>
        private static string m_b_class_file_name;

        /// <summary>
        /// Retains the fixture's c class file identifier.
        /// </summary>
        private static hid_t m_c_class_file = 1;

        /// <summary>
        /// Retains the fixture's c class file name path.
        /// </summary>
        private static string m_c_class_file_name;

        /// <summary>
        /// Creates the source datasets and virtual mappings used by the virtual-dataset tests.
        /// </summary>
        private static void createVDS()
        {
            // create files
            m_a_class_file = Utilities.H5TempFile(ref m_a_class_file_name,
                H5F.libver_t.LATEST, true);
            Assert.IsTrue(m_a_class_file >= 0);
            m_b_class_file = Utilities.H5TempFile(ref m_b_class_file_name,
                H5F.libver_t.LATEST, true);
            Assert.IsTrue(m_b_class_file >= 0);
            m_c_class_file = Utilities.H5TempFile(ref m_c_class_file_name,
                H5F.libver_t.LATEST, true);
            Assert.IsTrue(m_c_class_file >= 0);
            m_vds_class_file = Utilities.H5TempFile(ref m_vds_class_file_name);
            Assert.IsTrue(m_vds_class_file >= 0);

            //
            // create target datasets
            //
            var dcpl = H5P.create(H5P.DATASET_CREATE);
            Assert.IsTrue(dcpl >= 0);
            var fill_value = 1;
            var hnd = GCHandle.Alloc(fill_value, GCHandleType.Pinned);
            Assert.IsTrue(H5P.set_fill_value(dcpl, H5T.NATIVE_INT,
                hnd.AddrOfPinnedObject()) >= 0);

            hsize_t[] dims = { 6 };
            var src_dsp = H5S.create_simple(1, dims, null);

            // A
            fill_value = 1;
            var a = H5D.create(m_a_class_file, "A", H5T.STD_I32LE, src_dsp);
            Assert.IsTrue(a >= 0);
            Assert.IsTrue(H5D.close(a) >= 0);
            // B
            fill_value = 2;
            var b = H5D.create(m_b_class_file, "B", H5T.STD_I32LE, src_dsp);
            Assert.IsTrue(b >= 0);
            Assert.IsTrue(H5D.close(b) >= 0);
            // B
            fill_value = 3;
            var c = H5D.create(m_c_class_file, "C", H5T.STD_I32LE, src_dsp);
            Assert.IsTrue(c >= 0);
            Assert.IsTrue(H5D.close(c) >= 0);

            //
            // create the VDS
            //
            fill_value = -1;
            hsize_t[] vds_dims = { 4, 6 };
            var vds_dsp = H5S.create_simple(2, vds_dims, null);

            hsize_t[] start = { 0, 0 };
            hsize_t[] count = { 1, 1 };
            hsize_t[] block = { 1, 6 };

            start[0] = 0;
            Assert.IsTrue(H5S.select_hyperslab(vds_dsp, H5S.seloper_t.SET,
                start, null, count, block) >= 0);
            Assert.IsTrue(H5P.set_virtual(dcpl, vds_dsp,
                m_a_class_file_name, "A", src_dsp) >= 0);

            start[0] = 1;
            Assert.IsTrue(H5S.select_hyperslab(vds_dsp, H5S.seloper_t.SET,
                start, null, count, block) >= 0);
            Assert.IsTrue(H5P.set_virtual(dcpl, vds_dsp,
                m_b_class_file_name, "B", src_dsp) >= 0);

            start[0] = 2;
            Assert.IsTrue(H5S.select_hyperslab(vds_dsp, H5S.seloper_t.SET,
                start, null, count, block) >= 0);
            Assert.IsTrue(H5P.set_virtual(dcpl, vds_dsp,
                m_c_class_file_name, "C", src_dsp) >= 0);

            var vds = H5D.create(m_vds_class_file, "VDS", H5T.STD_I32LE,
                vds_dsp, H5P.DEFAULT, dcpl, H5P.DEFAULT);
            Assert.IsTrue(vds >= 0);
            Assert.IsTrue(H5D.close(vds) >= 0);

            Assert.IsTrue(H5S.close(vds_dsp) >= 0);
            Assert.IsTrue(H5S.close(src_dsp) >= 0);
            Assert.IsTrue(H5P.close(dcpl) >= 0);

            hnd.Free();

            // close the satellite files
            Assert.IsTrue(H5F.close(m_a_class_file) >= 0);
            Assert.IsTrue(H5F.close(m_b_class_file) >= 0);
            Assert.IsTrue(H5F.close(m_c_class_file) >= 0);
        }

        /// <summary>
        /// Closes and removes the source and virtual-dataset fixtures.
        /// </summary>
        private static void cleanupVDS()
        {
            Assert.IsTrue(H5F.close(m_vds_class_file) >= 0);

            File.Delete(m_vds_class_file_name);
            File.Delete(m_a_class_file_name);
            File.Delete(m_b_class_file_name);
            File.Delete(m_c_class_file_name);
        }

    }
}

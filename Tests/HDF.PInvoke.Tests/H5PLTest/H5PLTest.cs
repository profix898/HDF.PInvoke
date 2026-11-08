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

namespace UnitTests
{
    /// <summary>
    /// Groups regression checks for the H5PL API.
    /// </summary>
    [TestClass]
    public partial class H5PLTest
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

            m_lcpl = H5P.create(H5P.LINK_CREATE);
            Assert.IsTrue(H5P.set_create_intermediate_group(m_lcpl, 1) >= 0);

            m_lcpl_utf8 = H5P.copy(m_lcpl);
            Assert.IsTrue(
                H5P.set_char_encoding(m_lcpl_utf8, H5T.cset_t.UTF8) >= 0);
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
            Assert.IsTrue(H5P.close(m_lcpl) >= 0);
            Assert.IsTrue(H5P.close(m_lcpl_utf8) >= 0);

            // close the global test files
            Assert.IsTrue(H5F.close(m_v0_class_file) >= 0);
            Assert.IsTrue(H5F.close(m_v2_class_file) >= 0);
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
        /// Retains the link-creation property list used for ASCII link names.
        /// </summary>
        private static hid_t m_lcpl;

        /// <summary>
        /// Retains the link-creation property list configured for UTF-8 link names.
        /// </summary>
        private static hid_t m_lcpl_utf8;

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
    }
}

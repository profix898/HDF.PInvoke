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
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HDF.PInvoke;

using hid_t = System.Int64;

namespace UnitTests
{
    /// <summary>
    /// Groups regression checks for the H5S API.
    /// </summary>
    [TestClass]
    public partial class H5STest
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

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
    /// Groups regression checks for the H5A API.
    /// </summary>
    [TestClass]
    public partial class H5ATest
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

            m_acpl = H5P.create(H5P.ATTRIBUTE_CREATE);
            Assert.IsTrue(m_acpl >= 0);
            Assert.IsTrue(H5P.set_char_encoding(m_acpl, H5T.cset_t.UTF8) >= 0);
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
            Assert.IsTrue(H5P.close(m_acpl) >= 0);
            // close the global test files
            Assert.IsTrue(H5F.close(m_v0_class_file) >= 0);
            Assert.IsTrue(H5F.close(m_v2_class_file) >= 0);
            Assert.IsTrue(H5S.close(m_space_null) >= 0);
            Assert.IsTrue(H5S.close(m_space_scalar) >= 0);
            File.Delete(m_v0_class_file_name);
            File.Delete(m_v2_class_file_name);
        }

        /// <summary>
        /// Provides multilingual strings that exercise UTF-8 encoding beyond ASCII.
        /// </summary>
        private static string[] m_utf8strings = new string[] { "Ελληνικά", "日本語", "العربية", "экземпляр", "סקרן" };

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
        /// Retains the attribute-creation property list shared by the fixture.
        /// </summary>
        private static hid_t m_acpl = -1;

        // Callback for H5A.iterate and H5A.iterate_by_name
        // We expect an array list as op_data; add the attribute names to the
        // array list as we go
        /// <summary>
        /// Provides the dataset-iteration callback used to verify native callback dispatch.
        /// </summary>
        public herr_t DelegateMethod
            (
            hid_t location_id,
            IntPtr attr_name,
            ref H5A.info_t ainfo,
            IntPtr op_data
            )
        {
            var hnd = (GCHandle)op_data;
            var al = (hnd.Target as ArrayList);
            var len = 0;

            while (Marshal.ReadByte(attr_name, len) != 0)
            {
                ++len;
            }

            var buf = new byte[len];
            Marshal.Copy(attr_name, buf, 0, len);
            al.Add(Encoding.UTF8.GetString(buf));
            return 0;
        }
    }
}

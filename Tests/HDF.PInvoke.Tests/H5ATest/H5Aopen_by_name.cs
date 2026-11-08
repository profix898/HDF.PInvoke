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
using HDF.PInvoke;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using hid_t = System.Int64;
using htri_t = System.Int32;

namespace UnitTests
{
    /// <summary>
    /// Groups regression checks for the H5A API.
    /// </summary>
    public partial class H5ATest
    {
        /// <summary>
        /// Exercises the native <c>H5Aopen_by_name</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5Aopen_by_nameTest1()
        {
            var att = H5A.create(m_v2_test_file, "A", H5T.IEEE_F64LE,
                                                                m_space_scalar);
            Assert.IsTrue(att >= 0);
            Assert.IsTrue(H5A.close(att) >= 0);

            att = H5A.open_by_name(m_v2_test_file, ".", "A");
            Assert.IsTrue(att >= 0);
            Assert.IsTrue(H5A.close(att) >= 0);

            att = H5A.create(m_v0_test_file, "A", H5T.IEEE_F64LE,
                m_space_scalar);
            Assert.IsTrue(att >= 0);
            Assert.IsTrue(H5A.close(att) >= 0);

            att = H5A.open_by_name(m_v0_test_file, ".", "A");
            Assert.IsTrue(att >= 0);
            Assert.IsTrue(H5A.close(att) >= 0);
        }

        /// <summary>
        /// Exercises the native <c>H5Aopen_by_name</c> operation in scenario 2.
        /// </summary>
        [TestMethod]
        public void H5Aopen_by_nameTest2()
        {
            Assert.IsFalse(
                                                                H5A.open_by_name(Utilities.RandomInvalidHandle(), ".", "") >= 0);
            Assert.IsFalse(
                H5A.open_by_name(m_v2_class_file, ".", "") >= 0);
        }
    }
}

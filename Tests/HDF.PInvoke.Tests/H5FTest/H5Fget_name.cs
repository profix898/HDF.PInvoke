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
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HDF.PInvoke;

using ssize_t = System.IntPtr;

using hid_t = System.Int64;

namespace UnitTests
{
    /// <summary>
    /// Groups regression checks for the H5F API.
    /// </summary>
    public partial class H5FTest
    {
        /// <summary>
        /// Exercises the native <c>H5Fget_name</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5Fget_nameTest1()
        {
            StringBuilder nameBuilder = new StringBuilder(256);

            Assert.IsTrue(
                H5F.get_name(m_v0_test_file, nameBuilder,
                new IntPtr(nameBuilder.Capacity)).ToInt32() >= 0);

            string name = nameBuilder.ToString();
            // names should match
            Assert.AreEqual(m_v0_test_file_name, name);

            Assert.IsTrue(
                H5F.get_name(m_v2_test_file, nameBuilder,
                new IntPtr(nameBuilder.Capacity)).ToInt32() >= 0);

            name = nameBuilder.ToString();
            // names should match
            Assert.AreEqual(m_v2_test_file_name, name);
        }

        /// <summary>
        /// Exercises the native <c>H5Fget_name</c> operation in scenario 2.
        /// </summary>
        [TestMethod]
        public void H5Fget_nameTest2()
        {
            StringBuilder nameBuilder = new StringBuilder(256);

            Assert.IsTrue(
                H5F.get_name(Utilities.RandomInvalidHandle(), nameBuilder,
                new IntPtr(nameBuilder.Capacity)).ToInt32() < 0);
        }
    }
}

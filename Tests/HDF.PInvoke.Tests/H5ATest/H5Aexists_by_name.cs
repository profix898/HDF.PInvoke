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
        /// Exercises the native <c>H5Aexists_by_name</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5Aexists_by_nameTest1()
        {
            var check = H5A.exists_by_name(m_v0_class_file, ".", ".");
            Assert.IsTrue(check >= 0);
            check = H5A.exists_by_name(m_v0_class_file, ".", "NAC");
            Assert.IsTrue(check >= 0);
            check = H5A.exists_by_name(m_v2_class_file, ".", "A");
            Assert.IsTrue(check >= 0);
        }

        /// <summary>
        /// Exercises the native <c>H5Aexists_by_name</c> operation in scenario 2.
        /// </summary>
        [TestMethod]
        public void H5Aexists_by_nameTest2()
        {
            Assert.IsFalse(
                                                                H5A.exists(Utilities.RandomInvalidHandle(), ".") >= 0);
            Assert.IsFalse(
                H5A.exists(m_v2_class_file, "") >= 0);
        }
    }
}

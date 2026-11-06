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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HDF.PInvoke;

using hid_t = System.Int64;

namespace UnitTests
{
    /// <summary>
    /// Groups regression checks for the H5L API.
    /// </summary>
    public partial class H5LTest
    {
        /// <summary>
        /// Exercises the native <c>H5Lcreate_external</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5Lcreate_externalTest1()
        {
            Assert.IsTrue(
                                        H5L.create_external(m_v0_class_file_name, "/", m_v0_test_file,
                                        "A/B/C", m_lcpl) >= 0);
            Assert.IsTrue(
                H5L.create_external(m_v2_class_file_name, "/", m_v2_test_file,
                "A/B/C", m_lcpl) >= 0);
        }
    }
}

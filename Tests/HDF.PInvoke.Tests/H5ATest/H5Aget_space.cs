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

namespace UnitTests
{
    /// <summary>
    /// Groups regression checks for the H5A API.
    /// </summary>
    public partial class H5ATest
    {
        /// <summary>
        /// Exercises the native <c>H5Aget_space</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5Aget_spaceTest1()
        {
            var att = H5A.create(m_v2_test_file, "A", H5T.IEEE_F64LE,
                                                                m_space_scalar);
            Assert.IsTrue(att >= 0);
            var space = H5A.get_space(att);
            Assert.IsTrue(space >= 0);
            Assert.IsTrue(H5S.close(space) >= 0);
            Assert.IsTrue(H5A.close(att) >= 0);

            att = H5A.create(m_v0_test_file, "A", H5T.IEEE_F64LE,
                m_space_scalar);
            Assert.IsTrue(att >= 0);
            space = H5A.get_space(att);
            Assert.IsTrue(space >= 0);
            Assert.IsTrue(H5S.close(space) >= 0);
            Assert.IsTrue(H5A.close(att) >= 0);
        }

        /// <summary>
        /// Exercises the native <c>H5Aget_space</c> operation in scenario 2.
        /// </summary>
        [TestMethod]
        public void H5Aget_spaceTest2()
        {
            Assert.IsFalse(
                                                                H5A.get_space(Utilities.RandomInvalidHandle()) >= 0);
        }
    }
}

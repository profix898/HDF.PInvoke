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
    /// Groups regression checks for the H5O API.
    /// </summary>
    public partial class H5OTest
    {
        /// <summary>
        /// Exercises the native <c>H5Oclose</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5OcloseTest1()
        {
            hid_t gid = H5G.create(m_v0_test_file, "A");
            Assert.IsTrue(gid >= 0);
            Assert.IsTrue(H5O.close(gid) >= 0);

            gid = H5G.create(m_v2_test_file, "A");
            Assert.IsTrue(gid >= 0);
            Assert.IsTrue(H5O.close(gid) >= 0);
        }

        /// <summary>
        /// Exercises the native <c>H5Oclose</c> operation in scenario 2.
        /// </summary>
        [TestMethod]
        public void H5OcloseTest2()
        {
            hid_t gid = H5G.create(m_v0_test_file, "A");
            Assert.IsTrue(gid >= 0);
            Assert.IsTrue(H5O.close(gid) >= 0);
            Assert.IsTrue(H5O.close(gid) < 0);

            gid = H5G.create(m_v2_test_file, "A");
            Assert.IsTrue(gid >= 0);
            Assert.IsTrue(H5O.close(gid) >= 0);
            Assert.IsTrue(H5O.close(gid) < 0);
        }

        /// <summary>
        /// Exercises the native <c>H5Oclose</c> operation in scenario 3.
        /// </summary>
        [TestMethod]
        public void H5OcloseTest3()
        {
            hid_t gid = Utilities.RandomInvalidHandle();
            Assert.IsTrue(H5O.close(gid) < 0);
        }

        /// <summary>
        /// Exercises the native <c>H5Oclose</c> operation in scenario 4.
        /// </summary>
        [TestMethod]
        public void H5OcloseTest4()
        {
            hid_t space = H5S.create(H5S.class_t.SCALAR);
            Assert.IsTrue(space >= 0);
            Assert.IsTrue(H5O.close(space) < 0);
            Assert.IsTrue(H5S.close(space) >= 0);
        }
    }
}

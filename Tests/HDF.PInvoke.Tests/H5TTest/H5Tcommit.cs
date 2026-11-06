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

using hsize_t = System.UInt64;

using hid_t = System.Int64;

namespace UnitTests
{
    /// <summary>
    /// Groups regression checks for the H5T API.
    /// </summary>
    public partial class H5TTest
    {
        /// <summary>
        /// Exercises the native <c>H5Tcommit</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5TcommitTest1()
        {
            hid_t dtype = H5T.copy(H5T.IEEE_F64LE);
            Assert.IsTrue(dtype >= 0);
            Assert.IsTrue(H5T.commit(m_v0_test_file, "foo", dtype) >= 0);
            // can't commit twice
            Assert.IsFalse(H5T.commit(m_v0_test_file, "bar", dtype) >= 0);
            // can't commit to different files
            Assert.IsFalse(H5T.commit(m_v2_test_file, "bar", dtype) >= 0);
            Assert.IsTrue(H5T.close(dtype) >= 0);
        }

        /// <summary>
        /// Exercises the native <c>H5Tcommit</c> operation in scenario 2.
        /// </summary>
        [TestMethod]
        public void H5TcommitTest2()
        {
            // can't commit pre-defined types
            Assert.IsFalse(
                H5T.commit(m_v0_test_file, "foo", H5T.IEEE_F64BE) >= 0);
            Assert.IsFalse(
                H5T.commit(m_v2_test_file, "bar", H5T.IEEE_F64BE) >= 0);
        }

        /// <summary>
        /// Exercises the native <c>H5Tcommit</c> operation in scenario 3.
        /// </summary>
        [TestMethod]
        public void H5TcommitTest3()
        {
            Assert.IsFalse(
                                        H5T.commit(m_v0_test_file, "foo",
                                        Utilities.RandomInvalidHandle()) >= 0);
            Assert.IsFalse(
                H5T.commit(Utilities.RandomInvalidHandle(), "foo",
                Utilities.RandomInvalidHandle()) >= 0);
        }
    }
}

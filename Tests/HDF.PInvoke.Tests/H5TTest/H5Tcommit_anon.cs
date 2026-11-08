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
using hsize_t = System.UInt64;

namespace UnitTests
{
    /// <summary>
    /// Groups regression checks for the H5T API.
    /// </summary>
    public partial class H5TTest
    {
        /// <summary>
        /// Exercises the native <c>H5Tcommit_anon</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5Tcommit_anonTest1()
        {
            var dtype = H5T.copy(H5T.IEEE_F64LE);
            Assert.IsTrue(dtype >= 0);
            Assert.IsTrue(H5T.commit_anon(m_v0_test_file, dtype) >= 0);
            // can't commit twice
            Assert.IsFalse(H5T.commit_anon(m_v0_test_file, dtype) >= 0);
            // can't commit to different files
            Assert.IsFalse(H5T.commit_anon(m_v2_test_file, dtype) >= 0);
            Assert.IsTrue(H5T.close(dtype) >= 0);
        }

        /// <summary>
        /// Exercises the native <c>H5Tcommit_anon</c> operation in scenario 2.
        /// </summary>
        [TestMethod]
        public void H5Tcommit_anonTest2()
        {
            // can't commit pre-defined types
            Assert.IsFalse(
                H5T.commit_anon(m_v0_test_file, H5T.IEEE_F64BE) >= 0);
            Assert.IsFalse(
                H5T.commit_anon(m_v2_test_file, H5T.IEEE_F64BE) >= 0);
        }

        /// <summary>
        /// Exercises the native <c>H5Tcommit_anon</c> operation in scenario 3.
        /// </summary>
        [TestMethod]
        public void H5Tcommit_anonTest3()
        {
            Assert.IsFalse(
                                        H5T.commit_anon(m_v0_test_file,
                                        Utilities.RandomInvalidHandle()) >= 0);
            Assert.IsFalse(
                H5T.commit_anon(Utilities.RandomInvalidHandle(),
                Utilities.RandomInvalidHandle()) >= 0);
        }
    }
}

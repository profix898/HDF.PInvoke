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
    /// Groups regression checks for the H5I API.
    /// </summary>
    public partial class H5ITest
    {
        /// <summary>
        /// Exercises the native <c>H5Iget_type</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5Iget_typeTest1()
        {
            Assert.IsTrue(H5I.get_type(m_v0_class_file) == H5I.type_t.FILE);
            hid_t gid = H5G.create(m_v0_test_file, "A");
            Assert.IsTrue(gid > 0);
            Assert.IsTrue(H5I.get_type(gid) == H5I.type_t.GROUP);
            Assert.IsTrue(H5G.close(gid) >= 0);

            Assert.IsTrue(H5I.get_type(m_v2_class_file) == H5I.type_t.FILE);
            gid = H5G.create(m_v2_test_file, "A");
            Assert.IsTrue(gid > 0);
            Assert.IsTrue(H5I.get_type(gid) == H5I.type_t.GROUP);
            Assert.IsTrue(H5G.close(gid) >= 0);
        }

        /// <summary>
        /// Exercises the native <c>H5Iget_type</c> operation in scenario 2.
        /// </summary>
        [TestMethod]
        public void H5Iget_typeTest2()
        {
            Assert.IsTrue(H5I.get_type(Utilities.RandomInvalidHandle()) ==
                                        H5I.type_t.BADID);
        }
    }
}

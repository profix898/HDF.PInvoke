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
        /// Exercises the native <c>H5Adelete_by_name</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5Adelete_by_nameTest1()
        {
            var att = H5A.create(m_v0_class_file, "DNA", H5T.IEEE_F32BE,
                                        m_space_null);
            Assert.IsTrue(att >= 0);
            Assert.IsTrue(H5A.close(att) >= 0);

            att = H5A.create(m_v2_class_file, "DNA", H5T.IEEE_F32BE,
               m_space_null);
            Assert.IsTrue(att >= 0);
            Assert.IsTrue(H5A.close(att) >= 0);

            att = H5A.create(m_v0_class_file, "DSA", H5T.IEEE_F32BE,
                m_space_scalar);
            Assert.IsTrue(att >= 0);
            Assert.IsTrue(H5A.close(att) >= 0);

            att = H5A.create(m_v2_class_file, "DSA", H5T.IEEE_F32BE,
               m_space_scalar);
            Assert.IsTrue(att >= 0);
            Assert.IsTrue(H5A.close(att) >= 0);

            Assert.IsTrue(H5A.delete_by_name(m_v0_class_file, ".", "DNA") >= 0);
            Assert.IsTrue(H5A.delete_by_name(m_v0_class_file, ".", "DSA") >= 0);
            Assert.IsTrue(H5A.delete_by_name(m_v2_class_file, ".", "DNA") >= 0);
            Assert.IsTrue(H5A.delete_by_name(m_v2_class_file, ".", "DSA") >= 0);
        }

        /// <summary>
        /// Exercises the native <c>H5Adelete_by_name</c> operation in scenario 2.
        /// </summary>
        [TestMethod]
        public void H5Adelete_by_nameTest2()
        {
            Assert.IsFalse(
                                        H5A.delete_by_name(Utilities.RandomInvalidHandle(), "A", ".")
                                        >= 0);
            Assert.IsFalse(H5A.delete_by_name(m_v0_test_file, ".", ".") >= 0);
        }
    }
}

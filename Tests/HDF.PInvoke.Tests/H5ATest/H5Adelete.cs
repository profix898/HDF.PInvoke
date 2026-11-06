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
    public partial class H5ATest
    {
        [TestMethod]
        public void H5AdeleteTest1()
        {
            hid_t att = H5A.create(m_v0_class_file, "DNA", H5T.IEEE_F32BE,
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

            Assert.IsTrue(H5A.delete(m_v0_class_file, "DNA") >= 0);
            Assert.IsTrue(H5A.delete(m_v0_class_file, "DSA") >= 0);
            Assert.IsTrue(H5A.delete(m_v2_class_file, "DNA") >= 0);
            Assert.IsTrue(H5A.delete(m_v2_class_file, "DSA") >= 0);
        }

        [TestMethod]
        public void H5AdeleteTest2()
        {
            Assert.IsFalse(
                                        H5A.delete(Utilities.RandomInvalidHandle(), "A") >= 0);
            Assert.IsFalse(H5A.delete(m_v0_test_file, ".") >= 0);
        }
    }
}

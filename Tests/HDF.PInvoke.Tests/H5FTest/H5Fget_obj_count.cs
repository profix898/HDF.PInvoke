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

using ssize_t = System.IntPtr;

using hid_t = System.Int64;

namespace UnitTests
{
    public partial class H5FTest
    {
        [TestMethod]
        public void H5Fget_obj_countTest1()
        {
            Assert.IsTrue(
                                        H5F.get_obj_count(m_v0_class_file, H5F.OBJ_ALL).ToInt32() > 0);
            Assert.IsTrue(
                H5F.get_obj_count(m_v0_class_file,
                H5F.OBJ_DATASET).ToInt32() >= 0);
            Assert.IsTrue(
                H5F.get_obj_count(m_v0_class_file,
                H5F.OBJ_GROUP).ToInt32() >= 0);
            Assert.IsTrue(
                H5F.get_obj_count(m_v0_class_file,
                H5F.OBJ_DATATYPE).ToInt32() >= 0);
            Assert.IsTrue(
                H5F.get_obj_count(m_v0_class_file,
                H5F.OBJ_ATTR).ToInt32() >= 0);
            Assert.IsTrue(
                H5F.get_obj_count(m_v0_class_file,
                H5F.OBJ_ALL).ToInt32() > 0);
            Assert.IsTrue(
                H5F.get_obj_count(m_v0_class_file,
                H5F.OBJ_LOCAL).ToInt32() < 0);

            Assert.IsTrue(
                H5F.get_obj_count(m_v2_class_file, H5F.OBJ_ALL).ToInt32() > 0);
            Assert.IsTrue(
                H5F.get_obj_count(m_v2_class_file,
                H5F.OBJ_DATASET).ToInt32() >= 0);
            Assert.IsTrue(
                H5F.get_obj_count(m_v2_class_file,
                H5F.OBJ_GROUP).ToInt32() >= 0);
            Assert.IsTrue(
                H5F.get_obj_count(m_v2_class_file,
                H5F.OBJ_DATATYPE).ToInt32() >= 0);
            Assert.IsTrue(
                H5F.get_obj_count(m_v2_class_file,
                H5F.OBJ_ATTR).ToInt32() >= 0);
            Assert.IsTrue(
                H5F.get_obj_count(m_v2_class_file,
                H5F.OBJ_ALL).ToInt32() > 0);
            Assert.IsTrue(
                H5F.get_obj_count(m_v2_class_file,
                H5F.OBJ_LOCAL).ToInt32() < 0);

            Assert.IsTrue(
                H5F.get_obj_count(m_v2_test_file, H5F.OBJ_ALL).ToInt32() > 0);
            Assert.IsTrue(
                H5F.get_obj_count(m_v2_test_file,
                H5F.OBJ_DATASET).ToInt32() == 0);
            Assert.IsTrue(
                H5F.get_obj_count(m_v2_test_file,
                H5F.OBJ_GROUP).ToInt32() == 0);
            Assert.IsTrue(
                H5F.get_obj_count(m_v2_test_file,
                H5F.OBJ_DATATYPE).ToInt32() == 0);
            Assert.IsTrue(
                H5F.get_obj_count(m_v2_test_file,
                H5F.OBJ_ATTR).ToInt32() == 0);
            Assert.IsTrue(
                H5F.get_obj_count(m_v2_test_file,
                H5F.OBJ_ALL).ToInt32() > 0);
            Assert.IsTrue(
                H5F.get_obj_count(m_v2_test_file,
                H5F.OBJ_LOCAL).ToInt32() < 0);
        }

        [TestMethod]
        public void H5Fget_obj_countTest2()
        {
            Assert.IsFalse(
                                        H5F.get_obj_count(Utilities.RandomInvalidHandle(),
                                        H5F.OBJ_ALL).ToInt32() >= 0);
        }
    }
}

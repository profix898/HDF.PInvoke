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
    public partial class H5ETest
    {
        [TestMethod]
        public void H5EpushTest1()
        {
            Assert.IsTrue(
                                        H5E.push(H5E.DEFAULT, "hello.c", "sqrt", 77, H5E.ERR_CLS,
                                        H5E.NONE_MAJOR, H5E.NONE_MINOR, "Hello, World!") >= 0);
            Assert.IsTrue(
                H5E.get_num(H5E.DEFAULT).ToInt32() > 0);
        }

        [TestMethod]
        public void H5EpushTest2()
        {
            Assert.IsFalse(
                                        H5E.push(Utilities.RandomInvalidHandle(), "hello.c", "sqrt",
                                        77, H5E.ERR_CLS, H5E.NONE_MAJOR, H5E.NONE_MINOR,
                                        "Hello, World!") >= 0);
        }
    }
}

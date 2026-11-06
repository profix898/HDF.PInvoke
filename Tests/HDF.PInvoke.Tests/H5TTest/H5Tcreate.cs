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
    /// Groups regression checks for the H5T API.
    /// </summary>
    public partial class H5TTest
    {
        /// <summary>
        /// Exercises the native <c>H5Tcreate</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5TcreateTest1()
        {
            hid_t dtype = H5T.create(H5T.class_t.STRING, H5T.VARIABLE);
            Assert.IsTrue(dtype >= 0);
            Assert.IsTrue(H5T.is_variable_str(dtype) > 0);
            Assert.IsTrue(H5T.close(dtype) >= 0);
        }
    }
}

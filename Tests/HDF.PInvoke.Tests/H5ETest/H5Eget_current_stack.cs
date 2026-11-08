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
    /// Groups regression checks for the H5E API.
    /// </summary>
    public partial class H5ETest
    {
        /// <summary>
        /// Exercises the native <c>H5Eget_current_stack</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5Eget_current_stackTest1()
        {
            var est = H5E.get_current_stack();
            Assert.IsTrue(est >= 0);
            Assert.IsTrue(H5E.close_stack(est) >= 0);
        }
    }
}

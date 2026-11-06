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

using uint32_t = System.UInt32;

using hid_t = System.Int64;

namespace UnitTests
{
    /// <summary>
    /// Groups regression checks for the H5PL API.
    /// </summary>
    public partial class H5PLTest
    {
        /// <summary>
        /// Exercises the native <c>H5PLappend</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5PLappendTest1()
        {
            Assert.IsTrue(H5PL.append("foo") >= 0);
            uint32_t listsize = 0;
            Assert.IsTrue(H5PL.size(ref listsize) >= 0);
            Assert.IsTrue(listsize >= 1);
        }
    }
}

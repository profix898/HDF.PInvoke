/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * Copyright by The HDF Group.                                               *
 * Copyright by the Board of Trustees of the University of Illinois.         *
 * All rights reserved.                                                      *
 *                                                                           *
 * This file is part of HDF.PInvoke. Copyright notices and redistribution
 * terms for the wrapper and its native dependencies are in LICENSE.txt
 * at the repository and package root.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using HDF.PInvoke;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UnitTests
{
    [TestClass]
    public class _AssemblySpecific
    {
        [AssemblyInitialize()]
        public static void AssemblyInit(TestContext context)
        {
            // open the HDF5 library
            Assert.IsTrue(H5.open() >= 0);
        }

        [AssemblyCleanup()]
        public static void AssemblyCleanup()
        {
            // close the HDF5 library
            Assert.IsTrue(H5.close() >= 0);
        }
    }
}

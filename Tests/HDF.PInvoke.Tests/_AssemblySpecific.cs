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

namespace UnitTests
{
    /// <summary>
    /// Manages the HDF5 lifetime shared by the integration test assembly.
    /// </summary>
    [TestClass]
    public class _AssemblySpecific
    {
        /// <summary>
        /// Initializes HDF5 before the integration test assembly executes.
        /// </summary>
        [AssemblyInitialize()]
        public static void AssemblyInit(TestContext context)
        {
            // open the HDF5 library
            Assert.IsTrue(H5.open() >= 0);
        }

        /// <summary>
        /// Closes HDF5 after the integration test assembly has completed.
        /// </summary>
        [AssemblyCleanup()]
        public static void AssemblyCleanup()
        {
            // close the HDF5 library
            Assert.IsTrue(H5.close() >= 0);
            NativeLifecycleTests.VerifyAfterClose();
        }
    }
}

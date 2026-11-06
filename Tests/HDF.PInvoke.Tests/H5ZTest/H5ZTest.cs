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

namespace UnitTests
{
    [TestClass]
    public partial class H5ZTest
    {
        [ClassInitialize()]
        public static void ClassInit(TestContext testContext)
        {
        }

        [TestInitialize()]
        public void Init()
        {
            Utilities.DisableErrorPrinting();
        }

        [TestCleanup()]
        public void Cleanup()
        {
        }

        [ClassCleanup()]
        public static void ClassCleanup()
        {
        }
    }
}

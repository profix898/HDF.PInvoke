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
using herr_t = System.Int32;
using hid_t = System.Int64;

namespace UnitTests
{
    /// <summary>
    /// Groups regression checks for the H5E API.
    /// </summary>
    public partial class H5ETest
    {
        /// <summary>
        /// Exercises the native <c>H5Ewalk</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5EwalkTest1()
        {
            H5E.auto_t auto_cb = ErrorDelegateMethod;
            Assert.IsTrue(
                H5E.set_auto(H5E.DEFAULT, auto_cb, IntPtr.Zero) >= 0);

            H5E.walk_t walk_cb = WalkDelegateMethod;
            var client_data = IntPtr.Zero;
            Assert.IsTrue(
                H5E.walk(H5E.DEFAULT, H5E.direction_t.H5E_WALK_DOWNWARD,
                walk_cb, IntPtr.Zero) >= 0);
        }

        /// <summary>
        /// Exercises the native <c>H5Ewalk</c> operation in scenario 2.
        /// </summary>
        [TestMethod]
        public void H5EwalkTest2()
        {
            H5E.auto_t auto_cb = ErrorDelegateMethod;
            Assert.IsTrue(
                H5E.set_auto(H5E.DEFAULT, auto_cb, IntPtr.Zero) >= 0);

            H5E.walk_t walk_cb = WalkDelegateMethod;
            var client_data = IntPtr.Zero;

            Assert.IsTrue(
                H5E.push(H5E.DEFAULT, "hello.c", "sqrt", 77, H5E.ERR_CLS,
                H5E.NONE_MAJOR, H5E.NONE_MINOR, "Hello, World!") >= 0);

            Assert.IsTrue(
                H5E.push(H5E.DEFAULT, "hello.c", "sqr", 78, H5E.ERR_CLS,
                H5E.NONE_MAJOR, H5E.NONE_MINOR, "Hello, World!") >= 0);

            Assert.IsTrue(
                H5E.walk(H5E.DEFAULT, H5E.direction_t.H5E_WALK_DOWNWARD,
                walk_cb, IntPtr.Zero) >= 0);
        }

        /// <summary>
        /// Receives native error notifications for the error-stack callback tests.
        /// </summary>
        public herr_t ErrorDelegateMethod
            (
            hid_t estack,
            IntPtr client_data
            )
        {
            return 0;
        }

        /// <summary>
        /// Receives each error record while the test walks a native error stack.
        /// </summary>
        public herr_t WalkDelegateMethod
            (
            uint n,
            ref H5E.error_t err_desc,
            IntPtr client_data
            )
        {
            Assert.IsTrue(err_desc.line > 0);
            return 0;
        }
    }
}

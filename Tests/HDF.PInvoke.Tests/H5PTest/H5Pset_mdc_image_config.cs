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
using System.IO;
using System.Runtime.InteropServices;
using HDF.PInvoke;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using hid_t = System.Int64;
using hsize_t = System.UInt64;

namespace UnitTests
{
    /// <summary>
    /// Groups regression checks for the H5P API.
    /// </summary>
    public partial class H5PTest
    {
        /// <summary>
        /// Exercises the native <c>H5Pset_mdc_image_config</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5Pset_mdc_image_configTest1()
        {
            var fapl = H5P.create(H5P.FILE_ACCESS);
            Assert.IsTrue(fapl >= 0);

            Assert.IsTrue(H5P.set_libver_bounds(fapl, H5F.libver_t.LATEST, H5F.libver_t.LATEST) >= 0);

            var conf = new H5AC.cache_image_config_t
            {
                version = H5AC.CURR_CACHE_IMAGE_CONFIG_VERSION,
                generate_image = 1,
                save_resize_status = 0,
                entry_ageout = H5AC.CACHE_IMAGE__ENTRY_AGEOUT__NONE
            };

            Assert.IsTrue(H5P.set_mdc_image_config(fapl, ref conf) >= 0);
            var roundTrip = new H5AC.cache_image_config_t { version = H5AC.CURR_CACHE_IMAGE_CONFIG_VERSION };
            Assert.IsTrue(H5P.get_mdc_image_config(fapl, ref roundTrip) >= 0);
            Assert.AreEqual((byte)1, roundTrip.generate_image);
            Assert.AreEqual(H5AC.CACHE_IMAGE__ENTRY_AGEOUT__NONE, roundTrip.entry_ageout);

            Assert.IsTrue(H5P.close(fapl) >= 0);
        }
    }
}

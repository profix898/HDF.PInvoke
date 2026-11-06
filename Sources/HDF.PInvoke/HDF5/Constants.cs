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
using hid_t = System.Int32;

namespace HDF.PInvoke
{
    /// <summary>
    /// Defines logical native library names shared by the HDF5 import resolver and bindings.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// The logical name used to resolve the native HDF5 core library.
        /// </summary>
        public const string DLLFileName = "hdf5";

        /// <summary>
        /// The logical name used to resolve the native HDF5 high-level library.
        /// </summary>
        public const string HLDLLFileName = "hdf5_hl";
    }
}

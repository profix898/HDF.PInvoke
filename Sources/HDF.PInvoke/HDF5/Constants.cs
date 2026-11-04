/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * Copyright by The HDF Group.                                               *
 * Copyright by the Board of Trustees of the University of Illinois.         *
 * All rights reserved.                                                      *
 *                                                                           *
 * This file is part of HDF5.  The full HDF5 copyright notice, including     *
 * terms governing use, modification, and redistribution, is contained in    *
 * the files COPYING and Copyright.html.  COPYING can be found at the root   *
 * of the source code distribution tree; Copyright.html can be found at the  *
 * root level of an installed copy of the electronic HDF5 document set and   *
 * is linked from the top-level documents page.  It can also be found at     *
 * http://hdfgroup.org/HDF5/doc/Copyright.html.  If you do not have          *
 * access to either file, you may request a copy from help@hdfgroup.org.     *
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

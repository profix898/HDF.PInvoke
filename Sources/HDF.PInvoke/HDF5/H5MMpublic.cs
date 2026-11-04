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
using System.Runtime.InteropServices;
using System.Security;

using size_t = System.IntPtr;

namespace HDF.PInvoke
{
    /// <summary>
    /// Defines callbacks used to allocate and release HDF5-managed memory.
    /// </summary>
    public unsafe sealed class H5MM
    {
        /// <summary>
        /// Allocates a requested number of bytes for native HDF5 use.
        /// </summary>
        /// <param name="size">The buffer or value size in bytes.</param>
        /// <param name="alloc_info">The opaque caller-provided callback context.</param>
        /// <returns>The resulting native memory address, or zero if allocation fails.</returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr allocate_t(size_t size, IntPtr alloc_info);

        /// <summary>
        /// Releases memory supplied to the native HDF5 memory-management callback interface.
        /// </summary>
        /// <param name="mem">The address of the memory block to resize or release.</param>
        /// <param name="free_info">The opaque caller-provided callback context.</param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void free_t(IntPtr mem, IntPtr free_info);
    }
}

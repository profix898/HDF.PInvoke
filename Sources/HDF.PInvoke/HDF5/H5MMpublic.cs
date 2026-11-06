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

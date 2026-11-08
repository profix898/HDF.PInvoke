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

using herr_t = System.Int32;
using hsize_t = System.UInt64;
using size_t = System.IntPtr;

namespace HDF.PInvoke
{
    /// <summary>
    /// Defines native HDF5 virtual-file-driver types and constants.
    /// </summary>
    public unsafe sealed class H5FD
    {
        /// <summary>Queries virtual-file-driver capability flags, widening native unsigned long output on Windows.</summary>
        public static int driver_query(long driver, out ulong flags)
        {
            // Zero padding before a four-byte Windows write into the portable eight-byte result.
            ulong value = 0;
            var result = driver_query_native(driver, (IntPtr)(&value));
            flags = result >= 0 ? value : 0;
            return result;
        }

        /// <summary>Imports native driver flags using caller-owned unsigned-long storage.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5FDdriver_query", CallingConvention = CallingConvention.Cdecl)]
        private static extern int driver_query_native(long driver, IntPtr flags);

        /// <summary>
        /// Initializes HDF5 before this API is first used.
        /// </summary>
        static H5FD()
        {
            H5.open();
        }

        /// <summary>
        /// Define enum for the source of file image callbacks
        /// </summary>
        public enum file_image_op_t
        {
            /// <summary>
            /// Indicates that no file-image operation is specified.
            /// </summary>
            NO_OP,

            /// <summary>
            /// Identifies a file-image buffer operation caused by setting a property list.
            /// </summary>
            PROPERTY_LIST_SET,

            /// <summary>
            /// Identifies a file-image buffer operation caused by copying a property list.
            /// </summary>
            PROPERTY_LIST_COPY,

            /// <summary>
            /// Identifies a file-image buffer operation caused by retrieving a property list.
            /// </summary>
            PROPERTY_LIST_GET,

            /// <summary>
            /// Identifies a file-image buffer operation caused by closing a property list.
            /// </summary>
            PROPERTY_LIST_CLOSE,

            /// <summary>
            /// Identifies a file-image buffer operation caused by opening a file.
            /// </summary>
            FILE_OPEN,

            /// <summary>
            /// Identifies a file-image buffer operation caused by resizing a file.
            /// </summary>
            FILE_RESIZE,

            /// <summary>
            /// Identifies a file-image buffer operation caused by closing a file.
            /// </summary>
            FILE_CLOSE
        }

        /// <summary>
        /// Allocates a file-image buffer on behalf of the native library.
        /// </summary>
        /// <param name="size">The buffer or value size in bytes.</param>
        /// <param name="file_image_op">The file-image operation that caused the native library to invoke this callback.</param>
        /// <param name="udata">The opaque caller-provided callback context.</param>
        /// <returns>The resulting native memory address, or zero if allocation fails.</returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr image_malloc_t
        (size_t size, file_image_op_t file_image_op, IntPtr udata);

        /// <summary>
        /// Copies file-image data between caller-managed buffers.
        /// </summary>
        /// <param name="dest">The address of the destination buffer.</param>
        /// <param name="src">The address of the source buffer.</param>
        /// <param name="size">The buffer or value size in bytes.</param>
        /// <param name="file_image_op">The file-image operation that caused the native library to invoke this callback.</param>
        /// <param name="udata">The opaque caller-provided callback context.</param>
        /// <returns>The destination buffer address.</returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr image_memcpy_t
        (IntPtr dest, IntPtr src, size_t size, file_image_op_t file_image_op,
        IntPtr udata);

        /// <summary>
        /// Resizes a file-image buffer on behalf of the native library.
        /// </summary>
        /// <param name="ptr">The address of the memory block to resize or release.</param>
        /// <param name="size">The buffer or value size in bytes.</param>
        /// <param name="file_image_op">The file-image operation that caused the native library to invoke this callback.</param>
        /// <param name="udata">The opaque caller-provided callback context.</param>
        /// <returns>The resulting native memory address, or zero if allocation fails.</returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr image_realloc_t
        (IntPtr ptr, size_t size, file_image_op_t file_image_op, IntPtr udata);

        /// <summary>
        /// Releases a buffer previously allocated for a file image.
        /// </summary>
        /// <param name="ptr">The address of the memory block to resize or release.</param>
        /// <param name="file_image_op">The file-image operation that caused the native library to invoke this callback.</param>
        /// <param name="udata">The opaque caller-provided callback context.</param>
        /// <returns>A nonnegative value on success, or a negative value on failure.</returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate herr_t image_free_t
        (IntPtr ptr, file_image_op_t file_image_op, IntPtr udata);

        /// <summary>
        /// Copies the user-data context associated with file-image callbacks.
        /// </summary>
        /// <param name="udata">The opaque caller-provided callback context.</param>
        /// <returns>The resulting native memory address, or zero if allocation fails.</returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr udata_copy_t(IntPtr udata);

        /// <summary>
        /// Releases the user-data context associated with file-image callbacks.
        /// </summary>
        /// <param name="udata">The opaque caller-provided callback context.</param>
        /// <returns>A nonnegative value on success, or a negative value on failure.</returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate herr_t udata_free_t(IntPtr udata);

        /// <summary>
        /// Define structure to hold file image callbacks
        /// </summary>
        public struct file_image_callbacks_t
        {
            /// <summary>
            /// The callback used to allocate file-image buffers.
            /// </summary>
            public image_malloc_t image_malloc;

            /// <summary>
            /// The callback used to copy file-image buffers.
            /// </summary>
            public image_memcpy_t image_memcpy;

            /// <summary>
            /// The callback used to resize file-image buffers.
            /// </summary>
            public image_realloc_t image_realloc;

            /// <summary>
            /// The callback used to release file-image buffers.
            /// </summary>
            public image_free_t image_free;

            /// <summary>
            /// The callback used to copy the user-data context.
            /// </summary>
            public udata_copy_t udata_copy;

            /// <summary>
            /// The callback used to release the user-data context.
            /// </summary>
            public udata_free_t udata_free;

            /// <summary>
            /// The opaque user-data pointer passed to the file-image callbacks.
            /// </summary>
            public IntPtr udata;
        }
    }
}

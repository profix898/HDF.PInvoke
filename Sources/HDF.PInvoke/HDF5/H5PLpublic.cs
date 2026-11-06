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
using System.Text;

using herr_t = System.Int32;
using ssize_t = System.IntPtr;
using uint32_t = System.UInt32;

namespace HDF.PInvoke
{
    /// <summary>
    /// Provides configuration of the native HDF5 plugin search path and loading policy.
    /// </summary>
    public unsafe sealed class H5PL
    {
        /// <summary>
        /// Initializes HDF5 before this API is first used.
        /// </summary>
        static H5PL()
        {
            H5.open();
        }

        /// <summary>
        /// Identifies the category of a dynamically loaded HDF5 plugin.
        /// </summary>
        public enum type_t
        {
            /// <summary>
            /// Error.
            /// </summary>
            TYPE_ERROR = -1,

            /// <summary>
            /// Filter.
            /// </summary>
            TYPE_FILTER = 0,

            /// <summary>
            /// Sentinel: This must be last!.
            /// </summary>
            TYPE_NONE = 1
        };

        /// <summary>
        /// The plugin-loading mask bit that permits filter plugins.
        /// </summary>
        public const int FILTER_PLUGIN = 0x0001;

        /// <summary>
        /// The plugin-loading mask that permits all plugin categories.
        /// </summary>
        public const int ALL_PLUGIN = 0xffff;

        /// <summary>
        /// Append a plugin path to the plugin search path.
        /// See https://support.hdfgroup.org/HDF5/doc1.8/RM/RM_H5PL.html#Plugin-Append
        /// </summary>
        /// <param name="plugin_path">The plugin path</param>
        /// <returns>Returns a non-negative value if successful; otherwise
        /// returns a negative value.</returns>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5PLappend",
            CallingConvention = CallingConvention.Cdecl),
        SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
        public static extern herr_t append([MarshalAs(UnmanagedType.LPUTF8Str)] string plugin_path);

        /// <summary>
        /// Query the plugin path at the specified index.
        /// See https://support.hdfgroup.org/HDF5/doc1.8/RM/RM_H5PL.html#Plugin-Get
        /// </summary>
        /// <param name="index">Index</param>
        /// <param name="pathname">Path name</param>
        /// <param name="size">Buffer size (in bytes)</param>
        /// <returns>Returns the length of the path, a non-negative value if
        /// successful; otherwise returns a negative value.</returns>
        public static ssize_t get(uint32_t index,
            StringBuilder pathname, IntPtr size)
        {
            byte[] buffer = Utf8Buffer.Create(pathname, size);
            var result = get_utf8(index, buffer, size);

            if (result.ToInt64() >= 0)
            {
                Utf8Buffer.CopyTo(buffer, pathname);
            }

            return result;
        }

        /// <summary>
        /// Invokes the native <c>H5PLget</c> entry point using its explicit ABI declaration.
        /// </summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5PLget", CallingConvention = CallingConvention.Cdecl)]
        private static extern ssize_t get_utf8(uint32_t index,
            [Out] byte[] buffer, IntPtr size);

        /// <summary>
        /// Query state of the loading of dynamic plugins.
        /// See https://support.hdfgroup.org/HDF5/doc1.8/RM/RM_H5PL.html#Plugin-GetLoadingState
        /// </summary>
        /// <param name="plugin_flags">List of dynamic plugin types that are
        /// enabled or disabled.</param>
        /// <returns>Returns a non-negative value if successful; otherwise
        /// returns a negative value.</returns>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5PLget_loading_state",
            CallingConvention = CallingConvention.Cdecl),
        SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
        public static extern herr_t get_loading_state(ref int plugin_flags);

        /// <summary>
        /// Insert a plugin path at the specified index.
        /// See https://support.hdfgroup.org/HDF5/doc1.8/RM/RM_H5PL.html#Plugin-Insert
        /// </summary>
        /// <param name="plugin_path">The plugin path</param>
        /// <param name="index">Index</param>
        /// <returns>Returns a non-negative value if successful; otherwise
        /// returns a negative value.</returns>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5PLinsert",
           CallingConvention = CallingConvention.Cdecl),
        SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
        public static extern herr_t insert([MarshalAs(UnmanagedType.LPUTF8Str)] string plugin_path,
            uint32_t index);

        /// <summary>
        /// Insert a plugin path at the beginning of the list.
        /// See https://support.hdfgroup.org/HDF5/doc1.8/RM/RM_H5PL.html#Plugin-Prepend
        /// </summary>
        /// <param name="plugin_path">The plugin path</param>
        /// <returns>Returns a non-negative value if successful; otherwise
        /// returns a negative value.</returns>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5PLprepend",
            CallingConvention = CallingConvention.Cdecl),
        SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
        public static extern herr_t prepend([MarshalAs(UnmanagedType.LPUTF8Str)] string plugin_path);

        /// <summary>
        /// Remove the plugin path at the specified index.
        /// See https://support.hdfgroup.org/HDF5/doc1.8/RM/RM_H5PL.html#Plugin-Remove
        /// </summary>
        /// <param name="index">Index</param>
        /// <returns>Returns a non-negative value if successful; otherwise
        /// returns a negative value.</returns>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5PLremove",
           CallingConvention = CallingConvention.Cdecl),
        SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
        public static extern herr_t remove(uint32_t index);

        /// <summary>
        /// Replace the plugin path at the specified index.
        /// See https://support.hdfgroup.org/HDF5/doc1.8/RM/RM_H5PL.html#Plugin-Replace
        /// </summary>
        /// <param name="plugin_path">The plugin path</param>
        /// <param name="index">Index</param>
        /// <returns>Returns a non-negative value if successful; otherwise
        /// returns a negative value.</returns>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5PLreplace",
            CallingConvention = CallingConvention.Cdecl),
        SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
        public static extern herr_t replace([MarshalAs(UnmanagedType.LPUTF8Str)] string plugin_path,
            uint32_t index);

        /// <summary>
        /// Control the loading of dynamic plugins.
        /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5PL.html
        /// </summary>
        /// <param name="plugin_flags">The list of dynamic plugin types to
        /// enable or disable.</param>
        /// <returns>Returns a non-negative value if successful; otherwise
        /// returns a negative value.</returns>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5PLset_loading_state",
            CallingConvention = CallingConvention.Cdecl),
        SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
        public static extern herr_t set_loading_state(int plugin_flags);

        /// <summary>
        /// Query the size of the current list of plugin paths.
        /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5PL.html
        /// </summary>
        /// <param name="listsize">The size of the current list of plugin
        /// paths.</param>
        /// <returns>Returns a non-negative value if successful; otherwise
        /// returns a negative value.</returns>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5PLsize",
            CallingConvention = CallingConvention.Cdecl),
        SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
        public static extern herr_t size(ref uint32_t listsize);
    }
}

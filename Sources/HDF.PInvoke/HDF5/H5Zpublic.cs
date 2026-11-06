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
using htri_t = System.Int32;
using size_t = System.IntPtr;

using hid_t = System.Int64;

namespace HDF.PInvoke
{
    /// <summary>
    /// Provides native HDF5 data-filter registration and capability queries.
    /// </summary>
    public unsafe sealed class H5Z
    {
        static H5Z()
        {
            H5.open();
        }

        /// <summary>
        /// Filter IDs
        /// </summary>
        public enum filter_t
        {
            /// <summary>
            /// no filter [value = -1]
            /// </summary>
            ERROR = -1,

            /// <summary>
            /// reserved indefinitely [value = 0]
            /// </summary>
            NONE = 0,

            /// <summary>
            /// deflation like gzip ]value = 1]
            /// </summary>
            DEFLATE = 1,

            /// <summary>
            /// shuffle the data [value = 2]
            /// </summary>
            SHUFFLE = 2,

            /// <summary>
            /// fletcher32 checksum of EDC [value = 3]
            /// </summary>
            FLETCHER32 = 3,

            /// <summary>
            /// szip compression [value = 4]
            /// </summary>
            SZIP = 4,

            /// <summary>
            /// nbit compression [value = 5]
            /// </summary>
            NBIT = 5,

            /// <summary>
            /// scale+offset compression [value = 6]
            /// </summary>
            SCALEOFFSET = 6,

            /// <summary>
            /// filter ids below this value are reserved for library use [value = 256]
            /// </summary>
            RESERVED = 256,

            /// <summary>
            /// maximum filter id [value = 65535]
            /// </summary>
            MAX = 65535
        }

        /// <summary>
        /// Special parameters for ScaleOffset filter
        /// </summary>
        public enum SO_scale_type_t
        {
            /// <summary>
            /// Uses decimal scaling for floating-point scale-offset compression.
            /// </summary>
            FLOAT_DSCALE = 0,

            /// <summary>
            /// Selects floating-point exponent scaling; support depends on the native scale-offset filter.
            /// </summary>
            FLOAT_ESCALE = 1,

            /// <summary>
            /// Uses integer scale-offset compression.
            /// </summary>
            INT = 2
        }

        /// <summary>
        /// Values to decide if EDC is enabled for reading data
        /// </summary>
        public enum EDC_t
        {
            /// <summary>
            /// error value
            /// </summary>
            ERROR = -1,

            /// <summary>
            /// Disables error-detection checks during dataset reads.
            /// </summary>
            DISABLE = 0,

            /// <summary>
            /// Enables error-detection checks during dataset reads.
            /// </summary>
            ENABLE = 1,

            /// <summary>
            /// Indicates that no error-detection policy is specified.
            /// </summary>
            NO = 2
        }

        /// <summary>
        /// Reports that the filter supports encoding data.
        /// </summary>
        public const uint CONFIG_ENCODE_ENABLED = 0x0001u;

        /// <summary>
        /// Reports that the filter supports decoding data.
        /// </summary>
        public const uint CONFIG_DECODE_ENABLED = 0x0002u;

        /// <summary>
        /// Return values for filter callback function
        /// </summary>
        public enum cb_return_t
        {
            /// <summary>
            /// Reports an error from the filter-failure callback.
            /// </summary>
            ERROR = -1,

            /// <summary>
            /// I/O should fail if filter fails.
            /// </summary>
            FAIL = 0,

            /// <summary>
            /// I/O continues if filter fails.
            /// </summary>
            CONT = 1,

            /// <summary>
            /// The callback-result sentinel; it is not a usable callback result.
            /// </summary>
            NO = 2
        }

        /// <summary>
        /// Filter callback function definition
        /// </summary>
        /// <param name="filter">The identifier of the filter that failed.</param>
        /// <param name="buf">The native data buffer.</param>
        /// <param name="buf_size">The buffer capacity in bytes.</param>
        /// <param name="op_data">The caller-provided callback state.</param>
        /// <returns>CONT to continue I/O despite the filter failure, or FAIL to fail I/O.</returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate cb_return_t filter_func_t
        (filter_t filter, IntPtr buf, size_t buf_size, IntPtr op_data);

        /// <summary>
        /// Determines whether a filter can be applied to a dataset&apos;s datatype and creation properties.
        /// </summary>
        /// <param name="dcpl_id">The dataset-creation property-list identifier.</param>
        /// <param name="type_id">The dataset datatype identifier.</param>
        /// <param name="space_id">The dataset dataspace identifier.</param>
        /// <returns>A positive value if applicable, zero if not applicable, or a negative value on error.</returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate htri_t can_apply_func_t
        (hid_t dcpl_id, hid_t type_id, hid_t space_id);

        /// <summary>
        /// Updates a filter&apos;s local parameters for a dataset before data is written.
        /// </summary>
        /// <param name="dcpl_id">The dataset-creation property-list identifier.</param>
        /// <param name="type_id">The dataset datatype identifier.</param>
        /// <param name="space_id">The dataset dataspace identifier.</param>
        /// <returns>A nonnegative value on success, or a negative value on failure.</returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate herr_t set_local_func_t
        (hid_t dcpl_id, hid_t type_id, hid_t space_id);

        /// <summary>
        /// Transforms a chunk buffer while applying or reversing a dataset filter.
        /// </summary>
        /// <param name="flags">The native flags controlling the filter operation, including reverse filtering when decoding.</param>
        /// <param name="cd_nelmts">The number of filter client-data values.</param>
        /// <param name="cd_values">Pointer to cd_nelmts unsigned 32-bit client-data values. Read only during the callback.</param>
        /// <param name="nbytes">The buffer or value size in bytes.</param>
        /// <param name="buf_size">The buffer or value size in bytes.</param>
        /// <param name="buf">The data buffer supplied to the native operation.</param>
        /// <returns>The number of valid bytes in the transformed buffer, or zero on failure.</returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate size_t func_t
        (uint flags, size_t cd_nelmts, IntPtr cd_values, size_t nbytes,
                 ref size_t buf_size, ref IntPtr buf);

        /// <summary>
        /// The filter table maps filter identification numbers to structs that
        /// contain a pointers to the filter function and timing statistics.
        /// </summary>
        public struct class_t
        {
            /// <summary>
            /// Version number of the <code>class_t</code> struct
            /// </summary>
            public int version;

            /// <summary>
            /// Filter ID number
            /// </summary>
            public filter_t id;

            /// <summary>
            /// Does this filter have an encoder?
            /// </summary>
            public uint encoder_present;

            /// <summary>
            /// Does this filter have a decoder?
            /// </summary>
            public uint decoder_present;

            /// <summary>
            /// Comment for debugging
            /// </summary>
            public byte* name;

            /// <summary>
            /// The "can apply" callback for a filter
            /// </summary>
            public can_apply_func_t can_apply;

            /// <summary>
            /// The "set local" callback for a filter
            /// </summary>
            public set_local_func_t set_local;

            /// <summary>
            /// The actual filter function
            /// </summary>
            public func_t filter;
        }

        /// <summary>
        /// Determines whether a filter is available.
        /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5Z.html#Compression-FilterAvail
        /// </summary>
        /// <param name="filter">Filter identifier.</param>
        /// <returns>Returns a Boolean value if successful;
        /// otherwise returns a negative value.</returns>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Zfilter_avail",
            CallingConvention = CallingConvention.Cdecl),
        SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
        public static extern htri_t filter_avail(filter_t filter);

        /// <summary>
        /// Retrieves information about a filter.
        /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5Z.html#Compression-GetFilterInfo
        /// </summary>
        /// <param name="filter">Identifier of the filter to query.</param>
        /// <param name="filter_config">A bit field encoding the returned
        /// filter information</param>
        /// <returns>Returns a non-negative value on success, a negative value
        /// on failure.</returns>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Zget_filter_info",
            CallingConvention = CallingConvention.Cdecl),
        SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
        public static extern herr_t get_filter_info
            (filter_t filter, ref uint filter_config);

        /// <summary>
        /// Registers new filter.
        /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5Z.html#Compression-Register
        /// </summary>
        /// <param name="filter_class">A pointer to a buffer for the struct
        /// containing filter-definition information.</param>
        /// <returns>Returns a non-negative value if successful; otherwise
        /// returns a negative value.</returns>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Zregister",
            CallingConvention = CallingConvention.Cdecl),
        SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
        public static extern herr_t register(ref class_t filter_class);

        /// <summary>
        /// Unregisters a filter.
        /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5Z.html#Compression-Unregister
        /// </summary>
        /// <param name="filter">Identifier of the filter to be unregistered.</param>
        /// <returns>Returns a non-negative value if successful; otherwise
        /// returns a negative value.</returns>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Zunregister",
            CallingConvention = CallingConvention.Cdecl),
        SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
        public static extern herr_t unregister(filter_t filter);
    }
}

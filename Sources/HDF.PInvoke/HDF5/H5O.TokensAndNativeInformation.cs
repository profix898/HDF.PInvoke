using System;
using System.Runtime.InteropServices;

namespace HDF.PInvoke
{
    /// <summary>Provides current HDF5 reference and token-aware H5O operations.</summary>
    public unsafe sealed partial class H5O
    {
        /// <summary>Sets or removes an object's UTF-8 comment; attributes are preferable for new application metadata.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Oset_comment", CallingConvention = CallingConvention.Cdecl)]
        public static extern int set_comment(long objectId, [MarshalAs(UnmanagedType.LPUTF8Str)] string comment);

        /// <summary>Sets or removes a named object's UTF-8 comment; pass null to remove it.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Oset_comment_by_name", CallingConvention = CallingConvention.Cdecl)]
        public static extern int set_comment_by_name(long location, [MarshalAs(UnmanagedType.LPUTF8Str)] string name, [MarshalAs(UnmanagedType.LPUTF8Str)] string comment, long access = H5P.DEFAULT);

        /// <summary>Requests identity, object type and hard-link count.</summary>
        public const uint INFO_BASIC = 1;

        /// <summary>Requests object timestamps.</summary>
        public const uint INFO_TIME = 2;

        /// <summary>Requests the attribute count.</summary>
        public const uint INFO_NUM_ATTRS = 4;

        /// <summary>Requests all fields of modern object information.</summary>
        public const uint INFO_ALL = 7;

        /// <summary>Requests native object-header storage information.</summary>
        public const uint NATIVE_INFO_HDR = 8;

        /// <summary>Requests native index and heap storage information.</summary>
        public const uint NATIVE_INFO_META_SIZE = 16;

        /// <summary>Requests all native object storage information.</summary>
        public const uint NATIVE_INFO_ALL = 24;

        /// <summary>Stores a connector-specific 16-byte object identity token.</summary>
        [StructLayout(LayoutKind.Sequential, Size = 16)]
        public struct token_t
        {
            /// <summary>Retains opaque native bytes with byte alignment.</summary>
            private fixed byte storage[16];
        }

        /// <summary>Describes storage used by an object in the native file format.</summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct native_info_t
        {
            /// <summary>Describes the native object header.</summary>
            public hdr_info_t hdr;

            /// <summary>Describes object and attribute index/heap storage.</summary>
            public meta_size_t meta_size;
        }

        /// <summary>Opens an object by its connector-specific token; close the returned identifier with H5O.close.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Oopen_by_token", CallingConvention = CallingConvention.Cdecl)]
        public static extern long open_by_token(long location, token_t token);

        /// <summary>Compares tokens through the owning connector and returns their ordering.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Otoken_cmp", CallingConvention = CallingConvention.Cdecl)]
        public static extern int token_cmp(long location, ref token_t first, ref token_t second, out int comparison);

        /// <summary>Allocates a UTF-8 token string; release it with H5.free_memory after copying its contents.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Otoken_to_str", CallingConvention = CallingConvention.Cdecl)]
        public static extern int token_to_str(long location, ref token_t token, out IntPtr text);

        /// <summary>Decodes a token string using the owning connector.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Otoken_from_str", CallingConvention = CallingConvention.Cdecl)]
        public static extern int token_from_str(long location, [MarshalAs(UnmanagedType.LPUTF8Str)] string text, out token_t token);

        /// <summary>Gets native-format header and metadata storage information selected by NATIVE_INFO flags.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Oget_native_info", CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_native_info(long location, out native_info_t information, uint fields);

        /// <summary>Gets native-format storage information for a named object.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Oget_native_info_by_name", CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_native_info_by_name(long location, [MarshalAs(UnmanagedType.LPUTF8Str)] string name, out native_info_t information, uint fields, long access = H5P.DEFAULT);

        /// <summary>Gets native-format storage information for an indexed object.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Oget_native_info_by_idx", CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_native_info_by_idx(long location, [MarshalAs(UnmanagedType.LPUTF8Str)] string group, H5.index_t index, H5.iter_order_t order, ulong position, out native_info_t information, uint fields, long access = H5P.DEFAULT);

    }
}


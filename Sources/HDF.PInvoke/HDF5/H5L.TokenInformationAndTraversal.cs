using System;
using System.Runtime.InteropServices;

namespace HDF.PInvoke
{
    /// <summary>Provides current HDF5 reference and token-aware H5L operations.</summary>
    public unsafe sealed partial class H5L
    {
        /// <summary>Stores either a hard-link token or a soft/user-defined link value length.</summary>
        [StructLayout(LayoutKind.Explicit, Size = 16)]
        public struct info2_union_t
        {
            /// <summary>Identifies the object targeted by a hard link.</summary>
            [FieldOffset(0)] public H5O.token_t token;

            /// <summary>Gives the value buffer length for other link types.</summary>
            [FieldOffset(0)] public UIntPtr val_size;
        }

        /// <summary>Matches native token-aware link information on supported 64-bit platforms.</summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct info2_t
        {
            /// <summary>Identifies the link category.</summary>
            public type_t type;

            /// <summary>Indicates whether creation order is available, as a one-byte C boolean.</summary>
            public byte corder_valid;

            /// <summary>Gives the tracked creation order.</summary>
            public long corder;

            /// <summary>Identifies the link name's character encoding.</summary>
            public H5T.cset_t cset;

            /// <summary>Contains the link token or value length.</summary>
            public info2_union_t u;
        }

        /// <summary>Receives borrowed link information; return zero to continue, positive to stop, or negative on error. Do not throw through native code.</summary>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int iterate2_t(long group, [MarshalAs(UnmanagedType.LPUTF8Str)] string name, ref info2_t information, IntPtr state);

        /// <summary>Gets token-aware information about a link.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Lget_info2", CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_info2(long location, [MarshalAs(UnmanagedType.LPUTF8Str)] string name, out info2_t information, long access = H5P.DEFAULT);

        /// <summary>Gets token-aware link information by index.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Lget_info_by_idx2", CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_info_by_idx2(long location, [MarshalAs(UnmanagedType.LPUTF8Str)] string group, H5.index_t index, H5.iter_order_t order, ulong position, out info2_t information, long access = H5P.DEFAULT);

        /// <summary>Iterates links with token-aware information; callback and state are borrowed for this synchronous call.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Literate2", CallingConvention = CallingConvention.Cdecl)]
        public static extern int iterate2(long group, H5.index_t index, H5.iter_order_t order, ref ulong position, iterate2_t callback, IntPtr state);

        /// <summary>Iterates a named group's links; callback must not throw through native code.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Literate_by_name2", CallingConvention = CallingConvention.Cdecl)]
        public static extern int iterate_by_name2(long location, [MarshalAs(UnmanagedType.LPUTF8Str)] string group, H5.index_t index, H5.iter_order_t order, ref ulong position, iterate2_t callback, IntPtr state, long access = H5P.DEFAULT);

        /// <summary>Recursively visits links; callback and state are borrowed for this synchronous call.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Lvisit2", CallingConvention = CallingConvention.Cdecl)]
        public static extern int visit2(long group, H5.index_t index, H5.iter_order_t order, iterate2_t callback, IntPtr state);

        /// <summary>Recursively visits a named group's links; callback must not throw through native code.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Lvisit_by_name2", CallingConvention = CallingConvention.Cdecl)]
        public static extern int visit_by_name2(long location, [MarshalAs(UnmanagedType.LPUTF8Str)] string group, H5.index_t index, H5.iter_order_t order, iterate2_t callback, IntPtr state, long access = H5P.DEFAULT);

    }
}


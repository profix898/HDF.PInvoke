using System;
using System.Runtime.CompilerServices;
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

        /// <summary>Receives borrowed link information; return zero to continue, positive to stop, or negative on error.</summary>
        public delegate int iterate2_t(long group, string name, ref info2_t information, IntPtr state);

        /// <summary>Gets token-aware information about a link.</summary>
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Lget_info2", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int get_info2(long location, string name, out info2_t information, long access = H5P.DEFAULT);

        /// <summary>Gets token-aware link information by index.</summary>
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Lget_info_by_idx2", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int get_info_by_idx2(long location, string group, H5.index_t index, H5.iter_order_t order, ulong position, out info2_t information, long access = H5P.DEFAULT);

        /// <summary>Iterates links with token-aware information; callback and state are borrowed for this synchronous call.</summary>
        public static int iterate2(long group, H5.index_t index, H5.iter_order_t order, ref ulong position, iterate2_t callback, IntPtr state)
        {
            fixed (ulong* posPtr = &position)
            {
                return CallbackBridge.InvokeLink2(callback, state, (group, index, order, PosPtr: (IntPtr)posPtr),
                    static (s, cb, ctx) => iterate2_native(s.group, s.index, s.order, ref *(ulong*)s.PosPtr, cb, ctx));
            }
        }

        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Literate2"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        private static partial int iterate2_native(long group, H5.index_t index, H5.iter_order_t order, ref ulong position, IntPtr callback, IntPtr state);

        /// <summary>Iterates a named group's links with token-aware information.</summary>
        public static int iterate_by_name2(long location, string group, H5.index_t index, H5.iter_order_t order, ref ulong position, iterate2_t callback, IntPtr state, long access = H5P.DEFAULT)
        {
            fixed (ulong* posPtr = &position)
            {
                return CallbackBridge.InvokeLink2(callback, state, (location, group, index, order, PosPtr: (IntPtr)posPtr, access),
                    static (s, cb, ctx) => iterate_by_name2_native(s.location, s.group, s.index, s.order, ref *(ulong*)s.PosPtr, cb, ctx, s.access));
            }
        }

        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Literate_by_name2", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        private static partial int iterate_by_name2_native(long location, string group, H5.index_t index, H5.iter_order_t order, ref ulong position, IntPtr callback, IntPtr state, long access);

        /// <summary>Recursively visits links; callback and state are borrowed for this synchronous call.</summary>
        public static int visit2(long group, H5.index_t index, H5.iter_order_t order, iterate2_t callback, IntPtr state) =>
            CallbackBridge.InvokeLink2(callback, state, (group, index, order),
                static (s, cb, ctx) => visit2_native(s.group, s.index, s.order, cb, ctx));

        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Lvisit2"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        private static partial int visit2_native(long group, H5.index_t index, H5.iter_order_t order, IntPtr callback, IntPtr state);

        /// <summary>Recursively visits a named group's links with token-aware information.</summary>
        public static int visit_by_name2(long location, string group, H5.index_t index, H5.iter_order_t order, iterate2_t callback, IntPtr state, long access = H5P.DEFAULT) =>
            CallbackBridge.InvokeLink2(callback, state, (location, group, index, order, access),
                static (s, cb, ctx) => visit_by_name2_native(s.location, s.group, s.index, s.order, cb, ctx, s.access));

        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Lvisit_by_name2", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        private static partial int visit_by_name2_native(long location, string group, H5.index_t index, H5.iter_order_t order, IntPtr callback, IntPtr state, long access);
    }
}

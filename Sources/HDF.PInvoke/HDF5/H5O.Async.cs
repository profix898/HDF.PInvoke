using System;
using System.Runtime.InteropServices;

namespace HDF.PInvoke
{
    /// <summary>Provides native event-set operations with explicit asynchronous buffer ownership.</summary>
    public unsafe sealed partial class H5O
    {
        /// <summary>Invokes H5Oopen_async using the supplied event set; completion depends on connector capabilities.</summary>
        /// <remarks>All pointers, including null-terminated UTF-8 strings and output scalars, must remain valid until completion. Keep callbacks rooted until completion. Use H5ES.wait before reading outputs or releasing resources. H5ES.NONE requests synchronous execution. The packaged native connector does not provide background execution.</remarks>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Oopen_async", CallingConvention = CallingConvention.Cdecl)]
        public static extern long open_async(IntPtr app_file, IntPtr app_func, uint app_line, long loc_id, IntPtr name, long lapl_id, long es_id);

        /// <summary>Invokes H5Oopen_by_idx_async using the supplied event set; completion depends on connector capabilities.</summary>
        /// <remarks>All pointers, including null-terminated UTF-8 strings and output scalars, must remain valid until completion. Keep callbacks rooted until completion. Use H5ES.wait before reading outputs or releasing resources. H5ES.NONE requests synchronous execution. The packaged native connector does not provide background execution.</remarks>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Oopen_by_idx_async", CallingConvention = CallingConvention.Cdecl)]
        public static extern long open_by_idx_async(IntPtr app_file, IntPtr app_func, uint app_line, long loc_id, IntPtr group_name, H5.index_t idx_type, H5.iter_order_t order, ulong n, long lapl_id, long es_id);

        /// <summary>Invokes H5Oget_info_by_name_async using the supplied event set; completion depends on connector capabilities.</summary>
        /// <remarks>All pointers, including null-terminated UTF-8 strings and output scalars, must remain valid until completion. Keep callbacks rooted until completion. Use H5ES.wait before reading outputs or releasing resources. H5ES.NONE requests synchronous execution. The packaged native connector does not provide background execution.</remarks>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Oget_info_by_name_async", CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_info_by_name_async(IntPtr app_file, IntPtr app_func, uint app_line, long loc_id, IntPtr name, IntPtr oinfo, uint fields, long lapl_id, long es_id);

        /// <summary>Invokes H5Ocopy_async using the supplied event set; completion depends on connector capabilities.</summary>
        /// <remarks>All pointers, including null-terminated UTF-8 strings and output scalars, must remain valid until completion. Keep callbacks rooted until completion. Use H5ES.wait before reading outputs or releasing resources. H5ES.NONE requests synchronous execution. The packaged native connector does not provide background execution.</remarks>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Ocopy_async", CallingConvention = CallingConvention.Cdecl)]
        public static extern int copy_async(IntPtr app_file, IntPtr app_func, uint app_line, long src_loc_id, IntPtr src_name, long dst_loc_id, IntPtr dst_name, long ocpypl_id, long lcpl_id, long es_id);

        /// <summary>Invokes H5Oclose_async using the supplied event set; completion depends on connector capabilities.</summary>
        /// <remarks>All pointers, including null-terminated UTF-8 strings and output scalars, must remain valid until completion. Keep callbacks rooted until completion. Use H5ES.wait before reading outputs or releasing resources. H5ES.NONE requests synchronous execution. The packaged native connector does not provide background execution.</remarks>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Oclose_async", CallingConvention = CallingConvention.Cdecl)]
        public static extern int close_async(IntPtr app_file, IntPtr app_func, uint app_line, long object_id, long es_id);

        /// <summary>Invokes H5Oflush_async using the supplied event set; completion depends on connector capabilities.</summary>
        /// <remarks>All pointers, including null-terminated UTF-8 strings and output scalars, must remain valid until completion. Keep callbacks rooted until completion. Use H5ES.wait before reading outputs or releasing resources. H5ES.NONE requests synchronous execution. The packaged native connector does not provide background execution.</remarks>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Oflush_async", CallingConvention = CallingConvention.Cdecl)]
        public static extern int flush_async(IntPtr app_file, IntPtr app_func, uint app_line, long obj_id, long es_id);

        /// <summary>Invokes H5Orefresh_async using the supplied event set; completion depends on connector capabilities.</summary>
        /// <remarks>All pointers, including null-terminated UTF-8 strings and output scalars, must remain valid until completion. Keep callbacks rooted until completion. Use H5ES.wait before reading outputs or releasing resources. H5ES.NONE requests synchronous execution. The packaged native connector does not provide background execution.</remarks>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Orefresh_async", CallingConvention = CallingConvention.Cdecl)]
        public static extern int refresh_async(IntPtr app_file, IntPtr app_func, uint app_line, long oid, long es_id);

    }
}

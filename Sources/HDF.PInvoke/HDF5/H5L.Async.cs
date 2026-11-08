using System;
using System.Runtime.InteropServices;

namespace HDF.PInvoke
{
    /// <summary>Provides native event-set operations with explicit asynchronous buffer ownership.</summary>
    public unsafe sealed partial class H5L
    {
        /// <summary>Invokes H5Lcreate_hard_async using the supplied event set; completion depends on connector capabilities.</summary>
        /// <remarks>All pointers, including null-terminated UTF-8 strings and output scalars, must remain valid until completion. Keep callbacks rooted until completion. Use H5ES.wait before reading outputs or releasing resources. H5ES.NONE requests synchronous execution. The packaged native connector does not provide background execution.</remarks>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Lcreate_hard_async", CallingConvention = CallingConvention.Cdecl)]
        public static extern int create_hard_async(IntPtr app_file, IntPtr app_func, uint app_line, long cur_loc_id, IntPtr cur_name, long new_loc_id, IntPtr new_name, long lcpl_id, long lapl_id, long es_id);

        /// <summary>Invokes H5Lcreate_soft_async using the supplied event set; completion depends on connector capabilities.</summary>
        /// <remarks>All pointers, including null-terminated UTF-8 strings and output scalars, must remain valid until completion. Keep callbacks rooted until completion. Use H5ES.wait before reading outputs or releasing resources. H5ES.NONE requests synchronous execution. The packaged native connector does not provide background execution.</remarks>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Lcreate_soft_async", CallingConvention = CallingConvention.Cdecl)]
        public static extern int create_soft_async(IntPtr app_file, IntPtr app_func, uint app_line, IntPtr link_target, long link_loc_id, IntPtr link_name, long lcpl_id, long lapl_id, long es_id);

        /// <summary>Invokes H5Ldelete_async using the supplied event set; completion depends on connector capabilities.</summary>
        /// <remarks>All pointers, including null-terminated UTF-8 strings and output scalars, must remain valid until completion. Keep callbacks rooted until completion. Use H5ES.wait before reading outputs or releasing resources. H5ES.NONE requests synchronous execution. The packaged native connector does not provide background execution.</remarks>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Ldelete_async", CallingConvention = CallingConvention.Cdecl)]
        public static extern int delete_async(IntPtr app_file, IntPtr app_func, uint app_line, long loc_id, IntPtr name, long lapl_id, long es_id);

        /// <summary>Invokes H5Ldelete_by_idx_async using the supplied event set; completion depends on connector capabilities.</summary>
        /// <remarks>All pointers, including null-terminated UTF-8 strings and output scalars, must remain valid until completion. Keep callbacks rooted until completion. Use H5ES.wait before reading outputs or releasing resources. H5ES.NONE requests synchronous execution. The packaged native connector does not provide background execution.</remarks>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Ldelete_by_idx_async", CallingConvention = CallingConvention.Cdecl)]
        public static extern int delete_by_idx_async(IntPtr app_file, IntPtr app_func, uint app_line, long loc_id, IntPtr group_name, H5.index_t idx_type, H5.iter_order_t order, ulong n, long lapl_id, long es_id);

        /// <summary>Invokes H5Lexists_async using the supplied event set; completion depends on connector capabilities.</summary>
        /// <remarks>All pointers, including null-terminated UTF-8 strings and output scalars, must remain valid until completion. Keep callbacks rooted until completion. Use H5ES.wait before reading outputs or releasing resources. H5ES.NONE requests synchronous execution. The packaged native connector does not provide background execution.</remarks>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Lexists_async", CallingConvention = CallingConvention.Cdecl)]
        public static extern int exists_async(IntPtr app_file, IntPtr app_func, uint app_line, long loc_id, IntPtr name, IntPtr exists, long lapl_id, long es_id);

        /// <summary>Invokes H5Literate_async using the supplied event set; completion depends on connector capabilities.</summary>
        /// <remarks>All pointers, including null-terminated UTF-8 strings and output scalars, must remain valid until completion. Keep callbacks rooted until completion. Use H5ES.wait before reading outputs or releasing resources. H5ES.NONE requests synchronous execution. The packaged native connector does not provide background execution.</remarks>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Literate_async", CallingConvention = CallingConvention.Cdecl)]
        public static extern int iterate_async(IntPtr app_file, IntPtr app_func, uint app_line, long group_id, H5.index_t idx_type, H5.iter_order_t order, IntPtr idx_p, H5L.iterate2_t op, IntPtr op_data, long es_id);

    }
}

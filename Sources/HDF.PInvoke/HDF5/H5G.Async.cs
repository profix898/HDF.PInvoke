using System;
using System.Runtime.InteropServices;

namespace HDF.PInvoke
{
    /// <summary>Provides native event-set operations with explicit asynchronous buffer ownership.</summary>
    public unsafe sealed partial class H5G
    {
        /// <summary>Invokes H5Gcreate_async using the supplied event set; completion depends on connector capabilities.</summary>
        /// <remarks>All pointers, including null-terminated UTF-8 strings and output scalars, must remain valid until completion. Keep callbacks rooted until completion. Use H5ES.wait before reading outputs or releasing resources. H5ES.NONE requests synchronous execution. The packaged native connector does not provide background execution.</remarks>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Gcreate_async", CallingConvention = CallingConvention.Cdecl)]
        public static extern long create_async(IntPtr app_file, IntPtr app_func, uint app_line, long loc_id, IntPtr name, long lcpl_id, long gcpl_id, long gapl_id, long es_id);

        /// <summary>Invokes H5Gopen_async using the supplied event set; completion depends on connector capabilities.</summary>
        /// <remarks>All pointers, including null-terminated UTF-8 strings and output scalars, must remain valid until completion. Keep callbacks rooted until completion. Use H5ES.wait before reading outputs or releasing resources. H5ES.NONE requests synchronous execution. The packaged native connector does not provide background execution.</remarks>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Gopen_async", CallingConvention = CallingConvention.Cdecl)]
        public static extern long open_async(IntPtr app_file, IntPtr app_func, uint app_line, long loc_id, IntPtr name, long gapl_id, long es_id);

        /// <summary>Invokes H5Gget_info_async using the supplied event set; completion depends on connector capabilities.</summary>
        /// <remarks>All pointers, including null-terminated UTF-8 strings and output scalars, must remain valid until completion. Keep callbacks rooted until completion. Use H5ES.wait before reading outputs or releasing resources. H5ES.NONE requests synchronous execution. The packaged native connector does not provide background execution.</remarks>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Gget_info_async", CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_info_async(IntPtr app_file, IntPtr app_func, uint app_line, long loc_id, IntPtr ginfo, long es_id);

        /// <summary>Invokes H5Gget_info_by_name_async using the supplied event set; completion depends on connector capabilities.</summary>
        /// <remarks>All pointers, including null-terminated UTF-8 strings and output scalars, must remain valid until completion. Keep callbacks rooted until completion. Use H5ES.wait before reading outputs or releasing resources. H5ES.NONE requests synchronous execution. The packaged native connector does not provide background execution.</remarks>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Gget_info_by_name_async", CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_info_by_name_async(IntPtr app_file, IntPtr app_func, uint app_line, long loc_id, IntPtr name, IntPtr ginfo, long lapl_id, long es_id);

        /// <summary>Invokes H5Gget_info_by_idx_async using the supplied event set; completion depends on connector capabilities.</summary>
        /// <remarks>All pointers, including null-terminated UTF-8 strings and output scalars, must remain valid until completion. Keep callbacks rooted until completion. Use H5ES.wait before reading outputs or releasing resources. H5ES.NONE requests synchronous execution. The packaged native connector does not provide background execution.</remarks>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Gget_info_by_idx_async", CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_info_by_idx_async(IntPtr app_file, IntPtr app_func, uint app_line, long loc_id, IntPtr group_name, H5.index_t idx_type, H5.iter_order_t order, ulong n, IntPtr ginfo, long lapl_id, long es_id);

        /// <summary>Invokes H5Gclose_async using the supplied event set; completion depends on connector capabilities.</summary>
        /// <remarks>All pointers, including null-terminated UTF-8 strings and output scalars, must remain valid until completion. Keep callbacks rooted until completion. Use H5ES.wait before reading outputs or releasing resources. H5ES.NONE requests synchronous execution. The packaged native connector does not provide background execution.</remarks>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Gclose_async", CallingConvention = CallingConvention.Cdecl)]
        public static extern int close_async(IntPtr app_file, IntPtr app_func, uint app_line, long group_id, long es_id);

    }
}

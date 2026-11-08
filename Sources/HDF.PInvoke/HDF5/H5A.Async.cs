using System;
using System.Runtime.InteropServices;

namespace HDF.PInvoke
{
    /// <summary>Provides native event-set operations with explicit asynchronous buffer ownership.</summary>
    public unsafe sealed partial class H5A
    {
        /// <summary>Invokes H5Aclose_async using the supplied event set; completion depends on connector capabilities.</summary>
        /// <remarks>All pointers, including null-terminated UTF-8 strings and output scalars, must remain valid until completion. Keep callbacks rooted until completion. Use H5ES.wait before reading outputs or releasing resources. H5ES.NONE requests synchronous execution. The packaged native connector does not provide background execution.</remarks>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Aclose_async", CallingConvention = CallingConvention.Cdecl)]
        public static extern int close_async(IntPtr app_file, IntPtr app_func, uint app_line, long attr_id, long es_id);

        /// <summary>Invokes H5Acreate_async using the supplied event set; completion depends on connector capabilities.</summary>
        /// <remarks>All pointers, including null-terminated UTF-8 strings and output scalars, must remain valid until completion. Keep callbacks rooted until completion. Use H5ES.wait before reading outputs or releasing resources. H5ES.NONE requests synchronous execution. The packaged native connector does not provide background execution.</remarks>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Acreate_async", CallingConvention = CallingConvention.Cdecl)]
        public static extern long create_async(IntPtr app_file, IntPtr app_func, uint app_line, long loc_id, IntPtr attr_name, long type_id, long space_id, long acpl_id, long aapl_id, long es_id);

        /// <summary>Invokes H5Acreate_by_name_async using the supplied event set; completion depends on connector capabilities.</summary>
        /// <remarks>All pointers, including null-terminated UTF-8 strings and output scalars, must remain valid until completion. Keep callbacks rooted until completion. Use H5ES.wait before reading outputs or releasing resources. H5ES.NONE requests synchronous execution. The packaged native connector does not provide background execution.</remarks>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Acreate_by_name_async", CallingConvention = CallingConvention.Cdecl)]
        public static extern long create_by_name_async(IntPtr app_file, IntPtr app_func, uint app_line, long loc_id, IntPtr obj_name, IntPtr attr_name, long type_id, long space_id, long acpl_id, long aapl_id, long lapl_id, long es_id);

        /// <summary>Invokes H5Aexists_async using the supplied event set; completion depends on connector capabilities.</summary>
        /// <remarks>All pointers, including null-terminated UTF-8 strings and output scalars, must remain valid until completion. Keep callbacks rooted until completion. Use H5ES.wait before reading outputs or releasing resources. H5ES.NONE requests synchronous execution. The packaged native connector does not provide background execution.</remarks>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Aexists_async", CallingConvention = CallingConvention.Cdecl)]
        public static extern int exists_async(IntPtr app_file, IntPtr app_func, uint app_line, long obj_id, IntPtr attr_name, IntPtr exists, long es_id);

        /// <summary>Invokes H5Aexists_by_name_async using the supplied event set; completion depends on connector capabilities.</summary>
        /// <remarks>All pointers, including null-terminated UTF-8 strings and output scalars, must remain valid until completion. Keep callbacks rooted until completion. Use H5ES.wait before reading outputs or releasing resources. H5ES.NONE requests synchronous execution. The packaged native connector does not provide background execution.</remarks>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Aexists_by_name_async", CallingConvention = CallingConvention.Cdecl)]
        public static extern int exists_by_name_async(IntPtr app_file, IntPtr app_func, uint app_line, long loc_id, IntPtr obj_name, IntPtr attr_name, IntPtr exists, long lapl_id, long es_id);

        /// <summary>Invokes H5Aopen_async using the supplied event set; completion depends on connector capabilities.</summary>
        /// <remarks>All pointers, including null-terminated UTF-8 strings and output scalars, must remain valid until completion. Keep callbacks rooted until completion. Use H5ES.wait before reading outputs or releasing resources. H5ES.NONE requests synchronous execution. The packaged native connector does not provide background execution.</remarks>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Aopen_async", CallingConvention = CallingConvention.Cdecl)]
        public static extern long open_async(IntPtr app_file, IntPtr app_func, uint app_line, long obj_id, IntPtr attr_name, long aapl_id, long es_id);

        /// <summary>Invokes H5Aopen_by_idx_async using the supplied event set; completion depends on connector capabilities.</summary>
        /// <remarks>All pointers, including null-terminated UTF-8 strings and output scalars, must remain valid until completion. Keep callbacks rooted until completion. Use H5ES.wait before reading outputs or releasing resources. H5ES.NONE requests synchronous execution. The packaged native connector does not provide background execution.</remarks>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Aopen_by_idx_async", CallingConvention = CallingConvention.Cdecl)]
        public static extern long open_by_idx_async(IntPtr app_file, IntPtr app_func, uint app_line, long loc_id, IntPtr obj_name, H5.index_t idx_type, H5.iter_order_t order, ulong n, long aapl_id, long lapl_id, long es_id);

        /// <summary>Invokes H5Aopen_by_name_async using the supplied event set; completion depends on connector capabilities.</summary>
        /// <remarks>All pointers, including null-terminated UTF-8 strings and output scalars, must remain valid until completion. Keep callbacks rooted until completion. Use H5ES.wait before reading outputs or releasing resources. H5ES.NONE requests synchronous execution. The packaged native connector does not provide background execution.</remarks>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Aopen_by_name_async", CallingConvention = CallingConvention.Cdecl)]
        public static extern long open_by_name_async(IntPtr app_file, IntPtr app_func, uint app_line, long loc_id, IntPtr obj_name, IntPtr attr_name, long aapl_id, long lapl_id, long es_id);

        /// <summary>Invokes H5Aread_async using the supplied event set; completion depends on connector capabilities.</summary>
        /// <remarks>All pointers, including null-terminated UTF-8 strings and output scalars, must remain valid until completion. Keep callbacks rooted until completion. Use H5ES.wait before reading outputs or releasing resources. H5ES.NONE requests synchronous execution. The packaged native connector does not provide background execution.</remarks>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Aread_async", CallingConvention = CallingConvention.Cdecl)]
        public static extern int read_async(IntPtr app_file, IntPtr app_func, uint app_line, long attr_id, long dtype_id, IntPtr buf, long es_id);

        /// <summary>Invokes H5Arename_async using the supplied event set; completion depends on connector capabilities.</summary>
        /// <remarks>All pointers, including null-terminated UTF-8 strings and output scalars, must remain valid until completion. Keep callbacks rooted until completion. Use H5ES.wait before reading outputs or releasing resources. H5ES.NONE requests synchronous execution. The packaged native connector does not provide background execution.</remarks>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Arename_async", CallingConvention = CallingConvention.Cdecl)]
        public static extern int rename_async(IntPtr app_file, IntPtr app_func, uint app_line, long loc_id, IntPtr old_name, IntPtr new_name, long es_id);

        /// <summary>Invokes H5Arename_by_name_async using the supplied event set; completion depends on connector capabilities.</summary>
        /// <remarks>All pointers, including null-terminated UTF-8 strings and output scalars, must remain valid until completion. Keep callbacks rooted until completion. Use H5ES.wait before reading outputs or releasing resources. H5ES.NONE requests synchronous execution. The packaged native connector does not provide background execution.</remarks>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Arename_by_name_async", CallingConvention = CallingConvention.Cdecl)]
        public static extern int rename_by_name_async(IntPtr app_file, IntPtr app_func, uint app_line, long loc_id, IntPtr obj_name, IntPtr old_attr_name, IntPtr new_attr_name, long lapl_id, long es_id);

        /// <summary>Invokes H5Awrite_async using the supplied event set; completion depends on connector capabilities.</summary>
        /// <remarks>All pointers, including null-terminated UTF-8 strings and output scalars, must remain valid until completion. Keep callbacks rooted until completion. Use H5ES.wait before reading outputs or releasing resources. H5ES.NONE requests synchronous execution. The packaged native connector does not provide background execution.</remarks>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Awrite_async", CallingConvention = CallingConvention.Cdecl)]
        public static extern int write_async(IntPtr app_file, IntPtr app_func, uint app_line, long attr_id, long type_id, IntPtr buf, long es_id);

    }
}

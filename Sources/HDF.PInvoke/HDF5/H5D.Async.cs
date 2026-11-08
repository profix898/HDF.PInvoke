using System;
using System.Runtime.InteropServices;

namespace HDF.PInvoke
{
    /// <summary>Provides native event-set operations with explicit asynchronous buffer ownership.</summary>
    public unsafe sealed partial class H5D
    {
        /// <summary>Invokes H5Dcreate_async using the supplied event set; completion depends on connector capabilities.</summary>
        /// <remarks>All pointers, including null-terminated UTF-8 strings and output scalars, must remain valid until completion. Keep callbacks rooted until completion. Use H5ES.wait before reading outputs or releasing resources. H5ES.NONE requests synchronous execution. The packaged native connector does not provide background execution.</remarks>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Dcreate_async", CallingConvention = CallingConvention.Cdecl)]
        public static extern long create_async(IntPtr app_file, IntPtr app_func, uint app_line, long loc_id, IntPtr name, long type_id, long space_id, long lcpl_id, long dcpl_id, long dapl_id, long es_id);

        /// <summary>Invokes H5Dopen_async using the supplied event set; completion depends on connector capabilities.</summary>
        /// <remarks>All pointers, including null-terminated UTF-8 strings and output scalars, must remain valid until completion. Keep callbacks rooted until completion. Use H5ES.wait before reading outputs or releasing resources. H5ES.NONE requests synchronous execution. The packaged native connector does not provide background execution.</remarks>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Dopen_async", CallingConvention = CallingConvention.Cdecl)]
        public static extern long open_async(IntPtr app_file, IntPtr app_func, uint app_line, long loc_id, IntPtr name, long dapl_id, long es_id);

        /// <summary>Invokes H5Dget_space_async using the supplied event set; completion depends on connector capabilities.</summary>
        /// <remarks>All pointers, including null-terminated UTF-8 strings and output scalars, must remain valid until completion. Keep callbacks rooted until completion. Use H5ES.wait before reading outputs or releasing resources. H5ES.NONE requests synchronous execution. The packaged native connector does not provide background execution.</remarks>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Dget_space_async", CallingConvention = CallingConvention.Cdecl)]
        public static extern long get_space_async(IntPtr app_file, IntPtr app_func, uint app_line, long dset_id, long es_id);

        /// <summary>Invokes H5Dread_async using the supplied event set; completion depends on connector capabilities.</summary>
        /// <remarks>All pointers, including null-terminated UTF-8 strings and output scalars, must remain valid until completion. Keep callbacks rooted until completion. Use H5ES.wait before reading outputs or releasing resources. H5ES.NONE requests synchronous execution. The packaged native connector does not provide background execution.</remarks>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Dread_async", CallingConvention = CallingConvention.Cdecl)]
        public static extern int read_async(IntPtr app_file, IntPtr app_func, uint app_line, long dset_id, long mem_type_id, long mem_space_id, long file_space_id, long dxpl_id, IntPtr buf, long es_id);

        /// <summary>Invokes H5Dread_multi_async using the supplied event set; completion depends on connector capabilities.</summary>
        /// <remarks>All pointers, including null-terminated UTF-8 strings and output scalars, must remain valid until completion. Keep callbacks rooted until completion. Use H5ES.wait before reading outputs or releasing resources. H5ES.NONE requests synchronous execution. The packaged native connector does not provide background execution.</remarks>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Dread_multi_async", CallingConvention = CallingConvention.Cdecl)]
        public static extern int read_multi_async(IntPtr app_file, IntPtr app_func, uint app_line, UIntPtr count, IntPtr dset_id, IntPtr mem_type_id, IntPtr mem_space_id, IntPtr file_space_id, long dxpl_id, IntPtr buf, long es_id);

        /// <summary>Invokes H5Dwrite_async using the supplied event set; completion depends on connector capabilities.</summary>
        /// <remarks>All pointers, including null-terminated UTF-8 strings and output scalars, must remain valid until completion. Keep callbacks rooted until completion. Use H5ES.wait before reading outputs or releasing resources. H5ES.NONE requests synchronous execution. The packaged native connector does not provide background execution.</remarks>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Dwrite_async", CallingConvention = CallingConvention.Cdecl)]
        public static extern int write_async(IntPtr app_file, IntPtr app_func, uint app_line, long dset_id, long mem_type_id, long mem_space_id, long file_space_id, long dxpl_id, IntPtr buf, long es_id);

        /// <summary>Invokes H5Dwrite_multi_async using the supplied event set; completion depends on connector capabilities.</summary>
        /// <remarks>All pointers, including null-terminated UTF-8 strings and output scalars, must remain valid until completion. Keep callbacks rooted until completion. Use H5ES.wait before reading outputs or releasing resources. H5ES.NONE requests synchronous execution. The packaged native connector does not provide background execution.</remarks>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Dwrite_multi_async", CallingConvention = CallingConvention.Cdecl)]
        public static extern int write_multi_async(IntPtr app_file, IntPtr app_func, uint app_line, UIntPtr count, IntPtr dset_id, IntPtr mem_type_id, IntPtr mem_space_id, IntPtr file_space_id, long dxpl_id, IntPtr buf, long es_id);

        /// <summary>Invokes H5Dset_extent_async using the supplied event set; completion depends on connector capabilities.</summary>
        /// <remarks>All pointers, including null-terminated UTF-8 strings and output scalars, must remain valid until completion. Keep callbacks rooted until completion. Use H5ES.wait before reading outputs or releasing resources. H5ES.NONE requests synchronous execution. The packaged native connector does not provide background execution.</remarks>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Dset_extent_async", CallingConvention = CallingConvention.Cdecl)]
        public static extern int set_extent_async(IntPtr app_file, IntPtr app_func, uint app_line, long dset_id, IntPtr size, long es_id);

        /// <summary>Invokes H5Dclose_async using the supplied event set; completion depends on connector capabilities.</summary>
        /// <remarks>All pointers, including null-terminated UTF-8 strings and output scalars, must remain valid until completion. Keep callbacks rooted until completion. Use H5ES.wait before reading outputs or releasing resources. H5ES.NONE requests synchronous execution. The packaged native connector does not provide background execution.</remarks>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Dclose_async", CallingConvention = CallingConvention.Cdecl)]
        public static extern int close_async(IntPtr app_file, IntPtr app_func, uint app_line, long dset_id, long es_id);

    }
}

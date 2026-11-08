using System;
using System.Runtime.InteropServices;

namespace HDF.PInvoke
{
    /// <summary>Provides native event-set operations with explicit asynchronous buffer ownership.</summary>
    public unsafe sealed partial class H5F
    {
        /// <summary>Invokes H5Fcreate_async using the supplied event set; completion depends on connector capabilities.</summary>
        /// <remarks>All pointers, including null-terminated UTF-8 strings and output scalars, must remain valid until completion. Keep callbacks rooted until completion. Use H5ES.wait before reading outputs or releasing resources. H5ES.NONE requests synchronous execution. The packaged native connector does not provide background execution.</remarks>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Fcreate_async", CallingConvention = CallingConvention.Cdecl)]
        public static extern long create_async(IntPtr app_file, IntPtr app_func, uint app_line, IntPtr filename, uint flags, long fcpl_id, long fapl_id, long es_id);

        /// <summary>Invokes H5Fopen_async using the supplied event set; completion depends on connector capabilities.</summary>
        /// <remarks>All pointers, including null-terminated UTF-8 strings and output scalars, must remain valid until completion. Keep callbacks rooted until completion. Use H5ES.wait before reading outputs or releasing resources. H5ES.NONE requests synchronous execution. The packaged native connector does not provide background execution.</remarks>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Fopen_async", CallingConvention = CallingConvention.Cdecl)]
        public static extern long open_async(IntPtr app_file, IntPtr app_func, uint app_line, IntPtr filename, uint flags, long access_plist, long es_id);

        /// <summary>Invokes H5Freopen_async using the supplied event set; completion depends on connector capabilities.</summary>
        /// <remarks>All pointers, including null-terminated UTF-8 strings and output scalars, must remain valid until completion. Keep callbacks rooted until completion. Use H5ES.wait before reading outputs or releasing resources. H5ES.NONE requests synchronous execution. The packaged native connector does not provide background execution.</remarks>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Freopen_async", CallingConvention = CallingConvention.Cdecl)]
        public static extern long reopen_async(IntPtr app_file, IntPtr app_func, uint app_line, long file_id, long es_id);

        /// <summary>Invokes H5Fflush_async using the supplied event set; completion depends on connector capabilities.</summary>
        /// <remarks>All pointers, including null-terminated UTF-8 strings and output scalars, must remain valid until completion. Keep callbacks rooted until completion. Use H5ES.wait before reading outputs or releasing resources. H5ES.NONE requests synchronous execution. The packaged native connector does not provide background execution.</remarks>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Fflush_async", CallingConvention = CallingConvention.Cdecl)]
        public static extern int flush_async(IntPtr app_file, IntPtr app_func, uint app_line, long object_id, H5F.scope_t scope, long es_id);

        /// <summary>Invokes H5Fclose_async using the supplied event set; completion depends on connector capabilities.</summary>
        /// <remarks>All pointers, including null-terminated UTF-8 strings and output scalars, must remain valid until completion. Keep callbacks rooted until completion. Use H5ES.wait before reading outputs or releasing resources. H5ES.NONE requests synchronous execution. The packaged native connector does not provide background execution.</remarks>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Fclose_async", CallingConvention = CallingConvention.Cdecl)]
        public static extern int close_async(IntPtr app_file, IntPtr app_func, uint app_line, long file_id, long es_id);

    }
}

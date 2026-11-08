using System;
using System.Runtime.InteropServices;

namespace HDF.PInvoke
{
    /// <summary>Provides native event-set operations with explicit asynchronous buffer ownership.</summary>
    public unsafe sealed partial class H5R
    {
        /// <summary>Invokes H5Ropen_object_async using the supplied event set; completion depends on connector capabilities.</summary>
        /// <remarks>All pointers, including null-terminated UTF-8 strings and output scalars, must remain valid until completion. Keep callbacks rooted until completion. Use H5ES.wait before reading outputs or releasing resources. H5ES.NONE requests synchronous execution. The packaged native connector does not provide background execution.</remarks>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Ropen_object_async", CallingConvention = CallingConvention.Cdecl)]
        public static extern long open_object_async(IntPtr app_file, IntPtr app_func, uint app_line, IntPtr ref_ptr, long rapl_id, long oapl_id, long es_id);

        /// <summary>Invokes H5Ropen_region_async using the supplied event set; completion depends on connector capabilities.</summary>
        /// <remarks>All pointers, including null-terminated UTF-8 strings and output scalars, must remain valid until completion. Keep callbacks rooted until completion. Use H5ES.wait before reading outputs or releasing resources. H5ES.NONE requests synchronous execution. The packaged native connector does not provide background execution.</remarks>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Ropen_region_async", CallingConvention = CallingConvention.Cdecl)]
        public static extern long open_region_async(IntPtr app_file, IntPtr app_func, uint app_line, IntPtr ref_ptr, long rapl_id, long oapl_id, long es_id);

        /// <summary>Invokes H5Ropen_attr_async using the supplied event set; completion depends on connector capabilities.</summary>
        /// <remarks>All pointers, including null-terminated UTF-8 strings and output scalars, must remain valid until completion. Keep callbacks rooted until completion. Use H5ES.wait before reading outputs or releasing resources. H5ES.NONE requests synchronous execution. The packaged native connector does not provide background execution.</remarks>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Ropen_attr_async", CallingConvention = CallingConvention.Cdecl)]
        public static extern long open_attr_async(IntPtr app_file, IntPtr app_func, uint app_line, IntPtr ref_ptr, long rapl_id, long aapl_id, long es_id);

    }
}

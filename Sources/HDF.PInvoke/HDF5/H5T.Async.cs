using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace HDF.PInvoke
{
    /// <summary>Provides native event-set operations with explicit asynchronous buffer ownership.</summary>
    public unsafe sealed partial class H5T
    {
        /// <summary>Invokes H5Tclose_async using the supplied event set; completion depends on connector capabilities.</summary>
        /// <remarks>All pointers, including null-terminated UTF-8 strings and output scalars, must remain valid until completion. Keep callbacks rooted until completion. Use H5ES.wait before reading outputs or releasing resources. H5ES.NONE requests synchronous execution. The packaged native connector does not provide background execution.</remarks>
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Tclose_async"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int close_async(IntPtr app_file, IntPtr app_func, uint app_line, long type_id, long es_id);

        /// <summary>Invokes H5Tcommit_async using the supplied event set; completion depends on connector capabilities.</summary>
        /// <remarks>All pointers, including null-terminated UTF-8 strings and output scalars, must remain valid until completion. Keep callbacks rooted until completion. Use H5ES.wait before reading outputs or releasing resources. H5ES.NONE requests synchronous execution. The packaged native connector does not provide background execution.</remarks>
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Tcommit_async"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int commit_async(IntPtr app_file, IntPtr app_func, uint app_line, long loc_id, IntPtr name, long type_id, long lcpl_id, long tcpl_id, long tapl_id, long es_id);

        /// <summary>Invokes H5Topen_async using the supplied event set; completion depends on connector capabilities.</summary>
        /// <remarks>All pointers, including null-terminated UTF-8 strings and output scalars, must remain valid until completion. Keep callbacks rooted until completion. Use H5ES.wait before reading outputs or releasing resources. H5ES.NONE requests synchronous execution. The packaged native connector does not provide background execution.</remarks>
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Topen_async"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial long open_async(IntPtr app_file, IntPtr app_func, uint app_line, long loc_id, IntPtr name, long tapl_id, long es_id);

    }
}

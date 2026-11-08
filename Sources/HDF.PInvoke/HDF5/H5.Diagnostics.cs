using System;
using System.Runtime.InteropServices;

namespace HDF.PInvoke
{
    /// <summary>Provides advanced serial H5 operations.</summary>
    public unsafe sealed partial class H5
    {
        /// <summary>Gets the current byte sizes of native internal free lists.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5get_free_list_sizes", CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_free_list_sizes(out UIntPtr regular, out UIntPtr arrays, out UIntPtr blocks, out UIntPtr factories);

        /// <summary>Gets whether native library termination is in progress.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5is_library_terminating", CallingConvention = CallingConvention.Cdecl)]
        public static extern int is_library_terminating(out byte terminating);

    }
}


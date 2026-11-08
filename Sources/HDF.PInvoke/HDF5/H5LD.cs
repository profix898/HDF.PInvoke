using System;
using System.Runtime.InteropServices;

namespace HDF.PInvoke
{
    /// <summary>Provides native H5LD high-level operations.</summary>
    public unsafe sealed class H5LD
    {
        /// <summary>Initializes native HDF5 before the first high-level call.</summary>
        static H5LD()
        {
            H5.open();
        }

        /// <summary>Gets current dataset dimensions into a buffer containing one unsigned 64-bit value per dimension.</summary>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5LDget_dset_dims", CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_dset_dims(long did, IntPtr cur_dims);

        /// <summary>Gets the byte size of a dataset element or selected comma-separated compound fields.</summary>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5LDget_dset_type_size", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr get_dset_type_size(long did, [MarshalAs(UnmanagedType.LPUTF8Str)] string fields);

        /// <summary>Reads newly appended elements between previous and current dimensions into caller-owned storage.</summary>
        /// <remarks>Dimension arrays contain one unsigned 64-bit value per dimension. Allocate enough bytes for all newly selected elements using get_dset_type_size; null fields selects complete elements.</remarks>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5LDget_dset_elmts", CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_dset_elmts(long did, IntPtr prev_dims, IntPtr cur_dims, [MarshalAs(UnmanagedType.LPUTF8Str)] string fields, IntPtr buf);

    }
}

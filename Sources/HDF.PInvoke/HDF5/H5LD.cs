using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace HDF.PInvoke
{
    /// <summary>Provides native H5LD high-level operations.</summary>
    public unsafe sealed partial class H5LD
    {
        /// <summary>Initializes native HDF5 before the first high-level call.</summary>
        static H5LD()
        {
            H5.open();
        }

        /// <summary>Gets current dataset dimensions into a buffer containing one unsigned 64-bit value per dimension.</summary>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5LDget_dset_dims"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int get_dset_dims(long did, IntPtr cur_dims);

        /// <summary>Gets the byte size of a dataset element or selected comma-separated compound fields.</summary>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5LDget_dset_type_size", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial UIntPtr get_dset_type_size(long did, string fields);

        /// <summary>Reads newly appended elements between previous and current dimensions into caller-owned storage.</summary>
        /// <remarks>Dimension arrays contain one unsigned 64-bit value per dimension. Allocate enough bytes for all newly selected elements using get_dset_type_size; null fields selects complete elements.</remarks>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5LDget_dset_elmts", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int get_dset_elmts(long did, IntPtr prev_dims, IntPtr cur_dims, string fields, IntPtr buf);

    }
}

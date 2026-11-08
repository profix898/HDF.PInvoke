using System;
using System.Runtime.InteropServices;

namespace HDF.PInvoke
{
    /// <summary>Provides additional HDF5 2.2 H5T operations.</summary>
    public unsafe sealed partial class H5T
    {
        /// <summary>The predefined modern reference datatype, owned by HDF5; do not close it.</summary>
        public static long STD_REF => H5DLLImporter.Instance.GetHid("H5T_STD_REF_g");

        /// <summary>Creates a complex datatype from a floating-point base type; close the result with H5T.close.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Tcomplex_create", CallingConvention = CallingConvention.Cdecl)]
        public static extern long complex_create(long baseType);

        /// <summary>Decodes a datatype from an encoded buffer of the supplied byte length; close the result with H5T.close.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Tdecode2", CallingConvention = CallingConvention.Cdecl)]
        public static extern long decode2(IntPtr buffer, UIntPtr bufferSize);

        /// <summary>Releases variable-length data and reference resources described by the datatype and memory selection.</summary>
        /// <remarks>The buffer itself remains caller-owned. Use the memory datatype, dataspace and allocation settings used for the read.</remarks>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Treclaim", CallingConvention = CallingConvention.Cdecl)]
        public static extern int reclaim(long type, long space, long transfer, IntPtr buffer);

    }
}


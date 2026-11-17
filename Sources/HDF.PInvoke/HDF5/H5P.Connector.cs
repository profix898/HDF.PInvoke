using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace HDF.PInvoke
{
    /// <summary>Provides virtual object layer discovery and configuration.</summary>
    public unsafe sealed partial class H5P
    {
        /// <summary>Gets the configured connector identifier; release it with H5VL.close.</summary>
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Pget_vol_id"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int get_vol_id(long access, out long connector);

        /// <summary>Copies connector-specific information; free it with H5VL.free_connector_info using the matching connector.</summary>
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Pget_vol_info"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int get_vol_info(long access, out IntPtr information);

        /// <summary>Sets a connector and its connector-specific configuration; native code copies the supplied information.</summary>
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Pset_vol"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int set_vol(long access, long connector, IntPtr information);

        /// <summary>Gets the configured connector's capability flags.</summary>
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Pget_vol_cap_flags"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int get_vol_cap_flags(long access, out ulong flags);

    }
}


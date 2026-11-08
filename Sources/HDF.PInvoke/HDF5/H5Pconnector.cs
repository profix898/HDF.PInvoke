using System;
using System.Runtime.InteropServices;

namespace HDF.PInvoke
{
    /// <summary>Provides virtual object layer discovery and configuration.</summary>
    public unsafe sealed partial class H5P
    {
        /// <summary>Gets the configured connector identifier; release it with H5VL.close.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Pget_vol_id", CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_vol_id(long access, out long connector);

        /// <summary>Copies connector-specific information; free it with H5VL.free_connector_info using the matching connector.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Pget_vol_info", CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_vol_info(long access, out IntPtr information);

        /// <summary>Sets a connector and its connector-specific configuration; native code copies the supplied information.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Pset_vol", CallingConvention = CallingConvention.Cdecl)]
        public static extern int set_vol(long access, long connector, IntPtr information);

        /// <summary>Gets the configured connector's capability flags.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Pget_vol_cap_flags", CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_vol_cap_flags(long access, out ulong flags);

    }
}


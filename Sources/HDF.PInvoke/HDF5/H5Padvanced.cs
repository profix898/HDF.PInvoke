using System;
using System.Runtime.InteropServices;

namespace HDF.PInvoke
{
    /// <summary>Provides advanced serial H5P operations.</summary>
    public unsafe sealed partial class H5P
    {
        /// <summary>Decodes a property list; close the result with H5P.close.</summary>
        /// <remarks>The buffer must contain a complete trusted H5Pencode2 representation; the native API has no length parameter.</remarks>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Pdecode", CallingConvention = CallingConvention.Cdecl)]
        public static extern long decode(IntPtr buffer);

        /// <summary>Serializes a property list using the specified format bounds.</summary>
        /// <remarks>Pass a null buffer to query size; otherwise size supplies capacity. Caller owns the output buffer.</remarks>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Pencode2", CallingConvention = CallingConvention.Cdecl)]
        public static extern int encode2(long list, IntPtr buffer, ref UIntPtr size, long access = H5P.DEFAULT);

        /// <summary>Sets native file-integrity relaxation flags on a file-access property list; zero preserves all checks.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Pset_relax_file_integrity_checks", CallingConvention = CallingConvention.Cdecl)]
        public static extern int set_relax_file_integrity_checks(long list, ulong flags);

        /// <summary>Gets file-integrity relaxation flags.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Pget_relax_file_integrity_checks", CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_relax_file_integrity_checks(long list, out ulong flags);

        /// <summary>Selects spatial-tree indexing on a dataset-access property list using a one-byte C boolean.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Pset_virtual_spatial_tree", CallingConvention = CallingConvention.Cdecl)]
        public static extern int set_virtual_spatial_tree(long access, byte useTree);

        /// <summary>Gets whether virtual dataset mappings use a spatial tree.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Pget_virtual_spatial_tree", CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_virtual_spatial_tree(long access, out byte useTree);

        /// <summary>Selects a registered or loadable virtual file driver by name and driver-specific UTF-8 configuration.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Pset_driver_by_name", CallingConvention = CallingConvention.Cdecl)]
        public static extern int set_driver_by_name(long access, [MarshalAs(UnmanagedType.LPUTF8Str)] string name, [MarshalAs(UnmanagedType.LPUTF8Str)] string configuration);

        /// <summary>Selects a virtual file driver by its numeric class value and driver-specific configuration.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Pset_driver_by_value", CallingConvention = CallingConvention.Cdecl)]
        public static extern int set_driver_by_value(long access, int value, [MarshalAs(UnmanagedType.LPUTF8Str)] string configuration);

        /// <summary>Gets the driver configuration UTF-8 string length excluding its terminator; null buffer queries length.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Pget_driver_config_str", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr get_driver_config_str(long access, IntPtr buffer, UIntPtr capacity);

        /// <summary>Sets the memory category used by a multi-file driver.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Pset_multi_type", CallingConvention = CallingConvention.Cdecl)]
        public static extern int set_multi_type(long access, H5F.mem_t type);

        /// <summary>Gets the memory category used by a multi-file driver.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Pget_multi_type", CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_multi_type(long access, out H5F.mem_t type);

        /// <summary>Sets a dataset-transfer hyperslab selection.</summary>
        /// <remarks>Coordinate buffers contain rank unsigned 64-bit values. Stride and block may be null for native defaults.</remarks>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Pset_dataset_io_hyperslab_selection", CallingConvention = CallingConvention.Cdecl)]
        public static extern int set_dataset_io_hyperslab_selection(long transfer, uint rank, H5S.seloper_t operation, IntPtr start, IntPtr stride, IntPtr count, IntPtr block);

    }
}


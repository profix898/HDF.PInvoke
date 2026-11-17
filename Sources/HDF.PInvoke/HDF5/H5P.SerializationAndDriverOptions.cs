using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace HDF.PInvoke
{
    /// <summary>Provides advanced serial H5P operations.</summary>
    public unsafe sealed partial class H5P
    {
        /// <summary>Decodes a property list; close the result with H5P.close.</summary>
        /// <remarks>The buffer must contain a complete trusted H5Pencode2 representation; the native API has no length parameter.</remarks>
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Pdecode"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial long decode(IntPtr buffer);

        /// <summary>Serializes a property list using the specified format bounds.</summary>
        /// <remarks>Pass a null buffer to query size; otherwise size supplies capacity. Caller owns the output buffer.</remarks>
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Pencode2"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int encode2(long list, IntPtr buffer, ref UIntPtr size, long access = H5P.DEFAULT);

        /// <summary>Sets native file-integrity relaxation flags on a file-access property list; zero preserves all checks.</summary>
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Pset_relax_file_integrity_checks"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int set_relax_file_integrity_checks(long list, ulong flags);

        /// <summary>Gets file-integrity relaxation flags.</summary>
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Pget_relax_file_integrity_checks"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int get_relax_file_integrity_checks(long list, out ulong flags);

        /// <summary>Selects spatial-tree indexing on a dataset-access property list using a one-byte C boolean.</summary>
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Pset_virtual_spatial_tree"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int set_virtual_spatial_tree(long access, byte useTree);

        /// <summary>Gets whether virtual dataset mappings use a spatial tree.</summary>
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Pget_virtual_spatial_tree"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int get_virtual_spatial_tree(long access, out byte useTree);

        /// <summary>Selects a registered or loadable virtual file driver by name and driver-specific UTF-8 configuration.</summary>
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Pset_driver_by_name", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int set_driver_by_name(long access, string name, string configuration);

        /// <summary>Selects a virtual file driver by its numeric class value and driver-specific configuration.</summary>
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Pset_driver_by_value", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int set_driver_by_value(long access, int value, string configuration);

        /// <summary>Gets the driver configuration UTF-8 string length excluding its terminator; null buffer queries length.</summary>
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Pget_driver_config_str"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial IntPtr get_driver_config_str(long access, IntPtr buffer, UIntPtr capacity);

        /// <summary>Sets the memory category used by a multi-file driver.</summary>
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Pset_multi_type"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int set_multi_type(long access, H5F.mem_t type);

        /// <summary>Gets the memory category used by a multi-file driver.</summary>
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Pget_multi_type"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int get_multi_type(long access, out H5F.mem_t type);

        /// <summary>Sets a dataset-transfer hyperslab selection.</summary>
        /// <remarks>Coordinate buffers contain rank unsigned 64-bit values. Stride and block may be null for native defaults.</remarks>
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Pset_dataset_io_hyperslab_selection"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int set_dataset_io_hyperslab_selection(long transfer, uint rank, H5S.seloper_t operation, IntPtr start, IntPtr stride, IntPtr count, IntPtr block);

    }
}


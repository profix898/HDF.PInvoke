using System;
using System.Runtime.InteropServices;

namespace HDF.PInvoke
{
    /// <summary>Provides additional HDF5 2.2 H5P operations.</summary>
    public unsafe sealed partial class H5P
    {
        /// <summary>Sets file-locking policy using one-byte C booleans (zero or one).</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Pset_file_locking", CallingConvention = CallingConvention.Cdecl)]
        public static extern int set_file_locking(long access, byte useLocking, byte ignoreWhenDisabled);

        /// <summary>Gets file-locking policy; environment overrides may take precedence when opening a file.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Pget_file_locking", CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_file_locking(long access, out byte useLocking, out byte ignoreWhenDisabled);

        /// <summary>Gets whether metadata cache entries are evicted when objects close.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Pget_evict_on_close", CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_evict_on_close(long access, out byte evict);

        /// <summary>Sets the requested selection-I/O mode on a dataset-transfer property list.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Pset_selection_io", CallingConvention = CallingConvention.Cdecl)]
        public static extern int set_selection_io(long transfer, H5D.selection_io_mode_t mode);

        /// <summary>Gets the requested selection-I/O mode.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Pget_selection_io", CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_selection_io(long transfer, out H5D.selection_io_mode_t mode);

        /// <summary>Gets the bitmask of I/O strategies used by the last transfer.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Pget_actual_selection_io_mode", CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_actual_selection_io_mode(long transfer, out uint mode);

        /// <summary>Gets the bitmask explaining why selection I/O was not used.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Pget_no_selection_io_cause", CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_no_selection_io_cause(long transfer, out uint cause);

        /// <summary>Sets whether native writes may modify the caller's buffer; zero disables this optimization.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Pset_modify_write_buf", CallingConvention = CallingConvention.Cdecl)]
        public static extern int set_modify_write_buf(long transfer, byte allowModification);

        /// <summary>Gets whether native writes may modify the caller's buffer.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Pget_modify_write_buf", CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_modify_write_buf(long transfer, out byte allowModification);

    }
}


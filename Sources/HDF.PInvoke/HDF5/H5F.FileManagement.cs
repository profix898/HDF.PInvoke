using System;
using System.Runtime.InteropServices;

namespace HDF.PInvoke
{
    /// <summary>Provides additional HDF5 2.2 H5F operations.</summary>
    public unsafe sealed partial class H5F
    {
        /// <summary>Gets the file number, widening the native Windows unsigned long to 64 bits.</summary>
        public static int get_fileno(long file, out ulong number)
        {
            // Zero the high half before Windows writes its four-byte unsigned long result.
            ulong value = 0;
            var result = get_fileno_native(file, (IntPtr)(&value));
            number = result >= 0 ? value : 0;
            return result;
        }

        /// <summary>Imports the platform-dependent unsigned-long output into caller-owned storage.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Fget_fileno", CallingConvention = CallingConvention.Cdecl)]
        private static extern int get_fileno_native(long file, IntPtr number);

        /// <summary>Tests whether a UTF-8 filename is accessible through the configured file driver.</summary>
        /// <remarks>Returns positive if accessible, zero if not, or negative on error.</remarks>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Fis_accessible", CallingConvention = CallingConvention.Cdecl)]
        public static extern int is_accessible([MarshalAs(UnmanagedType.LPUTF8Str)] string filename, long access = H5P.DEFAULT);

        /// <summary>Deletes an HDF5 file through the configured driver after all handles have been closed.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Fdelete", CallingConvention = CallingConvention.Cdecl)]
        public static extern int delete([MarshalAs(UnmanagedType.LPUTF8Str)] string filename, long access = H5P.DEFAULT);

        /// <summary>Retrieves the file's end-of-allocation address.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Fget_eoa", CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_eoa(long file, out ulong address);

        /// <summary>Sets the end-of-allocation address to max(EOA, EOF) plus the requested byte increment.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Fincrement_filesize", CallingConvention = CallingConvention.Cdecl)]
        public static extern int increment_filesize(long file, ulong increment);

        /// <summary>Changes format-version bounds for an open file; existing objects are not downgraded.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Fset_libver_bounds", CallingConvention = CallingConvention.Cdecl)]
        public static extern int set_libver_bounds(long file, libver_t low, libver_t high);

    }
}


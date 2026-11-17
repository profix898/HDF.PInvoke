using System;
using System.Runtime.CompilerServices;
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
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Fget_fileno"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        private static partial int get_fileno_native(long file, IntPtr number);

        /// <summary>Tests whether a UTF-8 filename is accessible through the configured file driver.</summary>
        /// <remarks>Returns positive if accessible, zero if not, or negative on error.</remarks>
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Fis_accessible", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int is_accessible(string filename, long access = H5P.DEFAULT);

        /// <summary>Deletes an HDF5 file through the configured driver after all handles have been closed.</summary>
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Fdelete", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int delete(string filename, long access = H5P.DEFAULT);

        /// <summary>Retrieves the file's end-of-allocation address.</summary>
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Fget_eoa"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int get_eoa(long file, out ulong address);

        /// <summary>Sets the end-of-allocation address to max(EOA, EOF) plus the requested byte increment.</summary>
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Fincrement_filesize"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int increment_filesize(long file, ulong increment);

        /// <summary>Changes format-version bounds for an open file; existing objects are not downgraded.</summary>
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Fset_libver_bounds"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int set_libver_bounds(long file, libver_t low, libver_t high);

    }
}


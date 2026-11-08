using System;
using System.Runtime.InteropServices;

namespace HDF.PInvoke
{
    /// <summary>Provides advanced serial H5D operations.</summary>
    public unsafe sealed partial class H5D
    {
        /// <summary>Prints native dataset storage diagnostics; output is controlled by the HDF5 implementation.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Ddebug", CallingConvention = CallingConvention.Cdecl)]
        public static extern int debug(long dataset);

        /// <summary>Receives a borrowed coordinate array, filter mask, file address and stored byte size; return zero to continue and do not throw through native code.</summary>
        /// <remarks>The offset pointer contains one unsigned 64-bit coordinate per dataset dimension and is valid only during the callback.</remarks>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int chunk_iter_t(IntPtr offset, uint filters, ulong address, ulong size, IntPtr state);

        /// <summary>Iterates stored chunk metadata synchronously; callback and state are borrowed until the call returns.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Dchunk_iter", CallingConvention = CallingConvention.Cdecl)]
        public static extern int chunk_iter(long dataset, long transfer, chunk_iter_t callback, IntPtr state);

    }
}


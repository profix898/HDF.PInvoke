using System;
using System.Runtime.InteropServices;

namespace HDF.PInvoke
{
    /// <summary>Provides additional HDF5 2.2 H5D operations.</summary>
    public unsafe sealed partial class H5D
    {
        /// <summary>Controls selection I/O for dataset transfers.</summary>
        public enum selection_io_mode_t
        {
            /// <summary>Let the native library choose the I/O strategy.</summary>
            DEFAULT = 0,

            /// <summary>Disable selection I/O.</summary>
            OFF = 1,

            /// <summary>Request selection I/O when supported.</summary>
            ON = 2
        }

        /// <summary>Reads multiple datasets from one file in a single synchronous native call.</summary>
        /// <remarks>Arrays must have equal, nonzero lengths. Each buffer must remain allocated and large enough for its memory datatype and selection until the call returns.</remarks>
        public static int read_multi(long[] datasets, long[] memoryTypes, long[] memorySpaces, long[] fileSpaces, long transfer, IntPtr[] buffers)
        {
            ValidateMulti(datasets, memoryTypes, memorySpaces, fileSpaces, buffers);
            return read_multi_native(new UIntPtr((uint)datasets.Length), datasets, memoryTypes, memorySpaces, fileSpaces, transfer, buffers);
        }

        /// <summary>Writes multiple datasets in one file in a single synchronous native call.</summary>
        /// <remarks>Arrays must have equal, nonzero lengths. Each buffer must remain allocated and large enough for its memory datatype and selection until the call returns.</remarks>
        public static int write_multi(long[] datasets, long[] memoryTypes, long[] memorySpaces, long[] fileSpaces, long transfer, IntPtr[] buffers)
        {
            ValidateMulti(datasets, memoryTypes, memorySpaces, fileSpaces, buffers);
            return write_multi_native(new UIntPtr((uint)datasets.Length), datasets, memoryTypes, memorySpaces, fileSpaces, transfer, buffers);
        }

        /// <summary>Checks parallel arrays before native code reads their elements.</summary>
        private static void ValidateMulti(long[] datasets, long[] memoryTypes, long[] memorySpaces, long[] fileSpaces, IntPtr[] buffers)
        {
            if (datasets == null)
            {
                throw new ArgumentNullException(nameof(datasets));
            }

            if (memoryTypes == null)
            {
                throw new ArgumentNullException(nameof(memoryTypes));
            }

            if (memorySpaces == null)
            {
                throw new ArgumentNullException(nameof(memorySpaces));
            }

            if (fileSpaces == null)
            {
                throw new ArgumentNullException(nameof(fileSpaces));
            }

            if (buffers == null)
            {
                throw new ArgumentNullException(nameof(buffers));
            }

            if (datasets.Length == 0 || memoryTypes.Length != datasets.Length || memorySpaces.Length != datasets.Length || fileSpaces.Length != datasets.Length || buffers.Length != datasets.Length)
            {
                throw new ArgumentException("Provide equally sized, nonempty dataset, type, space and buffer arrays.");
            }
        }

        /// <summary>Imports native multi-dataset reads after array bounds validation.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Dread_multi", CallingConvention = CallingConvention.Cdecl)]
        private static extern int read_multi_native(UIntPtr count, [In] long[] datasets, [In] long[] memoryTypes, [In] long[] memorySpaces, [In] long[] fileSpaces, long transfer, [In] IntPtr[] buffers);

        /// <summary>Imports native multi-dataset writes after array bounds validation.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Dwrite_multi", CallingConvention = CallingConvention.Cdecl)]
        private static extern int write_multi_native(UIntPtr count, [In] long[] datasets, [In] long[] memoryTypes, [In] long[] memorySpaces, [In] long[] fileSpaces, long transfer, [In] IntPtr[] buffers);

        /// <summary>Reads an encoded chunk with an explicit byte capacity and returns its filter mask and required size.</summary>
        /// <remarks>Offset points to one unsigned 64-bit coordinate per dimension. Buffer size is an input capacity and output size; a null buffer queries size. The caller owns both buffers.</remarks>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Dread_chunk2", CallingConvention = CallingConvention.Cdecl)]
        public static extern int read_chunk2(long dataset, long transfer, IntPtr offset, out uint filters, IntPtr buffer, ref UIntPtr bufferSize);

    }
}


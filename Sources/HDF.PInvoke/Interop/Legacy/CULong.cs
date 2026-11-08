using System;
using System.Runtime.InteropServices;

namespace HDF.PInvoke
{
    /// <summary>
    /// Legacy representation of a C unsigned long in the padded HDF5 object-info structure.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct CULong
    {
        /// <summary>
        /// Stores the native unsigned value, including padding that is masked when read on Windows.
        /// </summary>
        private readonly ulong value;

        /// <summary>
        /// Initializes a managed C unsigned long value.
        /// </summary>
        /// <param name="value">The unsigned value.</param>
        public CULong(ulong value)
        {
            this.value = value;
        }

        /// <summary>
        /// Gets the value, excluding native structure padding on Windows.
        /// </summary>
        /// <value>The managed numeric value.</value>
        public ulong Value => LegacyNativeLibrary.IsWindows ? (uint)value : value;
    }
}

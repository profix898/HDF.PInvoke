using System;
using System.Runtime.InteropServices;

namespace HDF.PInvoke
{
    /// <summary>
    /// Legacy managed representation of a C long. Native Windows cache structures are converted by the bindings.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct CLong
    {
        private readonly long value;

        /// <summary>
        /// Initializes a managed C long value.
        /// </summary>
        /// <param name="value">The signed value.</param>
        public CLong(long value)
        {
            this.value = value;
        }

        /// <summary>
        /// Gets the signed value.
        /// </summary>
        /// <value>The managed numeric value.</value>
        public long Value => value;
    }
}

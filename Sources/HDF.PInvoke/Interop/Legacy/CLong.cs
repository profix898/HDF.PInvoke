using System.Globalization;
using HDF.PInvoke;

namespace System.Runtime.InteropServices
{
    /// <summary>
    /// Legacy polyfill matching the modern BCL representation of a native C <c>long</c>.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct CLong : IEquatable<CLong>
    {
        /// <summary>
        /// Stores the signed value; Windows structure bridges narrow it to the native 32-bit C <c>long</c> width.
        /// </summary>
        private readonly IntPtr value;

        /// <summary>
        /// Initializes a <see cref="CLong"/> from a 32-bit signed integer.
        /// </summary>
        /// <param name="value">The 32-bit signed integer value.</param>
        public CLong(int value)
        {
            this.value = (IntPtr)value;
        }

        /// <summary>
        /// Initializes a <see cref="CLong"/> from a pointer-sized signed integer.
        /// </summary>
        /// <param name="value">The signed integer value.</param>
        /// <exception cref="OverflowException">The value exceeds the 32-bit C <c>long</c> range on Windows.</exception>
        public CLong(IntPtr value)
        {
            if (LegacyNativeLibrary.IsWindows)
            {
                _ = checked((int)value.ToInt64());
            }

            this.value = value;
        }

        /// <summary>
        /// Gets the underlying signed integer value.
        /// </summary>
        /// <value>The native-sized signed value.</value>
        public IntPtr Value => value;

        /// <summary>
        /// Determines whether the specified object is equal to the current value.
        /// </summary>
        /// <param name="o">The object to compare with the current value.</param>
        /// <returns><see langword="true"/> if <paramref name="o"/> is an equal <see cref="CLong"/>; otherwise <see langword="false"/>.</returns>
        public override bool Equals(object o) => o is CLong other && Equals(other);

        /// <summary>
        /// Determines whether the specified <see cref="CLong"/> is equal to the current value.
        /// </summary>
        /// <param name="other">The value to compare with the current value.</param>
        /// <returns><see langword="true"/> if <paramref name="other"/> has the same value; otherwise <see langword="false"/>.</returns>
        public bool Equals(CLong other) => value == other.value;

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed hash code.</returns>
        public override int GetHashCode() => value.GetHashCode();

        /// <summary>
        /// Converts the numeric value of this instance to its equivalent string representation.
        /// </summary>
        /// <returns>The string representation of the value.</returns>
        public override string ToString() => value.ToInt64().ToString(CultureInfo.CurrentCulture);
    }
}

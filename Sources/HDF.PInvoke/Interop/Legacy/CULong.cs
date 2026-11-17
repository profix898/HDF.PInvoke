using System.Globalization;
using HDF.PInvoke;

namespace System.Runtime.InteropServices
{
    /// <summary>
    /// Legacy polyfill matching the modern BCL representation of a native C <c>unsigned long</c>.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct CULong : IEquatable<CULong>
    {
        /// <summary>
        /// Stores the unsigned value, including trailing native structure padding masked on Windows.
        /// </summary>
        private readonly UIntPtr value;

        /// <summary>
        /// Initializes a <see cref="CULong"/> from a 32-bit unsigned integer.
        /// </summary>
        /// <param name="value">The 32-bit unsigned integer value.</param>
        public CULong(uint value)
        {
            this.value = (UIntPtr)value;
        }

        /// <summary>
        /// Initializes a <see cref="CULong"/> from a pointer-sized unsigned integer.
        /// </summary>
        /// <param name="value">The unsigned integer value.</param>
        /// <exception cref="OverflowException">The value exceeds the 32-bit C <c>unsigned long</c> range on Windows.</exception>
        public CULong(UIntPtr value)
        {
            if (LegacyNativeLibrary.IsWindows)
            {
                _ = checked((uint)value.ToUInt64());
            }

            this.value = value;
        }

        /// <summary>
        /// Gets the underlying unsigned integer value, excluding native structure padding on Windows.
        /// </summary>
        /// <value>The native-sized unsigned value.</value>
        public UIntPtr Value => LegacyNativeLibrary.IsWindows ? (UIntPtr)(uint)value.ToUInt64() : value;

        /// <summary>
        /// Determines whether the specified object is equal to the current value.
        /// </summary>
        /// <param name="o">The object to compare with the current value.</param>
        /// <returns><see langword="true"/> if <paramref name="o"/> is an equal <see cref="CULong"/>; otherwise <see langword="false"/>.</returns>
        public override bool Equals(object o) => o is CULong other && Equals(other);

        /// <summary>
        /// Determines whether the specified <see cref="CULong"/> is equal to the current value.
        /// </summary>
        /// <param name="other">The value to compare with the current value.</param>
        /// <returns><see langword="true"/> if <paramref name="other"/> has the same value; otherwise <see langword="false"/>.</returns>
        public bool Equals(CULong other) => Value == other.Value;

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed hash code.</returns>
        public override int GetHashCode() => Value.GetHashCode();

        /// <summary>
        /// Converts the numeric value of this instance to its equivalent string representation.
        /// </summary>
        /// <returns>The string representation of the value.</returns>
        public override string ToString() => Value.ToUInt64().ToString(CultureInfo.CurrentCulture);
    }
}

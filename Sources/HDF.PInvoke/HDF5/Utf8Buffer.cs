using System;
using System.Text;

namespace HDF.PInvoke
{
    /// <summary>
    /// Adapts byte-counted native UTF-8 output to managed string builders.
    /// </summary>
    internal static class Utf8Buffer
    {
        /// <summary>
        /// Allocates the requested native byte capacity, rejecting sizes that cannot be represented by a managed array.
        /// </summary>
        internal static byte[] Create(StringBuilder target, IntPtr byteCount)
        {
            // Native capacities count UTF-8 bytes, not UTF-16 characters.
            long size = byteCount.ToInt64();

            if (size < 0 || size > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(byteCount));
            }

            return target == null || size == 0 ? null : new byte[(int)size];
        }

        /// <summary>
        /// Replaces the target text with the UTF-8 bytes preceding the first NUL terminator.
        /// </summary>
        internal static void CopyTo(byte[] bytes, StringBuilder target)
        {
            if (bytes == null || target == null)
            {
                return;
            }

            // Decode only the bytes preceding the native NUL terminator.
            int length = Array.IndexOf(bytes, (byte)0);

            if (length < 0)
            {
                length = bytes.Length;
            }

            target.Clear();
            target.Append(Encoding.UTF8.GetString(bytes, 0, length));
        }
    }
}

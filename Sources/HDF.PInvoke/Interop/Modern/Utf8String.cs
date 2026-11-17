using System;
using System.Buffers;
using System.Text;

namespace HDF.PInvoke
{
    /// <summary>
    /// Decodes length-prefixed native UTF-8 getters using a stack fast path with pooled retry for long values.
    /// </summary>
    internal static class Utf8String
    {
        /// <summary>Reserves stack storage for short UTF-8 results including their terminator.</summary>
        private const int StackCapacity = 256;

        /// <summary>Invokes a byte-counted getter with caller-owned output storage.</summary>
        internal delegate IntPtr Query<in TState>(TState state, Span<byte> buffer);

        /// <summary>
        /// Invokes a native UTF-8 getter into stack storage first, retrying with a pooled buffer when the result exceeds 255 bytes.
        /// </summary>
        internal static string Read<TState>(TState state, Query<TState> query)
        {
            Span<byte> stack = stackalloc byte[StackCapacity];
            var length = query(state, stack).ToInt64();

            if (length < 0)
            {
                return null;
            }

            if (length == 0)
            {
                return string.Empty;
            }

            if (length < StackCapacity)
            {
                return Encoding.UTF8.GetString(stack.Slice(0, (int)length));
            }

            // A getter may report a larger value on retry; never silently return a truncated string.
            while (true)
            {
                if (length >= int.MaxValue)
                {
                    throw new OverflowException("Native UTF-8 string exceeds maximum managed string capacity.");
                }

                var required = checked((int)length + 1);
                var rented = ArrayPool<byte>.Shared.Rent(required);
                try
                {
                    var span = rented.AsSpan(0, required);
                    var actual = query(state, span).ToInt64();
                    if (actual < 0)
                    {
                        return null;
                    }
                    if (actual == 0)
                    {
                        return string.Empty;
                    }
                    if (actual < required)
                    {
                        return Encoding.UTF8.GetString(span.Slice(0, (int)actual));
                    }
                    length = actual;
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(rented);
                }
            }
        }
    }
}

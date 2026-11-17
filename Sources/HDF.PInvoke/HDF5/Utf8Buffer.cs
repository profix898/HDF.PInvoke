using System;
using System.Text;

namespace HDF.PInvoke
{
    /// <summary>
    /// Adapts byte-counted native UTF-8 output to managed string builders without intermediate string allocations on modern .NET.
    /// </summary>
    internal static unsafe class Utf8Buffer
    {
        /// <summary>
        /// Rents or allocates temporary UTF-8 byte storage for a native output call.
        /// </summary>
        internal struct Scratch : IDisposable
        {
            internal readonly byte[] Buffer;
            internal readonly int Capacity;

            internal Scratch(byte[] buffer, int capacity)
            {
                Buffer = buffer;
                Capacity = capacity;
            }

            /// <summary>
            /// Returns pooled storage when running on modern .NET.
            /// </summary>
            public void Dispose()
            {
#if !NETSTANDARD2_0
                if (Buffer != null)
                {
                    System.Buffers.ArrayPool<byte>.Shared.Return(Buffer);
                }
#endif
            }
        }

        /// <summary>
        /// Validates the requested native byte capacity and acquires temporary storage when a target builder is supplied.
        /// </summary>
        internal static Scratch Rent(StringBuilder target, IntPtr byteCount)
        {
            // Native capacities count UTF-8 bytes, not UTF-16 characters.
            var size = byteCount.ToInt64();

            if (size < 0 || size > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(byteCount));
            }

            if (target == null || size == 0)
            {
                return default;
            }

            var capacity = (int)size;
#if NETSTANDARD2_0
            return new Scratch(new byte[capacity], capacity);
#else
            var rented = System.Buffers.ArrayPool<byte>.Shared.Rent(capacity);
            rented.AsSpan(0, capacity).Clear();
            return new Scratch(rented, capacity);
#endif
        }

        /// <summary>
        /// Replaces the target text with the UTF-8 bytes preceding the first NUL terminator.
        /// </summary>
        internal static void CopyTo(byte* pointer, int capacity, StringBuilder target)
        {
            if (pointer == null || capacity <= 0 || target == null)
            {
                return;
            }

#if NETSTANDARD2_0
            var length = 0;

            while (length < capacity && pointer[length] != 0)
            {
                length++;
            }

            target.Clear();

            if (length > 0)
            {
                target.Append(Encoding.UTF8.GetString(pointer, length));
            }
#else
            var span = new ReadOnlySpan<byte>(pointer, capacity);
            var terminator = span.IndexOf((byte)0);

            if (terminator >= 0)
            {
                span = span.Slice(0, terminator);
            }

            target.Clear();

            if (span.IsEmpty)
            {
                return;
            }

            char[] rentedChars = null;
            Span<char> chars = span.Length <= 512
                ? stackalloc char[512]
                : (rentedChars = System.Buffers.ArrayPool<char>.Shared.Rent(span.Length));

            try
            {
                var written = Encoding.UTF8.GetChars(span, chars);
                target.Append(chars.Slice(0, written));
            }
            finally
            {
                if (rentedChars != null)
                {
                    System.Buffers.ArrayPool<char>.Shared.Return(rentedChars);
                }
            }
#endif
        }
    }
}

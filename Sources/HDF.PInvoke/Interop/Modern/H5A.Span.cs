using System;

namespace HDF.PInvoke
{
    public unsafe sealed partial class H5A
    {
        /// <summary>
        /// Reads an attribute into a typed span without requiring manual pinning.
        /// </summary>
        /// <typeparam name="T">The unmanaged element type.</typeparam>
        /// <param name="attr_id">Identifier of the attribute to read.</param>
        /// <param name="type_id">Identifier of the memory datatype.</param>
        /// <param name="buf">Destination span receiving the attribute data.</param>
        /// <returns>A non-negative value if successful; otherwise a negative value.</returns>
        public static int read<T>(long attr_id, long type_id, Span<T> buf)
            where T : unmanaged
        {
            fixed (T* ptr = buf)
            {
                return read(attr_id, type_id, (IntPtr)ptr);
            }
        }

        /// <summary>
        /// Writes an attribute from a typed read-only span without requiring manual pinning.
        /// </summary>
        /// <typeparam name="T">The unmanaged element type.</typeparam>
        /// <param name="attr_id">Identifier of the attribute to write.</param>
        /// <param name="type_id">Identifier of the memory datatype.</param>
        /// <param name="buf">Source span containing the attribute data to write.</param>
        /// <returns>A non-negative value if successful; otherwise a negative value.</returns>
        public static int write<T>(long attr_id, long type_id, ReadOnlySpan<T> buf)
            where T : unmanaged
        {
            fixed (T* ptr = buf)
            {
                return write(attr_id, type_id, (IntPtr)ptr);
            }
        }

        /// <summary>
        /// Retrieves an attribute's UTF-8 name into a byte span, or queries its byte length when <paramref name="utf8Name"/> is empty.
        /// </summary>
        /// <param name="attr_id">Attribute identifier.</param>
        /// <param name="utf8Name">Destination UTF-8 buffer (including NUL terminator space), or empty to query length.</param>
        /// <returns>The UTF-8 name length in bytes excluding the NUL terminator, or a negative value on failure.</returns>
        public static IntPtr get_name(long attr_id, Span<byte> utf8Name)
        {
            fixed (byte* ptr = utf8Name)
            {
                return get_name_utf8(attr_id, (IntPtr)utf8Name.Length, ptr);
            }
        }

        /// <summary>
        /// Retrieves an attribute's complete UTF-8 name without truncation, or <see langword="null"/> on failure.
        /// </summary>
        /// <param name="attr_id">Attribute identifier.</param>
        /// <returns>The attribute name, <see cref="string.Empty"/> when empty, or <see langword="null"/> on failure.</returns>
        public static string get_name(long attr_id) => Utf8String.Read(attr_id, static (id, buf) => get_name(id, buf));

        /// <summary>
        /// Retrieves an indexed attribute's UTF-8 name into a byte span, or queries its byte length when <paramref name="utf8Name"/> is empty.
        /// </summary>
        /// <param name="loc_id">Location identifier.</param>
        /// <param name="obj_name">Object name relative to <paramref name="loc_id"/>.</param>
        /// <param name="idx_type">Index type.</param>
        /// <param name="order">Index traversal order.</param>
        /// <param name="n">Attribute position within the index.</param>
        /// <param name="utf8Name">Destination UTF-8 buffer (including NUL terminator space), or empty to query length.</param>
        /// <param name="lapl_id">Link access property list identifier.</param>
        /// <returns>The UTF-8 name length in bytes excluding the NUL terminator, or a negative value on failure.</returns>
        public static IntPtr get_name_by_idx(long loc_id, string obj_name, H5.index_t idx_type, H5.iter_order_t order, ulong n, Span<byte> utf8Name, long lapl_id = H5P.DEFAULT)
        {
            fixed (byte* ptr = utf8Name)
            {
                return get_name_by_idx_utf8(loc_id, obj_name, idx_type, order, n, ptr, (IntPtr)utf8Name.Length, lapl_id);
            }
        }

        /// <summary>
        /// Retrieves an indexed attribute's complete UTF-8 name without truncation, or <see langword="null"/> on failure.
        /// </summary>
        /// <param name="loc_id">Location identifier.</param>
        /// <param name="obj_name">Object name relative to <paramref name="loc_id"/>.</param>
        /// <param name="idx_type">Index type.</param>
        /// <param name="order">Index traversal order.</param>
        /// <param name="n">Attribute position within the index.</param>
        /// <param name="lapl_id">Link access property list identifier.</param>
        /// <returns>The attribute name, <see cref="string.Empty"/> when empty, or <see langword="null"/> on failure.</returns>
        public static string get_name_by_idx(long loc_id, string obj_name, H5.index_t idx_type, H5.iter_order_t order, ulong n, long lapl_id = H5P.DEFAULT) =>
            Utf8String.Read((loc_id, obj_name, idx_type, order, n, lapl_id), static (s, buf) => get_name_by_idx(s.loc_id, s.obj_name, s.idx_type, s.order, s.n, buf, s.lapl_id));
    }
}

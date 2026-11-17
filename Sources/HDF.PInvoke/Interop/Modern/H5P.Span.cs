using System;

namespace HDF.PInvoke
{
    public unsafe sealed partial class H5P
    {
        /// <summary>
        /// Sets the chunk dimensions on a dataset creation property list from a read-only span.
        /// </summary>
        /// <param name="plist_id">Dataset creation property list identifier.</param>
        /// <param name="dims">Chunk size in elements for each dataset dimension.</param>
        /// <returns>A non-negative value if successful; otherwise a negative value.</returns>
        public static int set_chunk(long plist_id, ReadOnlySpan<ulong> dims)
        {
            fixed (ulong* dimsPtr = dims)
            {
                return set_chunk(plist_id, dims.Length, dimsPtr);
            }
        }

        /// <summary>
        /// Retrieves the chunk dimensions from a dataset creation property list into a span.
        /// </summary>
        /// <param name="plist_id">Dataset creation property list identifier.</param>
        /// <param name="dims">Destination span receiving chunk dimensions, or empty to query rank.</param>
        /// <returns>Chunk dimensionality if successful; otherwise a negative value.</returns>
        public static int get_chunk(long plist_id, Span<ulong> dims)
        {
            fixed (ulong* dimsPtr = dims)
            {
                return get_chunk_native(plist_id, dims.Length, dimsPtr);
            }
        }

        /// <summary>
        /// Adds a filter and its client-data parameters to a property list's filter pipeline.
        /// </summary>
        /// <param name="plist_id">Property list identifier.</param>
        /// <param name="filter_id">Filter identifier.</param>
        /// <param name="flags">Filter flags.</param>
        /// <param name="cd_values">Auxiliary filter client-data parameters.</param>
        /// <returns>A non-negative value if successful; otherwise a negative value.</returns>
        public static int set_filter(long plist_id, H5Z.filter_t filter_id, uint flags, ReadOnlySpan<uint> cd_values)
        {
            fixed (uint* cdPtr = cd_values)
            {
                return set_filter_native(plist_id, filter_id, flags, (IntPtr)cd_values.Length, cdPtr);
            }
        }

        /// <summary>
        /// Updates an existing filter's flags and client-data parameters in a property list.
        /// </summary>
        /// <param name="plist_id">Property list identifier.</param>
        /// <param name="filter_id">Filter identifier.</param>
        /// <param name="flags">Filter flags.</param>
        /// <param name="cd_values">Auxiliary filter client-data parameters.</param>
        /// <returns>A non-negative value if successful; otherwise a negative value.</returns>
        public static int modify_filter(long plist_id, H5Z.filter_t filter_id, uint flags, ReadOnlySpan<uint> cd_values)
        {
            fixed (uint* cdPtr = cd_values)
            {
                return modify_filter_native(plist_id, filter_id, flags, (IntPtr)cd_values.Length, cdPtr);
            }
        }

        /// <summary>
        /// Sets the typed dataset fill value on a dataset creation property list.
        /// </summary>
        /// <typeparam name="T">The unmanaged fill-value type.</typeparam>
        /// <param name="plist_id">Dataset creation property list identifier.</param>
        /// <param name="type_id">Memory datatype identifier for <paramref name="value"/>.</param>
        /// <param name="value">The fill value to store.</param>
        /// <returns>A non-negative value if successful; otherwise a negative value.</returns>
        public static int set_fill_value<T>(long plist_id, long type_id, in T value)
            where T : unmanaged
        {
            if (!SpanValidation.Scalar<T>(type_id, nameof(value)))
            {
                return -1;
            }

            fixed (T* ptr = &value)
            {
                return set_fill_value(plist_id, type_id, (IntPtr)ptr);
            }
        }

        /// <summary>
        /// Retrieves the typed dataset fill value from a dataset creation property list.
        /// </summary>
        /// <typeparam name="T">The unmanaged fill-value type.</typeparam>
        /// <param name="plist_id">Dataset creation property list identifier.</param>
        /// <param name="type_id">Memory datatype identifier for <paramref name="value"/>.</param>
        /// <param name="value">Receives the fill value.</param>
        /// <returns>A non-negative value if successful; otherwise a negative value.</returns>
        public static int get_fill_value<T>(long plist_id, long type_id, out T value)
            where T : unmanaged
        {
            value = default;
            if (!SpanValidation.Scalar<T>(type_id, nameof(value)))
            {
                return -1;
            }

            fixed (T* ptr = &value)
            {
                return get_fill_value(plist_id, type_id, (IntPtr)ptr);
            }
        }

        /// <summary>
        /// Copies an initial file image into a file access property list from a read-only byte span.
        /// </summary>
        /// <param name="fapl_id">File access property list identifier.</param>
        /// <param name="buf">Initial file image bytes, or empty to clear.</param>
        /// <returns>A non-negative value if successful; otherwise a negative value.</returns>
        public static int set_file_image(long fapl_id, ReadOnlySpan<byte> buf)
        {
            fixed (byte* ptr = buf)
            {
                return set_file_image(fapl_id, (IntPtr)ptr, (IntPtr)buf.Length);
            }
        }

        /// <summary>
        /// Encodes a property list into a byte span, or queries the required size when <paramref name="buffer"/> is empty.
        /// </summary>
        /// <param name="list">Property list identifier.</param>
        /// <param name="buffer">Destination byte span, or empty to query required capacity.</param>
        /// <param name="size">Receives the encoded byte length.</param>
        /// <param name="access">File access property list controlling format bounds.</param>
        /// <returns>A non-negative value if successful; otherwise a negative value.</returns>
        public static int encode2(long list, Span<byte> buffer, out UIntPtr size, long access = H5P.DEFAULT)
        {
            size = (nuint)buffer.Length;

            fixed (byte* ptr = buffer)
            {
                return encode2(list, (IntPtr)ptr, ref size, access);
            }
        }

        /// <summary>
        /// Decodes a serialized property list from a read-only byte span; close the returned identifier with <see cref="close"/>.
        /// </summary>
        /// <remarks>The native decoder has no length argument. Supply only complete, trusted encoded data.</remarks>
        /// <param name="buffer">Serialized property list bytes produced by <see cref="encode2(long, Span{byte}, out UIntPtr, long)"/>.</param>
        /// <returns>A property list identifier if successful; otherwise a negative value.</returns>
        public static long decode(ReadOnlySpan<byte> buffer)
        {
            fixed (byte* ptr = buffer)
            {
                return decode((IntPtr)ptr);
            }
        }

        /// <summary>
        /// Retrieves the virtual file driver configuration UTF-8 string into a byte span, or queries its length when empty.
        /// </summary>
        /// <param name="access">File access property list identifier.</param>
        /// <param name="utf8Buffer">Destination UTF-8 buffer (including NUL terminator space), or empty to query length.</param>
        /// <returns>The UTF-8 configuration string length excluding the NUL terminator, or a negative value on failure.</returns>
        public static IntPtr get_driver_config_str(long access, Span<byte> utf8Buffer)
        {
            fixed (byte* ptr = utf8Buffer)
            {
                return get_driver_config_str(access, (IntPtr)ptr, (nuint)utf8Buffer.Length);
            }
        }

        /// <summary>
        /// Sets a dataset-transfer hyperslab selection using coordinate spans.
        /// </summary>
        /// <param name="transfer">Dataset transfer property list identifier.</param>
        /// <param name="rank">Selection rank.</param>
        /// <param name="operation">Selection combination operator.</param>
        /// <param name="start">Starting coordinates.</param>
        /// <param name="stride">Stride per dimension, or empty for unit stride.</param>
        /// <param name="count">Block count per dimension.</param>
        /// <param name="block">Block size per dimension, or empty for unit blocks.</param>
        /// <returns>A non-negative value if successful; otherwise a negative value.</returns>
        public static int set_dataset_io_hyperslab_selection(long transfer, uint rank, H5S.seloper_t operation, ReadOnlySpan<ulong> start, ReadOnlySpan<ulong> stride, ReadOnlySpan<ulong> count, ReadOnlySpan<ulong> block = default)
        {
            SpanValidation.Capacity(start.Length, rank, nameof(start));
            SpanValidation.Capacity(stride.Length, rank, nameof(stride), true);
            SpanValidation.Capacity(count.Length, rank, nameof(count));
            SpanValidation.Capacity(block.Length, rank, nameof(block), true);

            fixed (ulong* startPtr = start, stridePtr = stride, countPtr = count, blockPtr = block)
            {
                return set_dataset_io_hyperslab_selection(transfer, rank, operation, (IntPtr)startPtr, (IntPtr)stridePtr, (IntPtr)countPtr, (IntPtr)blockPtr);
            }
        }

        /// <summary>
        /// Retrieves a data-transform UTF-8 expression into a byte span, or queries its length when empty.
        /// </summary>
        /// <param name="plist_id">Dataset transfer property list identifier.</param>
        /// <param name="utf8Expression">Destination UTF-8 buffer (including NUL terminator space), or empty to query length.</param>
        /// <returns>The expression length in bytes excluding the NUL terminator, or a negative value on failure.</returns>
        public static IntPtr get_data_transform(long plist_id, Span<byte> utf8Expression)
        {
            fixed (byte* ptr = utf8Expression)
            {
                return get_data_transform_utf8(plist_id, ptr, (IntPtr)utf8Expression.Length);
            }
        }

        /// <summary>
        /// Retrieves a virtual dataset source-dataset UTF-8 name into a byte span, or queries its length when empty.
        /// </summary>
        /// <param name="dcpl_id">Dataset creation property list identifier.</param>
        /// <param name="index">Mapping index.</param>
        /// <param name="utf8Name">Destination UTF-8 buffer (including NUL terminator space), or empty to query length.</param>
        /// <returns>The dataset name length in bytes excluding the NUL terminator, or a negative value on failure.</returns>
        public static IntPtr get_virtual_dsetname(long dcpl_id, IntPtr index, Span<byte> utf8Name)
        {
            fixed (byte* ptr = utf8Name)
            {
                return get_virtual_dsetname_utf8(dcpl_id, index, ptr, (IntPtr)utf8Name.Length);
            }
        }

        /// <summary>
        /// Retrieves a virtual dataset source-file UTF-8 name into a byte span, or queries its length when empty.
        /// </summary>
        /// <param name="dcpl_id">Dataset creation property list identifier.</param>
        /// <param name="index">Mapping index.</param>
        /// <param name="utf8Name">Destination UTF-8 buffer (including NUL terminator space), or empty to query length.</param>
        /// <returns>The filename length in bytes excluding the NUL terminator, or a negative value on failure.</returns>
        public static IntPtr get_virtual_filename(long dcpl_id, IntPtr index, Span<byte> utf8Name)
        {
            fixed (byte* ptr = utf8Name)
            {
                return get_virtual_filename_utf8(dcpl_id, index, ptr, (IntPtr)utf8Name.Length);
            }
        }

        /// <summary>
        /// Retrieves the virtual dataset UTF-8 search prefix into a byte span, or queries its length when empty.
        /// </summary>
        /// <param name="dapl">Dataset access property list identifier.</param>
        /// <param name="utf8Prefix">Destination UTF-8 buffer (including NUL terminator space), or empty to query length.</param>
        /// <returns>The prefix length in bytes excluding the NUL terminator, or a negative value on failure.</returns>
        public static IntPtr get_virtual_prefix(long dapl, Span<byte> utf8Prefix)
        {
            fixed (byte* ptr = utf8Prefix)
            {
                return get_virtual_prefix_utf8(dapl, ptr, (IntPtr)utf8Prefix.Length);
            }
        }
    }
}

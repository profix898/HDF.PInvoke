using System;
using System.Runtime.CompilerServices;

namespace HDF.PInvoke
{
    public unsafe sealed partial class H5S
    {
        /// <summary>
        /// Creates a new fixed-size simple dataspace whose rank and dimensions come from <paramref name="dims"/>.
        /// </summary>
        /// <param name="dims">Size of each dimension.</param>
        /// <returns>A dataspace identifier if successful; otherwise a negative value.</returns>
        public static long create_simple(ReadOnlySpan<ulong> dims)
        {
            fixed (ulong* dimsPtr = dims)
            {
                return create_simple(dims.Length, dimsPtr, null);
            }
        }

        /// <summary>
        /// Creates a new simple dataspace with explicit initial and maximum dimension spans.
        /// </summary>
        /// <param name="dims">Initial size of each dimension.</param>
        /// <param name="maxdims">Maximum size of each dimension, or empty to match <paramref name="dims"/>.</param>
        /// <returns>A dataspace identifier if successful; otherwise a negative value.</returns>
        public static long create_simple(ReadOnlySpan<ulong> dims, ReadOnlySpan<ulong> maxdims)
        {
            if (!maxdims.IsEmpty && maxdims.Length != dims.Length)
            {
                throw new ArgumentException("Maximum dimensions must either be empty or match the rank of dims.", nameof(maxdims));
            }

            fixed (ulong* dimsPtr = dims, maxPtr = maxdims)
            {
                return create_simple(dims.Length, dimsPtr, maxPtr);
            }
        }

        /// <summary>
        /// Sets or resets the size of an existing simple dataspace using dimension spans.
        /// </summary>
        /// <param name="space_id">Dataspace identifier.</param>
        /// <param name="current_size">Current size of each dimension.</param>
        /// <param name="maximum_size">Maximum size of each dimension, or empty to match <paramref name="current_size"/>.</param>
        /// <returns>A non-negative value if successful; otherwise a negative value.</returns>
        public static int set_extent_simple(long space_id, ReadOnlySpan<ulong> current_size, ReadOnlySpan<ulong> maximum_size = default)
        {
            if (!maximum_size.IsEmpty && maximum_size.Length != current_size.Length)
            {
                throw new ArgumentException("Maximum dimensions must either be empty or match the rank of current_size.", nameof(maximum_size));
            }

            fixed (ulong* currentPtr = current_size, maxPtr = maximum_size)
            {
                return set_extent_simple_native(space_id, current_size.Length, currentPtr, maxPtr);
            }
        }

        /// <summary>
        /// Retrieves dataspace dimension sizes and optional maximum sizes into caller spans.
        /// </summary>
        /// <param name="space_id">Dataspace identifier.</param>
        /// <param name="dims">Span receiving current dimension sizes, or empty to omit.</param>
        /// <param name="maxdims">Span receiving maximum dimension sizes, or empty to omit.</param>
        /// <returns>The number of dimensions in the dataspace if successful; otherwise a negative value.</returns>
        [OverloadResolutionPriority(1)]
        public static int get_simple_extent_dims(long space_id, Span<ulong> dims, Span<ulong> maxdims = default)
        {
            var rank = H5S.get_simple_extent_ndims(space_id);
            if (rank < 0)
            {
                return -1;
            }
            SpanValidation.Capacity(dims.Length, (ulong)rank, nameof(dims), true);
            SpanValidation.Capacity(maxdims.Length, (ulong)rank, nameof(maxdims), true);

            fixed (ulong* dimsPtr = dims, maxPtr = maxdims)
            {
                return get_simple_extent_dims(space_id, dimsPtr, maxPtr);
            }
        }

        /// <summary>
        /// Selects or combines a hyperslab region using coordinate spans without heap allocations.
        /// </summary>
        /// <param name="space_id">Dataspace identifier.</param>
        /// <param name="op">Selection combination operator.</param>
        /// <param name="start">Starting offset of the hyperslab.</param>
        /// <param name="stride">Hyperslab stride per dimension, or empty for unit stride.</param>
        /// <param name="count">Number of blocks in each dimension.</param>
        /// <param name="block">Block size per dimension, or empty for unit blocks.</param>
        /// <returns>A non-negative value if successful; otherwise a negative value.</returns>
        [OverloadResolutionPriority(1)]
        public static int select_hyperslab(long space_id, seloper_t op, ReadOnlySpan<ulong> start, ReadOnlySpan<ulong> stride, ReadOnlySpan<ulong> count, ReadOnlySpan<ulong> block = default)
        {
            var rank = H5S.get_simple_extent_ndims(space_id);
            if (rank < 0)
            {
                return -1;
            }
            SpanValidation.Capacity(start.Length, (ulong)rank, nameof(start));
            SpanValidation.Capacity(stride.Length, (ulong)rank, nameof(stride), true);
            SpanValidation.Capacity(count.Length, (ulong)rank, nameof(count));
            SpanValidation.Capacity(block.Length, (ulong)rank, nameof(block), true);

            fixed (ulong* startPtr = start, stridePtr = stride, countPtr = count, blockPtr = block)
            {
                return select_hyperslab(space_id, op, startPtr, stridePtr, countPtr, blockPtr);
            }
        }

        /// <summary>
        /// Retrieves the parameters of a regular hyperslab selection into caller spans.
        /// </summary>
        /// <param name="space_id">Dataspace identifier.</param>
        /// <param name="start">Receives the starting coordinates, or empty to omit.</param>
        /// <param name="stride">Receives the stride per dimension, or empty to omit.</param>
        /// <param name="count">Receives the block count per dimension, or empty to omit.</param>
        /// <param name="block">Receives the block size per dimension, or empty to omit.</param>
        /// <returns>A non-negative value if successful; otherwise a negative value.</returns>
        [OverloadResolutionPriority(1)]
        public static int H5Sget_regular_hyperslab(long space_id, Span<ulong> start, Span<ulong> stride, Span<ulong> count, Span<ulong> block)
        {
            var rank = H5S.get_simple_extent_ndims(space_id);
            if (rank < 0)
            {
                return -1;
            }
            SpanValidation.Capacity(start.Length, (ulong)rank, nameof(start), true);
            SpanValidation.Capacity(stride.Length, (ulong)rank, nameof(stride), true);
            SpanValidation.Capacity(count.Length, (ulong)rank, nameof(count), true);
            SpanValidation.Capacity(block.Length, (ulong)rank, nameof(block), true);

            fixed (ulong* startPtr = start, stridePtr = stride, countPtr = count, blockPtr = block)
            {
                return get_regular_hyperslab_native(space_id, startPtr, stridePtr, countPtr, blockPtr);
            }
        }

        /// <summary>
        /// Retrieves the bounding box of the current selection into caller spans.
        /// </summary>
        /// <param name="space_id">Dataspace identifier.</param>
        /// <param name="start">Receives the starting bounding-box coordinates.</param>
        /// <param name="end">Receives the opposite bounding-box corner coordinates.</param>
        /// <returns>A non-negative value if successful; otherwise a negative value.</returns>
        [OverloadResolutionPriority(1)]
        public static int get_select_bounds(long space_id, Span<ulong> start, Span<ulong> end)
        {
            var rank = H5S.get_simple_extent_ndims(space_id);
            if (rank < 0)
            {
                return -1;
            }
            SpanValidation.Capacity(start.Length, (ulong)rank, nameof(start));
            SpanValidation.Capacity(end.Length, (ulong)rank, nameof(end));

            fixed (ulong* startPtr = start, endPtr = end)
            {
                return get_select_bounds_native(space_id, startPtr, endPtr);
            }
        }

        /// <summary>
        /// Selects individual dataspace elements from flattened zero-based point coordinates.
        /// </summary>
        /// <param name="space_id">Dataspace identifier.</param>
        /// <param name="op">Selection combination operator.</param>
        /// <param name="num_elements">Number of points in <paramref name="coord"/>.</param>
        /// <param name="coord">Flattened point coordinates (<c>num_elements * rank</c> values).</param>
        /// <returns>A non-negative value if successful; otherwise a negative value.</returns>
        [OverloadResolutionPriority(1)]
        public static int select_elements(long space_id, seloper_t op, IntPtr num_elements, ReadOnlySpan<ulong> coord)
        {
            var rank = H5S.get_simple_extent_ndims(space_id);
            if (rank < 0)
            {
                return -1;
            }
            SpanValidation.Capacity(coord.Length, checked((ulong)num_elements.ToInt64() * (ulong)rank), nameof(coord));

            fixed (ulong* coordPtr = coord)
            {
                return select_elements_native(space_id, op, num_elements, coordPtr);
            }
        }

        /// <summary>
        /// Retrieves point-selection coordinates into a span.
        /// </summary>
        /// <param name="space_id">Dataspace identifier.</param>
        /// <param name="startpoint">First point index to retrieve.</param>
        /// <param name="numpoints">Number of points to retrieve.</param>
        /// <param name="buf">Destination span (<c>numpoints * rank</c> values).</param>
        /// <returns>A non-negative value if successful; otherwise a negative value.</returns>
        [OverloadResolutionPriority(1)]
        public static int get_select_elem_pointlist(long space_id, ulong startpoint, ulong numpoints, Span<ulong> buf)
        {
            var rank = H5S.get_simple_extent_ndims(space_id);
            if (rank < 0)
            {
                return -1;
            }
            SpanValidation.Capacity(buf.Length, checked(numpoints * (ulong)rank), nameof(buf));

            fixed (ulong* bufPtr = buf)
            {
                return get_select_elem_pointlist_native(space_id, startpoint, numpoints, bufPtr);
            }
        }

        /// <summary>
        /// Retrieves hyperslab selection blocks into a span.
        /// </summary>
        /// <param name="space_id">Dataspace identifier.</param>
        /// <param name="startblock">First block index to retrieve.</param>
        /// <param name="numblocks">Number of blocks to retrieve.</param>
        /// <param name="buf">Destination span (<c>2 * numblocks * rank</c> values).</param>
        /// <returns>A non-negative value if successful; otherwise a negative value.</returns>
        [OverloadResolutionPriority(1)]
        public static int get_select_hyper_blocklist(long space_id, ulong startblock, ulong numblocks, Span<ulong> buf)
        {
            var rank = H5S.get_simple_extent_ndims(space_id);
            if (rank < 0)
            {
                return -1;
            }
            SpanValidation.Capacity(buf.Length, checked(2ul * numblocks * (ulong)rank), nameof(buf));

            fixed (ulong* bufPtr = buf)
            {
                return get_select_hyper_blocklist_native(space_id, startblock, numblocks, bufPtr);
            }
        }

        /// <summary>
        /// Sets the selection offset of a simple dataspace from a span, or resets to zero when empty.
        /// </summary>
        /// <param name="space_id">Dataspace identifier.</param>
        /// <param name="offset">Signed offset per dimension, or empty to reset to zero.</param>
        /// <returns>A non-negative value if successful; otherwise a negative value.</returns>
        [OverloadResolutionPriority(1)]
        public static int offset_simple(long space_id, ReadOnlySpan<long> offset)
        {
            var rank = H5S.get_simple_extent_ndims(space_id);
            if (rank < 0)
            {
                return -1;
            }
            SpanValidation.Capacity(offset.Length, (ulong)rank, nameof(offset), true);

            fixed (long* offsetPtr = offset)
            {
                return offset_simple_native(space_id, offsetPtr);
            }
        }

        /// <summary>
        /// Decodes a binary dataspace description from a read-only byte span.
        /// </summary>
        /// <remarks>The native decoder has no length argument. Supply only complete, trusted encoded data.</remarks>
        /// <param name="buf">Encoded dataspace bytes.</param>
        /// <returns>A new dataspace identifier if successful; otherwise a negative value.</returns>
        [OverloadResolutionPriority(1)]
        public static long decode(ReadOnlySpan<byte> buf)
        {
            fixed (byte* ptr = buf)
            {
                return decode_native(ptr);
            }
        }

        /// <summary>
        /// Encodes a dataspace description into a byte span, or queries required size when <paramref name="buf"/> is empty.
        /// </summary>
        /// <param name="obj_id">Dataspace identifier.</param>
        /// <param name="buf">Destination byte span, or empty to query required byte length.</param>
        /// <param name="nalloc">Receives the required byte size.</param>
        /// <returns>A non-negative value if successful; otherwise a negative value.</returns>
        public static int encode(long obj_id, Span<byte> buf, out IntPtr nalloc)
        {
            nalloc = (IntPtr)buf.Length;

            fixed (byte* ptr = buf)
            {
                return encode_native(obj_id, ptr, ref nalloc);
            }
        }

        /// <summary>
        /// Serializes a dataspace using file format bounds from <paramref name="access"/>, or queries required size when <paramref name="buffer"/> is empty.
        /// </summary>
        /// <param name="space">Dataspace identifier.</param>
        /// <param name="buffer">Destination byte span, or empty to query required byte length.</param>
        /// <param name="size">Receives the encoded byte length.</param>
        /// <param name="access">File access property list controlling format bounds.</param>
        /// <returns>A non-negative value if successful; otherwise a negative value.</returns>
        public static int encode2(long space, Span<byte> buffer, out UIntPtr size, long access = H5P.DEFAULT)
        {
            size = (nuint)buffer.Length;

            fixed (byte* ptr = buffer)
            {
                return encode2(space, (IntPtr)ptr, ref size, access);
            }
        }
    }
}

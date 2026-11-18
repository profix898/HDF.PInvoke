using System;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;

namespace HDF.PInvoke
{
    public unsafe sealed partial class H5D
    {
        /// <summary>
        /// Visits a typed dataset element and its borrowed dataspace coordinates without per-element heap allocations.
        /// </summary>
        /// <typeparam name="T">The unmanaged element type stored in the buffer.</typeparam>
        /// <param name="elem">A mutable reference to the current element in memory.</param>
        /// <param name="type_id">Datatype identifier for the elements stored in the buffer.</param>
        /// <param name="point">Borrowed coordinates of the element within the dataspace; valid only during the callback.</param>
        /// <returns>Zero to continue iteration, a positive value to stop early, or a negative value on failure.</returns>
        public delegate int span_operator_t<T>(ref T elem, long type_id, ReadOnlySpan<ulong> point)
            where T : unmanaged;

        /// <summary>
        /// Reads raw data from a dataset into a typed span without requiring manual pinning.
        /// </summary>
        /// <typeparam name="T">The unmanaged element type.</typeparam>
        /// <param name="dset_id">Identifier of the dataset to read from.</param>
        /// <param name="mem_type_id">Identifier of the memory datatype.</param>
        /// <param name="mem_space_id">Identifier of the memory dataspace.</param>
        /// <param name="file_space_id">Identifier of the dataset's dataspace in the file.</param>
        /// <param name="plist_id">Identifier of a transfer property list for this I/O operation.</param>
        /// <param name="buf">Destination span receiving the data read from the file.</param>
        /// <returns>A non-negative value if successful; otherwise a negative value.</returns>
        public static int read<T>(long dset_id, long mem_type_id, long mem_space_id, long file_space_id, long plist_id, Span<T> buf)
            where T : unmanaged
        {
            if (!SpanValidation.Dataset<T>(dset_id, mem_type_id, mem_space_id, file_space_id, buf.Length, nameof(buf)))
            {
                return -1;
            }

            fixed (T* ptr = buf)
            {
                return read(dset_id, mem_type_id, mem_space_id, file_space_id, plist_id, (IntPtr)ptr);
            }
        }

        /// <summary>
        /// Writes raw data from a typed read-only span to a dataset without requiring manual pinning.
        /// </summary>
        /// <typeparam name="T">The unmanaged element type.</typeparam>
        /// <param name="dset_id">Identifier of the dataset to write to.</param>
        /// <param name="mem_type_id">Identifier of the memory datatype.</param>
        /// <param name="mem_space_id">Identifier of the memory dataspace.</param>
        /// <param name="file_space_id">Identifier of the dataset's dataspace in the file.</param>
        /// <param name="plist_id">Identifier of a transfer property list for this I/O operation.</param>
        /// <param name="buf">Source span containing data to be written to the file.</param>
        /// <returns>A non-negative value if successful; otherwise a negative value.</returns>
        public static int write<T>(long dset_id, long mem_type_id, long mem_space_id, long file_space_id, long plist_id, ReadOnlySpan<T> buf)
            where T : unmanaged
        {
            if (!SpanValidation.Dataset<T>(dset_id, mem_type_id, mem_space_id, file_space_id, buf.Length, nameof(buf)))
            {
                return -1;
            }

            fixed (T* ptr = buf)
            {
                return write(dset_id, mem_type_id, mem_space_id, file_space_id, plist_id, (IntPtr)ptr);
            }
        }

        /// <summary>
        /// Fills selected dataspace elements in a typed span with the specified fill value.
        /// </summary>
        /// <typeparam name="TFill">The unmanaged fill-value type.</typeparam>
        /// <typeparam name="TElement">The unmanaged destination-element type.</typeparam>
        /// <param name="fill_value">The fill value to replicate across selected elements.</param>
        /// <param name="fill_type_id">Datatype identifier for <paramref name="fill_value"/>.</param>
        /// <param name="buf">Destination memory buffer span to initialize.</param>
        /// <param name="buf_type_id">Datatype identifier for elements in <paramref name="buf"/>.</param>
        /// <param name="space_id">Dataspace identifier describing the buffer extent and selection.</param>
        /// <returns>A non-negative value if successful; otherwise a negative value.</returns>
        public static int fill<TFill, TElement>(in TFill fill_value, long fill_type_id, Span<TElement> buf, long buf_type_id, long space_id)
            where TFill : unmanaged
            where TElement : unmanaged
        {
            if (!SpanValidation.Scalar<TFill>(fill_type_id, nameof(fill_value)) || !SpanValidation.Selection<TElement>(buf_type_id, space_id, buf.Length, nameof(buf)))
            {
                return -1;
            }

            fixed (TFill* fillPtr = &fill_value)
            fixed (TElement* bufPtr = buf)
            {
                return fill((IntPtr)fillPtr, fill_type_id, (IntPtr)bufPtr, buf_type_id, space_id);
            }
        }

        /// <summary>
        /// Zero-fills selected dataspace elements in a typed span.
        /// </summary>
        /// <typeparam name="TElement">The unmanaged destination-element type.</typeparam>
        /// <param name="buf">Destination memory buffer span to zero-fill.</param>
        /// <param name="buf_type_id">Datatype identifier for elements in <paramref name="buf"/>.</param>
        /// <param name="space_id">Dataspace identifier describing the buffer extent and selection.</param>
        /// <returns>A non-negative value if successful; otherwise a negative value.</returns>
        public static int fill<TElement>(Span<TElement> buf, long buf_type_id, long space_id)
            where TElement : unmanaged
        {
            if (!SpanValidation.Selection<TElement>(buf_type_id, space_id, buf.Length, nameof(buf)))
            {
                return -1;
            }

            fixed (TElement* bufPtr = buf)
            {
                return fill(IntPtr.Zero, buf_type_id, (IntPtr)bufPtr, buf_type_id, space_id);
            }
        }

        /// <summary>
        /// Iterates over all selected elements in a typed span without allocating coordinate arrays per element.
        /// </summary>
        /// <typeparam name="T">The unmanaged element type stored in <paramref name="buf"/>.</typeparam>
        /// <param name="buf">Span containing the elements to iterate over.</param>
        /// <param name="type_id">Datatype identifier for the elements stored in <paramref name="buf"/>.</param>
        /// <param name="space_id">Dataspace identifier for <paramref name="buf"/>.</param>
        /// <param name="op">Callback invoked for each selected element.</param>
        /// <returns>The last non-zero operator return value, zero when all elements were processed, or a negative value on failure.</returns>
        public static int iterate<T>(Span<T> buf, long type_id, long space_id, span_operator_t<T> op)
            where T : unmanaged
        {
            if (H5T.get_size(type_id).ToInt64() != sizeof(T))
            {
                throw new ArgumentException("Iteration requires T to match the native element size.", nameof(type_id));
            }
            if (!SpanValidation.Selection<T>(type_id, space_id, buf.Length, nameof(buf)))
            {
                return -1;
            }

            fixed (T* bufPtr = buf)
            {
                if (op == null)
                {
                    return iterate_native((IntPtr)bufPtr, type_id, space_id, IntPtr.Zero, IntPtr.Zero);
                }

                var context = new SpanIterateContext
                {
                    CallbackHandle = GCHandle.Alloc(op),
                    Invoker = &InvokeSpanOperator<T>
                };
                int result;

                try
                {
                    var callbackPtr = (IntPtr)(delegate* unmanaged[Cdecl]<IntPtr, long, uint, ulong*, IntPtr, int>)&SpanIterateCallback;
                    result = iterate_native((IntPtr)bufPtr, type_id, space_id, callbackPtr, (IntPtr)(&context));
                }
                finally
                {
                    context.CallbackHandle.Free();
                }

                if (context.FailureHandle.IsAllocated)
                {
                    var failure = (Exception)context.FailureHandle.Target;
                    context.FailureHandle.Free();
                    ExceptionDispatchInfo.Capture(failure).Throw();
                }

                return result;
            }
        }

        /// <summary>
        /// Reads multiple datasets from one file in a single synchronous native call using span metadata.
        /// </summary>
        /// <param name="datasets">Dataset identifiers.</param>
        /// <param name="memoryTypes">Memory datatype identifiers.</param>
        /// <param name="memorySpaces">Memory dataspace identifiers.</param>
        /// <param name="fileSpaces">File dataspace identifiers.</param>
        /// <param name="transfer">Dataset transfer property list identifier.</param>
        /// <param name="buffers">Destination buffer addresses, one per dataset.</param>
        /// <returns>A non-negative value if successful; otherwise a negative value.</returns>
        [OverloadResolutionPriority(-1)]
        public static int read_multi(ReadOnlySpan<long> datasets, ReadOnlySpan<long> memoryTypes, ReadOnlySpan<long> memorySpaces, ReadOnlySpan<long> fileSpaces, long transfer, ReadOnlySpan<IntPtr> buffers)
        {
            ValidateMultiSpan(datasets, memoryTypes, memorySpaces, fileSpaces, buffers);

            fixed (long* datasetsPtr = datasets, typesPtr = memoryTypes, memSpacesPtr = memorySpaces, fileSpacesPtr = fileSpaces)
            fixed (IntPtr* buffersPtr = buffers)
            {
                return read_multi_native((nuint)datasets.Length, datasetsPtr, typesPtr, memSpacesPtr, fileSpacesPtr, transfer, buffersPtr);
            }
        }

        /// <summary>
        /// Writes multiple datasets in one file in a single synchronous native call using span metadata.
        /// </summary>
        /// <param name="datasets">Dataset identifiers.</param>
        /// <param name="memoryTypes">Memory datatype identifiers.</param>
        /// <param name="memorySpaces">Memory dataspace identifiers.</param>
        /// <param name="fileSpaces">File dataspace identifiers.</param>
        /// <param name="transfer">Dataset transfer property list identifier.</param>
        /// <param name="buffers">Source buffer addresses, one per dataset.</param>
        /// <returns>A non-negative value if successful; otherwise a negative value.</returns>
        [OverloadResolutionPriority(-1)]
        public static int write_multi(ReadOnlySpan<long> datasets, ReadOnlySpan<long> memoryTypes, ReadOnlySpan<long> memorySpaces, ReadOnlySpan<long> fileSpaces, long transfer, ReadOnlySpan<IntPtr> buffers)
        {
            ValidateMultiSpan(datasets, memoryTypes, memorySpaces, fileSpaces, buffers);

            fixed (long* datasetsPtr = datasets, typesPtr = memoryTypes, memSpacesPtr = memorySpaces, fileSpacesPtr = fileSpaces)
            fixed (IntPtr* buffersPtr = buffers)
            {
                return write_multi_native((nuint)datasets.Length, datasetsPtr, typesPtr, memSpacesPtr, fileSpacesPtr, transfer, buffersPtr);
            }
        }

        /// <summary>
        /// Reads a raw data chunk directly from a dataset into a byte span.
        /// </summary>
        /// <param name="dset_id">Dataset identifier.</param>
        /// <param name="dxpl_id">Dataset transfer property list identifier.</param>
        /// <param name="offset">Logical position of the chunk's first element in the dataspace.</param>
        /// <param name="filter_mask">Receives the filter mask indicating which filters were skipped.</param>
        /// <param name="buf">Destination buffer receiving the raw chunk bytes.</param>
        /// <returns>A non-negative value if successful; otherwise a negative value.</returns>
        /// <exception cref="ArgumentException">The offset is empty or the destination cannot hold the entire encoded chunk.</exception>
        public static int read_chunk(long dset_id, long dxpl_id, ReadOnlySpan<ulong> offset, ref uint filter_mask, Span<byte> buf)
        {

            if (offset.IsEmpty)
            {
                throw new ArgumentException("Chunk offset coordinates must not be empty.", nameof(offset));
            }

            var status = read_chunk2(dset_id, dxpl_id, offset, out filter_mask, buf, out var required);
            if (status >= 0 && required.ToUInt64() > (ulong)buf.Length)
            {
                throw new ArgumentException("The destination is too small for the encoded chunk.", nameof(buf));
            }

            return status;
        }

        /// <summary>
        /// Reads an encoded chunk with explicit byte capacity checking into a byte span.
        /// </summary>
        /// <param name="dataset">Dataset identifier.</param>
        /// <param name="transfer">Dataset transfer property list identifier.</param>
        /// <param name="offset">Logical position of the chunk's first element in the dataspace.</param>
        /// <param name="filters">Receives the filter mask for the chunk.</param>
        /// <param name="buffer">Destination buffer span, or empty to query required byte capacity.</param>
        /// <param name="bufferSize">Receives the chunk's byte size.</param>
        /// <returns>A non-negative value if successful; otherwise a negative value.</returns>
        public static int read_chunk2(long dataset, long transfer, ReadOnlySpan<ulong> offset, out uint filters, Span<byte> buffer, out UIntPtr bufferSize)
        {
            filters = 0;
            bufferSize = UIntPtr.Zero;
            var rank = SpanValidation.DatasetRank(dataset);
            if (rank < 0)
            {
                return -1;
            }
            SpanValidation.Capacity(offset.Length, (ulong)rank, nameof(offset));

            bufferSize = (nuint)buffer.Length;

            fixed (ulong* offsetPtr = offset)
            fixed (byte* bufPtr = buffer)
            {
                return read_chunk2(dataset, transfer, (IntPtr)offsetPtr, out filters, (IntPtr)bufPtr, ref bufferSize);
            }
        }

        /// <summary>
        /// Writes a raw data chunk from a read-only byte span directly to a dataset.
        /// </summary>
        /// <param name="dset_id">Dataset identifier.</param>
        /// <param name="dxpl_id">Dataset transfer property list identifier.</param>
        /// <param name="filter_mask">Mask identifying filters skipped on the chunk.</param>
        /// <param name="offset">Logical position of the chunk's first element in the dataspace.</param>
        /// <param name="data">Raw chunk bytes to write.</param>
        /// <returns>A non-negative value if successful; otherwise a negative value.</returns>
        public static int write_chunk(long dset_id, long dxpl_id, uint filter_mask, ReadOnlySpan<ulong> offset, ReadOnlySpan<byte> data)
        {
            var rank = SpanValidation.DatasetRank(dset_id);
            if (rank < 0)
            {
                return -1;
            }
            SpanValidation.Capacity(offset.Length, (ulong)rank, nameof(offset));

            if (offset.IsEmpty)
            {
                throw new ArgumentException("Chunk offset coordinates must not be empty.", nameof(offset));
            }

            fixed (byte* dataPtr = data)
            {
                return write_chunk(dset_id, dxpl_id, filter_mask, ref MemoryMarshal.GetReference(offset), (IntPtr)data.Length, (IntPtr)dataPtr);
            }
        }

        /// <summary>
        /// Retrieves information about a chunk specified by its index into a coordinate span.
        /// </summary>
        /// <param name="dset_id">Dataset identifier.</param>
        /// <param name="fspace_id">File dataspace selection identifier.</param>
        /// <param name="index">Chunk index in the selection.</param>
        /// <param name="offset">Span receiving the chunk coordinates, or empty to omit coordinates.</param>
        /// <param name="filter_mask">Filter mask indicating filters used when written.</param>
        /// <param name="addr">Chunk address in the file.</param>
        /// <param name="size">Chunk size in bytes.</param>
        /// <returns>A non-negative value if successful; otherwise a negative value.</returns>
        [OverloadResolutionPriority(1)]
        public static int get_chunk_info(long dset_id, long fspace_id, ulong index, Span<ulong> offset, ref uint filter_mask, ref ulong addr, ref ulong size)
        {
            var rank = SpanValidation.DatasetRank(dset_id);
            if (rank < 0)
            {
                return -1;
            }
            SpanValidation.Capacity(offset.Length, (ulong)rank, nameof(offset), true);

            fixed (ulong* offsetPtr = offset)
            {
                return get_chunk_info(dset_id, fspace_id, index, offsetPtr, ref filter_mask, ref addr, ref size);
            }
        }

        /// <summary>
        /// Retrieves information about a chunk specified by its coordinates.
        /// </summary>
        /// <param name="dset_id">Dataset identifier.</param>
        /// <param name="offset">Logical coordinates of the chunk's first element.</param>
        /// <param name="filter_mask">Filter mask indicating filters used when written.</param>
        /// <param name="addr">Chunk address in the file.</param>
        /// <param name="size">Chunk size in bytes.</param>
        /// <returns>A non-negative value if successful; otherwise a negative value.</returns>
        [OverloadResolutionPriority(1)]
        public static int get_chunk_info_by_coord(long dset_id, ReadOnlySpan<ulong> offset, ref uint filter_mask, ref ulong addr, ref ulong size)
        {
            var rank = SpanValidation.DatasetRank(dset_id);
            if (rank < 0)
            {
                return -1;
            }
            SpanValidation.Capacity(offset.Length, (ulong)rank, nameof(offset));

            fixed (ulong* offsetPtr = offset)
            {
                return get_chunk_info_by_coord(dset_id, offsetPtr, ref filter_mask, ref addr, ref size);
            }
        }

        /// <summary>
        /// Determines the storage size in bytes of a chunk specified by its coordinates.
        /// </summary>
        /// <param name="dset_id">Dataset identifier.</param>
        /// <param name="offset">Logical coordinates of the chunk's first element.</param>
        /// <param name="chunk_bytes">Receives the chunk storage size in bytes.</param>
        /// <returns>A non-negative value if successful; otherwise a negative value.</returns>
        [OverloadResolutionPriority(1)]
        public static int get_chunk_storage_size(long dset_id, ReadOnlySpan<ulong> offset, ref ulong chunk_bytes)
        {
            var rank = SpanValidation.DatasetRank(dset_id);
            if (rank < 0)
            {
                return -1;
            }
            SpanValidation.Capacity(offset.Length, (ulong)rank, nameof(offset));

            fixed (ulong* offsetPtr = offset)
            {
                return get_chunk_storage_size(dset_id, offsetPtr, ref chunk_bytes);
            }
        }

        /// <summary>
        /// Changes the sizes of a dataset's dimensions from a read-only span.
        /// </summary>
        /// <param name="dset_id">Dataset identifier.</param>
        /// <param name="size">New extent for each dataset dimension.</param>
        /// <returns>A non-negative value if successful; otherwise a negative value.</returns>
        [OverloadResolutionPriority(1)]
        public static int set_extent(long dset_id, ReadOnlySpan<ulong> size)
        {
            var rank = SpanValidation.DatasetRank(dset_id);
            if (rank < 0)
            {
                return -1;
            }
            // Empty input remains a null pointer so native error handling is preserved.
            SpanValidation.Capacity(size.Length, (ulong)rank, nameof(size), true);

            fixed (ulong* sizePtr = size)
            {
                return set_extent(dset_id, sizePtr);
            }
        }

        /// <summary>Checks matching dataset, type, selection and buffer metadata.</summary>
        private static void ValidateMultiSpan(ReadOnlySpan<long> datasets, ReadOnlySpan<long> memoryTypes, ReadOnlySpan<long> memorySpaces, ReadOnlySpan<long> fileSpaces, ReadOnlySpan<IntPtr> buffers)
        {
            if (datasets.IsEmpty || memoryTypes.Length != datasets.Length || memorySpaces.Length != datasets.Length || fileSpaces.Length != datasets.Length || buffers.Length != datasets.Length)
            {
                throw new ArgumentException("Provide equally sized, nonempty dataset, type, space and buffer spans.");
            }
        }

        /// <summary>Roots callback state for one synchronous native iteration.</summary>
        private struct SpanIterateContext
        {
            /// <summary>Keeps the typed callback alive until native iteration returns.</summary>
            internal GCHandle CallbackHandle;
            /// <summary>Retains the first exception for rethrow after native code returns.</summary>
            internal GCHandle FailureHandle;
            /// <summary>Dispatches to the closed generic callback without reflection.</summary>
            internal delegate*<object, IntPtr, long, ReadOnlySpan<ulong>, int> Invoker;
        }

        /// <summary>Projects a native address into the unmanaged element type.</summary>
        private static int InvokeSpanOperator<T>(object callback, IntPtr elem, long typeId, ReadOnlySpan<ulong> point)
                    where T : unmanaged =>
                    ((span_operator_t<T>)callback)(ref *(T*)elem, typeId, point);

        /// <summary>Contains managed exceptions while bridging native iteration callbacks.</summary>
        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
        private static int SpanIterateCallback(IntPtr elem, long typeId, uint ndim, ulong* point, IntPtr contextPtr)
        {
            var context = (SpanIterateContext*)contextPtr;

            try
            {
                var coordinates = ndim == 0 ? ReadOnlySpan<ulong>.Empty : new ReadOnlySpan<ulong>(point, checked((int)ndim));
                return context->Invoker(context->CallbackHandle.Target, elem, typeId, coordinates);
            }
            catch (Exception error)
            {
                if (!context->FailureHandle.IsAllocated)
                {
                    context->FailureHandle = GCHandle.Alloc(error);
                }

                return -1;
            }
        }
    }
}

using System;

namespace HDF.PInvoke
{
    public unsafe sealed partial class H5DO
    {
        /// <summary>
        /// Appends elements from a typed read-only span to a chunked dataset along the specified axis.
        /// </summary>
        /// <typeparam name="T">The unmanaged element type.</typeparam>
        /// <param name="dset_id">Dataset identifier.</param>
        /// <param name="dxpl_id">Dataset transfer property list identifier.</param>
        /// <param name="axis">Zero-based dataset dimension along which to append.</param>
        /// <param name="num_elem">Number of hyperslab elements to add along <paramref name="axis"/>.</param>
        /// <param name="memtype">Memory datatype identifier.</param>
        /// <param name="buffer">Source elements to append.</param>
        /// <returns>A non-negative value if successful; otherwise a negative value.</returns>
        public static int append<T>(long dset_id, long dxpl_id, uint axis, IntPtr num_elem, long memtype, ReadOnlySpan<T> buffer)
            where T : unmanaged
        {
            fixed (T* ptr = buffer)
            {
                return append(dset_id, dxpl_id, axis, num_elem, memtype, (IntPtr)ptr);
            }
        }
    }

    public unsafe sealed partial class H5LT
    {
        /// <summary>
        /// Creates and writes a dataset from dimension and element spans without manual pinning.
        /// </summary>
        /// <typeparam name="T">The unmanaged element type.</typeparam>
        /// <param name="location">File or group identifier.</param>
        /// <param name="name">Dataset name.</param>
        /// <param name="dimensions">Size of each dataset dimension.</param>
        /// <param name="type">Datatype identifier.</param>
        /// <param name="buffer">Source elements covering the product of <paramref name="dimensions"/>.</param>
        /// <returns>A non-negative value if successful; otherwise a negative value.</returns>
        public static int make_dataset<T>(long location, string name, ReadOnlySpan<ulong> dimensions, long type, ReadOnlySpan<T> buffer)
            where T : unmanaged
        {
            ulong required = 1;

            foreach (var dim in dimensions)
            {
                required = checked(required * dim);
            }

            if ((ulong)buffer.Length < required)
            {
                throw new ArgumentException("Source span contains fewer elements than the product of dimensions.", nameof(buffer));
            }

            fixed (ulong* dimsPtr = dimensions)
            fixed (T* bufPtr = buffer)
            {
                return make_dataset(location, name, dimensions.Length, (IntPtr)dimsPtr, type, (IntPtr)bufPtr);
            }
        }

        /// <summary>
        /// Reads a dataset into a typed span using the specified memory datatype.
        /// </summary>
        /// <typeparam name="T">The unmanaged element type.</typeparam>
        /// <param name="location">File or group identifier.</param>
        /// <param name="name">Dataset name.</param>
        /// <param name="type">Memory datatype identifier.</param>
        /// <param name="buffer">Destination span sized to hold all dataset elements.</param>
        /// <returns>A non-negative value if successful; otherwise a negative value.</returns>
        public static int read_dataset<T>(long location, string name, long type, Span<T> buffer)
            where T : unmanaged
        {
            fixed (T* bufPtr = buffer)
            {
                return read_dataset(location, name, type, (IntPtr)bufPtr);
            }
        }

        /// <summary>
        /// Retrieves dataset dimensions, datatype class, and element byte size using a dimension span.
        /// </summary>
        /// <param name="location">File or group identifier.</param>
        /// <param name="name">Dataset name.</param>
        /// <param name="dimensions">Destination span sized to the dataset rank, or empty to omit dimensions.</param>
        /// <param name="typeClass">Receives the datatype class.</param>
        /// <param name="typeSize">Receives the datatype byte size.</param>
        /// <returns>A non-negative value if successful; otherwise a negative value.</returns>
        public static int get_dataset_info(long location, string name, Span<ulong> dimensions, out H5T.class_t typeClass, out UIntPtr typeSize)
        {
            fixed (ulong* dimsPtr = dimensions)
            {
                return get_dataset_info(location, name, (IntPtr)dimsPtr, out typeClass, out typeSize);
            }
        }

        /// <summary>
        /// Reads an attribute into a typed span using the specified memory datatype.
        /// </summary>
        /// <typeparam name="T">The unmanaged element type.</typeparam>
        /// <param name="location">Location identifier.</param>
        /// <param name="objectName">Target object name.</param>
        /// <param name="attributeName">Attribute name.</param>
        /// <param name="type">Memory datatype identifier.</param>
        /// <param name="buffer">Destination span sized to hold all attribute elements.</param>
        /// <returns>A non-negative value if successful; otherwise a negative value.</returns>
        public static int get_attribute<T>(long location, string objectName, string attributeName, long type, Span<T> buffer)
            where T : unmanaged
        {
            fixed (T* bufPtr = buffer)
            {
                return get_attribute(location, objectName, attributeName, type, (IntPtr)bufPtr);
            }
        }

        /// <summary>
        /// Retrieves attribute dimensions, datatype class, and element byte size using a dimension span.
        /// </summary>
        /// <param name="location">Location identifier.</param>
        /// <param name="objectName">Target object name.</param>
        /// <param name="attributeName">Attribute name.</param>
        /// <param name="dimensions">Destination span sized to the attribute rank, or empty to omit dimensions.</param>
        /// <param name="typeClass">Receives the datatype class.</param>
        /// <param name="typeSize">Receives the datatype byte size.</param>
        /// <returns>A non-negative value if successful; otherwise a negative value.</returns>
        public static int get_attribute_info(long location, string objectName, string attributeName, Span<ulong> dimensions, out H5T.class_t typeClass, out UIntPtr typeSize)
        {
            fixed (ulong* dimsPtr = dimensions)
            {
                return get_attribute_info(location, objectName, attributeName, (IntPtr)dimsPtr, out typeClass, out typeSize);
            }
        }

        /// <summary>
        /// Opens a copied in-memory HDF5 file image from a read-only byte span; close the returned file with <see cref="H5F.close"/>.
        /// </summary>
        /// <param name="buffer">In-memory HDF5 file image bytes.</param>
        /// <param name="flags">Image open flags; do not set <see cref="FILE_IMAGE_DONT_COPY"/> unless the backing memory remains pinned until file close.</param>
        /// <returns>A file identifier if successful; otherwise a negative value.</returns>
        public static long open_file_image(ReadOnlySpan<byte> buffer, uint flags = 0)
        {
            if ((flags & FILE_IMAGE_DONT_COPY) != 0)
            {
                throw new ArgumentException("Span-backed open_file_image copies the buffer before returning; omit FILE_IMAGE_DONT_COPY or pass a permanently pinned IntPtr.", nameof(flags));
            }

            fixed (byte* ptr = buffer)
            {
                return open_file_image((IntPtr)ptr, (nuint)buffer.Length, flags);
            }
        }

        /// <summary>
        /// Formats a datatype description into a UTF-8 byte span, or queries the required size when <paramref name="utf8Buffer"/> is empty.
        /// </summary>
        /// <param name="type">Datatype identifier.</param>
        /// <param name="utf8Buffer">Destination UTF-8 buffer, or empty to query required storage.</param>
        /// <param name="language">Output language format.</param>
        /// <param name="size">Receives required storage including the NUL terminator.</param>
        /// <returns>A non-negative value if successful; otherwise a negative value.</returns>
        public static int dtype_to_text(long type, Span<byte> utf8Buffer, lang_t language, out UIntPtr size)
        {
            size = (nuint)utf8Buffer.Length;

            fixed (byte* ptr = utf8Buffer)
            {
                return dtype_to_text(type, (IntPtr)ptr, language, ref size);
            }
        }
    }

    public unsafe sealed partial class H5IM
    {
        /// <summary>
        /// Creates an 8-bit indexed image dataset from a read-only pixel span.
        /// </summary>
        /// <param name="loc_id">File or group identifier.</param>
        /// <param name="dset_name">Image dataset name.</param>
        /// <param name="width">Image width in pixels.</param>
        /// <param name="height">Image height in pixels.</param>
        /// <param name="buffer">Pixel data containing at least <c>width * height</c> bytes.</param>
        /// <returns>A non-negative value if successful; otherwise a negative value.</returns>
        public static int make_image_8bit(long loc_id, string dset_name, ulong width, ulong height, ReadOnlySpan<byte> buffer)
        {
            if ((ulong)buffer.Length < checked(width * height))
            {
                throw new ArgumentException("Pixel span is smaller than width * height.", nameof(buffer));
            }

            fixed (byte* ptr = buffer)
            {
                return make_image_8bit(loc_id, dset_name, width, height, (IntPtr)ptr);
            }
        }

        /// <summary>
        /// Creates a 24-bit true-color image dataset from a read-only pixel span.
        /// </summary>
        /// <param name="loc_id">File or group identifier.</param>
        /// <param name="dset_name">Image dataset name.</param>
        /// <param name="width">Image width in pixels.</param>
        /// <param name="height">Image height in pixels.</param>
        /// <param name="interlace">Interlace mode (<c>INTERLACE_PIXEL</c> or <c>INTERLACE_PLANE</c>).</param>
        /// <param name="buffer">Pixel data containing at least <c>width * height * 3</c> bytes.</param>
        /// <returns>A non-negative value if successful; otherwise a negative value.</returns>
        public static int make_image_24bit(long loc_id, string dset_name, ulong width, ulong height, string interlace, ReadOnlySpan<byte> buffer)
        {
            if ((ulong)buffer.Length < checked(width * height * 3ul))
            {
                throw new ArgumentException("Pixel span is smaller than width * height * 3.", nameof(buffer));
            }

            fixed (byte* ptr = buffer)
            {
                return make_image_24bit(loc_id, dset_name, width, height, interlace, (IntPtr)ptr);
            }
        }

        /// <summary>
        /// Reads image pixel data into a byte span.
        /// </summary>
        /// <param name="loc_id">File or group identifier.</param>
        /// <param name="dset_name">Image dataset name.</param>
        /// <param name="buffer">Destination span sized to hold all image bytes.</param>
        /// <returns>A non-negative value if successful; otherwise a negative value.</returns>
        public static int read_image(long loc_id, string dset_name, Span<byte> buffer)
        {
            fixed (byte* ptr = buffer)
            {
                return read_image(loc_id, dset_name, (IntPtr)ptr);
            }
        }

        /// <summary>
        /// Creates a palette dataset from a two-element dimension span and palette byte data.
        /// </summary>
        /// <param name="loc_id">File or group identifier.</param>
        /// <param name="pal_name">Palette dataset name.</param>
        /// <param name="pal_dims">Two-element palette dimensions (<c>[entries, components]</c>).</param>
        /// <param name="pal_data">Palette byte data covering the product of <paramref name="pal_dims"/>.</param>
        /// <returns>A non-negative value if successful; otherwise a negative value.</returns>
        public static int make_palette(long loc_id, string pal_name, ReadOnlySpan<ulong> pal_dims, ReadOnlySpan<byte> pal_data)
        {
            if (pal_dims.Length != 2 || (ulong)pal_data.Length < checked(pal_dims[0] * pal_dims[1]))
            {
                throw new ArgumentException("Palette requires two dimensions and at least dims[0] * dims[1] bytes of data.");
            }

            fixed (ulong* dimsPtr = pal_dims)
            fixed (byte* dataPtr = pal_data)
            {
                return make_palette(loc_id, pal_name, (IntPtr)dimsPtr, (IntPtr)dataPtr);
            }
        }

        /// <summary>
        /// Retrieves the two dimensions of an attached image palette into a span.
        /// </summary>
        /// <param name="loc_id">File or group identifier.</param>
        /// <param name="image_name">Image dataset name.</param>
        /// <param name="pal_number">Zero-based palette index.</param>
        /// <param name="pal_dims">Destination span of length at least 2.</param>
        /// <returns>A non-negative value if successful; otherwise a negative value.</returns>
        public static int get_palette_info(long loc_id, string image_name, int pal_number, Span<ulong> pal_dims)
        {
            if (pal_dims.Length < 2)
            {
                throw new ArgumentException("Palette dimension span must have at least two elements.", nameof(pal_dims));
            }

            fixed (ulong* dimsPtr = pal_dims)
            {
                return get_palette_info(loc_id, image_name, pal_number, (IntPtr)dimsPtr);
            }
        }

        /// <summary>
        /// Reads an attached palette's bytes into a span.
        /// </summary>
        /// <param name="loc_id">File or group identifier.</param>
        /// <param name="image_name">Image dataset name.</param>
        /// <param name="pal_number">Zero-based palette index.</param>
        /// <param name="pal_data">Destination span sized from <see cref="get_palette_info(long, string, int, Span{ulong})"/>.</param>
        /// <returns>A non-negative value if successful; otherwise a negative value.</returns>
        public static int get_palette(long loc_id, string image_name, int pal_number, Span<byte> pal_data)
        {
            fixed (byte* dataPtr = pal_data)
            {
                return get_palette(loc_id, image_name, pal_number, (IntPtr)dataPtr);
            }
        }
    }

    public unsafe sealed partial class H5PT
    {
        /// <summary>
        /// Appends fixed-length packets from a typed read-only span to a packet table.
        /// </summary>
        /// <typeparam name="T">The unmanaged packet record type.</typeparam>
        /// <param name="table_id">Packet table identifier.</param>
        /// <param name="data">Packets to append.</param>
        /// <returns>A non-negative value if successful; otherwise a negative value.</returns>
        public static int append<T>(long table_id, ReadOnlySpan<T> data)
            where T : unmanaged
        {
            fixed (T* ptr = data)
            {
                return append(table_id, (nuint)data.Length, (IntPtr)ptr);
            }
        }

        /// <summary>
        /// Reads fixed-length packets at the current index into a typed span and advances the index.
        /// </summary>
        /// <typeparam name="T">The unmanaged packet record type.</typeparam>
        /// <param name="table_id">Packet table identifier.</param>
        /// <param name="data">Destination span whose length specifies the number of packets to read.</param>
        /// <returns>A non-negative value if successful; otherwise a negative value.</returns>
        public static int get_next<T>(long table_id, Span<T> data)
            where T : unmanaged
        {
            fixed (T* ptr = data)
            {
                return get_next(table_id, (nuint)data.Length, (IntPtr)ptr);
            }
        }

        /// <summary>
        /// Reads fixed-length packets starting at <paramref name="start"/> into a typed span.
        /// </summary>
        /// <typeparam name="T">The unmanaged packet record type.</typeparam>
        /// <param name="table_id">Packet table identifier.</param>
        /// <param name="start">Zero-based starting packet index.</param>
        /// <param name="data">Destination span whose length specifies the number of packets to read.</param>
        /// <returns>A non-negative value if successful; otherwise a negative value.</returns>
        public static int read_packets<T>(long table_id, ulong start, Span<T> data)
            where T : unmanaged
        {
            fixed (T* ptr = data)
            {
                return read_packets(table_id, start, (nuint)data.Length, (IntPtr)ptr);
            }
        }
    }

    public unsafe sealed partial class H5DS
    {
        /// <summary>
        /// Retrieves a dimension's UTF-8 label into a byte span, or queries its byte length when empty.
        /// </summary>
        /// <param name="dataset">Dataset identifier.</param>
        /// <param name="dimension">Zero-based dimension index.</param>
        /// <param name="utf8Buffer">Destination UTF-8 buffer (including NUL terminator space), or empty to query length.</param>
        /// <returns>The label length in bytes excluding the NUL terminator, or a negative value on failure.</returns>
        public static IntPtr get_label(long dataset, uint dimension, Span<byte> utf8Buffer)
        {
            fixed (byte* ptr = utf8Buffer)
            {
                return get_label(dataset, dimension, (IntPtr)ptr, (nuint)utf8Buffer.Length);
            }
        }

        /// <summary>
        /// Retrieves a dimension's complete UTF-8 label without truncation, <see cref="string.Empty"/> if unset, or <see langword="null"/> on failure.
        /// </summary>
        /// <param name="dataset">Dataset identifier.</param>
        /// <param name="dimension">Zero-based dimension index.</param>
        /// <returns>The dimension label, <see cref="string.Empty"/> if unset, or <see langword="null"/> on failure.</returns>
        public static string get_label(long dataset, uint dimension) =>
            Utf8String.Read((dataset, dimension), static (s, buf) => get_label(s.dataset, s.dimension, buf));

        /// <summary>
        /// Retrieves a dimension scale's UTF-8 name into a byte span, or queries its byte length when empty.
        /// </summary>
        /// <param name="scale">Dimension scale dataset identifier.</param>
        /// <param name="utf8Buffer">Destination UTF-8 buffer (including NUL terminator space), or empty to query length.</param>
        /// <returns>The scale name length in bytes excluding the NUL terminator, or a negative value on failure.</returns>
        public static IntPtr get_scale_name(long scale, Span<byte> utf8Buffer)
        {
            fixed (byte* ptr = utf8Buffer)
            {
                return get_scale_name(scale, (IntPtr)ptr, (nuint)utf8Buffer.Length);
            }
        }

        /// <summary>
        /// Retrieves a dimension scale's complete UTF-8 name without truncation, <see cref="string.Empty"/> if unset, or <see langword="null"/> on failure.
        /// </summary>
        /// <param name="scale">Dimension scale dataset identifier.</param>
        /// <returns>The scale name, <see cref="string.Empty"/> if unset, or <see langword="null"/> on failure.</returns>
        public static string get_scale_name(long scale) => Utf8String.Read(scale, static (id, buf) => get_scale_name(id, buf));
    }

    public unsafe sealed partial class H5LD
    {
        /// <summary>
        /// Retrieves current dataset dimension sizes into a span.
        /// </summary>
        /// <param name="did">Dataset identifier.</param>
        /// <param name="cur_dims">Destination span sized to at least the dataset rank.</param>
        /// <returns>A non-negative value if successful; otherwise a negative value.</returns>
        public static int get_dset_dims(long did, Span<ulong> cur_dims)
        {
            fixed (ulong* ptr = cur_dims)
            {
                return get_dset_dims(did, (IntPtr)ptr);
            }
        }

        /// <summary>
        /// Reads newly appended dataset elements between <paramref name="prev_dims"/> and <paramref name="cur_dims"/> into a typed span.
        /// </summary>
        /// <typeparam name="T">The unmanaged element or field-projection type.</typeparam>
        /// <param name="did">Dataset identifier.</param>
        /// <param name="prev_dims">Previous dataset dimensions.</param>
        /// <param name="cur_dims">Current dataset dimensions.</param>
        /// <param name="fields">Comma-separated compound field names, or <see langword="null"/> for whole elements.</param>
        /// <param name="buf">Destination span sized to hold all newly selected elements.</param>
        /// <returns>A non-negative value if successful; otherwise a negative value.</returns>
        public static int get_dset_elmts<T>(long did, ReadOnlySpan<ulong> prev_dims, ReadOnlySpan<ulong> cur_dims, string fields, Span<T> buf)
            where T : unmanaged
        {
            if (prev_dims.Length != cur_dims.Length)
            {
                throw new ArgumentException("Previous and current dimension spans must have equal rank.");
            }

            fixed (ulong* prevPtr = prev_dims, curPtr = cur_dims)
            fixed (T* bufPtr = buf)
            {
                return get_dset_elmts(did, (IntPtr)prevPtr, (IntPtr)curPtr, fields, (IntPtr)bufPtr);
            }
        }
    }
}

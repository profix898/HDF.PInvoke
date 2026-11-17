using System;
using System.Runtime.CompilerServices;

namespace HDF.PInvoke
{
    public unsafe sealed partial class H5F
    {
        /// <summary>
        /// Retrieves the UTF-8 filename associated with an object into a byte span, or queries its length when empty.
        /// </summary>
        /// <param name="obj_id">Object or file identifier.</param>
        /// <param name="utf8Name">Destination UTF-8 buffer (including NUL terminator space), or empty to query length.</param>
        /// <returns>The filename length in bytes excluding the NUL terminator, or a negative value on failure.</returns>
        public static IntPtr get_name(long obj_id, Span<byte> utf8Name)
        {
            fixed (byte* ptr = utf8Name)
            {
                return get_name_utf8(obj_id, ptr, (IntPtr)utf8Name.Length);
            }
        }

        /// <summary>
        /// Retrieves the complete UTF-8 filename associated with an object without truncation, or <see langword="null"/> on failure.
        /// </summary>
        /// <param name="obj_id">Object or file identifier.</param>
        /// <returns>The filename, <see cref="string.Empty"/> when empty, or <see langword="null"/> on failure.</returns>
        public static string get_name(long obj_id) => Utf8String.Read(obj_id, static (id, buf) => get_name(id, buf));

        /// <summary>
        /// Copies an open file's image into a byte span, or queries the required byte size when <paramref name="buf"/> is empty.
        /// </summary>
        /// <param name="file_id">File identifier.</param>
        /// <param name="buf">Destination byte span, or empty to query required length.</param>
        /// <returns>The required file-image size in bytes if successful; otherwise a negative value.</returns>
        public static IntPtr get_file_image(long file_id, Span<byte> buf)
        {
            fixed (byte* ptr = buf)
            {
                return get_file_image(file_id, (IntPtr)ptr, (IntPtr)buf.Length);
            }
        }

        /// <summary>
        /// Retrieves open object identifiers for a file into a span.
        /// </summary>
        /// <param name="file_id">File identifier, or <see cref="OBJ_ALL"/>.</param>
        /// <param name="types">Object category mask.</param>
        /// <param name="obj_id_list">Destination span receiving open identifiers.</param>
        /// <returns>The number of identifiers placed into <paramref name="obj_id_list"/>, or a negative value on failure.</returns>
        public static IntPtr get_obj_ids(long file_id, uint types, Span<long> obj_id_list)
        {
            fixed (long* ptr = obj_id_list)
            {
                return get_obj_ids(file_id, types, (IntPtr)obj_id_list.Length, (IntPtr)ptr);
            }
        }
    }

    public unsafe sealed partial class H5I
    {
        /// <summary>
        /// Retrieves an object's UTF-8 path name into a byte span, or queries its byte length when empty.
        /// </summary>
        /// <param name="obj_id">Object identifier.</param>
        /// <param name="utf8Name">Destination UTF-8 buffer (including NUL terminator space), or empty to query length.</param>
        /// <returns>The name length in bytes excluding the NUL terminator, zero if unnamed, or a negative value on failure.</returns>
        public static IntPtr get_name(long obj_id, Span<byte> utf8Name)
        {
            fixed (byte* ptr = utf8Name)
            {
                return get_name_utf8(obj_id, ptr, (IntPtr)utf8Name.Length);
            }
        }

        /// <summary>
        /// Retrieves an object's complete UTF-8 path name without truncation, <see cref="string.Empty"/> if unnamed, or <see langword="null"/> on failure.
        /// </summary>
        /// <param name="obj_id">Object identifier.</param>
        /// <returns>The object path name, <see cref="string.Empty"/> if unnamed, or <see langword="null"/> on failure.</returns>
        public static string get_name(long obj_id) => Utf8String.Read(obj_id, static (id, buf) => get_name(id, buf));
    }

    public unsafe sealed partial class H5L
    {
        /// <summary>
        /// Retrieves an indexed link's UTF-8 name into a byte span, or queries its byte length when empty.
        /// </summary>
        /// <param name="loc_id">File or group identifier.</param>
        /// <param name="group_name">Group name relative to <paramref name="loc_id"/>.</param>
        /// <param name="idx_type">Index type.</param>
        /// <param name="order">Index traversal order.</param>
        /// <param name="n">Link index position.</param>
        /// <param name="utf8Name">Destination UTF-8 buffer (including NUL terminator space), or empty to query length.</param>
        /// <param name="lapl_id">Link access property list identifier.</param>
        /// <returns>The link name length in bytes excluding the NUL terminator, or a negative value on failure.</returns>
        public static IntPtr get_name_by_idx(long loc_id, string group_name, H5.index_t idx_type, H5.iter_order_t order, ulong n, Span<byte> utf8Name, long lapl_id = H5P.DEFAULT)
        {
            fixed (byte* ptr = utf8Name)
            {
                return get_name_by_idx_utf8(loc_id, group_name, idx_type, order, n, ptr, (IntPtr)utf8Name.Length, lapl_id);
            }
        }

        /// <summary>
        /// Retrieves an indexed link's complete UTF-8 name without truncation, or <see langword="null"/> on failure.
        /// </summary>
        /// <param name="loc_id">File or group identifier.</param>
        /// <param name="group_name">Group name relative to <paramref name="loc_id"/>.</param>
        /// <param name="idx_type">Index type.</param>
        /// <param name="order">Index traversal order.</param>
        /// <param name="n">Link index position.</param>
        /// <param name="lapl_id">Link access property list identifier.</param>
        /// <returns>The link name, <see cref="string.Empty"/> when empty, or <see langword="null"/> on failure.</returns>
        public static string get_name_by_idx(long loc_id, string group_name, H5.index_t idx_type, H5.iter_order_t order, ulong n, long lapl_id = H5P.DEFAULT) =>
            Utf8String.Read((loc_id, group_name, idx_type, order, n, lapl_id), static (s, buf) => get_name_by_idx(s.loc_id, s.group_name, s.idx_type, s.order, s.n, buf, s.lapl_id));

        /// <summary>
        /// Retrieves a soft or user-defined link's value into a byte span.
        /// </summary>
        /// <param name="loc_id">File or group identifier.</param>
        /// <param name="name">Link name.</param>
        /// <param name="buf">Destination buffer receiving the link payload.</param>
        /// <param name="lapl_id">Link access property list identifier.</param>
        /// <returns>A non-negative value if successful; otherwise a negative value.</returns>
        public static int get_val(long loc_id, string name, Span<byte> buf, long lapl_id = H5P.DEFAULT)
        {
            fixed (byte* ptr = buf)
            {
                return get_val(loc_id, name, (IntPtr)ptr, (IntPtr)buf.Length, lapl_id);
            }
        }
    }

    public unsafe sealed partial class H5O
    {
        /// <summary>
        /// Retrieves an object's UTF-8 comment into a byte span, or queries its byte length when empty.
        /// </summary>
        /// <param name="obj_id">Object identifier.</param>
        /// <param name="utf8Comment">Destination UTF-8 buffer (including NUL terminator space), or empty to query length.</param>
        /// <returns>The comment length in bytes excluding the NUL terminator, zero if none, or a negative value on failure.</returns>
        public static IntPtr get_comment(long obj_id, Span<byte> utf8Comment)
        {
            fixed (byte* ptr = utf8Comment)
            {
                return get_comment_utf8(obj_id, ptr, (IntPtr)utf8Comment.Length);
            }
        }

        /// <summary>
        /// Retrieves an object's complete UTF-8 comment without truncation, <see cref="string.Empty"/> if unset, or <see langword="null"/> on failure.
        /// </summary>
        /// <param name="obj_id">Object identifier.</param>
        /// <returns>The comment text, <see cref="string.Empty"/> if unset, or <see langword="null"/> on failure.</returns>
        public static string get_comment(long obj_id) => Utf8String.Read(obj_id, static (id, buf) => get_comment(id, buf));

        /// <summary>
        /// Retrieves a named object's UTF-8 comment into a byte span, or queries its byte length when empty.
        /// </summary>
        /// <param name="loc_id">Location identifier.</param>
        /// <param name="name">Relative object path.</param>
        /// <param name="utf8Comment">Destination UTF-8 buffer (including NUL terminator space), or empty to query length.</param>
        /// <param name="lapl_id">Link access property list identifier.</param>
        /// <returns>The comment length in bytes excluding the NUL terminator, zero if none, or a negative value on failure.</returns>
        public static IntPtr get_comment_by_name(long loc_id, string name, Span<byte> utf8Comment, long lapl_id = H5P.DEFAULT)
        {
            fixed (byte* ptr = utf8Comment)
            {
                return get_comment_by_name_utf8(loc_id, name, ptr, (IntPtr)utf8Comment.Length, lapl_id);
            }
        }

        /// <summary>
        /// Retrieves a named object's complete UTF-8 comment without truncation, <see cref="string.Empty"/> if unset, or <see langword="null"/> on failure.
        /// </summary>
        /// <param name="loc_id">Location identifier.</param>
        /// <param name="name">Relative object path.</param>
        /// <param name="lapl_id">Link access property list identifier.</param>
        /// <returns>The comment text, <see cref="string.Empty"/> if unset, or <see langword="null"/> on failure.</returns>
        public static string get_comment_by_name(long loc_id, string name, long lapl_id = H5P.DEFAULT) =>
            Utf8String.Read((loc_id, name, lapl_id), static (s, buf) => get_comment_by_name(s.loc_id, s.name, buf, s.lapl_id));
    }

    public unsafe sealed partial class H5E
    {
        /// <summary>
        /// Retrieves an error class's UTF-8 name into a byte span, or queries its byte length when empty.
        /// </summary>
        /// <param name="class_id">Error class identifier.</param>
        /// <param name="utf8Name">Destination UTF-8 buffer (including NUL terminator space), or empty to query length.</param>
        /// <returns>The class name length in bytes excluding the NUL terminator, or a negative value on failure.</returns>
        public static IntPtr get_class_name(long class_id, Span<byte> utf8Name)
        {
            fixed (byte* ptr = utf8Name)
            {
                return get_class_name_utf8(class_id, ptr, (IntPtr)utf8Name.Length);
            }
        }

        /// <summary>
        /// Retrieves an error class's complete UTF-8 name without truncation, or <see langword="null"/> on failure.
        /// </summary>
        /// <param name="class_id">Error class identifier.</param>
        /// <returns>The error class name, <see cref="string.Empty"/> when empty, or <see langword="null"/> on failure.</returns>
        public static string get_class_name(long class_id) => Utf8String.Read(class_id, static (id, buf) => get_class_name(id, buf));

        /// <summary>
        /// Retrieves an error message's UTF-8 text into a byte span, or queries its byte length when empty.
        /// </summary>
        /// <param name="msg_id">Error message identifier.</param>
        /// <param name="msg_type">Receives the error message category.</param>
        /// <param name="utf8Msg">Destination UTF-8 buffer (including NUL terminator space), or empty to query length.</param>
        /// <returns>The message length in bytes excluding the NUL terminator, or a negative value on failure.</returns>
        public static IntPtr get_msg(long msg_id, ref type_t msg_type, Span<byte> utf8Msg)
        {
            fixed (byte* ptr = utf8Msg)
            {
                return get_msg_utf8(msg_id, ref msg_type, ptr, (IntPtr)utf8Msg.Length);
            }
        }

        /// <summary>
        /// Retrieves an error message's complete UTF-8 text and category without truncation, or <see langword="null"/> on failure.
        /// </summary>
        /// <param name="msg_id">Error message identifier.</param>
        /// <param name="msg_type">Receives the error message category.</param>
        /// <returns>The error message text, <see cref="string.Empty"/> when empty, or <see langword="null"/> on failure.</returns>
        public static string get_msg(long msg_id, out type_t msg_type)
        {
            var observedType = default(type_t);
            var text = Utf8String.Read((msg_id, TypePtr: (IntPtr)(&observedType)), static (s, buf) => get_msg(s.msg_id, ref *(type_t*)s.TypePtr, buf));
            msg_type = observedType;
            return text;
        }
    }

    public unsafe sealed partial class H5PL
    {
        /// <summary>
        /// Retrieves a plugin search path in UTF-8 into a byte span, or queries its byte length when empty.
        /// </summary>
        /// <param name="index">Plugin search-path table index.</param>
        /// <param name="utf8Pathname">Destination UTF-8 buffer (including NUL terminator space), or empty to query length.</param>
        /// <returns>The path length in bytes excluding the NUL terminator, or a negative value on failure.</returns>
        public static IntPtr get(uint index, Span<byte> utf8Pathname)
        {
            fixed (byte* ptr = utf8Pathname)
            {
                return get_utf8(index, ptr, (IntPtr)utf8Pathname.Length);
            }
        }

        /// <summary>
        /// Retrieves a complete plugin search path in UTF-8 without truncation, or <see langword="null"/> on failure.
        /// </summary>
        /// <param name="index">Plugin search-path table index.</param>
        /// <returns>The plugin directory path, <see cref="string.Empty"/> when empty, or <see langword="null"/> on failure.</returns>
        public static string get(uint index) => Utf8String.Read(index, static (idx, buf) => get(idx, buf));
    }

    public unsafe sealed partial class H5R
    {
        /// <summary>
        /// Retrieves a legacy reference's target UTF-8 path into a byte span, or queries its byte length when empty.
        /// </summary>
        /// <param name="loc_id">File or object identifier.</param>
        /// <param name="ref_type">Reference category.</param>
        /// <param name="refer">Address of the legacy reference buffer.</param>
        /// <param name="utf8Name">Destination UTF-8 buffer (including NUL terminator space), or empty to query length.</param>
        /// <returns>The name length in bytes excluding the NUL terminator, or a negative value on failure.</returns>
        public static IntPtr get_name(long loc_id, type_t ref_type, IntPtr refer, Span<byte> utf8Name)
        {
            fixed (byte* ptr = utf8Name)
            {
                return get_name_utf8(loc_id, ref_type, refer, ptr, (IntPtr)utf8Name.Length);
            }
        }

        /// <summary>
        /// Retrieves a modern reference's UTF-8 filename into a byte span, or queries its byte length when empty.
        /// </summary>
        /// <param name="reference">Reference instance.</param>
        /// <param name="utf8Buffer">Destination UTF-8 buffer (including NUL terminator space), or empty to query length.</param>
        /// <returns>The filename length in bytes excluding the NUL terminator, or a negative value on failure.</returns>
        public static IntPtr get_file_name(ref ref_t reference, Span<byte> utf8Buffer)
        {
            fixed (byte* ptr = utf8Buffer)
            {
                return get_file_name(ref reference, (IntPtr)ptr, (nuint)utf8Buffer.Length);
            }
        }

        /// <summary>
        /// Retrieves a modern reference's complete UTF-8 filename without truncation, or <see langword="null"/> on failure.
        /// </summary>
        /// <param name="reference">Reference instance.</param>
        /// <returns>The target filename, <see cref="string.Empty"/> when empty, or <see langword="null"/> on failure.</returns>
        public static string get_file_name(ref ref_t reference)
        {
            fixed (ref_t* refPtr = &reference)
            {
                return Utf8String.Read((IntPtr)refPtr, static (p, buf) => get_file_name(ref *(ref_t*)p, buf));
            }
        }

        /// <summary>
        /// Retrieves a modern reference's UTF-8 object path into a byte span, or queries its byte length when empty.
        /// </summary>
        /// <param name="reference">Reference instance.</param>
        /// <param name="referenceAccess">Reference access property list identifier.</param>
        /// <param name="utf8Buffer">Destination UTF-8 buffer (including NUL terminator space), or empty to query length.</param>
        /// <returns>The object path length in bytes excluding the NUL terminator, or a negative value on failure.</returns>
        public static IntPtr get_obj_name(ref ref_t reference, long referenceAccess, Span<byte> utf8Buffer)
        {
            fixed (byte* ptr = utf8Buffer)
            {
                return get_obj_name(ref reference, referenceAccess, (IntPtr)ptr, (nuint)utf8Buffer.Length);
            }
        }

        /// <summary>
        /// Retrieves a modern reference's complete UTF-8 object path without truncation, or <see langword="null"/> on failure.
        /// </summary>
        /// <param name="reference">Reference instance.</param>
        /// <param name="referenceAccess">Reference access property list identifier.</param>
        /// <returns>The target object path, <see cref="string.Empty"/> when empty, or <see langword="null"/> on failure.</returns>
        public static string get_obj_name(ref ref_t reference, long referenceAccess = H5P.DEFAULT)
        {
            fixed (ref_t* refPtr = &reference)
            {
                return Utf8String.Read((RefPtr: (IntPtr)refPtr, referenceAccess), static (s, buf) => get_obj_name(ref *(ref_t*)s.RefPtr, s.referenceAccess, buf));
            }
        }

        /// <summary>
        /// Retrieves a modern attribute reference's UTF-8 attribute name into a byte span, or queries its byte length when empty.
        /// </summary>
        /// <param name="reference">Attribute reference instance.</param>
        /// <param name="utf8Buffer">Destination UTF-8 buffer (including NUL terminator space), or empty to query length.</param>
        /// <returns>The attribute name length in bytes excluding the NUL terminator, or a negative value on failure.</returns>
        public static IntPtr get_attr_name(ref ref_t reference, Span<byte> utf8Buffer)
        {
            fixed (byte* ptr = utf8Buffer)
            {
                return get_attr_name(ref reference, (IntPtr)ptr, (nuint)utf8Buffer.Length);
            }
        }

        /// <summary>
        /// Retrieves a modern attribute reference's complete UTF-8 attribute name without truncation, or <see langword="null"/> on failure.
        /// </summary>
        /// <param name="reference">Attribute reference instance.</param>
        /// <returns>The target attribute name, <see cref="string.Empty"/> when empty, or <see langword="null"/> on failure.</returns>
        public static string get_attr_name(ref ref_t reference)
        {
            fixed (ref_t* refPtr = &reference)
            {
                return Utf8String.Read((IntPtr)refPtr, static (p, buf) => get_attr_name(ref *(ref_t*)p, buf));
            }
        }
    }

    public unsafe sealed partial class H5T
    {
        /// <summary>
        /// Creates an array datatype from a base datatype and dimension span.
        /// </summary>
        /// <param name="base_type_id">Element datatype identifier.</param>
        /// <param name="dims">Size of each array dimension.</param>
        /// <returns>A datatype identifier if successful; otherwise a negative value.</returns>
        public static long array_create(long base_type_id, ReadOnlySpan<ulong> dims)
        {
            fixed (ulong* dimsPtr = dims)
            {
                return array_create_native(base_type_id, (uint)dims.Length, dimsPtr);
            }
        }

        /// <summary>
        /// Retrieves the dimensions of an array datatype into a span.
        /// </summary>
        /// <param name="adtype_id">Array datatype identifier.</param>
        /// <param name="dims">Destination span receiving dimension sizes.</param>
        /// <returns>The rank of the array datatype if successful; otherwise a negative value.</returns>
        [OverloadResolutionPriority(1)]
        public static int get_array_dims(long adtype_id, Span<ulong> dims)
        {
            var rank = get_array_ndims(adtype_id);
            if (rank < 0)
            {
                return -1;
            }
            SpanValidation.Capacity(dims.Length, (ulong)rank, nameof(dims));

            fixed (ulong* dimsPtr = dims)
            {
                return get_array_dims_native(adtype_id, dimsPtr);
            }
        }

        /// <summary>
        /// Converts <paramref name="nelmts"/> elements in place within <paramref name="buf"/> between two HDF5 datatypes.
        /// </summary>
        /// <param name="src_type_id">Source datatype identifier.</param>
        /// <param name="dest_type_id">Destination datatype identifier.</param>
        /// <param name="nelmts">Number of elements to convert.</param>
        /// <param name="buf">In-place conversion buffer large enough for the larger of source and destination element sizes.</param>
        /// <param name="background">Optional background buffer for compound conversions, or empty if unused.</param>
        /// <param name="plist_id">Dataset transfer property list identifier.</param>
        /// <returns>A non-negative value if successful; otherwise a negative value.</returns>
        public static int convert(long src_type_id, long dest_type_id, IntPtr nelmts, Span<byte> buf, Span<byte> background = default, long plist_id = H5P.DEFAULT)
        {
            var count = checked((ulong)nelmts.ToInt64());
            if (!SpanValidation.Packed<byte>(src_type_id, count, buf.Length, nameof(buf)) ||
                !SpanValidation.Packed<byte>(dest_type_id, count, buf.Length, nameof(buf)) ||
                (!background.IsEmpty && !SpanValidation.Packed<byte>(dest_type_id, count, background.Length, nameof(background))))
            {
                return -1;
            }

            fixed (byte* bufPtr = buf, bkgPtr = background)
            {
                return convert(src_type_id, dest_type_id, nelmts, (IntPtr)bufPtr, (IntPtr)bkgPtr, plist_id);
            }
        }

        /// <summary>
        /// Inserts a typed enumeration member into an enumeration datatype.
        /// </summary>
        /// <typeparam name="T">The unmanaged underlying integer type of the enumeration.</typeparam>
        /// <param name="dtype_id">Enumeration datatype identifier.</param>
        /// <param name="name">UTF-8 member name.</param>
        /// <param name="value">Member value.</param>
        /// <returns>A non-negative value if successful; otherwise a negative value.</returns>
        public static int enum_insert<T>(long dtype_id, string name, in T value)
            where T : unmanaged
        {
            if (!SpanValidation.Scalar<T>(dtype_id, nameof(value)))
            {
                return -1;
            }

            fixed (T* ptr = &value)
            {
                return enum_insert(dtype_id, name, (IntPtr)ptr);
            }
        }

        /// <summary>
        /// Retrieves the UTF-8 symbol name of a typed enumeration value into a byte span.
        /// </summary>
        /// <typeparam name="T">The unmanaged underlying integer type of the enumeration.</typeparam>
        /// <param name="dtype_id">Enumeration datatype identifier.</param>
        /// <param name="value">Member value to look up.</param>
        /// <param name="utf8Name">Destination UTF-8 buffer.</param>
        /// <returns>A non-negative value if successful; otherwise a negative value.</returns>
        public static int enum_nameof<T>(long dtype_id, in T value, Span<byte> utf8Name)
            where T : unmanaged
        {
            if (!SpanValidation.Scalar<T>(dtype_id, nameof(value)))
            {
                return -1;
            }

            fixed (T* valPtr = &value)
            fixed (byte* namePtr = utf8Name)
            {
                return enum_nameof_utf8(dtype_id, (IntPtr)valPtr, namePtr, (IntPtr)utf8Name.Length);
            }
        }

        /// <summary>
        /// Retrieves the typed value corresponding to an enumeration member name.
        /// </summary>
        /// <typeparam name="T">The unmanaged underlying integer type of the enumeration.</typeparam>
        /// <param name="dtype_id">Enumeration datatype identifier.</param>
        /// <param name="name">UTF-8 member name.</param>
        /// <param name="value">Receives the member value.</param>
        /// <returns>A non-negative value if successful; otherwise a negative value.</returns>
        public static int enum_valueof<T>(long dtype_id, string name, out T value)
            where T : unmanaged
        {
            value = default;
            if (!SpanValidation.Scalar<T>(dtype_id, nameof(value)))
            {
                return -1;
            }

            fixed (T* ptr = &value)
            {
                return enum_valueof(dtype_id, name, (IntPtr)ptr);
            }
        }

        /// <summary>
        /// Encodes a datatype description into a byte span, or queries required size when <paramref name="buf"/> is empty.
        /// </summary>
        /// <param name="obj_id">Datatype identifier.</param>
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
        /// Decodes a datatype from a bounded read-only byte span; close the returned identifier with <see cref="close"/>.
        /// </summary>
        /// <param name="buffer">Encoded datatype bytes.</param>
        /// <returns>A datatype identifier if successful; otherwise a negative value.</returns>
        public static long decode2(ReadOnlySpan<byte> buffer)
        {
            fixed (byte* ptr = buffer)
            {
                return decode2((IntPtr)ptr, (nuint)buffer.Length);
            }
        }
    }

    public unsafe sealed partial class H5VL
    {
        /// <summary>
        /// Retrieves an object's VOL connector UTF-8 name into a byte span, or queries its byte length when empty.
        /// </summary>
        /// <param name="objectId">Object identifier.</param>
        /// <param name="utf8Buffer">Destination UTF-8 buffer (including NUL terminator space), or empty to query length.</param>
        /// <returns>The connector name length in bytes excluding the NUL terminator, or a negative value on failure.</returns>
        public static IntPtr get_connector_name(long objectId, Span<byte> utf8Buffer)
        {
            fixed (byte* ptr = utf8Buffer)
            {
                return get_connector_name(objectId, (IntPtr)ptr, (nuint)utf8Buffer.Length);
            }
        }

        /// <summary>
        /// Retrieves an object's complete VOL connector UTF-8 name without truncation, or <see langword="null"/> on failure.
        /// </summary>
        /// <param name="objectId">Object identifier.</param>
        /// <returns>The connector name, <see cref="string.Empty"/> when empty, or <see langword="null"/> on failure.</returns>
        public static string get_connector_name(long objectId) => Utf8String.Read(objectId, static (id, buf) => get_connector_name(id, buf));
    }
}

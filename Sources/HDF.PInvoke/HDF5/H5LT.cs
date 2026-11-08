using System;
using System.Runtime.InteropServices;

namespace HDF.PInvoke
{
    /// <summary>Provides native high-level dataset and attribute convenience operations.</summary>
    public unsafe sealed class H5LT
    {
        /// <summary>Initializes native HDF5 before the first operation.</summary>
        static H5LT()
        {
            H5.open();
        }

        /// <summary>Selects the textual datatype language.</summary>
        public enum lang_t
        {
            /// <summary>Invalid language.</summary>
            ERR = -1,

            /// <summary>HDF5 data definition language.</summary>
            DDL = 0,

            /// <summary>C language selector; availability depends on the native formatter.</summary>
            C = 1,

            /// <summary>Fortran language selector; availability depends on the native formatter.</summary>
            FORTRAN = 2,

            /// <summary>Invalid terminal value.</summary>
            NO_LANG = 3
        }

        /// <summary>Opens a file image for read/write access.</summary>
        public const uint FILE_IMAGE_OPEN_RW = 1;

        /// <summary>Avoids copying the caller's file image; keep it alive until file close.</summary>
        public const uint FILE_IMAGE_DONT_COPY = 2;

        /// <summary>Keeps caller ownership when combined with FILE_IMAGE_DONT_COPY.</summary>
        public const uint FILE_IMAGE_DONT_RELEASE = 4;

        /// <summary>Creates and writes a dataset from caller-owned data.</summary>
        /// <remarks>Dimensions must contain rank unsigned 64-bit values; buffer must cover the product of dimensions in the memory datatype.</remarks>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5LTmake_dataset", CallingConvention = CallingConvention.Cdecl)]
        public static extern int make_dataset(long location, [MarshalAs(UnmanagedType.LPUTF8Str)] string name, int rank, IntPtr dimensions, long type, IntPtr buffer);

        /// <summary>Creates a scalar string dataset from a UTF-8 input string.</summary>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5LTmake_dataset_string", CallingConvention = CallingConvention.Cdecl)]
        public static extern int make_dataset_string(long location, [MarshalAs(UnmanagedType.LPUTF8Str)] string name, [MarshalAs(UnmanagedType.LPUTF8Str)] string value);

        /// <summary>Reads a dataset into caller-owned storage using the supplied memory datatype.</summary>
        /// <remarks>Query dimensions first and allocate enough bytes for every converted element.</remarks>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5LTread_dataset", CallingConvention = CallingConvention.Cdecl)]
        public static extern int read_dataset(long location, [MarshalAs(UnmanagedType.LPUTF8Str)] string name, long type, IntPtr buffer);

        /// <summary>Reads a string dataset into caller-owned storage including its terminator.</summary>
        /// <remarks>Query datatype size first; this native API does not accept a buffer capacity.</remarks>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5LTread_dataset_string", CallingConvention = CallingConvention.Cdecl)]
        public static extern int read_dataset_string(long location, [MarshalAs(UnmanagedType.LPUTF8Str)] string name, IntPtr buffer);

        /// <summary>Gets the rank of a dataset.</summary>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5LTget_dataset_ndims", CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_dataset_ndims(long location, [MarshalAs(UnmanagedType.LPUTF8Str)] string name, out int rank);

        /// <summary>Gets dataset dimensions, datatype class and element size.</summary>
        /// <remarks>Dimensions must have space for the rank returned by get_dataset_ndims; pass null when dimensions are not needed.</remarks>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5LTget_dataset_info", CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_dataset_info(long location, [MarshalAs(UnmanagedType.LPUTF8Str)] string name, IntPtr dimensions, out H5T.class_t typeClass, out UIntPtr typeSize);

        /// <summary>Returns positive if the dataset exists, zero if not, or negative on error.</summary>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5LTfind_dataset", CallingConvention = CallingConvention.Cdecl)]
        public static extern int find_dataset(long location, [MarshalAs(UnmanagedType.LPUTF8Str)] string name);

        /// <summary>Creates or replaces a string attribute using UTF-8 input.</summary>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5LTset_attribute_string", CallingConvention = CallingConvention.Cdecl)]
        public static extern int set_attribute_string(long location, [MarshalAs(UnmanagedType.LPUTF8Str)] string objectName, [MarshalAs(UnmanagedType.LPUTF8Str)] string attributeName, [MarshalAs(UnmanagedType.LPUTF8Str)] string value);

        /// <summary>Reads an attribute using the specified memory datatype.</summary>
        /// <remarks>Caller must allocate space for all converted elements, obtained from get_attribute_info.</remarks>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5LTget_attribute", CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_attribute(long location, [MarshalAs(UnmanagedType.LPUTF8Str)] string objectName, [MarshalAs(UnmanagedType.LPUTF8Str)] string attributeName, long type, IntPtr buffer);

        /// <summary>Reads a string attribute into caller-owned storage including its terminator.</summary>
        /// <remarks>Query datatype size first; this native API does not accept a buffer capacity.</remarks>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5LTget_attribute_string", CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_attribute_string(long location, [MarshalAs(UnmanagedType.LPUTF8Str)] string objectName, [MarshalAs(UnmanagedType.LPUTF8Str)] string attributeName, IntPtr buffer);

        /// <summary>Gets the rank of an attribute.</summary>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5LTget_attribute_ndims", CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_attribute_ndims(long location, [MarshalAs(UnmanagedType.LPUTF8Str)] string objectName, [MarshalAs(UnmanagedType.LPUTF8Str)] string attributeName, out int rank);

        /// <summary>Gets attribute dimensions, datatype class and element size.</summary>
        /// <remarks>Dimensions must have room for the rank reported by get_attribute_ndims; null omits dimensions.</remarks>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5LTget_attribute_info", CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_attribute_info(long location, [MarshalAs(UnmanagedType.LPUTF8Str)] string objectName, [MarshalAs(UnmanagedType.LPUTF8Str)] string attributeName, IntPtr dimensions, out H5T.class_t typeClass, out UIntPtr typeSize);

        /// <summary>Parses a textual datatype description; close the returned datatype with H5T.close.</summary>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5LTtext_to_dtype", CallingConvention = CallingConvention.Cdecl)]
        public static extern long text_to_dtype([MarshalAs(UnmanagedType.LPUTF8Str)] string text, lang_t language);

        /// <summary>Formats a datatype description; pass a null buffer to query required storage.</summary>
        /// <remarks>Size supplies byte capacity and receives required storage including the terminating null.</remarks>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5LTdtype_to_text", CallingConvention = CallingConvention.Cdecl)]
        public static extern int dtype_to_text(long type, IntPtr buffer, lang_t language, ref UIntPtr size);

        /// <summary>Returns positive if an attribute exists, zero if absent, or negative on error.</summary>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5LTfind_attribute", CallingConvention = CallingConvention.Cdecl)]
        public static extern int find_attribute(long objectId, [MarshalAs(UnmanagedType.LPUTF8Str)] string name);

        /// <summary>Checks a path and optionally its final object using a one-byte boolean.</summary>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5LTpath_valid", CallingConvention = CallingConvention.Cdecl)]
        public static extern int path_valid(long location, [MarshalAs(UnmanagedType.LPUTF8Str)] string path, byte checkObject);

        /// <summary>Opens an in-memory HDF5 file image; close the file with H5F.close.</summary>
        /// <remarks>With no flags, HDF5 copies the image. DONT_COPY borrows the original buffer until file close; combine DONT_RELEASE to retain caller ownership.</remarks>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5LTopen_file_image", CallingConvention = CallingConvention.Cdecl)]
        public static extern long open_file_image(IntPtr buffer, UIntPtr size, uint flags);

        /// <summary>Creates a dataset from native char values.</summary>
        /// <remarks>Dimensions contains rank unsigned 64-bit values; buffer must hold their product in native elements.</remarks>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5LTmake_dataset_char", CallingConvention = CallingConvention.Cdecl)]
        public static extern int make_dataset_char(long location, [MarshalAs(UnmanagedType.LPUTF8Str)] string name, int rank, IntPtr dimensions, IntPtr buffer);

        /// <summary>Reads a dataset as native char values.</summary>
        /// <remarks>Caller must allocate enough native elements for the dataset dimensions.</remarks>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5LTread_dataset_char", CallingConvention = CallingConvention.Cdecl)]
        public static extern int read_dataset_char(long location, [MarshalAs(UnmanagedType.LPUTF8Str)] string name, IntPtr buffer);

        /// <summary>Creates a dataset from native short values.</summary>
        /// <remarks>Dimensions contains rank unsigned 64-bit values; buffer must hold their product in native elements.</remarks>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5LTmake_dataset_short", CallingConvention = CallingConvention.Cdecl)]
        public static extern int make_dataset_short(long location, [MarshalAs(UnmanagedType.LPUTF8Str)] string name, int rank, IntPtr dimensions, IntPtr buffer);

        /// <summary>Reads a dataset as native short values.</summary>
        /// <remarks>Caller must allocate enough native elements for the dataset dimensions.</remarks>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5LTread_dataset_short", CallingConvention = CallingConvention.Cdecl)]
        public static extern int read_dataset_short(long location, [MarshalAs(UnmanagedType.LPUTF8Str)] string name, IntPtr buffer);

        /// <summary>Creates a dataset from native int values.</summary>
        /// <remarks>Dimensions contains rank unsigned 64-bit values; buffer must hold their product in native elements.</remarks>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5LTmake_dataset_int", CallingConvention = CallingConvention.Cdecl)]
        public static extern int make_dataset_int(long location, [MarshalAs(UnmanagedType.LPUTF8Str)] string name, int rank, IntPtr dimensions, IntPtr buffer);

        /// <summary>Reads a dataset as native int values.</summary>
        /// <remarks>Caller must allocate enough native elements for the dataset dimensions.</remarks>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5LTread_dataset_int", CallingConvention = CallingConvention.Cdecl)]
        public static extern int read_dataset_int(long location, [MarshalAs(UnmanagedType.LPUTF8Str)] string name, IntPtr buffer);

        /// <summary>Creates a dataset from native C long (four bytes on Windows, eight on Unix) values.</summary>
        /// <remarks>Dimensions contains rank unsigned 64-bit values; buffer must hold their product in native elements.</remarks>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5LTmake_dataset_long", CallingConvention = CallingConvention.Cdecl)]
        public static extern int make_dataset_long(long location, [MarshalAs(UnmanagedType.LPUTF8Str)] string name, int rank, IntPtr dimensions, IntPtr buffer);

        /// <summary>Reads a dataset as native C long (four bytes on Windows, eight on Unix) values.</summary>
        /// <remarks>Caller must allocate enough native elements for the dataset dimensions.</remarks>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5LTread_dataset_long", CallingConvention = CallingConvention.Cdecl)]
        public static extern int read_dataset_long(long location, [MarshalAs(UnmanagedType.LPUTF8Str)] string name, IntPtr buffer);

        /// <summary>Creates a dataset from native float values.</summary>
        /// <remarks>Dimensions contains rank unsigned 64-bit values; buffer must hold their product in native elements.</remarks>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5LTmake_dataset_float", CallingConvention = CallingConvention.Cdecl)]
        public static extern int make_dataset_float(long location, [MarshalAs(UnmanagedType.LPUTF8Str)] string name, int rank, IntPtr dimensions, IntPtr buffer);

        /// <summary>Reads a dataset as native float values.</summary>
        /// <remarks>Caller must allocate enough native elements for the dataset dimensions.</remarks>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5LTread_dataset_float", CallingConvention = CallingConvention.Cdecl)]
        public static extern int read_dataset_float(long location, [MarshalAs(UnmanagedType.LPUTF8Str)] string name, IntPtr buffer);

        /// <summary>Creates a dataset from native double values.</summary>
        /// <remarks>Dimensions contains rank unsigned 64-bit values; buffer must hold their product in native elements.</remarks>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5LTmake_dataset_double", CallingConvention = CallingConvention.Cdecl)]
        public static extern int make_dataset_double(long location, [MarshalAs(UnmanagedType.LPUTF8Str)] string name, int rank, IntPtr dimensions, IntPtr buffer);

        /// <summary>Reads a dataset as native double values.</summary>
        /// <remarks>Caller must allocate enough native elements for the dataset dimensions.</remarks>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5LTread_dataset_double", CallingConvention = CallingConvention.Cdecl)]
        public static extern int read_dataset_double(long location, [MarshalAs(UnmanagedType.LPUTF8Str)] string name, IntPtr buffer);

        /// <summary>Creates or replaces an attribute from native char values.</summary>
        /// <remarks>Count is the number of elements, not bytes; caller owns the input buffer.</remarks>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5LTset_attribute_char", CallingConvention = CallingConvention.Cdecl)]
        public static extern int set_attribute_char(long location, [MarshalAs(UnmanagedType.LPUTF8Str)] string objectName, [MarshalAs(UnmanagedType.LPUTF8Str)] string attributeName, IntPtr buffer, UIntPtr count);

        /// <summary>Reads an attribute as native char values.</summary>
        /// <remarks>Query dimensions first and allocate enough native elements; this API has no capacity parameter.</remarks>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5LTget_attribute_char", CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_attribute_char(long location, [MarshalAs(UnmanagedType.LPUTF8Str)] string objectName, [MarshalAs(UnmanagedType.LPUTF8Str)] string attributeName, IntPtr buffer);

        /// <summary>Creates or replaces an attribute from native uchar values.</summary>
        /// <remarks>Count is the number of elements, not bytes; caller owns the input buffer.</remarks>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5LTset_attribute_uchar", CallingConvention = CallingConvention.Cdecl)]
        public static extern int set_attribute_uchar(long location, [MarshalAs(UnmanagedType.LPUTF8Str)] string objectName, [MarshalAs(UnmanagedType.LPUTF8Str)] string attributeName, IntPtr buffer, UIntPtr count);

        /// <summary>Reads an attribute as native uchar values.</summary>
        /// <remarks>Query dimensions first and allocate enough native elements; this API has no capacity parameter.</remarks>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5LTget_attribute_uchar", CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_attribute_uchar(long location, [MarshalAs(UnmanagedType.LPUTF8Str)] string objectName, [MarshalAs(UnmanagedType.LPUTF8Str)] string attributeName, IntPtr buffer);

        /// <summary>Creates or replaces an attribute from native short values.</summary>
        /// <remarks>Count is the number of elements, not bytes; caller owns the input buffer.</remarks>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5LTset_attribute_short", CallingConvention = CallingConvention.Cdecl)]
        public static extern int set_attribute_short(long location, [MarshalAs(UnmanagedType.LPUTF8Str)] string objectName, [MarshalAs(UnmanagedType.LPUTF8Str)] string attributeName, IntPtr buffer, UIntPtr count);

        /// <summary>Reads an attribute as native short values.</summary>
        /// <remarks>Query dimensions first and allocate enough native elements; this API has no capacity parameter.</remarks>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5LTget_attribute_short", CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_attribute_short(long location, [MarshalAs(UnmanagedType.LPUTF8Str)] string objectName, [MarshalAs(UnmanagedType.LPUTF8Str)] string attributeName, IntPtr buffer);

        /// <summary>Creates or replaces an attribute from native ushort values.</summary>
        /// <remarks>Count is the number of elements, not bytes; caller owns the input buffer.</remarks>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5LTset_attribute_ushort", CallingConvention = CallingConvention.Cdecl)]
        public static extern int set_attribute_ushort(long location, [MarshalAs(UnmanagedType.LPUTF8Str)] string objectName, [MarshalAs(UnmanagedType.LPUTF8Str)] string attributeName, IntPtr buffer, UIntPtr count);

        /// <summary>Reads an attribute as native ushort values.</summary>
        /// <remarks>Query dimensions first and allocate enough native elements; this API has no capacity parameter.</remarks>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5LTget_attribute_ushort", CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_attribute_ushort(long location, [MarshalAs(UnmanagedType.LPUTF8Str)] string objectName, [MarshalAs(UnmanagedType.LPUTF8Str)] string attributeName, IntPtr buffer);

        /// <summary>Creates or replaces an attribute from native int values.</summary>
        /// <remarks>Count is the number of elements, not bytes; caller owns the input buffer.</remarks>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5LTset_attribute_int", CallingConvention = CallingConvention.Cdecl)]
        public static extern int set_attribute_int(long location, [MarshalAs(UnmanagedType.LPUTF8Str)] string objectName, [MarshalAs(UnmanagedType.LPUTF8Str)] string attributeName, IntPtr buffer, UIntPtr count);

        /// <summary>Reads an attribute as native int values.</summary>
        /// <remarks>Query dimensions first and allocate enough native elements; this API has no capacity parameter.</remarks>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5LTget_attribute_int", CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_attribute_int(long location, [MarshalAs(UnmanagedType.LPUTF8Str)] string objectName, [MarshalAs(UnmanagedType.LPUTF8Str)] string attributeName, IntPtr buffer);

        /// <summary>Creates or replaces an attribute from native uint values.</summary>
        /// <remarks>Count is the number of elements, not bytes; caller owns the input buffer.</remarks>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5LTset_attribute_uint", CallingConvention = CallingConvention.Cdecl)]
        public static extern int set_attribute_uint(long location, [MarshalAs(UnmanagedType.LPUTF8Str)] string objectName, [MarshalAs(UnmanagedType.LPUTF8Str)] string attributeName, IntPtr buffer, UIntPtr count);

        /// <summary>Reads an attribute as native uint values.</summary>
        /// <remarks>Query dimensions first and allocate enough native elements; this API has no capacity parameter.</remarks>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5LTget_attribute_uint", CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_attribute_uint(long location, [MarshalAs(UnmanagedType.LPUTF8Str)] string objectName, [MarshalAs(UnmanagedType.LPUTF8Str)] string attributeName, IntPtr buffer);

        /// <summary>Creates or replaces an attribute from native C long (four bytes on Windows, eight on Unix) values.</summary>
        /// <remarks>Count is the number of elements, not bytes; caller owns the input buffer.</remarks>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5LTset_attribute_long", CallingConvention = CallingConvention.Cdecl)]
        public static extern int set_attribute_long(long location, [MarshalAs(UnmanagedType.LPUTF8Str)] string objectName, [MarshalAs(UnmanagedType.LPUTF8Str)] string attributeName, IntPtr buffer, UIntPtr count);

        /// <summary>Reads an attribute as native C long (four bytes on Windows, eight on Unix) values.</summary>
        /// <remarks>Query dimensions first and allocate enough native elements; this API has no capacity parameter.</remarks>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5LTget_attribute_long", CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_attribute_long(long location, [MarshalAs(UnmanagedType.LPUTF8Str)] string objectName, [MarshalAs(UnmanagedType.LPUTF8Str)] string attributeName, IntPtr buffer);

        /// <summary>Creates or replaces an attribute from native long_long values.</summary>
        /// <remarks>Count is the number of elements, not bytes; caller owns the input buffer.</remarks>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5LTset_attribute_long_long", CallingConvention = CallingConvention.Cdecl)]
        public static extern int set_attribute_long_long(long location, [MarshalAs(UnmanagedType.LPUTF8Str)] string objectName, [MarshalAs(UnmanagedType.LPUTF8Str)] string attributeName, IntPtr buffer, UIntPtr count);

        /// <summary>Reads an attribute as native long_long values.</summary>
        /// <remarks>Query dimensions first and allocate enough native elements; this API has no capacity parameter.</remarks>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5LTget_attribute_long_long", CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_attribute_long_long(long location, [MarshalAs(UnmanagedType.LPUTF8Str)] string objectName, [MarshalAs(UnmanagedType.LPUTF8Str)] string attributeName, IntPtr buffer);

        /// <summary>Creates or replaces an attribute from native C ulong (four bytes on Windows, eight on Unix) values.</summary>
        /// <remarks>Count is the number of elements, not bytes; caller owns the input buffer.</remarks>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5LTset_attribute_ulong", CallingConvention = CallingConvention.Cdecl)]
        public static extern int set_attribute_ulong(long location, [MarshalAs(UnmanagedType.LPUTF8Str)] string objectName, [MarshalAs(UnmanagedType.LPUTF8Str)] string attributeName, IntPtr buffer, UIntPtr count);

        /// <summary>Reads an attribute as native C ulong (four bytes on Windows, eight on Unix) values.</summary>
        /// <remarks>Query dimensions first and allocate enough native elements; this API has no capacity parameter.</remarks>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5LTget_attribute_ulong", CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_attribute_ulong(long location, [MarshalAs(UnmanagedType.LPUTF8Str)] string objectName, [MarshalAs(UnmanagedType.LPUTF8Str)] string attributeName, IntPtr buffer);

        /// <summary>Creates or replaces an attribute from native ullong values.</summary>
        /// <remarks>Count is the number of elements, not bytes; caller owns the input buffer.</remarks>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5LTset_attribute_ullong", CallingConvention = CallingConvention.Cdecl)]
        public static extern int set_attribute_ullong(long location, [MarshalAs(UnmanagedType.LPUTF8Str)] string objectName, [MarshalAs(UnmanagedType.LPUTF8Str)] string attributeName, IntPtr buffer, UIntPtr count);

        /// <summary>Reads an attribute as native ullong values.</summary>
        /// <remarks>Query dimensions first and allocate enough native elements; this API has no capacity parameter.</remarks>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5LTget_attribute_ullong", CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_attribute_ullong(long location, [MarshalAs(UnmanagedType.LPUTF8Str)] string objectName, [MarshalAs(UnmanagedType.LPUTF8Str)] string attributeName, IntPtr buffer);

        /// <summary>Creates or replaces an attribute from native float values.</summary>
        /// <remarks>Count is the number of elements, not bytes; caller owns the input buffer.</remarks>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5LTset_attribute_float", CallingConvention = CallingConvention.Cdecl)]
        public static extern int set_attribute_float(long location, [MarshalAs(UnmanagedType.LPUTF8Str)] string objectName, [MarshalAs(UnmanagedType.LPUTF8Str)] string attributeName, IntPtr buffer, UIntPtr count);

        /// <summary>Reads an attribute as native float values.</summary>
        /// <remarks>Query dimensions first and allocate enough native elements; this API has no capacity parameter.</remarks>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5LTget_attribute_float", CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_attribute_float(long location, [MarshalAs(UnmanagedType.LPUTF8Str)] string objectName, [MarshalAs(UnmanagedType.LPUTF8Str)] string attributeName, IntPtr buffer);

        /// <summary>Creates or replaces an attribute from native double values.</summary>
        /// <remarks>Count is the number of elements, not bytes; caller owns the input buffer.</remarks>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5LTset_attribute_double", CallingConvention = CallingConvention.Cdecl)]
        public static extern int set_attribute_double(long location, [MarshalAs(UnmanagedType.LPUTF8Str)] string objectName, [MarshalAs(UnmanagedType.LPUTF8Str)] string attributeName, IntPtr buffer, UIntPtr count);

        /// <summary>Reads an attribute as native double values.</summary>
        /// <remarks>Query dimensions first and allocate enough native elements; this API has no capacity parameter.</remarks>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5LTget_attribute_double", CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_attribute_double(long location, [MarshalAs(UnmanagedType.LPUTF8Str)] string objectName, [MarshalAs(UnmanagedType.LPUTF8Str)] string attributeName, IntPtr buffer);

    }
}


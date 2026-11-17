using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace HDF.PInvoke
{
    /// <summary>Provides native high-level dataset and attribute convenience operations.</summary>
    public unsafe sealed partial class H5LT
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
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5LTmake_dataset", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int make_dataset(long location, string name, int rank, IntPtr dimensions, long type, IntPtr buffer);

        /// <summary>Creates a scalar string dataset from a UTF-8 input string.</summary>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5LTmake_dataset_string", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int make_dataset_string(long location, string name, string value);

        /// <summary>Reads a dataset into caller-owned storage using the supplied memory datatype.</summary>
        /// <remarks>Query dimensions first and allocate enough bytes for every converted element.</remarks>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5LTread_dataset", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int read_dataset(long location, string name, long type, IntPtr buffer);

        /// <summary>Reads a string dataset into caller-owned storage including its terminator.</summary>
        /// <remarks>Query datatype size first; this native API does not accept a buffer capacity.</remarks>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5LTread_dataset_string", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int read_dataset_string(long location, string name, IntPtr buffer);

        /// <summary>Gets the rank of a dataset.</summary>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5LTget_dataset_ndims", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int get_dataset_ndims(long location, string name, out int rank);

        /// <summary>Gets dataset dimensions, datatype class and element size.</summary>
        /// <remarks>Dimensions must have space for the rank returned by get_dataset_ndims; pass null when dimensions are not needed.</remarks>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5LTget_dataset_info", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int get_dataset_info(long location, string name, IntPtr dimensions, out H5T.class_t typeClass, out UIntPtr typeSize);

        /// <summary>Returns positive if the dataset exists, zero if not, or negative on error.</summary>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5LTfind_dataset", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int find_dataset(long location, string name);

        /// <summary>Creates or replaces a string attribute using UTF-8 input.</summary>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5LTset_attribute_string", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int set_attribute_string(long location, string objectName, string attributeName, string value);

        /// <summary>Reads an attribute using the specified memory datatype.</summary>
        /// <remarks>Caller must allocate space for all converted elements, obtained from get_attribute_info.</remarks>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5LTget_attribute", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int get_attribute(long location, string objectName, string attributeName, long type, IntPtr buffer);

        /// <summary>Reads a string attribute into caller-owned storage including its terminator.</summary>
        /// <remarks>Query datatype size first; this native API does not accept a buffer capacity.</remarks>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5LTget_attribute_string", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int get_attribute_string(long location, string objectName, string attributeName, IntPtr buffer);

        /// <summary>Gets the rank of an attribute.</summary>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5LTget_attribute_ndims", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int get_attribute_ndims(long location, string objectName, string attributeName, out int rank);

        /// <summary>Gets attribute dimensions, datatype class and element size.</summary>
        /// <remarks>Dimensions must have room for the rank reported by get_attribute_ndims; null omits dimensions.</remarks>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5LTget_attribute_info", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int get_attribute_info(long location, string objectName, string attributeName, IntPtr dimensions, out H5T.class_t typeClass, out UIntPtr typeSize);

        /// <summary>Parses a textual datatype description; close the returned datatype with H5T.close.</summary>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5LTtext_to_dtype", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial long text_to_dtype(string text, lang_t language);

        /// <summary>Formats a datatype description; pass a null buffer to query required storage.</summary>
        /// <remarks>Size supplies byte capacity and receives required storage including the terminating null.</remarks>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5LTdtype_to_text"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int dtype_to_text(long type, IntPtr buffer, lang_t language, ref UIntPtr size);

        /// <summary>Returns positive if an attribute exists, zero if absent, or negative on error.</summary>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5LTfind_attribute", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int find_attribute(long objectId, string name);

        /// <summary>Checks a path and optionally its final object using a one-byte boolean.</summary>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5LTpath_valid", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int path_valid(long location, string path, byte checkObject);

        /// <summary>Opens an in-memory HDF5 file image; close the file with H5F.close.</summary>
        /// <remarks>With no flags, HDF5 copies the image. DONT_COPY borrows the original buffer until file close; combine DONT_RELEASE to retain caller ownership.</remarks>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5LTopen_file_image"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial long open_file_image(IntPtr buffer, UIntPtr size, uint flags);

        /// <summary>Creates a dataset from native char values.</summary>
        /// <remarks>Dimensions contains rank unsigned 64-bit values; buffer must hold their product in native elements.</remarks>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5LTmake_dataset_char", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int make_dataset_char(long location, string name, int rank, IntPtr dimensions, IntPtr buffer);

        /// <summary>Reads a dataset as native char values.</summary>
        /// <remarks>Caller must allocate enough native elements for the dataset dimensions.</remarks>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5LTread_dataset_char", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int read_dataset_char(long location, string name, IntPtr buffer);

        /// <summary>Creates a dataset from native short values.</summary>
        /// <remarks>Dimensions contains rank unsigned 64-bit values; buffer must hold their product in native elements.</remarks>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5LTmake_dataset_short", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int make_dataset_short(long location, string name, int rank, IntPtr dimensions, IntPtr buffer);

        /// <summary>Reads a dataset as native short values.</summary>
        /// <remarks>Caller must allocate enough native elements for the dataset dimensions.</remarks>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5LTread_dataset_short", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int read_dataset_short(long location, string name, IntPtr buffer);

        /// <summary>Creates a dataset from native int values.</summary>
        /// <remarks>Dimensions contains rank unsigned 64-bit values; buffer must hold their product in native elements.</remarks>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5LTmake_dataset_int", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int make_dataset_int(long location, string name, int rank, IntPtr dimensions, IntPtr buffer);

        /// <summary>Reads a dataset as native int values.</summary>
        /// <remarks>Caller must allocate enough native elements for the dataset dimensions.</remarks>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5LTread_dataset_int", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int read_dataset_int(long location, string name, IntPtr buffer);

        /// <summary>Creates a dataset from native C long (four bytes on Windows, eight on Unix) values.</summary>
        /// <remarks>Dimensions contains rank unsigned 64-bit values; buffer must hold their product in native elements.</remarks>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5LTmake_dataset_long", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int make_dataset_long(long location, string name, int rank, IntPtr dimensions, IntPtr buffer);

        /// <summary>Reads a dataset as native C long (four bytes on Windows, eight on Unix) values.</summary>
        /// <remarks>Caller must allocate enough native elements for the dataset dimensions.</remarks>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5LTread_dataset_long", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int read_dataset_long(long location, string name, IntPtr buffer);

        /// <summary>Creates a dataset from native float values.</summary>
        /// <remarks>Dimensions contains rank unsigned 64-bit values; buffer must hold their product in native elements.</remarks>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5LTmake_dataset_float", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int make_dataset_float(long location, string name, int rank, IntPtr dimensions, IntPtr buffer);

        /// <summary>Reads a dataset as native float values.</summary>
        /// <remarks>Caller must allocate enough native elements for the dataset dimensions.</remarks>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5LTread_dataset_float", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int read_dataset_float(long location, string name, IntPtr buffer);

        /// <summary>Creates a dataset from native double values.</summary>
        /// <remarks>Dimensions contains rank unsigned 64-bit values; buffer must hold their product in native elements.</remarks>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5LTmake_dataset_double", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int make_dataset_double(long location, string name, int rank, IntPtr dimensions, IntPtr buffer);

        /// <summary>Reads a dataset as native double values.</summary>
        /// <remarks>Caller must allocate enough native elements for the dataset dimensions.</remarks>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5LTread_dataset_double", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int read_dataset_double(long location, string name, IntPtr buffer);

        /// <summary>Creates or replaces an attribute from native char values.</summary>
        /// <remarks>Count is the number of elements, not bytes; caller owns the input buffer.</remarks>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5LTset_attribute_char", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int set_attribute_char(long location, string objectName, string attributeName, IntPtr buffer, UIntPtr count);

        /// <summary>Reads an attribute as native char values.</summary>
        /// <remarks>Query dimensions first and allocate enough native elements; this API has no capacity parameter.</remarks>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5LTget_attribute_char", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int get_attribute_char(long location, string objectName, string attributeName, IntPtr buffer);

        /// <summary>Creates or replaces an attribute from native uchar values.</summary>
        /// <remarks>Count is the number of elements, not bytes; caller owns the input buffer.</remarks>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5LTset_attribute_uchar", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int set_attribute_uchar(long location, string objectName, string attributeName, IntPtr buffer, UIntPtr count);

        /// <summary>Reads an attribute as native uchar values.</summary>
        /// <remarks>Query dimensions first and allocate enough native elements; this API has no capacity parameter.</remarks>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5LTget_attribute_uchar", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int get_attribute_uchar(long location, string objectName, string attributeName, IntPtr buffer);

        /// <summary>Creates or replaces an attribute from native short values.</summary>
        /// <remarks>Count is the number of elements, not bytes; caller owns the input buffer.</remarks>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5LTset_attribute_short", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int set_attribute_short(long location, string objectName, string attributeName, IntPtr buffer, UIntPtr count);

        /// <summary>Reads an attribute as native short values.</summary>
        /// <remarks>Query dimensions first and allocate enough native elements; this API has no capacity parameter.</remarks>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5LTget_attribute_short", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int get_attribute_short(long location, string objectName, string attributeName, IntPtr buffer);

        /// <summary>Creates or replaces an attribute from native ushort values.</summary>
        /// <remarks>Count is the number of elements, not bytes; caller owns the input buffer.</remarks>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5LTset_attribute_ushort", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int set_attribute_ushort(long location, string objectName, string attributeName, IntPtr buffer, UIntPtr count);

        /// <summary>Reads an attribute as native ushort values.</summary>
        /// <remarks>Query dimensions first and allocate enough native elements; this API has no capacity parameter.</remarks>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5LTget_attribute_ushort", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int get_attribute_ushort(long location, string objectName, string attributeName, IntPtr buffer);

        /// <summary>Creates or replaces an attribute from native int values.</summary>
        /// <remarks>Count is the number of elements, not bytes; caller owns the input buffer.</remarks>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5LTset_attribute_int", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int set_attribute_int(long location, string objectName, string attributeName, IntPtr buffer, UIntPtr count);

        /// <summary>Reads an attribute as native int values.</summary>
        /// <remarks>Query dimensions first and allocate enough native elements; this API has no capacity parameter.</remarks>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5LTget_attribute_int", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int get_attribute_int(long location, string objectName, string attributeName, IntPtr buffer);

        /// <summary>Creates or replaces an attribute from native uint values.</summary>
        /// <remarks>Count is the number of elements, not bytes; caller owns the input buffer.</remarks>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5LTset_attribute_uint", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int set_attribute_uint(long location, string objectName, string attributeName, IntPtr buffer, UIntPtr count);

        /// <summary>Reads an attribute as native uint values.</summary>
        /// <remarks>Query dimensions first and allocate enough native elements; this API has no capacity parameter.</remarks>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5LTget_attribute_uint", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int get_attribute_uint(long location, string objectName, string attributeName, IntPtr buffer);

        /// <summary>Creates or replaces an attribute from native C long (four bytes on Windows, eight on Unix) values.</summary>
        /// <remarks>Count is the number of elements, not bytes; caller owns the input buffer.</remarks>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5LTset_attribute_long", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int set_attribute_long(long location, string objectName, string attributeName, IntPtr buffer, UIntPtr count);

        /// <summary>Reads an attribute as native C long (four bytes on Windows, eight on Unix) values.</summary>
        /// <remarks>Query dimensions first and allocate enough native elements; this API has no capacity parameter.</remarks>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5LTget_attribute_long", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int get_attribute_long(long location, string objectName, string attributeName, IntPtr buffer);

        /// <summary>Creates or replaces an attribute from native long_long values.</summary>
        /// <remarks>Count is the number of elements, not bytes; caller owns the input buffer.</remarks>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5LTset_attribute_long_long", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int set_attribute_long_long(long location, string objectName, string attributeName, IntPtr buffer, UIntPtr count);

        /// <summary>Reads an attribute as native long_long values.</summary>
        /// <remarks>Query dimensions first and allocate enough native elements; this API has no capacity parameter.</remarks>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5LTget_attribute_long_long", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int get_attribute_long_long(long location, string objectName, string attributeName, IntPtr buffer);

        /// <summary>Creates or replaces an attribute from native C ulong (four bytes on Windows, eight on Unix) values.</summary>
        /// <remarks>Count is the number of elements, not bytes; caller owns the input buffer.</remarks>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5LTset_attribute_ulong", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int set_attribute_ulong(long location, string objectName, string attributeName, IntPtr buffer, UIntPtr count);

        /// <summary>Reads an attribute as native C ulong (four bytes on Windows, eight on Unix) values.</summary>
        /// <remarks>Query dimensions first and allocate enough native elements; this API has no capacity parameter.</remarks>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5LTget_attribute_ulong", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int get_attribute_ulong(long location, string objectName, string attributeName, IntPtr buffer);

        /// <summary>Creates or replaces an attribute from native ullong values.</summary>
        /// <remarks>Count is the number of elements, not bytes; caller owns the input buffer.</remarks>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5LTset_attribute_ullong", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int set_attribute_ullong(long location, string objectName, string attributeName, IntPtr buffer, UIntPtr count);

        /// <summary>Reads an attribute as native ullong values.</summary>
        /// <remarks>Query dimensions first and allocate enough native elements; this API has no capacity parameter.</remarks>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5LTget_attribute_ullong", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int get_attribute_ullong(long location, string objectName, string attributeName, IntPtr buffer);

        /// <summary>Creates or replaces an attribute from native float values.</summary>
        /// <remarks>Count is the number of elements, not bytes; caller owns the input buffer.</remarks>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5LTset_attribute_float", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int set_attribute_float(long location, string objectName, string attributeName, IntPtr buffer, UIntPtr count);

        /// <summary>Reads an attribute as native float values.</summary>
        /// <remarks>Query dimensions first and allocate enough native elements; this API has no capacity parameter.</remarks>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5LTget_attribute_float", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int get_attribute_float(long location, string objectName, string attributeName, IntPtr buffer);

        /// <summary>Creates or replaces an attribute from native double values.</summary>
        /// <remarks>Count is the number of elements, not bytes; caller owns the input buffer.</remarks>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5LTset_attribute_double", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int set_attribute_double(long location, string objectName, string attributeName, IntPtr buffer, UIntPtr count);

        /// <summary>Reads an attribute as native double values.</summary>
        /// <remarks>Query dimensions first and allocate enough native elements; this API has no capacity parameter.</remarks>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5LTget_attribute_double", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int get_attribute_double(long location, string objectName, string attributeName, IntPtr buffer);

    }
}


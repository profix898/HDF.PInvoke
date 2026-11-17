using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace HDF.PInvoke
{
    /// <summary>Provides native H5TB high-level operations.</summary>
    public unsafe sealed partial class H5TB
    {
        /// <summary>Initializes native HDF5 before the first high-level call.</summary>
        static H5TB()
        {
            H5.open();
        }

        /// <summary>Creates a compound table with native field-name, offset and datatype arrays containing nfields entries.</summary>
        /// <remarks>Offsets and sizes use native size_t elements. Data buffers must cover the requested record count and record size. String-pointer arrays contain null-terminated UTF-8 strings; output string buffers must be sized from the corresponding datatype/attribute metadata before calling.</remarks>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5TBmake_table", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int make_table(string table_title, long loc_id, string dset_name, ulong nfields, ulong nrecords, UIntPtr type_size, IntPtr field_names, IntPtr field_offset, IntPtr field_types, ulong chunk_size, IntPtr fill_data, int compress, IntPtr buf);

        /// <summary>Appends records using caller-defined native record size and field offsets.</summary>
        /// <remarks>Offsets and sizes use native size_t elements. Data buffers must cover the requested record count and record size. String-pointer arrays contain null-terminated UTF-8 strings; output string buffers must be sized from the corresponding datatype/attribute metadata before calling.</remarks>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5TBappend_records", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int append_records(long loc_id, string dset_name, ulong nrecords, UIntPtr type_size, IntPtr field_offset, IntPtr dst_sizes, IntPtr buf);

        /// <summary>Writes a range of records using caller-defined native record layout.</summary>
        /// <remarks>Offsets and sizes use native size_t elements. Data buffers must cover the requested record count and record size. String-pointer arrays contain null-terminated UTF-8 strings; output string buffers must be sized from the corresponding datatype/attribute metadata before calling.</remarks>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5TBwrite_records", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int write_records(long loc_id, string dset_name, ulong start, ulong nrecords, UIntPtr type_size, IntPtr field_offset, IntPtr dst_sizes, IntPtr buf);

        /// <summary>Writes comma-separated named fields from caller-owned records.</summary>
        /// <remarks>Offsets and sizes use native size_t elements. Data buffers must cover the requested record count and record size. String-pointer arrays contain null-terminated UTF-8 strings; output string buffers must be sized from the corresponding datatype/attribute metadata before calling.</remarks>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5TBwrite_fields_name", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int write_fields_name(long loc_id, string dset_name, string field_names, ulong start, ulong nrecords, UIntPtr type_size, IntPtr field_offset, IntPtr dst_sizes, IntPtr buf);

        /// <summary>Writes indexed fields from caller-owned records; field_index contains nfields native int values.</summary>
        /// <remarks>Offsets and sizes use native size_t elements. Data buffers must cover the requested record count and record size. String-pointer arrays contain null-terminated UTF-8 strings; output string buffers must be sized from the corresponding datatype/attribute metadata before calling.</remarks>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5TBwrite_fields_index", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int write_fields_index(long loc_id, string dset_name, ulong nfields, IntPtr field_index, ulong start, ulong nrecords, UIntPtr type_size, IntPtr field_offset, IntPtr dst_sizes, IntPtr buf);

        /// <summary>Reads all records into caller-owned storage with the supplied native record layout.</summary>
        /// <remarks>Offsets and sizes use native size_t elements. Data buffers must cover the requested record count and record size. String-pointer arrays contain null-terminated UTF-8 strings; output string buffers must be sized from the corresponding datatype/attribute metadata before calling.</remarks>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5TBread_table", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int read_table(long loc_id, string dset_name, UIntPtr dst_size, IntPtr dst_offset, IntPtr dst_sizes, IntPtr dst_buf);

        /// <summary>Reads comma-separated named fields into caller-owned records.</summary>
        /// <remarks>Offsets and sizes use native size_t elements. Data buffers must cover the requested record count and record size. String-pointer arrays contain null-terminated UTF-8 strings; output string buffers must be sized from the corresponding datatype/attribute metadata before calling.</remarks>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5TBread_fields_name", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int read_fields_name(long loc_id, string dset_name, string field_names, ulong start, ulong nrecords, UIntPtr type_size, IntPtr field_offset, IntPtr dst_sizes, IntPtr buf);

        /// <summary>Reads indexed fields into caller-owned records; field_index contains nfields native int values.</summary>
        /// <remarks>Offsets and sizes use native size_t elements. Data buffers must cover the requested record count and record size. String-pointer arrays contain null-terminated UTF-8 strings; output string buffers must be sized from the corresponding datatype/attribute metadata before calling.</remarks>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5TBread_fields_index", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int read_fields_index(long loc_id, string dset_name, ulong nfields, IntPtr field_index, ulong start, ulong nrecords, UIntPtr type_size, IntPtr field_offset, IntPtr dst_sizes, IntPtr buf);

        /// <summary>Reads a record range into caller-owned storage with the supplied record layout.</summary>
        /// <remarks>Offsets and sizes use native size_t elements. Data buffers must cover the requested record count and record size. String-pointer arrays contain null-terminated UTF-8 strings; output string buffers must be sized from the corresponding datatype/attribute metadata before calling.</remarks>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5TBread_records", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int read_records(long loc_id, string dset_name, ulong start, ulong nrecords, UIntPtr type_size, IntPtr dst_offset, IntPtr dst_sizes, IntPtr buf);

        /// <summary>Gets a table's field and record counts.</summary>
        /// <remarks>Offsets and sizes use native size_t elements. Data buffers must cover the requested record count and record size. String-pointer arrays contain null-terminated UTF-8 strings; output string buffers must be sized from the corresponding datatype/attribute metadata before calling.</remarks>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5TBget_table_info", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int get_table_info(long loc_id, string dset_name, out ulong nfields, out ulong nrecords);

        /// <summary>Gets native field sizes, offsets, record size and optionally field names.</summary>
        /// <remarks>Offsets and sizes use native size_t elements. Data buffers must cover the requested record count and record size. String-pointer arrays contain null-terminated UTF-8 strings; output string buffers must be sized from the corresponding datatype/attribute metadata before calling.</remarks>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5TBget_field_info", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int get_field_info(long loc_id, string dset_name, IntPtr field_names, IntPtr field_sizes, IntPtr field_offsets, out UIntPtr type_size);

        /// <summary>Deletes a range of table records.</summary>
        /// <remarks>Offsets and sizes use native size_t elements. Data buffers must cover the requested record count and record size. String-pointer arrays contain null-terminated UTF-8 strings; output string buffers must be sized from the corresponding datatype/attribute metadata before calling.</remarks>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5TBdelete_record", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int delete_record(long loc_id, string dset_name, ulong start, ulong nrecords);

        /// <summary>Inserts records at the specified position.</summary>
        /// <remarks>Offsets and sizes use native size_t elements. Data buffers must cover the requested record count and record size. String-pointer arrays contain null-terminated UTF-8 strings; output string buffers must be sized from the corresponding datatype/attribute metadata before calling.</remarks>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5TBinsert_record", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int insert_record(long loc_id, string dset_name, ulong start, ulong nrecords, UIntPtr dst_size, IntPtr dst_offset, IntPtr dst_sizes, IntPtr buf);

        /// <summary>Copies a record range between tables with compatible schemas.</summary>
        /// <remarks>Offsets and sizes use native size_t elements. Data buffers must cover the requested record count and record size. String-pointer arrays contain null-terminated UTF-8 strings; output string buffers must be sized from the corresponding datatype/attribute metadata before calling.</remarks>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5TBadd_records_from", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int add_records_from(long loc_id, string dset_name1, ulong start1, ulong nrecords, string dset_name2, ulong start2);

        /// <summary>Creates a new table by concatenating two compatible tables.</summary>
        /// <remarks>Offsets and sizes use native size_t elements. Data buffers must cover the requested record count and record size. String-pointer arrays contain null-terminated UTF-8 strings; output string buffers must be sized from the corresponding datatype/attribute metadata before calling.</remarks>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5TBcombine_tables", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int combine_tables(long loc_id1, string dset_name1, long loc_id2, string dset_name2, string dset_name3);

        /// <summary>Inserts a field with optional fill and per-record data buffers.</summary>
        /// <remarks>Offsets and sizes use native size_t elements. Data buffers must cover the requested record count and record size. String-pointer arrays contain null-terminated UTF-8 strings; output string buffers must be sized from the corresponding datatype/attribute metadata before calling.</remarks>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5TBinsert_field", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int insert_field(long loc_id, string dset_name, string field_name, long field_type, ulong position, IntPtr fill_data, IntPtr buf);

        /// <summary>Removes a named field from the table.</summary>
        /// <remarks>Offsets and sizes use native size_t elements. Data buffers must cover the requested record count and record size. String-pointer arrays contain null-terminated UTF-8 strings; output string buffers must be sized from the corresponding datatype/attribute metadata before calling.</remarks>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5TBdelete_field", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int delete_field(long loc_id, string dset_name, string field_name);

        /// <summary>Reads a table title from an open dataset identifier into caller-owned storage.</summary>
        /// <remarks>Offsets and sizes use native size_t elements. Data buffers must cover the requested record count and record size. String-pointer arrays contain null-terminated UTF-8 strings; output string buffers must be sized from the corresponding datatype/attribute metadata before calling.</remarks>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5TBAget_title"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int Aget_title(long loc_id, IntPtr table_title);

        /// <summary>Reads table fill values into caller-owned native record storage.</summary>
        /// <remarks>Offsets and sizes use native size_t elements. Data buffers must cover the requested record count and record size. String-pointer arrays contain null-terminated UTF-8 strings; output string buffers must be sized from the corresponding datatype/attribute metadata before calling.</remarks>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5TBAget_fill", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int Aget_fill(long loc_id, string dset_name, long dset_id, IntPtr dst_buf);

    }
}

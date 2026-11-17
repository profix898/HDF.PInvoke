using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace HDF.PInvoke
{
    /// <summary>Provides native H5PT high-level operations.</summary>
    public unsafe sealed partial class H5PT
    {
        /// <summary>Initializes native HDF5 before the first high-level call.</summary>
        static H5PT()
        {
            H5.open();
        }

        /// <summary>Creates a packet table using dataset-creation properties; release it with H5PT.close.</summary>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5PTcreate", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial long create(long loc_id, string dset_name, long dtype_id, ulong chunk_size, long plist_id);

        /// <summary>Opens a packet table; release it with H5PT.close.</summary>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5PTopen", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial long open(long loc_id, string dset_name);

        /// <summary>Closes a packet table and its owned dataset and datatype identifiers.</summary>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5PTclose"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int close(long table_id);

        /// <summary>Creates a fixed-length packet table with the requested chunk size and compression level.</summary>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5PTcreate_fl", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial long create_fl(long loc_id, string dset_name, long dtype_id, ulong chunk_size, int compression);

        /// <summary>Appends nrecords packets from caller-owned storage.</summary>
        /// <remarks>Allocate at least the packet count times the memory datatype size. Variable-length read results require free_vlen_buff before releasing the buffer.</remarks>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5PTappend"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int append(long table_id, UIntPtr nrecords, IntPtr data);

        /// <summary>Reads nrecords packets at the current index and advances it.</summary>
        /// <remarks>Allocate at least the packet count times the memory datatype size. Variable-length read results require free_vlen_buff before releasing the buffer.</remarks>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5PTget_next"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int get_next(long table_id, UIntPtr nrecords, IntPtr data);

        /// <summary>Reads nrecords packets starting at the specified record index.</summary>
        /// <remarks>Allocate at least the packet count times the memory datatype size. Variable-length read results require free_vlen_buff before releasing the buffer.</remarks>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5PTread_packets"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int read_packets(long table_id, ulong start, UIntPtr nrecords, IntPtr data);

        /// <summary>Gets the packet count.</summary>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5PTget_num_packets"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int get_num_packets(long table_id, out ulong nrecords);

        /// <summary>Returns nonnegative for a valid packet-table identifier and negative otherwise.</summary>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5PTis_valid"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int is_valid(long table_id);

        /// <summary>Returns positive for variable-length packets, zero for fixed-length packets, or negative on error.</summary>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5PTis_varlen"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int is_varlen(long table_id);

        /// <summary>Gets the borrowed dataset identifier owned by the packet table; do not close it separately.</summary>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5PTget_dataset"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial long get_dataset(long table_id);

        /// <summary>Gets the borrowed datatype identifier owned by the packet table; do not close it separately.</summary>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5PTget_type"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial long get_type(long table_id);

        /// <summary>Resets a packet table's current index to its first packet.</summary>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5PTcreate_index"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int create_index(long table_id);

        /// <summary>Sets the packet table's current index.</summary>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5PTset_index"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int set_index(long table_id, ulong pt_index);

        /// <summary>Gets the packet table's current index.</summary>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5PTget_index"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int get_index(long table_id, out ulong pt_index);

        /// <summary>Reclaims variable-length data for bufflen packets returned by a read; caller retains the buffer storage.</summary>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5PTfree_vlen_buff"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int free_vlen_buff(long table_id, UIntPtr bufflen, IntPtr buff);

    }
}

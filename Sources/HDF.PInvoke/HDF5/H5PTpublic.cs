using System;
using System.Runtime.InteropServices;

namespace HDF.PInvoke
{
    /// <summary>Provides native H5PT high-level operations.</summary>
    public unsafe sealed class H5PT
    {
        /// <summary>Initializes native HDF5 before the first high-level call.</summary>
        static H5PT() { H5.open(); }

        /// <summary>Creates a packet table using dataset-creation properties; release it with H5PT.close.</summary>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5PTcreate", CallingConvention = CallingConvention.Cdecl)]
        public static extern long create(long loc_id, [MarshalAs(UnmanagedType.LPUTF8Str)] string dset_name, long dtype_id, ulong chunk_size, long plist_id);

        /// <summary>Opens a packet table; release it with H5PT.close.</summary>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5PTopen", CallingConvention = CallingConvention.Cdecl)]
        public static extern long open(long loc_id, [MarshalAs(UnmanagedType.LPUTF8Str)] string dset_name);

        /// <summary>Closes a packet table and its owned dataset and datatype identifiers.</summary>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5PTclose", CallingConvention = CallingConvention.Cdecl)]
        public static extern int close(long table_id);

        /// <summary>Creates a fixed-length packet table with the requested chunk size and compression level.</summary>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5PTcreate_fl", CallingConvention = CallingConvention.Cdecl)]
        public static extern long create_fl(long loc_id, [MarshalAs(UnmanagedType.LPUTF8Str)] string dset_name, long dtype_id, ulong chunk_size, int compression);

        /// <summary>Appends nrecords packets from caller-owned storage.</summary>
        /// <remarks>Allocate at least the packet count times the memory datatype size. Variable-length read results require free_vlen_buff before releasing the buffer.</remarks>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5PTappend", CallingConvention = CallingConvention.Cdecl)]
        public static extern int append(long table_id, UIntPtr nrecords, IntPtr data);

        /// <summary>Reads nrecords packets at the current index and advances it.</summary>
        /// <remarks>Allocate at least the packet count times the memory datatype size. Variable-length read results require free_vlen_buff before releasing the buffer.</remarks>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5PTget_next", CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_next(long table_id, UIntPtr nrecords, IntPtr data);

        /// <summary>Reads nrecords packets starting at the specified record index.</summary>
        /// <remarks>Allocate at least the packet count times the memory datatype size. Variable-length read results require free_vlen_buff before releasing the buffer.</remarks>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5PTread_packets", CallingConvention = CallingConvention.Cdecl)]
        public static extern int read_packets(long table_id, ulong start, UIntPtr nrecords, IntPtr data);

        /// <summary>Gets the packet count.</summary>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5PTget_num_packets", CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_num_packets(long table_id, out ulong nrecords);

        /// <summary>Returns nonnegative for a valid packet-table identifier and negative otherwise.</summary>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5PTis_valid", CallingConvention = CallingConvention.Cdecl)]
        public static extern int is_valid(long table_id);

        /// <summary>Returns positive for variable-length packets, zero for fixed-length packets, or negative on error.</summary>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5PTis_varlen", CallingConvention = CallingConvention.Cdecl)]
        public static extern int is_varlen(long table_id);

        /// <summary>Gets the borrowed dataset identifier owned by the packet table; do not close it separately.</summary>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5PTget_dataset", CallingConvention = CallingConvention.Cdecl)]
        public static extern long get_dataset(long table_id);

        /// <summary>Gets the borrowed datatype identifier owned by the packet table; do not close it separately.</summary>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5PTget_type", CallingConvention = CallingConvention.Cdecl)]
        public static extern long get_type(long table_id);

        /// <summary>Resets a packet table's current index to its first packet.</summary>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5PTcreate_index", CallingConvention = CallingConvention.Cdecl)]
        public static extern int create_index(long table_id);

        /// <summary>Sets the packet table's current index.</summary>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5PTset_index", CallingConvention = CallingConvention.Cdecl)]
        public static extern int set_index(long table_id, ulong pt_index);

        /// <summary>Gets the packet table's current index.</summary>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5PTget_index", CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_index(long table_id, out ulong pt_index);

        /// <summary>Reclaims variable-length data for bufflen packets returned by a read; caller retains the buffer storage.</summary>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5PTfree_vlen_buff", CallingConvention = CallingConvention.Cdecl)]
        public static extern int free_vlen_buff(long table_id, UIntPtr bufflen, IntPtr buff);

    }
}

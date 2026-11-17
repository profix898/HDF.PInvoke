using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace HDF.PInvoke
{
    /// <summary>Provides advanced serial H5S operations.</summary>
    public unsafe sealed partial class H5S
    {
        /// <summary>Requests selection sequences sorted by offset.</summary>
        public const uint SEL_ITER_GET_SEQ_LIST_SORTED = 1;

        /// <summary>Shares the dataspace selection; keep the dataspace open and unchanged while iterating.</summary>
        public const uint SEL_ITER_SHARE_WITH_DATASPACE = 2;

        /// <summary>Returns a new dataspace combining a hyperslab and the existing selection; close it with H5S.close.</summary>
        /// <remarks>Coordinate buffers contain one unsigned 64-bit value per dimension. Stride and block may be null for native defaults.</remarks>
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Scombine_hyperslab"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial long combine_hyperslab(long space, seloper_t operation, IntPtr start, IntPtr stride, IntPtr count, IntPtr block);

        /// <summary>Returns a new dataspace combining two selections; close it with H5S.close.</summary>
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Scombine_select"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial long combine_select(long first, seloper_t operation, long second);

        /// <summary>Combines the second selection into the first dataspace in place.</summary>
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Smodify_select"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int modify_select(long first, seloper_t operation, long second);

        /// <summary>Serializes a dataspace using file format bounds from the access property list.</summary>
        /// <remarks>Pass a null buffer to query size; otherwise size supplies capacity. Caller owns the output buffer.</remarks>
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Sencode2"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int encode2(long space, IntPtr buffer, ref UIntPtr size, long access = H5P.DEFAULT);

        /// <summary>Creates a selection iterator; close it with sel_iter_close.</summary>
        /// <remarks>With SEL_ITER_SHARE_WITH_DATASPACE the dataspace must remain open and unmodified until iterator closure.</remarks>
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Ssel_iter_create"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial long sel_iter_create(long space, UIntPtr elementSize, uint flags = 0);

        /// <summary>Closes a selection iterator.</summary>
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Ssel_iter_close"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int sel_iter_close(long iterator);

        /// <summary>Resets a selection iterator to another dataspace selection.</summary>
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Ssel_iter_reset"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int sel_iter_reset(long iterator, long space);

        /// <summary>Retrieves selected byte sequences and advances the iterator.</summary>
        /// <remarks>Offsets must hold maximumSequences unsigned 64-bit values; lengths must hold maximumSequences native size_t values. Caller owns both buffers.</remarks>
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Ssel_iter_get_seq_list"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int sel_iter_get_seq_list(long iterator, UIntPtr maximumSequences, UIntPtr maximumElements, out UIntPtr sequenceCount, out UIntPtr elementCount, IntPtr offsets, IntPtr lengths);

        /// <summary>Subtracts signed 64-bit offsets from the selection coordinates.</summary>
        /// <remarks>Offsets must point to one signed 64-bit value per dimension.</remarks>
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Sselect_adjust"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int select_adjust(long space, IntPtr offsets);

        /// <summary>Copies the selection from one dataspace into another.</summary>
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Sselect_copy"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int select_copy(long destination, long source);

        /// <summary>Tests whether a selection intersects a block with inclusive endpoints.</summary>
        /// <remarks>Both buffers contain one unsigned 64-bit coordinate per dimension.</remarks>
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Sselect_intersect_block"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int select_intersect_block(long space, IntPtr start, IntPtr end);

        /// <summary>Projects the source/intersection overlap onto the destination selection, returning a dataspace to close.</summary>
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Sselect_project_intersection"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial long select_project_intersection(long source, long destination, long intersection);

        /// <summary>Returns positive for selections with equal shape, zero if different, or negative on error.</summary>
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Sselect_shape_same"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int select_shape_same(long first, long second);

    }
}


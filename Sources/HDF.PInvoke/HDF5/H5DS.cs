using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace HDF.PInvoke
{
    /// <summary>Provides native dimension-scale operations.</summary>
    public unsafe sealed partial class H5DS
    {
        /// <summary>Initializes native HDF5 before the first operation.</summary>
        static H5DS()
        {
            H5.open();
        }

        /// <summary>Receives a dataset, dimension, borrowed scale identifier and caller state; do not throw through native code.</summary>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int iterate_t(long dataset, uint dimension, long scale, IntPtr state);

        /// <summary>Gets whether dimension scales use modern reference values.</summary>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5DSwith_new_ref"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int with_new_ref(long objectId, out byte modernReferences);

        /// <summary>Attaches a scale dataset to a dataset dimension.</summary>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5DSattach_scale"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int attach_scale(long dataset, long scale, uint dimension);

        /// <summary>Detaches a scale from a dataset dimension without closing either dataset.</summary>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5DSdetach_scale"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int detach_scale(long dataset, long scale, uint dimension);

        /// <summary>Marks a dataset as a dimension scale with the supplied UTF-8 name.</summary>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5DSset_scale", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int set_scale(long scale, string name);

        /// <summary>Gets the number of scales attached to a dimension, or a negative value on error.</summary>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5DSget_num_scales"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int get_num_scales(long dataset, uint dimension);

        /// <summary>Sets a dimension's UTF-8 label.</summary>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5DSset_label", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int set_label(long dataset, uint dimension, string label);

        /// <summary>Gets a label's UTF-8 length excluding its terminator; a null buffer queries length.</summary>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5DSget_label"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial IntPtr get_label(long dataset, uint dimension, IntPtr buffer, UIntPtr capacity);

        /// <summary>Gets a scale name's UTF-8 length excluding its terminator; a null buffer queries length.</summary>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5DSget_scale_name"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial IntPtr get_scale_name(long scale, IntPtr buffer, UIntPtr capacity);

        /// <summary>Returns positive if the dataset is a dimension scale, zero if not, or negative on error.</summary>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5DSis_scale"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int is_scale(long dataset);

        /// <summary>Synchronously visits scales, borrowing callback/state until return.</summary>
        /// <remarks>Return zero to continue, positive to stop, or negative on error. The callback must not throw through native code.</remarks>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5DSiterate_scales"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int iterate_scales(long dataset, uint dimension, ref int index, iterate_t callback, IntPtr state);

        /// <summary>Tests whether a scale is attached to a dataset dimension.</summary>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5DSis_attached"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int is_attached(long dataset, long scale, uint dimension);

    }
}


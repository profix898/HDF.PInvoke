using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace HDF.PInvoke
{
    /// <summary>Provides current HDF5 reference and token-aware H5R operations.</summary>
    public unsafe sealed partial class H5R
    {
        /// <summary>Owns opaque native reference resources in 64 bytes of eight-byte-aligned storage.</summary>
        /// <remarks>Initialize through create or copy APIs. Use H5R.copy rather than struct assignment to duplicate ownership, and H5R.destroy for every successfully created reference. Output destinations must not already own resources: destroy an existing reference before replacing it through an out parameter.</remarks>
        [StructLayout(LayoutKind.Sequential, Size = 64)]
        public struct ref_t
        {
            /// <summary>Preserves native union size and alignment; applications must not interpret these bytes.</summary>
            private fixed ulong storage[8];
        }

        /// <summary>Creates an owning object reference; release it with H5R.destroy.</summary>
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Rcreate_object", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int create_object(long location, string name, long access, out ref_t reference);

        /// <summary>Creates an owning reference to a dataset selection; release it with H5R.destroy.</summary>
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Rcreate_region", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int create_region(long location, string name, long space, long access, out ref_t reference);

        /// <summary>Creates an owning attribute reference; release it with H5R.destroy.</summary>
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Rcreate_attr", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int create_attr(long location, string name, string attribute, long access, out ref_t reference);

        /// <summary>Releases resources owned by a reference. Do not use shallow copies after destruction.</summary>
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Rdestroy"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int destroy(ref ref_t reference);

        /// <summary>Returns the type of a modern reference.</summary>
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Rget_type"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial type_t get_type(ref ref_t reference);

        /// <summary>Returns positive if references identify the same target and selection, zero if unequal, or negative on error.</summary>
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Requal"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int equal(ref ref_t first, ref ref_t second);

        /// <summary>Creates an independently owned copy; destroy source and destination separately.</summary>
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Rcopy"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int copy(ref ref_t source, out ref_t destination);

        /// <summary>Opens the referenced object; close the returned identifier with H5O.close.</summary>
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Ropen_object"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial long open_object(ref ref_t reference, long referenceAccess = H5P.DEFAULT, long objectAccess = H5P.DEFAULT);

        /// <summary>Opens the referenced selection; close the returned dataspace with H5S.close.</summary>
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Ropen_region"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial long open_region(ref ref_t reference, long referenceAccess = H5P.DEFAULT, long objectAccess = H5P.DEFAULT);

        /// <summary>Opens the referenced attribute; close the returned identifier with H5A.close.</summary>
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Ropen_attr"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial long open_attr(ref ref_t reference, long referenceAccess = H5P.DEFAULT, long attributeAccess = H5P.DEFAULT);

        /// <summary>Gets the type of the object targeted by a modern reference.</summary>
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Rget_obj_type3"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int get_obj_type3(ref ref_t reference, long referenceAccess, out H5O.type_t type);

        /// <summary>Returns the UTF-8 filename length excluding the terminator; a null buffer queries length. Capacity includes terminator space.</summary>
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Rget_file_name"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial IntPtr get_file_name(ref ref_t reference, IntPtr buffer, UIntPtr size);

        /// <summary>Returns the UTF-8 object name length excluding the terminator; a null buffer queries length. Capacity includes terminator space.</summary>
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Rget_obj_name"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial IntPtr get_obj_name(ref ref_t reference, long referenceAccess, IntPtr buffer, UIntPtr size);

        /// <summary>Returns the UTF-8 attribute name length excluding the terminator; a null buffer queries length. Capacity includes terminator space.</summary>
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Rget_attr_name"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial IntPtr get_attr_name(ref ref_t reference, IntPtr buffer, UIntPtr size);

    }
}


using System;
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
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Rcreate_object", CallingConvention = CallingConvention.Cdecl)]
        public static extern int create_object(long location, [MarshalAs(UnmanagedType.LPUTF8Str)] string name, long access, out ref_t reference);

        /// <summary>Creates an owning reference to a dataset selection; release it with H5R.destroy.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Rcreate_region", CallingConvention = CallingConvention.Cdecl)]
        public static extern int create_region(long location, [MarshalAs(UnmanagedType.LPUTF8Str)] string name, long space, long access, out ref_t reference);

        /// <summary>Creates an owning attribute reference; release it with H5R.destroy.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Rcreate_attr", CallingConvention = CallingConvention.Cdecl)]
        public static extern int create_attr(long location, [MarshalAs(UnmanagedType.LPUTF8Str)] string name, [MarshalAs(UnmanagedType.LPUTF8Str)] string attribute, long access, out ref_t reference);

        /// <summary>Releases resources owned by a reference. Do not use shallow copies after destruction.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Rdestroy", CallingConvention = CallingConvention.Cdecl)]
        public static extern int destroy(ref ref_t reference);

        /// <summary>Returns the type of a modern reference.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Rget_type", CallingConvention = CallingConvention.Cdecl)]
        public static extern type_t get_type(ref ref_t reference);

        /// <summary>Returns positive if references identify the same target and selection, zero if unequal, or negative on error.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Requal", CallingConvention = CallingConvention.Cdecl)]
        public static extern int equal(ref ref_t first, ref ref_t second);

        /// <summary>Creates an independently owned copy; destroy source and destination separately.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Rcopy", CallingConvention = CallingConvention.Cdecl)]
        public static extern int copy(ref ref_t source, out ref_t destination);

        /// <summary>Opens the referenced object; close the returned identifier with H5O.close.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Ropen_object", CallingConvention = CallingConvention.Cdecl)]
        public static extern long open_object(ref ref_t reference, long referenceAccess = H5P.DEFAULT, long objectAccess = H5P.DEFAULT);

        /// <summary>Opens the referenced selection; close the returned dataspace with H5S.close.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Ropen_region", CallingConvention = CallingConvention.Cdecl)]
        public static extern long open_region(ref ref_t reference, long referenceAccess = H5P.DEFAULT, long objectAccess = H5P.DEFAULT);

        /// <summary>Opens the referenced attribute; close the returned identifier with H5A.close.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Ropen_attr", CallingConvention = CallingConvention.Cdecl)]
        public static extern long open_attr(ref ref_t reference, long referenceAccess = H5P.DEFAULT, long attributeAccess = H5P.DEFAULT);

        /// <summary>Gets the type of the object targeted by a modern reference.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Rget_obj_type3", CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_obj_type3(ref ref_t reference, long referenceAccess, out H5O.type_t type);

        /// <summary>Returns the UTF-8 filename length excluding the terminator; a null buffer queries length. Capacity includes terminator space.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Rget_file_name", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr get_file_name(ref ref_t reference, IntPtr buffer, UIntPtr size);

        /// <summary>Returns the UTF-8 object name length excluding the terminator; a null buffer queries length. Capacity includes terminator space.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Rget_obj_name", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr get_obj_name(ref ref_t reference, long referenceAccess, IntPtr buffer, UIntPtr size);

        /// <summary>Returns the UTF-8 attribute name length excluding the terminator; a null buffer queries length. Capacity includes terminator space.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Rget_attr_name", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr get_attr_name(ref ref_t reference, IntPtr buffer, UIntPtr size);

    }
}


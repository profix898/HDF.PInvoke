/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * Copyright by The HDF Group.                                               *
 * Copyright by the Board of Trustees of the University of Illinois.         *
 * All rights reserved.                                                      *
 *                                                                           *
 * This file is part of HDF.PInvoke. Copyright notices and redistribution
 * terms for the wrapper and its native dependencies are in LICENSE.txt
 * at the repository and package root.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using haddr_t = System.UInt64;
using herr_t = System.Int32;
using hid_t = System.Int64;
using hsize_t = System.UInt64;
using size_t = System.IntPtr;
using ssize_t = System.IntPtr;
using uint32_t = System.UInt32;

namespace HDF.PInvoke
{
    /// <summary>
    /// Provides native HDF5 object and dataset-region reference operations.
    /// </summary>
    public unsafe sealed partial class H5R
    {
        /// <summary>
        /// Initializes HDF5 before this API is first used.
        /// </summary>
        static H5R()
        {
            H5.open();
        }

        /// <summary>
        /// Reference types allowed.
        /// </summary>
        public enum type_t
        {
            /// <summary>
            /// invalid Reference Type
            /// </summary>
            BADTYPE = -1,

            /// <summary>
            /// Object reference
            /// </summary>
            OBJECT,

            /// <summary>
            /// Dataset Region Reference
            /// </summary>
            DATASET_REGION,

            /// <summary>
            /// highest type (Invalid as true type)
            /// </summary>
            MAXTYPE = 5,

            /// <summary>Legacy object reference, equivalent to OBJECT.</summary>
            OBJECT1 = OBJECT,
            /// <summary>Legacy region reference, equivalent to DATASET_REGION.</summary>
            DATASET_REGION1 = DATASET_REGION,
            /// <summary>Modern object reference stored in ref_t.</summary>
            OBJECT2 = 2,
            /// <summary>Modern region reference stored in ref_t.</summary>
            DATASET_REGION2 = 3,
            /// <summary>Modern attribute reference stored in ref_t.</summary>
            ATTR = 4
        }

        /// <summary>
        /// The buffer size in bytes required by a legacy object reference.
        /// </summary>
        public const int OBJ_REF_BUF_SIZE = sizeof(haddr_t);

        /// <summary>
        /// The buffer size in bytes required by a legacy dataset-region reference.
        /// </summary>
        public const int DSET_REG_REF_BUF_SIZE = sizeof(haddr_t) + 4;

        /// <summary>
        /// Creates a reference.
        /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5R.html#Reference-Create
        /// </summary>
        /// <param name="refer">Reference created by the function call.</param>
        /// <param name="loc_id">Location identifier used to locate the object
        /// being pointed to.</param>
        /// <param name="name">Name of object at location
        /// <paramref name="loc_id"/>.</param>
        /// <param name="ref_type">Type of reference.</param>
        /// <param name="space_id">Dataspace identifier with selection. Used
        /// only for dataset region references; pass as -1 if reference is an
        /// object reference.</param>
        /// <returns>Returns a non-negative value if successful; otherwise
        /// returns a negative value.</returns>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Rcreate",
            CallingConvention = CallingConvention.Cdecl),
        SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
        public static extern herr_t create
            (IntPtr refer, hid_t loc_id, byte[] name, type_t ref_type,
            hid_t space_id);

        /// <summary>
        /// Creates a reference.
        /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5R.html#Reference-Create
        /// </summary>
        /// <param name="refer">Reference created by the function call.</param>
        /// <param name="loc_id">Location identifier used to locate the object
        /// being pointed to.</param>
        /// <param name="name">Name of object at location
        /// <paramref name="loc_id"/>.</param>
        /// <param name="ref_type">Type of reference.</param>
        /// <param name="space_id">Dataspace identifier with selection. Used
        /// only for dataset region references; pass as -1 if reference is an
        /// object reference.</param>
        /// <returns>Returns a non-negative value if successful; otherwise
        /// returns a negative value.</returns>
        /// <remarks>String inputs use UTF-8. For encoded output, prefer the byte-buffer overload.</remarks>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Rcreate",
            CharSet = CharSet.Ansi,
            CallingConvention = CallingConvention.Cdecl),
        SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
        public static extern herr_t create
            (IntPtr refer, hid_t loc_id, [MarshalAs(UnmanagedType.LPUTF8Str)] string name, type_t ref_type,
            hid_t space_id);

        /// <summary>
        /// Opens the HDF5 object referenced.
        /// </summary>
        /// <param name="obj_id">Valid identifier for the file containing the
        /// referenced object or any object in that file.</param>
        /// <param name="oapl_id">The object-access property-list identifier.</param>
        /// <param name="ref_type">The reference type of
        /// <paramref name="refer"/>.</param>
        /// <param name="refer">Reference to open.</param>
        /// <returns>Returns identifier of referenced object if successful;
        /// otherwise returns a negative value.</returns>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Rdereference2",
            CallingConvention = CallingConvention.Cdecl),
        SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
        public static extern hid_t dereference
            (hid_t obj_id, hid_t oapl_id, type_t ref_type, IntPtr refer);

        /// <summary>
        /// Retrieves a name for a referenced object.
        /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5R.html#Reference-GetName
        /// </summary>
        /// <param name="loc_id">Identifier for the file containing the
        /// reference or for any object in that file.</param>
        /// <param name="ref_type">Type of reference.</param>
        /// <param name="refer">An object or dataset region reference.</param>
        /// <param name="name">A buffer to place the name of the referenced
        /// object or dataset region. If <code>NULL</code>, then this call will
        /// return the size in bytes of the name.</param>
        /// <param name="size">The size of the <paramref name="name"/>
        /// buffer.</param>
        /// <returns>Returns the length of the name if successful, returning 0
        /// (zero) if no name is associated with the identifier. Otherwise
        /// returns a negative value.</returns>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Rget_name",
            CallingConvention = CallingConvention.Cdecl),
        SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
        public static extern ssize_t get_name
            (hid_t loc_id, type_t ref_type, IntPtr refer, [In][Out] byte[] name,
            size_t size);

        /// <summary>
        /// Retrieves a name for a referenced object.
        /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5R.html#Reference-GetName
        /// </summary>
        /// <param name="loc_id">Identifier for the file containing the
        /// reference or for any object in that file.</param>
        /// <param name="ref_type">Type of reference.</param>
        /// <param name="refer">An object or dataset region reference.</param>
        /// <param name="name">A buffer to place the name of the referenced
        /// object or dataset region. If <code>NULL</code>, then this call will
        /// return the size in bytes of the name.</param>
        /// <param name="size">The size of the <paramref name="name"/>
        /// buffer.</param>
        /// <returns>Returns the length of the name if successful, returning 0
        /// (zero) if no name is associated with the identifier. Otherwise
        /// returns a negative value.</returns>
        /// <remarks>String inputs use UTF-8. For encoded output, prefer the byte-buffer overload.</remarks>
        public static ssize_t get_name(hid_t loc_id, type_t ref_type, IntPtr refer, StringBuilder name,
            size_t size)
        {
            var buffer = Utf8Buffer.Create(name, size);
            var result = get_name_utf8(loc_id, ref_type, refer, buffer, size);

            if (result.ToInt64() >= 0)
            {
                Utf8Buffer.CopyTo(buffer, name);
            }

            return result;
        }

        /// <summary>
        /// Invokes the native <c>H5Rget_name</c> entry point using its explicit ABI declaration.
        /// </summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Rget_name", CallingConvention = CallingConvention.Cdecl)]
        private static extern ssize_t get_name_utf8(hid_t loc_id, type_t ref_type, IntPtr refer, [Out] byte[] buffer,
            size_t size);

        /// <summary>
        /// Retrieves the type of object that an object reference points to.
        /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5R.html#Reference-GetObjType2
        /// </summary>
        /// <param name="loc_id">The dataset containing the reference object or
        /// the group containing that dataset.</param>
        /// <param name="ref_type">Type of reference to query.</param>
        /// <param name="refer">Reference to query.</param>
        /// <param name="obj_type">Type of referenced object.</param>
        /// <returns>Returns a non-negative value if successful; otherwise
        /// returns a negative value.</returns>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Rget_obj_type2",
            CallingConvention = CallingConvention.Cdecl),
        SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
        public static extern herr_t get_obj_type
            (hid_t loc_id, type_t ref_type, IntPtr refer,
            ref H5O.type_t obj_type);

        /// <summary>
        /// Sets up a dataspace and selection as specified by a region reference.
        /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5R.html#Reference-GetRegion
        /// </summary>
        /// <param name="loc_id">File identifier or identifier for any object
        /// in the file containing the referenced region</param>
        /// <param name="ref_type">Reference type of <paramref name="refer"/>,
        /// which must be <code>DATASET_REGION</code>.</param>
        /// <param name="refer">Region reference to open</param>
        /// <returns>Returns a valid dataspace identifier if successful;
        /// otherwise returns a negative value.</returns>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Rget_region",
            CallingConvention = CallingConvention.Cdecl),
        SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
        public static extern hid_t get_region
            (hid_t loc_id, type_t ref_type, IntPtr refer);
    }
}

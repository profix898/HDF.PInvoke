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
using hbool_t = System.Byte;
using herr_t = System.Int32;
using hid_t = System.Int64;
using hsize_t = System.UInt64;
using htri_t = System.Int32;
using size_t = System.IntPtr;
using ssize_t = System.IntPtr;

namespace HDF.PInvoke
{
    /// <summary>
    /// Provides HDF5 identifier registration, lookup, and reference-count operations.
    /// </summary>
    public unsafe sealed class H5I
    {
        /// <summary>
        /// Initializes HDF5 before this API is first used.
        /// </summary>
        static H5I()
        {
            H5.open();
        }

        /// <summary>
        /// Identifies the native object category associated with an HDF5 identifier.
        /// </summary>
        public enum type_t : int
        {
            /// <summary>
            /// uninitialized type.
            /// </summary>
            UNINIT = -2,

            /// <summary>
            /// invalid Type.
            /// </summary>
            BADID = -1,

            /// <summary>
            /// type ID for File objects.
            /// </summary>
            FILE = 1,

            /// <summary>
            /// type ID for Group objects.
            /// </summary>
            GROUP = 2,

            /// <summary>
            /// type ID for Datatype objects.
            /// </summary>
            DATATYPE = 3,

            /// <summary>
            /// type ID for Dataspace objects.
            /// </summary>
            DATASPACE = 4,

            /// <summary>
            /// type ID for Dataset objects.
            /// </summary>
            DATASET = 5,

            /// <summary>
            /// type ID for Map objects.
            /// </summary>
            MAP = 6,

            /// <summary>
            /// type ID for Attribute objects.
            /// </summary>
            ATTR = 7,

            /// <summary>
            /// type ID for virtual file layer.
            /// </summary>
            VFL = 8,

            /// <summary>
            /// type ID for virtual object layer.
            /// </summary>
            VOL = 9,

            /// <summary>
            /// type ID for generic property list classes.
            /// </summary>
            GENPROP_CLS = 10,

            /// <summary>
            /// type ID for generic property lists.
            /// </summary>
            GENPROP_LST = 11,

            /// <summary>
            /// type ID for error classes.
            /// </summary>
            ERROR_CLASS = 12,

            /// <summary>
            /// type ID for error messages.
            /// </summary>
            ERROR_MSG = 13,

            /// <summary>
            /// type ID for error stacks.
            /// </summary>
            ERROR_STACK = 14,

            /// <summary>
            /// type ID for dataspace selection iterator.
            /// </summary>
            SPACE_SEL_ITER = 15,

            /// <summary>
            /// type ID for event sets.
            /// </summary>
            EVENTSET = 16,

            /// <summary>
            /// number of library types, MUST BE LAST!.
            /// </summary>
            NTYPES = 17
        }

        /// <summary>
        /// The size of the native HDF5 identifier type in bytes.
        /// </summary>
        public const int H5_SIZEOF_HID_T = sizeof(long);

        /// <summary>
        /// An invalid object ID. This is also negative for error return.
        /// </summary>
        public const hid_t H5I_INVALID_HID = -1;

        /// <summary>
        /// Function for freeing objects. This function will be called with an
        /// object ID type number and a pointer to the object. The function
        /// should free the object and return non-negative to indicate that
        /// the object can be removed from the ID type. If the function returns
        /// negative (failure) then the object will remain in the ID type.
        /// </summary>
        /// <param name="obj">The address of the registered native object.</param>
        /// <param name="request">Receives an asynchronous request pointer when supported by the callback, or zero for synchronous completion.</param>
        /// <returns>A nonnegative value on success, or a negative value on failure.</returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate herr_t free_t(IntPtr obj, ref IntPtr request);

        /// <summary>
        /// Type of the function to compare objects and keys
        /// </summary>
        /// <param name="obj">The address of the registered native object.</param>
        /// <param name="id">The identifier associated with the native object.</param>
        /// <param name="key">The caller-provided search key.</param>
        /// <returns>Zero when the object does not match the search criteria, or a nonzero value when it matches.</returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int search_func_t(IntPtr obj, hid_t id, IntPtr key);

        /// <summary>
        /// Deletes all identifiers of the given type.
        /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5I.html#Identify-ClearType
        /// </summary>
        /// <param name="type">Identifier of identifier type which is to be
        /// cleared of identifiers</param>
        /// <param name="force">Whether or not to force deletion of all
        /// identifiers</param>
        /// <returns>Returns non-negative on success, negative on failure.</returns>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Iclear_type",
            CallingConvention = CallingConvention.Cdecl),
        SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
        public extern static herr_t clear_type(type_t type, hbool_t force);

        /// <summary>
        /// Decrements the reference count for an object.
        /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5I.html#Identify-DecRef
        /// </summary>
        /// <param name="obj_id">Object identifier whose reference count will
        /// be modified.</param>
        /// <returns>Returns a non-negative reference count of the object
        /// identifier after decrementing it, if successful; otherwise a
        /// negative value is returned.</returns>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Idec_ref",
            CallingConvention = CallingConvention.Cdecl),
        SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
        public extern static int dec_ref(hid_t obj_id);

        /// <summary>
        /// Decrements the reference count on an identifier type.
        /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5I.html#Identify-DecTypeRef
        /// </summary>
        /// <param name="type">The identifier of the type whose reference count
        /// is to be decremented</param>
        /// <returns>Returns the current reference count on success, negative
        /// on failure.</returns>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Idec_type_ref",
            CallingConvention = CallingConvention.Cdecl),
        SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
        public extern static int dec_type_ref(type_t type);

        /// <summary>
        /// Removes the type type and all identifiers within that type.
        /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5I.html#Identify-DestroyType
        /// </summary>
        /// <param name="type">Identifier of identifier type which is to be
        /// destroyed</param>
        /// <returns>Returns non-negative on success, negative on failure.</returns>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Idestroy_type",
            CallingConvention = CallingConvention.Cdecl),
        SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
        public extern static herr_t destroy_type(type_t type);

        /// <summary>
        /// Retrieves an identifier for the file containing the specified object.
        /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5I.html#Identify-GetFileId
        /// </summary>
        /// <param name="obj_id">Identifier of the object whose associated file
        /// identifier will be returned.</param>
        /// <returns>Returns a file identifier on success, negative on failure.S</returns>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Iget_file_id",
            CallingConvention = CallingConvention.Cdecl),
        SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
        public extern static hid_t get_file_id(hid_t obj_id);

        /// <summary>
        /// Retrieves a name of an object based on the object identifier.
        /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5I.html#Identify-GetName
        /// </summary>
        /// <param name="obj_id">Identifier of the object. This identifier can
        /// refer to a group, dataset, or named datatype.</param>
        /// <param name="name">A name associated with the identifier.</param>
        /// <param name="size">The size of the name buffer; must be the size of
        /// the name in bytes plus 1 for a <code>NULL</code> terminator.</param>
        /// <returns>Returns the length of the name if successful, returning 0
        /// (zero) if no name is associated with the identifier. Otherwise
        /// returns a negative value.</returns>
        /// <remarks>Managed strings are marshalled as UTF-8.</remarks>
        public static ssize_t get_name(hid_t obj_id, StringBuilder name, size_t size)
        {
            var buffer = Utf8Buffer.Create(name, size);
            var result = get_name_utf8(obj_id, buffer, size);

            if (result.ToInt64() >= 0)
            {
                Utf8Buffer.CopyTo(buffer, name);
            }

            return result;
        }

        /// <summary>
        /// Invokes the native <c>H5Iget_name</c> entry point using its explicit ABI declaration.
        /// </summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Iget_name", CallingConvention = CallingConvention.Cdecl)]
        private static extern ssize_t get_name_utf8(hid_t obj_id, [Out] byte[] buffer, size_t size);

        /// <summary>
        /// Retrieves the reference count for an object.
        /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5I.html#Identify-GetRef
        /// </summary>
        /// <param name="obj_id">Object identifier whose reference count will
        /// be retrieved.</param>
        /// <returns>Returns a non-negative current reference count of the
        /// object identifier if successful; otherwise a negative value is
        /// returned.</returns>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Iget_ref",
            CallingConvention = CallingConvention.Cdecl),
        SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
        public extern static int get_ref(hid_t obj_id);

        /// <summary>
        /// Retrieves the type of an object.
        /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5I.html#Identify-GetType
        /// </summary>
        /// <param name="obj_id">Object identifier whose type is to be
        /// determined.</param>
        /// <returns>Returns the object type if successful; otherwise
        /// <code>H5I_BADID</code>.</returns>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Iget_type",
            CallingConvention = CallingConvention.Cdecl),
        SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
        public extern static type_t get_type(hid_t obj_id);

        /// <summary>
        /// Retrieves the reference count on an ID type.
        /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5I.html#Identify-GetTypeRef
        /// </summary>
        /// <param name="type">The identifier of the type whose reference count
        /// is to be retrieved</param>
        /// <returns>Returns the current reference count on success, negative
        /// on failure.</returns>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Iget_type_ref",
            CallingConvention = CallingConvention.Cdecl),
        SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
        public extern static int H5Iget_type_ref(type_t type);

        /// <summary>
        /// Increments the reference count for an object.
        /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5I.html#Identify-IncRef
        /// </summary>
        /// <param name="obj_id">Object identifier whose reference count will
        /// be modified.</param>
        /// <returns>Returns a non-negative reference count of the object ID
        /// after incrementing it if successful; otherwise a negative value is
        /// returned.</returns>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Iinc_ref",
            CallingConvention = CallingConvention.Cdecl),
        SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
        public extern static int inc_ref(hid_t obj_id);

        /// <summary>
        /// Increments the reference count on an ID type.
        /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5I.html#Identify-IncTypeRef
        /// </summary>
        /// <param name="type">The identifier of the type whose reference count
        /// is to be incremented</param>
        /// <returns>Returns the current reference count on success, negative
        /// on failure.</returns>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Iinc_type_ref",
            CallingConvention = CallingConvention.Cdecl),
        SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
        public extern static int inc_type_ref(type_t type);

        /// <summary>
        /// Determines whether an identifier is valid.
        /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5I.html#Identify-IsValid
        /// </summary>
        /// <param name="obj_id">Identifier to validate</param>
        /// <returns>Returns <code>TRUE</code> if <paramref name="obj_id"/> is
        /// valid and <code>FALSE</code> if invalid.
        /// Otherwise returns a negative value.</returns>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Iis_valid",
            CallingConvention = CallingConvention.Cdecl),
        SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
        public extern static htri_t is_valid(hid_t obj_id);

        /// <summary>
        /// Returns the number of identifiers in a given identifier type.
        /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5I.html#Identify-NMembers
        /// </summary>
        /// <param name="type">Identifier for the identifier type whose member
        /// count will be retrieved</param>
        /// <param name="num_members">Number of identifiers of the specified
        /// identifier type.</param>
        /// <returns>Returns a non-negative value on success; otherwise returns
        /// negative value.</returns>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Inmembers",
            CallingConvention = CallingConvention.Cdecl),
        SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
        public extern static herr_t nmembers
            (type_t type, ref hsize_t num_members);

        /// <summary>
        /// Returns the object referenced by id.
        /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5I.html#Identify-ObjectVerify
        /// </summary>
        /// <param name="id">ID to be dereferenced</param>
        /// <param name="id_type">ID type to which id should belong</param>
        /// <returns>Pointer to the object referenced by id on success,
        /// <code>NULL</code> on failure.</returns>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Iobject_verify",
            CallingConvention = CallingConvention.Cdecl),
        SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
        public extern static IntPtr object_verify
            (hid_t id, type_t id_type);

        /// <summary>
        /// Creates and returns a new ID.
        /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5I.html#Identify-Register
        /// </summary>
        /// <param name="type">The identifier of the type to which the new ID
        /// will belong</param>
        /// <param name="obj">Pointer to memory for the library to store</param>
        /// <returns>Returns the new ID on success, negative on failure.</returns>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Iregister",
            CallingConvention = CallingConvention.Cdecl),
        SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
        public extern static hid_t register(type_t type, IntPtr obj);

        /// <summary>
        /// Creates and returns a new ID type.
        /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5I.html#Identify-RegisterType
        /// </summary>
        /// <param name="hash_size">Size of the hash table (in entries) used to
        /// store IDs for the new type</param>
        /// <param name="reserved">Number of reserved IDs for the new type</param>
        /// <param name="free_func">Function used to deallocate space for a
        /// single ID</param>
        /// <returns>Returns the type identifier on success, negative on
        /// failure.</returns>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Iregister_type1",
            CallingConvention = CallingConvention.Cdecl),
        SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
        public extern static type_t register_type
            (size_t hash_size, uint reserved, free_t free_func);

        /// <summary>
        /// Removes an ID from internal storage.
        /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5I.html#Identify-RemoveVerify
        /// </summary>
        /// <param name="id">The ID to be removed from internal storage</param>
        /// <param name="id_type">The identifier of the type whose reference
        /// count is to be retrieved</param>
        /// <returns>Returns a pointer to the memory referred to by id on
        /// success, <code>NULL</code> on failure.</returns>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Iremove_verify",
            CallingConvention = CallingConvention.Cdecl),
        SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
        public extern static IntPtr remove_verify
            (hid_t id, type_t id_type);

        /// <summary>
        /// Finds the memory referred to by an ID within the given ID type such
        /// that some criterion is satisfied.
        /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5I.html#Identify-Search
        /// </summary>
        /// <param name="type">The identifier of the type to be searched</param>
        /// <param name="func">The function defining the search criteria</param>
        /// <param name="key">A key for the search function</param>
        /// <returns>Returns a pointer to the object which satisfies the search
        /// function on success, NULL on failure.</returns>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Isearch",
            CallingConvention = CallingConvention.Cdecl),
        SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
        public extern static IntPtr search
            (type_t type, search_func_t func, IntPtr key);

        /// <summary>
        /// Determines whether an identifier type is registered.
        /// See https://www.hdfgroup.org/HDF5/doc/RM/RM_H5I.html#Identify-TypeExists
        /// </summary>
        /// <param name="type">Identifier type.</param>
        /// <returns>Returns 1 if the type is registered and 0 if not. Returns
        /// a negative value on failure.</returns>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Itype_exists",
            CallingConvention = CallingConvention.Cdecl),
        SuppressUnmanagedCodeSecurity, SecuritySafeCritical]
        public extern static htri_t type_exists(type_t type);
    }
}

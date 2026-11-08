using System;
using System.Runtime.InteropServices;

namespace HDF.PInvoke
{
    /// <summary>Provides virtual object layer discovery and configuration.</summary>
    public unsafe sealed class H5VL
    {
        /// <summary>Initializes native HDF5 before connector operations.</summary>
        static H5VL()
        {
            H5.open();
        }

        /// <summary>Identifies a VOL operation category.</summary>
        public enum subclass_t
        {
            /// <summary>Identifies none operations.</summary>
            NONE = 0,

            /// <summary>Identifies info operations.</summary>
            INFO = 1,

            /// <summary>Identifies wrap operations.</summary>
            WRAP = 2,

            /// <summary>Identifies attr operations.</summary>
            ATTR = 3,

            /// <summary>Identifies dataset operations.</summary>
            DATASET = 4,

            /// <summary>Identifies datatype operations.</summary>
            DATATYPE = 5,

            /// <summary>Identifies file operations.</summary>
            FILE = 6,

            /// <summary>Identifies group operations.</summary>
            GROUP = 7,

            /// <summary>Identifies link operations.</summary>
            LINK = 8,

            /// <summary>Identifies object operations.</summary>
            OBJECT = 9,

            /// <summary>Identifies request operations.</summary>
            REQUEST = 10,

            /// <summary>Identifies blob operations.</summary>
            BLOB = 11,

            /// <summary>Identifies token operations.</summary>
            TOKEN = 12,
        }

        /// <summary>Registers a loadable connector and returns an identifier to release with H5VL.close.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5VLregister_connector_by_name", CallingConvention = CallingConvention.Cdecl)]
        public static extern long register_connector_by_name([MarshalAs(UnmanagedType.LPUTF8Str)] string name, long initialize = H5P.DEFAULT);

        /// <summary>Registers a connector by class value; release the returned identifier with H5VL.close.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5VLregister_connector_by_value", CallingConvention = CallingConvention.Cdecl)]
        public static extern long register_connector_by_value(int value, long initialize = H5P.DEFAULT);

        /// <summary>Tests connector registration by name.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5VLis_connector_registered_by_name", CallingConvention = CallingConvention.Cdecl)]
        public static extern int is_connector_registered_by_name([MarshalAs(UnmanagedType.LPUTF8Str)] string name);

        /// <summary>Tests connector registration by class value.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5VLis_connector_registered_by_value", CallingConvention = CallingConvention.Cdecl)]
        public static extern int is_connector_registered_by_value(int value);

        /// <summary>Gets the object's connector identifier; release it with H5VL.close.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5VLget_connector_id", CallingConvention = CallingConvention.Cdecl)]
        public static extern long get_connector_id(long objectId);

        /// <summary>Gets a registered connector identifier by name; release it with H5VL.close.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5VLget_connector_id_by_name", CallingConvention = CallingConvention.Cdecl)]
        public static extern long get_connector_id_by_name([MarshalAs(UnmanagedType.LPUTF8Str)] string name);

        /// <summary>Gets a registered connector identifier by class value; release it with H5VL.close.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5VLget_connector_id_by_value", CallingConvention = CallingConvention.Cdecl)]
        public static extern long get_connector_id_by_value(int value);

        /// <summary>Gets an object's connector name length excluding the terminator; a null buffer queries length.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5VLget_connector_name", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr get_connector_name(long objectId, IntPtr buffer, UIntPtr capacity);

        /// <summary>Releases a connector identifier.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5VLclose", CallingConvention = CallingConvention.Cdecl)]
        public static extern int close(long connector);

        /// <summary>Unregisters a connector after all its objects have been closed.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5VLunregister_connector", CallingConvention = CallingConvention.Cdecl)]
        public static extern int unregister_connector(long connector);

        /// <summary>Queries optional-operation capability flags for an object.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5VLquery_optional", CallingConvention = CallingConvention.Cdecl)]
        public static extern int query_optional(long objectId, subclass_t subclass, int operation, out ulong flags);

        /// <summary>Gets whether an object uses the native connector.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5VLobject_is_native", CallingConvention = CallingConvention.Cdecl)]
        public static extern int object_is_native(long objectId, out byte native);

        /// <summary>Frees connector-specific information returned by H5P.get_vol_info.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5VLfree_connector_info", CallingConvention = CallingConvention.Cdecl)]
        public static extern int free_connector_info(long connector, IntPtr information);

        /// <summary>No special connector capabilities.</summary>
        public const ulong CAP_FLAG_NONE = 0x0000000000000000UL;

        /// <summary>Connector is threadsafe.</summary>
        public const ulong CAP_FLAG_THREADSAFE = 0x0000000000000001UL;

        /// <summary>Connector performs operations asynchronously.</summary>
        public const ulong CAP_FLAG_ASYNC = 0x0000000000000002UL;

        /// <summary>Connector produces native file format.</summary>
        public const ulong CAP_FLAG_NATIVE_FILES = 0x0000000000000004UL;

        /// <summary>H5A create/delete/exists/open/close/read/write.</summary>
        public const ulong CAP_FLAG_ATTR_BASIC = 0x0000000000000008UL;

        /// <summary>All other H5A API calls.</summary>
        public const ulong CAP_FLAG_ATTR_MORE = 0x0000000000000010UL;

        /// <summary>H5D create/open/close/read/write.</summary>
        public const ulong CAP_FLAG_DATASET_BASIC = 0x0000000000000020UL;

        /// <summary>All other H5D API calls.</summary>
        public const ulong CAP_FLAG_DATASET_MORE = 0x0000000000000040UL;

        /// <summary>H5F create/open/close/read/write.</summary>
        public const ulong CAP_FLAG_FILE_BASIC = 0x0000000000000080UL;

        /// <summary>All other H5F API calls.</summary>
        public const ulong CAP_FLAG_FILE_MORE = 0x0000000000000100UL;

        /// <summary>H5G create/open/close.</summary>
        public const ulong CAP_FLAG_GROUP_BASIC = 0x0000000000000200UL;

        /// <summary>All other H5G API calls.</summary>
        public const ulong CAP_FLAG_GROUP_MORE = 0x0000000000000400UL;

        /// <summary>H5L exists/delete.</summary>
        public const ulong CAP_FLAG_LINK_BASIC = 0x0000000000000800UL;

        /// <summary>All other H5L API calls.</summary>
        public const ulong CAP_FLAG_LINK_MORE = 0x0000000000001000UL;

        /// <summary>H5M create/open/close/get*type/get_count/put/get/exists/delete.</summary>
        public const ulong CAP_FLAG_MAP_BASIC = 0x0000000000002000UL;

        /// <summary>All other H5M API calls.</summary>
        public const ulong CAP_FLAG_MAP_MORE = 0x0000000000004000UL;

        /// <summary>H5O open/close/exists.</summary>
        public const ulong CAP_FLAG_OBJECT_BASIC = 0x0000000000008000UL;

        /// <summary>All other H5O API calls.</summary>
        public const ulong CAP_FLAG_OBJECT_MORE = 0x0000000000010000UL;

        /// <summary>H5Rdestroy.</summary>
        public const ulong CAP_FLAG_REF_BASIC = 0x0000000000020000UL;

        /// <summary>All other H5R API calls.</summary>
        public const ulong CAP_FLAG_REF_MORE = 0x0000000000040000UL;

        /// <summary>Connector supports object references.</summary>
        public const ulong CAP_FLAG_OBJ_REF = 0x0000000000080000UL;

        /// <summary>Connector supports regional references.</summary>
        public const ulong CAP_FLAG_REG_REF = 0x0000000000100000UL;

        /// <summary>Connector supports attribute references.</summary>
        public const ulong CAP_FLAG_ATTR_REF = 0x0000000000200000UL;

        /// <summary>Connector supports stored datatypes.</summary>
        public const ulong CAP_FLAG_STORED_DATATYPES = 0x0000000000400000UL;

        /// <summary>Connector tracks creation order.</summary>
        public const ulong CAP_FLAG_CREATION_ORDER = 0x0000000000800000UL;

        /// <summary>Connector supports iteration functions.</summary>
        public const ulong CAP_FLAG_ITERATE = 0x0000000001000000UL;

        /// <summary>Connector can return a meaningful storage size.</summary>
        public const ulong CAP_FLAG_STORAGE_SIZE = 0x0000000002000000UL;

        /// <summary>&quot;by index&quot; API calls are supported.</summary>
        public const ulong CAP_FLAG_BY_IDX = 0x0000000004000000UL;

        /// <summary>Connector can return the property lists used to create an object.</summary>
        public const ulong CAP_FLAG_GET_PLIST = 0x0000000008000000UL;

        /// <summary>flush/refresh calls are supported.</summary>
        public const ulong CAP_FLAG_FLUSH_REFRESH = 0x0000000010000000UL;

        /// <summary>External links are supported.</summary>
        public const ulong CAP_FLAG_EXTERNAL_LINKS = 0x0000000020000000UL;

        /// <summary>Hard links are supported.</summary>
        public const ulong CAP_FLAG_HARD_LINKS = 0x0000000040000000UL;

        /// <summary>Soft links are supported.</summary>
        public const ulong CAP_FLAG_SOFT_LINKS = 0x0000000080000000UL;

        /// <summary>User-defined links are supported.</summary>
        public const ulong CAP_FLAG_UD_LINKS = 0x0000000100000000UL;

        /// <summary>Connector tracks creation, etc. times.</summary>
        public const ulong CAP_FLAG_TRACK_TIMES = 0x0000000200000000UL;

        /// <summary>H5Fmount/unmount supported.</summary>
        public const ulong CAP_FLAG_MOUNT = 0x0000000400000000UL;

        /// <summary>Connector implements a filter pipeline.</summary>
        public const ulong CAP_FLAG_FILTERS = 0x0000000800000000UL;

        /// <summary>Connector allows fill values to be set.</summary>
        public const ulong CAP_FLAG_FILL_VALUES = 0x0000001000000000UL;

        /// <summary>VOL connector supports this operation.</summary>
        public const ulong OPT_QUERY_SUPPORTED = 0x0001UL;

        /// <summary>Operation reads data for object.</summary>
        public const ulong OPT_QUERY_READ_DATA = 0x0002UL;

        /// <summary>Operation writes data for object.</summary>
        public const ulong OPT_QUERY_WRITE_DATA = 0x0004UL;

        /// <summary>Operation reads metadata for object.</summary>
        public const ulong OPT_QUERY_QUERY_METADATA = 0x0008UL;

        /// <summary>Operation modifies metadata for object.</summary>
        public const ulong OPT_QUERY_MODIFY_METADATA = 0x0010UL;

        /// <summary>Operation is collective (operations without this flag are assumed to be independent).</summary>
        public const ulong OPT_QUERY_COLLECTIVE = 0x0020UL;

        /// <summary>Operation may NOT be executed asynchronously.</summary>
        public const ulong OPT_QUERY_NO_ASYNC = 0x0040UL;

        /// <summary>Operation involves multiple objects.</summary>
        public const ulong OPT_QUERY_MULTI_OBJ = 0x0080UL;

    }
}


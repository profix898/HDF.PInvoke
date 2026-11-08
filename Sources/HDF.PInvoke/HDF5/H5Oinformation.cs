using System;
using System.Runtime.InteropServices;

namespace HDF.PInvoke
{
    /// <summary>Normalizes platform-dependent object information for current token-aware APIs.</summary>
    public unsafe sealed partial class H5O
    {
        /// <summary>Contains current object information with a platform-independent 64-bit file number.</summary>
        /// <remarks>This managed representation matches Unix layout. Use the wrapper methods for native calls; Windows uses a separate native layout.</remarks>
        [StructLayout(LayoutKind.Sequential)]
        public struct info2_t
        {
            /// <summary>File identity assigned by the connector.</summary>
            public ulong fileno;
            /// <summary>Opaque object identity.</summary>
            public token_t token;
            /// <summary>Object category.</summary>
            public type_t type;
            /// <summary>Hard-link reference count.</summary>
            public uint rc;
            /// <summary>Last access time, in native time_t units.</summary>
            public long atime;
            /// <summary>Last modification time.</summary>
            public long mtime;
            /// <summary>Last metadata change time.</summary>
            public long ctime;
            /// <summary>Object birth time.</summary>
            public long btime;
            /// <summary>Number of attached attributes.</summary>
            public ulong num_attrs;
        }

        /// <summary>Matches Windows native object information, where unsigned long occupies four bytes.</summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct WindowsInfo2
        {
            /// <summary>File identity assigned by the connector.</summary>
            public uint fileno;
            /// <summary>Opaque object identity.</summary>
            public token_t token;
            /// <summary>Object category.</summary>
            public type_t type;
            /// <summary>Hard-link reference count.</summary>
            public uint rc;
            /// <summary>Last access time, in native time_t units.</summary>
            public long atime;
            /// <summary>Last modification time.</summary>
            public long mtime;
            /// <summary>Last metadata change time.</summary>
            public long ctime;
            /// <summary>Object birth time.</summary>
            public long btime;
            /// <summary>Number of attached attributes.</summary>
            public ulong num_attrs;
        }

        /// <summary>Copies native information while widening the Windows file number.</summary>
        private static info2_t ReadInfo2(IntPtr pointer)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return Marshal.PtrToStructure<info2_t>(pointer);
            var value = Marshal.PtrToStructure<WindowsInfo2>(pointer);
            return new info2_t { fileno = value.fileno, token = value.token, type = value.type, rc = value.rc, atime = value.atime, mtime = value.mtime, ctime = value.ctime, btime = value.btime, num_attrs = value.num_attrs };
        }

        /// <summary>Copies a completed asynchronous H5O_info2_t output into the portable managed representation.</summary>
        /// <remarks>The pointer must address at least 72 bytes of valid native information on supported 64-bit platforms. Wait for operation completion before calling this method.</remarks>
        public static info2_t read_info2(IntPtr pointer)
        {
            if (pointer == IntPtr.Zero) throw new ArgumentNullException(nameof(pointer));
            return ReadInfo2(pointer);
        }

        /// <summary>Receives borrowed current object information; return zero to continue, positive to stop, or negative on error. Do not throw through native code.</summary>
        public delegate int iterate2_t(long root, string name, ref info2_t information, IntPtr state);

        /// <summary>Receives a native information pointer before converting its platform-dependent layout.</summary>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int NativeIterate2(long root, [MarshalAs(UnmanagedType.LPUTF8Str)] string name, IntPtr information, IntPtr state);

        /// <summary>Gets current object information, normalizing Windows and Unix native layouts; only requested fields are populated.</summary>
        public static int get_info3(long location, out info2_t information, uint fields = INFO_ALL)
        {
            // Zero unrequested fields and reserve enough aligned storage for both native layouts.
            info2_t storage = default;
            var result = get_info3_native(location, (IntPtr)(&storage), fields);
            information = result >= 0 ? ReadInfo2((IntPtr)(&storage)) : default;
            return result;
        }

        /// <summary>Imports the native information getter using opaque output storage.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Oget_info3", CallingConvention = CallingConvention.Cdecl)]
        private static extern int get_info3_native(long location, IntPtr information, uint fields);

        /// <summary>Gets current object information, normalizing Windows and Unix native layouts; only requested fields are populated.</summary>
        public static int get_info_by_name3(long location, [MarshalAs(UnmanagedType.LPUTF8Str)] string name, out info2_t information, uint fields = INFO_ALL, long access = H5P.DEFAULT)
        {
            // Zero unrequested fields and reserve enough aligned storage for both native layouts.
            info2_t storage = default;
            var result = get_info_by_name3_native(location, name, (IntPtr)(&storage), fields, access);
            information = result >= 0 ? ReadInfo2((IntPtr)(&storage)) : default;
            return result;
        }

        /// <summary>Imports the native information getter using opaque output storage.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Oget_info_by_name3", CallingConvention = CallingConvention.Cdecl)]
        private static extern int get_info_by_name3_native(long location, [MarshalAs(UnmanagedType.LPUTF8Str)] string name, IntPtr information, uint fields, long access);

        /// <summary>Gets current object information, normalizing Windows and Unix native layouts; only requested fields are populated.</summary>
        public static int get_info_by_idx3(long location, [MarshalAs(UnmanagedType.LPUTF8Str)] string group, H5.index_t index, H5.iter_order_t order, ulong position, out info2_t information, uint fields = INFO_ALL, long access = H5P.DEFAULT)
        {
            // Zero unrequested fields and reserve enough aligned storage for both native layouts.
            info2_t storage = default;
            var result = get_info_by_idx3_native(location, group, index, order, position, (IntPtr)(&storage), fields, access);
            information = result >= 0 ? ReadInfo2((IntPtr)(&storage)) : default;
            return result;
        }

        /// <summary>Imports the native information getter using opaque output storage.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Oget_info_by_idx3", CallingConvention = CallingConvention.Cdecl)]
        private static extern int get_info_by_idx3_native(long location, [MarshalAs(UnmanagedType.LPUTF8Str)] string group, H5.index_t index, H5.iter_order_t order, ulong position, IntPtr information, uint fields, long access);

        /// <summary>Synchronously visits objects with current information; callback and state must remain valid until this call returns.</summary>
        public static int visit3(long location, H5.index_t index, H5.iter_order_t order, iterate2_t callback, IntPtr state, uint fields = INFO_ALL)
        {
            if (callback == null) throw new ArgumentNullException(nameof(callback));
            NativeIterate2 bridge = (root, objectName, pointer, data) =>
            {
                var information = ReadInfo2(pointer);
                return callback(root, objectName, ref information, data);
            };
            var result = visit3_native(location, index, order, bridge, state, fields);
            GC.KeepAlive(bridge);
            return result;
        }

        /// <summary>Imports object traversal with a native layout callback.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Ovisit3", CallingConvention = CallingConvention.Cdecl)]
        private static extern int visit3_native(long location, H5.index_t index, H5.iter_order_t order, NativeIterate2 callback, IntPtr state, uint fields);

        /// <summary>Synchronously visits objects with current information; callback and state must remain valid until this call returns.</summary>
        public static int visit_by_name3(long location, [MarshalAs(UnmanagedType.LPUTF8Str)] string name, H5.index_t index, H5.iter_order_t order, iterate2_t callback, IntPtr state, uint fields = INFO_ALL, long access = H5P.DEFAULT)
        {
            if (callback == null) throw new ArgumentNullException(nameof(callback));
            NativeIterate2 bridge = (root, objectName, pointer, data) =>
            {
                var information = ReadInfo2(pointer);
                return callback(root, objectName, ref information, data);
            };
            var result = visit_by_name3_native(location, name, index, order, bridge, state, fields, access);
            GC.KeepAlive(bridge);
            return result;
        }

        /// <summary>Imports object traversal with a native layout callback.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Ovisit_by_name3", CallingConvention = CallingConvention.Cdecl)]
        private static extern int visit_by_name3_native(long location, [MarshalAs(UnmanagedType.LPUTF8Str)] string name, H5.index_t index, H5.iter_order_t order, NativeIterate2 callback, IntPtr state, uint fields, long access);

    }
}


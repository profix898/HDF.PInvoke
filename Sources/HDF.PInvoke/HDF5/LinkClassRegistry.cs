using System;
using System.Runtime.InteropServices;
using System.Text;

namespace HDF.PInvoke
{
    // HDF5 copies the registration, but borrows its comment and callback pointers.
    /// <summary>
    /// Keeps registered link callbacks and their native UTF-8 descriptions alive until HDF5 releases them.
    /// </summary>
    internal static class LinkClassRegistry
    {
        /// <summary>
        /// Matches the native link-class layout with explicitly owned comment and callback pointers.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct ClassInfo
        {
            /// <summary>
            /// Identifies the native link-class structure version.
            /// </summary>
            internal int version;

            /// <summary>
            /// Identifies the registered user-defined link type.
            /// </summary>
            internal H5L.type_t id;

            /// <summary>
            /// Points to the registration's owned, NUL-terminated UTF-8 description.
            /// </summary>
            internal IntPtr comment;

            /// <summary>
            /// Points to the callback invoked when a link is created.
            /// </summary>
            internal IntPtr create_func;

            /// <summary>
            /// Points to the callback invoked when a link is moved.
            /// </summary>
            internal IntPtr move_func;

            /// <summary>
            /// Points to the callback invoked when a link is copied.
            /// </summary>
            internal IntPtr copy_func;

            /// <summary>
            /// Points to the callback invoked when a link is followed.
            /// </summary>
            internal IntPtr trav_func;

            /// <summary>
            /// Points to the callback invoked when a link is deleted.
            /// </summary>
            internal IntPtr del_func;

            /// <summary>
            /// Points to the callback invoked to query a link's value.
            /// </summary>
            internal IntPtr query_func;
        }

        /// <summary>
        /// Pairs a native UTF-8 description buffer with the managed delegates rooted for its registration.
        /// </summary>
        private struct Registration
        {
            /// <summary>
            /// Points to the owned NUL-terminated UTF-8 comment buffer.
            /// </summary>
            internal IntPtr Comment;

            /// <summary>
            /// Retains managed callback delegates while HDF5 holds their function pointers.
            /// </summary>
            internal H5L.class_t Callbacks;
        }

        /// <summary>
        /// Invokes the native <c>H5Lregister</c> entry point using its explicit ABI declaration.
        /// </summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Lregister", CallingConvention = CallingConvention.Cdecl)]
        private static extern int RegisterNative(ref ClassInfo info);

        /// <summary>
        /// Invokes the native <c>H5Lunregister</c> entry point using its explicit ABI declaration.
        /// </summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Lunregister", CallingConvention = CallingConvention.Cdecl)]
        private static extern int UnregisterNative(H5L.type_t id);

        // IDs are bounded; reserve storage before native registration can retain any pointers.
        /// <summary>
        /// Roots each successful registration by link-type identifier without allocating after the native call.
        /// </summary>
        private static readonly Registration[] Registrations = new Registration[(int)H5L.type_t.MAX + 1];

        /// <summary>
        /// Allocates a NUL-terminated UTF-8 string buffer whose lifetime belongs to a native registration.
        /// </summary>
        internal static IntPtr AllocateUtf8(string text)
        {
            if (text == null)
            {
                return IntPtr.Zero;
            }

            var bytes = Encoding.UTF8.GetBytes(text + "\0");
            var pointer = Marshal.AllocHGlobal(bytes.Length);

            try
            {
                Marshal.Copy(bytes, 0, pointer, bytes.Length);
                return pointer;
            }
            catch
            {
                Marshal.FreeHGlobal(pointer);
                throw;
            }
        }

        /// <summary>
        /// Registers a native link class and replaces retained resources only after native success.
        /// </summary>
        internal static int Register(ref H5L.class_t source)
        {
            var native = new ClassInfo
            {
                version = source.version,
                id = source.id,
                comment = AllocateUtf8(source.comment),
                create_func = source.create_func == null ? IntPtr.Zero : Marshal.GetFunctionPointerForDelegate<H5L.create_func_t>(source.create_func),
                move_func = source.move_func == null ? IntPtr.Zero : Marshal.GetFunctionPointerForDelegate<H5L.move_func_t>(source.move_func),
                copy_func = source.copy_func == null ? IntPtr.Zero : Marshal.GetFunctionPointerForDelegate<H5L.copy_func_t>(source.copy_func),
                trav_func = source.trav_func == null ? IntPtr.Zero : Marshal.GetFunctionPointerForDelegate<H5L.traverse_func_t>(source.trav_func),
                del_func = source.del_func == null ? IntPtr.Zero : Marshal.GetFunctionPointerForDelegate<H5L.delete_func_t>(source.del_func),
                query_func = source.query_func == null ? IntPtr.Zero : Marshal.GetFunctionPointerForDelegate<H5L.query_func_t>(source.query_func)
            };
            var retained = false;

            try
            {
                lock (Registrations)
                {
                    var status = RegisterNative(ref native);
                    GC.KeepAlive(source);

                    if (status >= 0)
                    {
                        var index = (int)native.id;
                        Marshal.FreeHGlobal(Registrations[index].Comment);
                        Registrations[index] = new Registration { Comment = native.comment, Callbacks = source };
                        retained = true;
                    }

                    return status;
                }
            }
            finally
            {
                if (!retained)
                {
                    Marshal.FreeHGlobal(native.comment);
                }
            }
        }

        /// <summary>
        /// Releases retained descriptions and callbacks after HDF5 has successfully shut down.
        /// </summary>
        internal static void Clear()
        {
            lock (Registrations)
            {
                // Native shutdown has finished invoking callbacks before their managed roots are released.
                foreach (var registration in Registrations)
                {
                    Marshal.FreeHGlobal(registration.Comment);
                }

                Array.Clear(Registrations, 0, Registrations.Length);
            }
        }

        /// <summary>
        /// Releases a registration's retained resources only after native unregistration succeeds.
        /// </summary>
        internal static int Unregister(H5L.type_t id)
        {
            lock (Registrations)
            {
                var status = UnregisterNative(id);

                if (status >= 0)
                {
                    var index = (int)id;
                    Marshal.FreeHGlobal(Registrations[index].Comment);
                    Registrations[index] = default;
                }

                return status;
            }
        }
    }
}

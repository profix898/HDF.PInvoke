using System;
using System.Runtime.InteropServices;
using System.Text;

namespace HDF.PInvoke
{
    // HDF5 copies the registration, but borrows its comment and callback pointers.
    internal static class LinkClassRegistry
    {
        [StructLayout(LayoutKind.Sequential)]
        internal struct ClassInfo
        {
            internal int version;
            internal H5L.type_t id;
            internal IntPtr comment;
            internal H5L.create_func_t create_func;
            internal H5L.move_func_t move_func;
            internal H5L.copy_func_t copy_func;
            internal H5L.traverse_func_t trav_func;
            internal H5L.delete_func_t del_func;
            internal H5L.query_func_t query_func;
        }

        [DllImport(Constants.DLLFileName, EntryPoint = "H5Lregister", CallingConvention = CallingConvention.Cdecl)]
        private static extern int RegisterNative(ref ClassInfo info);

        [DllImport(Constants.DLLFileName, EntryPoint = "H5Lunregister", CallingConvention = CallingConvention.Cdecl)]
        private static extern int UnregisterNative(H5L.type_t id);

        // IDs are bounded; reserve storage before native registration can retain any pointers.
        private static readonly ClassInfo[] Registrations = new ClassInfo[(int)H5L.type_t.MAX + 1];

        private static IntPtr AllocateComment(string comment)
        {
            if (comment == null)
            {
                return IntPtr.Zero;
            }

            byte[] bytes = Encoding.UTF8.GetBytes(comment + "\0");
            IntPtr pointer = Marshal.AllocHGlobal(bytes.Length);

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

        internal static int Register(ref H5L.class_t source)
        {
            var native = new ClassInfo
            {
                version = source.version,
                id = source.id,
                comment = AllocateComment(source.comment),
                create_func = source.create_func,
                move_func = source.move_func,
                copy_func = source.copy_func,
                trav_func = source.trav_func,
                del_func = source.del_func,
                query_func = source.query_func
            };
            bool retained = false;

            try
            {
                lock (Registrations)
                {
                    int status = RegisterNative(ref native);

                    if (status >= 0)
                    {
                        int index = (int)native.id;
                        Marshal.FreeHGlobal(Registrations[index].comment);
                        Registrations[index] = native;
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

        internal static void Clear()
        {
            lock (Registrations)
            {
                // Native shutdown has finished invoking callbacks before their managed roots are released.
                foreach (ClassInfo registration in Registrations)
                {
                    Marshal.FreeHGlobal(registration.comment);
                }

                Array.Clear(Registrations, 0, Registrations.Length);
            }
        }

        internal static int Unregister(H5L.type_t id)
        {
            lock (Registrations)
            {
                int status = UnregisterNative(id);

                if (status >= 0)
                {
                    int index = (int)id;
                    Marshal.FreeHGlobal(Registrations[index].comment);
                    Registrations[index] = default;
                }

                return status;
            }
        }
    }
}

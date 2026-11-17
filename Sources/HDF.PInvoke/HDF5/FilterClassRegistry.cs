using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace HDF.PInvoke
{
    // HDF5 copies H5Z_class2_t by value, borrowing its name and callback pointers until unregistration or shutdown.
    /// <summary>
    /// Keeps registered filter callbacks and their native UTF-8 names alive until HDF5 releases them.
    /// </summary>
    internal static partial class FilterClassRegistry
    {
        /// <summary>
        /// Matches the native <c>H5Z_class2_t</c> layout using blittable pointer fields.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct ClassInfo
        {
            /// <summary>
            /// Identifies the native filter-class structure version.
            /// </summary>
            internal int version;

            /// <summary>
            /// Identifies the registered filter.
            /// </summary>
            internal H5Z.filter_t id;

            /// <summary>
            /// Indicates whether the filter provides an encoder.
            /// </summary>
            internal uint encoder_present;

            /// <summary>
            /// Indicates whether the filter provides a decoder.
            /// </summary>
            internal uint decoder_present;

            /// <summary>
            /// Points to the registration's owned, NUL-terminated UTF-8 filter name.
            /// </summary>
            internal IntPtr name;

            /// <summary>
            /// Points to the optional <c>can_apply</c> callback.
            /// </summary>
            internal IntPtr can_apply;

            /// <summary>
            /// Points to the optional <c>set_local</c> callback.
            /// </summary>
            internal IntPtr set_local;

            /// <summary>
            /// Points to the filter transformation callback.
            /// </summary>
            internal IntPtr filter;
        }

        /// <summary>
        /// Pairs a native UTF-8 filter name buffer with the managed delegates rooted for its registration.
        /// </summary>
        private struct Registration
        {
            /// <summary>
            /// Points to the owned NUL-terminated UTF-8 filter name buffer.
            /// </summary>
            internal IntPtr Name;

            /// <summary>
            /// Retains managed callback delegates while HDF5 holds their function pointers.
            /// </summary>
            internal H5Z.class_t Callbacks;
        }

        /// <summary>
        /// Invokes the native <c>H5Zregister</c> entry point using its explicit ABI declaration.
        /// </summary>
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Zregister"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        private static partial int RegisterNative(ref ClassInfo info);

        /// <summary>
        /// Invokes the native <c>H5Zunregister</c> entry point using its explicit ABI declaration.
        /// </summary>
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Zunregister"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        private static partial int UnregisterNative(H5Z.filter_t id);

        /// <summary>
        /// Roots each successful filter registration by identifier.
        /// </summary>
        private static readonly Dictionary<H5Z.filter_t, Registration> Registrations = new Dictionary<H5Z.filter_t, Registration>();

        /// <summary>
        /// Registers a native filter class and replaces retained resources only after native success.
        /// </summary>
        internal static int Register(ref H5Z.class_t source)
        {
            var native = new ClassInfo
            {
                version = source.version,
                id = source.id,
                encoder_present = source.encoder_present,
                decoder_present = source.decoder_present,
                name = LinkClassRegistry.AllocateUtf8(source.name),
                can_apply = source.can_apply == null ? IntPtr.Zero : Marshal.GetFunctionPointerForDelegate<H5Z.can_apply_func_t>(source.can_apply),
                set_local = source.set_local == null ? IntPtr.Zero : Marshal.GetFunctionPointerForDelegate<H5Z.set_local_func_t>(source.set_local),
                filter = source.filter == null ? IntPtr.Zero : Marshal.GetFunctionPointerForDelegate<H5Z.func_t>(source.filter)
            };
            var retained = false;

            try
            {
                lock (Registrations)
                {
                    // Reserve the dictionary slot before the native call so storing the registration cannot fail afterward.
                    var hadPrevious = Registrations.TryGetValue(source.id, out var previous);

                    if (!hadPrevious)
                    {
                        Registrations[source.id] = default;
                    }

                    try
                    {
                        var status = RegisterNative(ref native);
                        GC.KeepAlive(source);

                        if (status >= 0)
                        {
                            Marshal.FreeHGlobal(previous.Name);
                            Registrations[source.id] = new Registration { Name = native.name, Callbacks = source };
                            retained = true;
                        }

                        return status;
                    }
                    finally
                    {
                        if (!retained && !hadPrevious)
                        {
                            Registrations.Remove(source.id);
                        }
                    }
                }
            }
            finally
            {
                if (!retained)
                {
                    Marshal.FreeHGlobal(native.name);
                }
            }
        }

        /// <summary>
        /// Releases retained filter names and callbacks after HDF5 has successfully shut down.
        /// </summary>
        internal static void Clear()
        {
            lock (Registrations)
            {
                foreach (var registration in Registrations.Values)
                {
                    Marshal.FreeHGlobal(registration.Name);
                }

                Registrations.Clear();
            }
        }

        /// <summary>
        /// Releases a filter registration's retained resources only after native unregistration succeeds.
        /// </summary>
        internal static int Unregister(H5Z.filter_t id)
        {
            lock (Registrations)
            {
                var status = UnregisterNative(id);

                if (status >= 0 && Registrations.TryGetValue(id, out var registration))
                {
                    Marshal.FreeHGlobal(registration.Name);
                    Registrations.Remove(id);
                }

                return status;
            }
        }
    }
}

using System;
using System.Runtime.InteropServices;

namespace HDF.PInvoke
{
    // Old runtimes cannot marshal UTF-8 string fields. Decode callback-owned pointers explicitly.
    /// <summary>
    /// Bridges native error-stack records to managed callbacks without relying on UTF-8 field marshalling.
    /// </summary>
    internal static class LegacyErrorWalk
    {
        /// <summary>
        /// Matches the native error record while retaining borrowed strings as pointers.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct ErrorInfo
        {
            /// <summary>
            /// Identifies the error class and its major and minor error categories.
            /// </summary>
            internal long cls_id, maj_num, min_num;

            /// <summary>
            /// Contains the native source line that reported the error.
            /// </summary>
            internal uint line;

            /// <summary>
            /// Borrows the function name, file name, and description for the duration of the callback.
            /// </summary>
            internal IntPtr func_name, file_name, desc;
        }

        /// <summary>
        /// Receives a borrowed native error record during stack traversal.
        /// </summary>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int Callback(uint number, IntPtr info, IntPtr data);

        /// <summary>
        /// Invokes H5Ewalk2 with a pointer-based callback signature.
        /// </summary>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int Walk(long stack, H5E.direction_t direction, Callback callback, IntPtr data);

        /// <summary>
        /// Resolves the native walk function on demand and retains the successful delegate.
        /// </summary>
        private static class Cache
        {
            /// <summary>
            /// Prevents eager initialization before the walk function is requested.
            /// </summary>
            static Cache()
            {
            }

            /// <summary>
            /// Retains the resolved delegate, or remains null after an unsuccessful lookup.
            /// </summary>
            private static Walk function;

            /// <summary>
            /// Gets the cached native delegate, allowing lookup to be retried after failure.
            /// </summary>
            internal static Walk Function => System.Threading.LazyInitializer.EnsureInitialized(
                ref function, () => NativeDependencies.GetFunction<Walk>(Constants.DLLFileName, "H5Ewalk2"));
        }

        /// <summary>
        /// Decodes borrowed error strings and rethrows callback exceptions only after native traversal returns.
        /// </summary>
        internal static int Invoke(long stack, H5E.direction_t direction, H5E.walk_t callback, IntPtr data)
        {
            Exception failure = null;
            Callback bridge = callback == null ? (Callback)null : (number, pointer, state) =>
            {
                try
                {
                    var raw = Marshal.PtrToStructure<ErrorInfo>(pointer);
                    var decoder = UTF8Marshaler.GetInstance(null);
                    var info = new H5E.error_t
                    {
                        cls_id = raw.cls_id,
                        maj_num = raw.maj_num,
                        min_num = raw.min_num,
                        line = raw.line,
                        func_name = (string)decoder.MarshalNativeToManaged(raw.func_name),
                        file_name = (string)decoder.MarshalNativeToManaged(raw.file_name),
                        desc = (string)decoder.MarshalNativeToManaged(raw.desc)
                    };
                    return callback(number, ref info, state);
                }
                catch (Exception error)
                {
                    // Never unwind a managed exception through HDF5's native callback stack.
                    failure = error;
                    return -1;
                }
            };
            int result = Cache.Function(stack, direction, bridge, data);
            GC.KeepAlive(bridge);

            if (failure != null)
            {
                System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(failure).Throw();
            }

            return result;
        }
    }
}

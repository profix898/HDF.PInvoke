using System;
using System.Runtime.InteropServices;

namespace HDF.PInvoke
{
    // Old runtimes cannot marshal UTF-8 string fields. Decode callback-owned pointers explicitly.
    internal static class LegacyErrorWalk
    {
        [StructLayout(LayoutKind.Sequential)]
        internal struct ErrorInfo
        {
            internal long cls_id, maj_num, min_num;
            internal uint line;
            internal IntPtr func_name, file_name, desc;
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int Callback(uint number, IntPtr info, IntPtr data);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int Walk(long stack, H5E.direction_t direction, Callback callback, IntPtr data);

        private static class Cache
        {
            static Cache()
            {
            }

            private static Walk function;

            internal static Walk Function => System.Threading.LazyInitializer.EnsureInitialized(
                ref function, () => NativeDependencies.GetFunction<Walk>(Constants.DLLFileName, "H5Ewalk2"));
        }

        internal static int Invoke(long stack, H5E.direction_t direction, H5E.walk_t callback, IntPtr data)
        {
            Exception failure = null;
            Callback bridge = callback == null ? (Callback)null : (number, pointer, state) =>
            {
                try
                {
                    var raw = Marshal.PtrToStructure<ErrorInfo>(pointer);
                    var decoder = Utf8Marshaler.GetInstance(null);
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

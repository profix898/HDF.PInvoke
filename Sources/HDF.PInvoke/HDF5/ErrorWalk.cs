using System;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;

namespace HDF.PInvoke
{
    // Decode borrowed callback-owned UTF-8 error strings explicitly on both targets without non-blittable struct marshalling.
    /// <summary>
    /// Bridges native error-stack records to managed callbacks using a blittable pointer layout.
    /// </summary>
    internal static unsafe class ErrorWalk
    {
        /// <summary>
        /// Matches the native <c>H5E_error2_t</c> record while retaining borrowed strings as pointers.
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
        /// Invokes the native <c>H5Ewalk2</c> entry point using a pointer-based callback signature.
        /// </summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Ewalk2", CallingConvention = CallingConvention.Cdecl)]
        private static extern int WalkNative(long stack, H5E.direction_t direction, IntPtr callback, IntPtr data);

#if NETSTANDARD2_0
        /// <summary>
        /// Receives a borrowed native error record during legacy stack traversal.
        /// </summary>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int Callback(uint number, IntPtr info, IntPtr data);
#else
        /// <summary>
        /// Holds caller state for the static unmanaged error-walk callback bridge.
        /// </summary>
        private sealed class WalkState
        {
            internal H5E.walk_t Callback;
            internal IntPtr Data;
            internal Exception Failure;
        }

        /// <summary>
        /// Decodes borrowed error strings and captures managed exceptions before returning to native HDF5.
        /// </summary>
        [UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        private static int WalkCallback(uint number, ErrorInfo* raw, IntPtr contextPtr)
        {
            var context = (WalkState)GCHandle.FromIntPtr(contextPtr).Target;

            try
            {
                var info = new H5E.error_t
                {
                    cls_id = raw->cls_id,
                    maj_num = raw->maj_num,
                    min_num = raw->min_num,
                    line = raw->line,
                    func_name = Marshal.PtrToStringUTF8(raw->func_name),
                    file_name = Marshal.PtrToStringUTF8(raw->file_name),
                    desc = Marshal.PtrToStringUTF8(raw->desc)
                };
                return context.Callback(number, ref info, context.Data);
            }
            catch (Exception error)
            {
                // Never unwind a managed exception through HDF5's native callback stack.
                context.Failure = error;
                return -1;
            }
        }
#endif

        /// <summary>
        /// Decodes borrowed error strings and rethrows callback exceptions only after native traversal returns.
        /// </summary>
        internal static int Invoke(long stack, H5E.direction_t direction, H5E.walk_t callback, IntPtr data)
        {
            if (callback == null)
            {
                return WalkNative(stack, direction, IntPtr.Zero, data);
            }

#if NETSTANDARD2_0
            Exception failure = null;
            Callback bridge = (number, pointer, state) =>
            {
                try
                {
                    var raw = *(ErrorInfo*)pointer;
                    var decoder = UTF8Marshaler.GetInstance("borrowed");
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
            var result = WalkNative(stack, direction, Marshal.GetFunctionPointerForDelegate<Callback>(bridge), data);
            GC.KeepAlive(bridge);

            if (failure != null)
            {
                ExceptionDispatchInfo.Capture(failure).Throw();
            }

            return result;
#else
            var context = new WalkState { Callback = callback, Data = data };
            var handle = GCHandle.Alloc(context);
            int result;

            try
            {
                var callbackPtr = (IntPtr)(delegate* unmanaged[Cdecl]<uint, ErrorInfo*, IntPtr, int>)&WalkCallback;
                result = WalkNative(stack, direction, callbackPtr, GCHandle.ToIntPtr(handle));
            }
            finally
            {
                handle.Free();
            }

            if (context.Failure != null)
            {
                ExceptionDispatchInfo.Capture(context.Failure).Throw();
            }

            return result;
#endif
        }
    }
}

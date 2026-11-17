using System;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;

namespace HDF.PInvoke
{
    /// <summary>
    /// Bridges synchronous HDF5 iteration and scatter callbacks without runtime delegate marshalling,
    /// capturing managed exceptions before native stack frames unwind.
    /// </summary>
    internal static unsafe class CallbackBridge
    {
        internal delegate int NativeScopedInvoker<in TState>(TState state, IntPtr callbackPtr, IntPtr contextPtr);

#if NETSTANDARD2_0
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int NativeAttrCallback(long location, IntPtr attrName, IntPtr ainfo, IntPtr opData);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int NativeScatterCallback(IntPtr* srcBuf, IntPtr* srcBufBytesUsed, IntPtr opData);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int NativeLinkIterate1Callback(long group, IntPtr name, IntPtr info, IntPtr opData);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int NativeLinkIterate2Callback(long group, IntPtr name, IntPtr info, IntPtr state);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int NativeObjIterate1Callback(long obj, IntPtr name, IntPtr info, IntPtr opData);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int NativeObjIterate2Callback(long root, IntPtr name, IntPtr info, IntPtr state);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int NativePropIterateCallback(long id, IntPtr name, IntPtr iterData);

        private static string DecodeUtf8(IntPtr pointer) =>
            (string)UTF8Marshaler.GetInstance("borrowed").MarshalNativeToManaged(pointer);

        internal static int InvokeAttr<TState>(H5A.operator_t callback, IntPtr opData, TState state, NativeScopedInvoker<TState> nativeCall)
        {
            if (callback == null)
            {
                return nativeCall(state, IntPtr.Zero, opData);
            }

            Exception failure = null;
            NativeAttrCallback bridge = (location, attrName, ainfoPtr, data) =>
            {
                try
                {
                    return callback(location, attrName, ref *(H5A.info_t*)ainfoPtr, data);
                }
                catch (Exception error)
                {
                    failure = error;
                    return -1;
                }
            };
            var result = nativeCall(state, Marshal.GetFunctionPointerForDelegate<NativeAttrCallback>(bridge), opData);
            GC.KeepAlive(bridge);
            Rethrow(failure);
            return result;
        }

        internal static int InvokeScatter<TState>(H5D.scatter_func_t callback, IntPtr opData, TState state, NativeScopedInvoker<TState> nativeCall)
        {
            if (callback == null)
            {
                return nativeCall(state, IntPtr.Zero, opData);
            }

            Exception failure = null;
            NativeScatterCallback bridge = (srcBuf, srcBytes, data) =>
            {
                try
                {
                    return callback(ref *srcBuf, ref *srcBytes, data);
                }
                catch (Exception error)
                {
                    failure = error;
                    return -1;
                }
            };
            var result = nativeCall(state, Marshal.GetFunctionPointerForDelegate<NativeScatterCallback>(bridge), opData);
            GC.KeepAlive(bridge);
            Rethrow(failure);
            return result;
        }

        internal static int InvokeLink1<TState>(H5L.iterate_t callback, IntPtr opData, TState state, NativeScopedInvoker<TState> nativeCall)
        {
            if (callback == null)
            {
                return nativeCall(state, IntPtr.Zero, opData);
            }

            Exception failure = null;
            NativeLinkIterate1Callback bridge = (group, name, infoPtr, data) =>
            {
                try
                {
                    return callback(group, name, ref *(H5L.info_t*)infoPtr, data);
                }
                catch (Exception error)
                {
                    failure = error;
                    return -1;
                }
            };
            var result = nativeCall(state, Marshal.GetFunctionPointerForDelegate<NativeLinkIterate1Callback>(bridge), opData);
            GC.KeepAlive(bridge);
            Rethrow(failure);
            return result;
        }

        internal static int InvokeLink2<TState>(H5L.iterate2_t callback, IntPtr userState, TState state, NativeScopedInvoker<TState> nativeCall)
        {
            if (callback == null)
            {
                return nativeCall(state, IntPtr.Zero, userState);
            }

            Exception failure = null;
            NativeLinkIterate2Callback bridge = (group, namePtr, infoPtr, data) =>
            {
                try
                {
                    return callback(group, DecodeUtf8(namePtr), ref *(H5L.info2_t*)infoPtr, data);
                }
                catch (Exception error)
                {
                    failure = error;
                    return -1;
                }
            };
            var result = nativeCall(state, Marshal.GetFunctionPointerForDelegate<NativeLinkIterate2Callback>(bridge), userState);
            GC.KeepAlive(bridge);
            Rethrow(failure);
            return result;
        }

        internal static int InvokeObject1<TState>(H5O.iterate_t callback, IntPtr opData, TState state, NativeScopedInvoker<TState> nativeCall)
        {
            if (callback == null)
            {
                return nativeCall(state, IntPtr.Zero, opData);
            }

            Exception failure = null;
            NativeObjIterate1Callback bridge = (obj, name, infoPtr, data) =>
            {
                try
                {
                    return callback(obj, name, ref *(H5O.info_t*)infoPtr, data);
                }
                catch (Exception error)
                {
                    failure = error;
                    return -1;
                }
            };
            var result = nativeCall(state, Marshal.GetFunctionPointerForDelegate<NativeObjIterate1Callback>(bridge), opData);
            GC.KeepAlive(bridge);
            Rethrow(failure);
            return result;
        }

        internal static int InvokeObject2<TState>(H5O.iterate2_t callback, IntPtr userState, TState state, NativeScopedInvoker<TState> nativeCall)
        {
            Exception failure = null;
            NativeObjIterate2Callback bridge = (root, namePtr, infoPtr, data) =>
            {
                try
                {
                    var info = H5O.ReadInfo2(infoPtr);
                    return callback(root, DecodeUtf8(namePtr), ref info, data);
                }
                catch (Exception error)
                {
                    failure = error;
                    return -1;
                }
            };
            var result = nativeCall(state, Marshal.GetFunctionPointerForDelegate<NativeObjIterate2Callback>(bridge), userState);
            GC.KeepAlive(bridge);
            Rethrow(failure);
            return result;
        }

        internal static int InvokeProp<TState>(H5P.iterate_t callback, IntPtr iterData, TState state, NativeScopedInvoker<TState> nativeCall)
        {
            if (callback == null)
            {
                return nativeCall(state, IntPtr.Zero, iterData);
            }

            Exception failure = null;
            NativePropIterateCallback bridge = (id, namePtr, data) =>
            {
                try
                {
                    return callback(id, DecodeUtf8(namePtr), data);
                }
                catch (Exception error)
                {
                    failure = error;
                    return -1;
                }
            };
            var result = nativeCall(state, Marshal.GetFunctionPointerForDelegate<NativePropIterateCallback>(bridge), iterData);
            GC.KeepAlive(bridge);
            Rethrow(failure);
            return result;
        }

        private static void Rethrow(Exception failure)
        {
            if (failure != null)
            {
                ExceptionDispatchInfo.Capture(failure).Throw();
            }
        }
#else
        private struct Context
        {
            internal GCHandle CallbackHandle;
            internal GCHandle FailureHandle;
            internal IntPtr UserData;

            internal void Capture(Exception error)
            {
                if (!FailureHandle.IsAllocated)
                {
                    FailureHandle = GCHandle.Alloc(error);
                }
            }
        }

        private static int Execute<TState>(Delegate callback, IntPtr userData, TState state, IntPtr thunk, NativeScopedInvoker<TState> nativeCall)
        {
            if (callback == null)
            {
                return nativeCall(state, IntPtr.Zero, userData);
            }

            var context = new Context
            {
                CallbackHandle = GCHandle.Alloc(callback),
                UserData = userData
            };
            int result;

            try
            {
                result = nativeCall(state, thunk, (IntPtr)(&context));
            }
            finally
            {
                context.CallbackHandle.Free();
            }

            if (context.FailureHandle.IsAllocated)
            {
                var failure = (Exception)context.FailureHandle.Target;
                context.FailureHandle.Free();
                ExceptionDispatchInfo.Capture(failure).Throw();
            }

            return result;
        }

        internal static int InvokeAttr<TState>(H5A.operator_t callback, IntPtr opData, TState state, NativeScopedInvoker<TState> nativeCall) =>
            Execute(callback, opData, state, (IntPtr)(delegate* unmanaged[Cdecl]<long, IntPtr, H5A.info_t*, IntPtr, int>)&AttrCallback, nativeCall);

        internal static int InvokeScatter<TState>(H5D.scatter_func_t callback, IntPtr opData, TState state, NativeScopedInvoker<TState> nativeCall) =>
            Execute(callback, opData, state, (IntPtr)(delegate* unmanaged[Cdecl]<IntPtr*, IntPtr*, IntPtr, int>)&ScatterCallback, nativeCall);

        internal static int InvokeLink1<TState>(H5L.iterate_t callback, IntPtr opData, TState state, NativeScopedInvoker<TState> nativeCall) =>
            Execute(callback, opData, state, (IntPtr)(delegate* unmanaged[Cdecl]<long, IntPtr, H5L.info_t*, IntPtr, int>)&Link1Callback, nativeCall);

        internal static int InvokeLink2<TState>(H5L.iterate2_t callback, IntPtr userState, TState state, NativeScopedInvoker<TState> nativeCall) =>
            Execute(callback, userState, state, (IntPtr)(delegate* unmanaged[Cdecl]<long, IntPtr, H5L.info2_t*, IntPtr, int>)&Link2Callback, nativeCall);

        internal static int InvokeObject1<TState>(H5O.iterate_t callback, IntPtr opData, TState state, NativeScopedInvoker<TState> nativeCall) =>
            Execute(callback, opData, state, (IntPtr)(delegate* unmanaged[Cdecl]<long, IntPtr, H5O.info_t*, IntPtr, int>)&Object1Callback, nativeCall);

        internal static int InvokeObject2<TState>(H5O.iterate2_t callback, IntPtr userState, TState state, NativeScopedInvoker<TState> nativeCall) =>
            Execute(callback, userState, state, (IntPtr)(delegate* unmanaged[Cdecl]<long, IntPtr, IntPtr, IntPtr, int>)&Object2Callback, nativeCall);

        internal static int InvokeProp<TState>(H5P.iterate_t callback, IntPtr iterData, TState state, NativeScopedInvoker<TState> nativeCall) =>
            Execute(callback, iterData, state, (IntPtr)(delegate* unmanaged[Cdecl]<long, IntPtr, IntPtr, int>)&PropCallback, nativeCall);

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
        private static int AttrCallback(long location, IntPtr attrName, H5A.info_t* ainfo, IntPtr contextPtr)
        {
            var context = (Context*)contextPtr;

            try
            {
                return ((H5A.operator_t)context->CallbackHandle.Target)(location, attrName, ref *ainfo, context->UserData);
            }
            catch (Exception error)
            {
                context->Capture(error);
                return -1;
            }
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
        private static int ScatterCallback(IntPtr* srcBuf, IntPtr* srcBytes, IntPtr contextPtr)
        {
            var context = (Context*)contextPtr;

            try
            {
                return ((H5D.scatter_func_t)context->CallbackHandle.Target)(ref *srcBuf, ref *srcBytes, context->UserData);
            }
            catch (Exception error)
            {
                context->Capture(error);
                return -1;
            }
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
        private static int Link1Callback(long group, IntPtr name, H5L.info_t* info, IntPtr contextPtr)
        {
            var context = (Context*)contextPtr;

            try
            {
                return ((H5L.iterate_t)context->CallbackHandle.Target)(group, name, ref *info, context->UserData);
            }
            catch (Exception error)
            {
                context->Capture(error);
                return -1;
            }
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
        private static int Link2Callback(long group, IntPtr namePtr, H5L.info2_t* info, IntPtr contextPtr)
        {
            var context = (Context*)contextPtr;

            try
            {
                return ((H5L.iterate2_t)context->CallbackHandle.Target)(group, Marshal.PtrToStringUTF8(namePtr), ref *info, context->UserData);
            }
            catch (Exception error)
            {
                context->Capture(error);
                return -1;
            }
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
        private static int Object1Callback(long obj, IntPtr name, H5O.info_t* info, IntPtr contextPtr)
        {
            var context = (Context*)contextPtr;

            try
            {
                return ((H5O.iterate_t)context->CallbackHandle.Target)(obj, name, ref *info, context->UserData);
            }
            catch (Exception error)
            {
                context->Capture(error);
                return -1;
            }
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
        private static int Object2Callback(long root, IntPtr namePtr, IntPtr infoPtr, IntPtr contextPtr)
        {
            var context = (Context*)contextPtr;

            try
            {
                var info = H5O.ReadInfo2(infoPtr);
                return ((H5O.iterate2_t)context->CallbackHandle.Target)(root, Marshal.PtrToStringUTF8(namePtr), ref info, context->UserData);
            }
            catch (Exception error)
            {
                context->Capture(error);
                return -1;
            }
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
        private static int PropCallback(long id, IntPtr namePtr, IntPtr contextPtr)
        {
            var context = (Context*)contextPtr;

            try
            {
                return ((H5P.iterate_t)context->CallbackHandle.Target)(id, Marshal.PtrToStringUTF8(namePtr), context->UserData);
            }
            catch (Exception error)
            {
                context->Capture(error);
                return -1;
            }
        }
#endif
    }
}

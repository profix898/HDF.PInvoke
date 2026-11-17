using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace HDF.PInvoke
{
    /// <summary>Provides native version checks and rooted library-close callbacks.</summary>
    public unsafe sealed partial class H5
    {
        /// <summary>Retains callback delegates while native HDF5 stores their function pointers.</summary>
        private static readonly List<atclose_t> CloseCallbacks = new List<atclose_t>();

        /// <summary>Receives caller state during explicit native shutdown; do not throw through native code.</summary>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void atclose_t(IntPtr state);

        /// <summary>Registers a callback retained until a successful H5.close; native callbacks run in reverse registration order.</summary>
        /// <remarks>Coordinate explicit H5.close before managed runtime teardown. Call H5.dont_atexit before any initialization when automatic native shutdown must be disabled. State remains caller-owned; do not recursively close or register callbacks during shutdown.</remarks>
        public static int atclose(atclose_t callback, IntPtr state)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

#if !NETSTANDARD2_0
            // Multicast delegates expose only their last target through Target and Method.
            foreach (var target in callback.GetInvocationList())
            {
                if (typeof(H5).Assembly.IsCollectible ||
                    (target.Target != null && target.Target.GetType().Assembly.IsCollectible) ||
                    (RuntimeFeature.IsDynamicCodeSupported && target.Method.IsCollectible))
                {
                    throw new InvalidOperationException("H5.atclose callbacks cannot be registered from a collectible AssemblyLoadContext because native HDF5 cannot unregister them before unload.");
                }
            }
#endif

            lock (CloseCallbacks)
            {
                CloseCallbacks.Add(callback);
                var accepted = false;

                try
                {
                    var result = atclose_native(callback, state);
                    accepted = result >= 0;
                    return result;
                }
                finally
                {
                    // Failed registration must not leave an otherwise unreferenced delegate rooted.
                    if (!accepted)
                    {
                        CloseCallbacks.Remove(callback);
                    }
                }
            }
        }

        /// <summary>Registers the callback address after establishing its managed lifetime.</summary>
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5atclose"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        private static partial int atclose_native(atclose_t callback, IntPtr state);

        /// <summary>Checks native version compatibility using HDF5's native policy.</summary>
        /// <remarks>On a mismatch native HDF5 may terminate the process. Use H5.get_libversion for a nonterminating version comparison; the wrapper loader already checks native major/minor compatibility.</remarks>
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5check_version"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int check_version(uint major, uint minor, uint release);
    }
}


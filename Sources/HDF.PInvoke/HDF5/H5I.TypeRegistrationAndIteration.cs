using System;
using System.Runtime.InteropServices;

namespace HDF.PInvoke
{
    /// <summary>Provides advanced serial H5I operations.</summary>
    public unsafe sealed partial class H5I
    {
        /// <summary>Receives an identifier and caller state; do not throw through native code.</summary>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int iterate_t(long identifier, IntPtr state);

        /// <summary>Registers an identifier type without the obsolete hash-size argument.</summary>
        /// <remarks>Keep release rooted until the type and all registered identifiers have been destroyed; callbacks must not throw through native code.</remarks>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Iregister_type2", CallingConvention = CallingConvention.Cdecl)]
        public static extern type_t register_type2(uint reserved, free_t release);

        /// <summary>Synchronously iterates an application-defined identifier type.</summary>
        /// <remarks>Callback and state are borrowed for this call; return zero to continue, positive to stop, or negative on error.</remarks>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Iiterate", CallingConvention = CallingConvention.Cdecl)]
        public static extern int iterate(type_t type, iterate_t callback, IntPtr state);

    }
}


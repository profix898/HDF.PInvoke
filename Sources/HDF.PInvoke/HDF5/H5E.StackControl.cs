using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace HDF.PInvoke
{
    /// <summary>Provides advanced serial H5E operations.</summary>
    public unsafe sealed partial class H5E
    {
        /// <summary>Appends an error stack; a nonzero closeSource transfers ownership and closes the source stack.</summary>
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Eappend_stack"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int append_stack(long destination, long source, byte closeSource);

        /// <summary>Gets whether recording errors on the stack is paused.</summary>
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Eis_paused"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int is_paused(long stack, out byte paused);

        /// <summary>Pauses recording errors on the specified stack.</summary>
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Epause_stack"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int pause_stack(long stack);

        /// <summary>Resumes recording errors on the specified stack.</summary>
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5Eresume_stack"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int resume_stack(long stack);

    }
}


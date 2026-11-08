using System;
using System.Runtime.InteropServices;

namespace HDF.PInvoke
{
    /// <summary>Provides advanced serial H5E operations.</summary>
    public unsafe sealed partial class H5E
    {
        /// <summary>Appends an error stack; a nonzero closeSource transfers ownership and closes the source stack.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Eappend_stack", CallingConvention = CallingConvention.Cdecl)]
        public static extern int append_stack(long destination, long source, byte closeSource);

        /// <summary>Gets whether recording errors on the stack is paused.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Eis_paused", CallingConvention = CallingConvention.Cdecl)]
        public static extern int is_paused(long stack, out byte paused);

        /// <summary>Pauses recording errors on the specified stack.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Epause_stack", CallingConvention = CallingConvention.Cdecl)]
        public static extern int pause_stack(long stack);

        /// <summary>Resumes recording errors on the specified stack.</summary>
        [DllImport(Constants.DLLFileName, EntryPoint = "H5Eresume_stack", CallingConvention = CallingConvention.Cdecl)]
        public static extern int resume_stack(long stack);

    }
}


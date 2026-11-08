using System;
using System.Runtime.InteropServices;

namespace HDF.PInvoke
{
    /// <summary>Provides native H5IM high-level operations.</summary>
    public unsafe sealed class H5IM
    {
        /// <summary>Initializes native HDF5 before the first high-level call.</summary>
        static H5IM() { H5.open(); }

        /// <summary>Creates an indexed image from width times height bytes of pixel data.</summary>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5IMmake_image_8bit", CallingConvention = CallingConvention.Cdecl)]
        public static extern int make_image_8bit(long loc_id, [MarshalAs(UnmanagedType.LPUTF8Str)] string dset_name, ulong width, ulong height, IntPtr buffer);

        /// <summary>Creates a true-color image from width times height times three bytes in the selected interlace order.</summary>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5IMmake_image_24bit", CallingConvention = CallingConvention.Cdecl)]
        public static extern int make_image_24bit(long loc_id, [MarshalAs(UnmanagedType.LPUTF8Str)] string dset_name, ulong width, ulong height, [MarshalAs(UnmanagedType.LPUTF8Str)] string interlace, IntPtr buffer);

        /// <summary>Gets image dimensions, plane count, interlace text and palette count.</summary>
        /// <remarks>The interlace output has no capacity argument. Allocate enough space for the INTERLACE_MODE attribute, including its terminator; inspect attribute metadata for untrusted files.</remarks>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5IMget_image_info", CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_image_info(long loc_id, [MarshalAs(UnmanagedType.LPUTF8Str)] string dset_name, out ulong width, out ulong height, out ulong planes, IntPtr interlace, out long npals);

        /// <summary>Reads all image pixels into caller-owned storage sized from image dimensions and planes.</summary>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5IMread_image", CallingConvention = CallingConvention.Cdecl)]
        public static extern int read_image(long loc_id, [MarshalAs(UnmanagedType.LPUTF8Str)] string dset_name, IntPtr buffer);

        /// <summary>Creates a palette; pal_dims points to two unsigned 64-bit dimensions and pal_data to their product in bytes.</summary>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5IMmake_palette", CallingConvention = CallingConvention.Cdecl)]
        public static extern int make_palette(long loc_id, [MarshalAs(UnmanagedType.LPUTF8Str)] string pal_name, IntPtr pal_dims, IntPtr pal_data);

        /// <summary>Attaches a named palette to an image.</summary>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5IMlink_palette", CallingConvention = CallingConvention.Cdecl)]
        public static extern int link_palette(long loc_id, [MarshalAs(UnmanagedType.LPUTF8Str)] string image_name, [MarshalAs(UnmanagedType.LPUTF8Str)] string pal_name);

        /// <summary>Detaches a named palette from an image.</summary>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5IMunlink_palette", CallingConvention = CallingConvention.Cdecl)]
        public static extern int unlink_palette(long loc_id, [MarshalAs(UnmanagedType.LPUTF8Str)] string image_name, [MarshalAs(UnmanagedType.LPUTF8Str)] string pal_name);

        /// <summary>Gets the number of attached palettes.</summary>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5IMget_npalettes", CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_npalettes(long loc_id, [MarshalAs(UnmanagedType.LPUTF8Str)] string image_name, out long npals);

        /// <summary>Gets the two unsigned 64-bit dimensions of an attached palette.</summary>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5IMget_palette_info", CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_palette_info(long loc_id, [MarshalAs(UnmanagedType.LPUTF8Str)] string image_name, int pal_number, IntPtr pal_dims);

        /// <summary>Reads an attached palette into caller-owned storage sized from its dimensions.</summary>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5IMget_palette", CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_palette(long loc_id, [MarshalAs(UnmanagedType.LPUTF8Str)] string image_name, int pal_number, IntPtr pal_data);

        /// <summary>Returns positive if a dataset follows the HDF5 image convention, zero if not, or negative on error.</summary>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5IMis_image", CallingConvention = CallingConvention.Cdecl)]
        public static extern int is_image(long loc_id, [MarshalAs(UnmanagedType.LPUTF8Str)] string dset_name);

        /// <summary>Returns positive if a dataset follows the HDF5 palette convention, zero if not, or negative on error.</summary>
        [DllImport(Constants.HLDLLFileName, EntryPoint = "H5IMis_palette", CallingConvention = CallingConvention.Cdecl)]
        public static extern int is_palette(long loc_id, [MarshalAs(UnmanagedType.LPUTF8Str)] string dset_name);

    }
}

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace HDF.PInvoke
{
    /// <summary>Provides native H5IM high-level operations.</summary>
    public unsafe sealed partial class H5IM
    {
        /// <summary>Initializes native HDF5 before the first high-level call.</summary>
        static H5IM()
        {
            H5.open();
        }

        /// <summary>Creates an indexed image from width times height bytes of pixel data.</summary>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5IMmake_image_8bit", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int make_image_8bit(long loc_id, string dset_name, ulong width, ulong height, IntPtr buffer);

        /// <summary>Creates a true-color image from width times height times three bytes in the selected interlace order.</summary>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5IMmake_image_24bit", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int make_image_24bit(long loc_id, string dset_name, ulong width, ulong height, string interlace, IntPtr buffer);

        /// <summary>Gets image dimensions, plane count, interlace text and palette count.</summary>
        /// <remarks>The interlace output has no capacity argument. Allocate enough space for the INTERLACE_MODE attribute, including its terminator; inspect attribute metadata for untrusted files.</remarks>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5IMget_image_info", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int get_image_info(long loc_id, string dset_name, out ulong width, out ulong height, out ulong planes, IntPtr interlace, out long npals);

        /// <summary>Reads all image pixels into caller-owned storage sized from image dimensions and planes.</summary>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5IMread_image", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int read_image(long loc_id, string dset_name, IntPtr buffer);

        /// <summary>Creates a palette; pal_dims points to two unsigned 64-bit dimensions and pal_data to their product in bytes.</summary>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5IMmake_palette", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int make_palette(long loc_id, string pal_name, IntPtr pal_dims, IntPtr pal_data);

        /// <summary>Attaches a named palette to an image.</summary>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5IMlink_palette", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int link_palette(long loc_id, string image_name, string pal_name);

        /// <summary>Detaches a named palette from an image.</summary>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5IMunlink_palette", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int unlink_palette(long loc_id, string image_name, string pal_name);

        /// <summary>Gets the number of attached palettes.</summary>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5IMget_npalettes", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int get_npalettes(long loc_id, string image_name, out long npals);

        /// <summary>Gets the two unsigned 64-bit dimensions of an attached palette.</summary>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5IMget_palette_info", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int get_palette_info(long loc_id, string image_name, int pal_number, IntPtr pal_dims);

        /// <summary>Reads an attached palette into caller-owned storage sized from its dimensions.</summary>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5IMget_palette", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int get_palette(long loc_id, string image_name, int pal_number, IntPtr pal_data);

        /// <summary>Returns positive if a dataset follows the HDF5 image convention, zero if not, or negative on error.</summary>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5IMis_image", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int is_image(long loc_id, string dset_name);

        /// <summary>Returns positive if a dataset follows the HDF5 palette convention, zero if not, or negative on error.</summary>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5IMis_palette", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int is_palette(long loc_id, string dset_name);

    }
}

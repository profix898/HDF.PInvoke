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
        public static int get_palette_info(long loc_id, string image_name, int pal_number, IntPtr pal_dims) =>
            pal_dims == IntPtr.Zero ? -1 : ReadPalette(loc_id, image_name, pal_number, pal_dims, IntPtr.Zero, -1);

        /// <summary>Reads a two-dimensional palette into caller-owned storage sized from its dimensions.</summary>
        public static int get_palette(long loc_id, string image_name, int pal_number, IntPtr pal_data) =>
            pal_data == IntPtr.Zero ? -1 : ReadPalette(loc_id, image_name, pal_number, IntPtr.Zero, pal_data, -1);

        /// <summary>Checks the reference index, dataset rank and byte capacity before copying palette data.</summary>
        private static int ReadPalette(long location, string name, int index, IntPtr dimensions, IntPtr data, int capacity)
        {
            if (index < 0)
            {
                return -1;
            }

            long image = -1, attribute = -1, refsSpace = -1, palette = -1, paletteSpace = -1;
            try
            {
                image = H5D.open(location, name);
                if (image < 0)
                {
                    return -1;
                }

                attribute = H5A.open(image, "PALETTE");
                if (attribute < 0 || (refsSpace = H5A.get_space(attribute)) < 0)
                {
                    return -1;
                }

                var count = H5S.get_simple_extent_npoints(refsSpace);
                var refSize = H5T.get_size(H5T.STD_REF_OBJ).ToInt64();
                if (index >= count || refSize <= 0 || count > int.MaxValue / refSize)
                {
                    return -1;
                }

                // Request the legacy object-reference memory type, irrespective of the attribute's stored width.
                var refs = new byte[(int)(count * refSize)];
                fixed (byte* first = refs)
                {
                    if (H5A.read(attribute, H5T.STD_REF_OBJ, (IntPtr)first) < 0)
                    {
                        return -1;
                    }

                    palette = H5R.dereference(image, H5P.DEFAULT, H5R.type_t.OBJECT, (IntPtr)(first + index * refSize));
                }

                if (palette < 0 || H5I.get_type(palette) != H5I.type_t.DATASET ||
                    (paletteSpace = H5D.get_space(palette)) < 0 || H5S.get_simple_extent_ndims(paletteSpace) != 2)
                {
                    return -1;
                }

                if (dimensions != IntPtr.Zero)
                {
                    return H5S.get_simple_extent_dims(paletteSpace, (ulong*)dimensions, null) < 0 ? -1 : 0;
                }

                var elements = H5S.get_simple_extent_npoints(paletteSpace);
                if (elements < 0)
                {
                    return -1;
                }

                if (capacity >= 0 && elements > capacity)
                {
                    throw new ArgumentException("The destination is too small for the palette.", "pal_data");
                }

                return H5D.read(palette, H5T.NATIVE_UCHAR, paletteSpace, H5S.ALL, H5P.DEFAULT, data);
            }
            finally
            {
                if (paletteSpace >= 0) H5S.close(paletteSpace);
                if (palette >= 0) H5I.dec_ref(palette);
                if (refsSpace >= 0) H5S.close(refsSpace);
                if (attribute >= 0) H5A.close(attribute);
                if (image >= 0) H5D.close(image);
            }
        }

        /// <summary>Returns positive if a dataset follows the HDF5 image convention, zero if not, or negative on error.</summary>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5IMis_image", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int is_image(long loc_id, string dset_name);

        /// <summary>Returns positive if a dataset follows the HDF5 palette convention, zero if not, or negative on error.</summary>
        [LibraryImport(Constants.HLDLLFileName, EntryPoint = "H5IMis_palette", StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int is_palette(long loc_id, string dset_name);

    }
}

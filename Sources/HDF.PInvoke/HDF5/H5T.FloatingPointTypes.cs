namespace HDF.PInvoke
{
    /// <summary>Exposes additional predefined native datatypes.</summary>
    public unsafe sealed partial class H5T
    {
        /// <summary>Gets the native IEEE_F16BE datatype, owned by HDF5. Optional native types may return an invalid identifier when unsupported.</summary>
        public static long IEEE_F16BE => H5DLLImporter.Instance.GetHid("H5T_IEEE_F16BE_g");

        /// <summary>Gets the native IEEE_F16LE datatype, owned by HDF5. Optional native types may return an invalid identifier when unsupported.</summary>
        public static long IEEE_F16LE => H5DLLImporter.Instance.GetHid("H5T_IEEE_F16LE_g");

        /// <summary>Gets the native FLOAT_BFLOAT16BE datatype, owned by HDF5. Optional native types may return an invalid identifier when unsupported.</summary>
        public static long FLOAT_BFLOAT16BE => H5DLLImporter.Instance.GetHid("H5T_FLOAT_BFLOAT16BE_g");

        /// <summary>Gets the native FLOAT_BFLOAT16LE datatype, owned by HDF5. Optional native types may return an invalid identifier when unsupported.</summary>
        public static long FLOAT_BFLOAT16LE => H5DLLImporter.Instance.GetHid("H5T_FLOAT_BFLOAT16LE_g");

        /// <summary>Gets the native FLOAT_F8E4M3 datatype, owned by HDF5. Optional native types may return an invalid identifier when unsupported.</summary>
        public static long FLOAT_F8E4M3 => H5DLLImporter.Instance.GetHid("H5T_FLOAT_F8E4M3_g");

        /// <summary>Gets the native FLOAT_F8E5M2 datatype, owned by HDF5. Optional native types may return an invalid identifier when unsupported.</summary>
        public static long FLOAT_F8E5M2 => H5DLLImporter.Instance.GetHid("H5T_FLOAT_F8E5M2_g");

        /// <summary>Gets the native FLOAT_F6E2M3 datatype, owned by HDF5. Optional native types may return an invalid identifier when unsupported.</summary>
        public static long FLOAT_F6E2M3 => H5DLLImporter.Instance.GetHid("H5T_FLOAT_F6E2M3_g");

        /// <summary>Gets the native FLOAT_F6E3M2 datatype, owned by HDF5. Optional native types may return an invalid identifier when unsupported.</summary>
        public static long FLOAT_F6E3M2 => H5DLLImporter.Instance.GetHid("H5T_FLOAT_F6E3M2_g");

        /// <summary>Gets the native FLOAT_F4E2M1 datatype, owned by HDF5. Optional native types may return an invalid identifier when unsupported.</summary>
        public static long FLOAT_F4E2M1 => H5DLLImporter.Instance.GetHid("H5T_FLOAT_F4E2M1_g");

        /// <summary>Gets the native COMPLEX_IEEE_F16BE datatype, owned by HDF5. Optional native types may return an invalid identifier when unsupported.</summary>
        public static long COMPLEX_IEEE_F16BE => H5DLLImporter.Instance.GetHid("H5T_COMPLEX_IEEE_F16BE_g");

        /// <summary>Gets the native COMPLEX_IEEE_F16LE datatype, owned by HDF5. Optional native types may return an invalid identifier when unsupported.</summary>
        public static long COMPLEX_IEEE_F16LE => H5DLLImporter.Instance.GetHid("H5T_COMPLEX_IEEE_F16LE_g");

        /// <summary>Gets the native COMPLEX_IEEE_F32BE datatype, owned by HDF5. Optional native types may return an invalid identifier when unsupported.</summary>
        public static long COMPLEX_IEEE_F32BE => H5DLLImporter.Instance.GetHid("H5T_COMPLEX_IEEE_F32BE_g");

        /// <summary>Gets the native COMPLEX_IEEE_F32LE datatype, owned by HDF5. Optional native types may return an invalid identifier when unsupported.</summary>
        public static long COMPLEX_IEEE_F32LE => H5DLLImporter.Instance.GetHid("H5T_COMPLEX_IEEE_F32LE_g");

        /// <summary>Gets the native COMPLEX_IEEE_F64BE datatype, owned by HDF5. Optional native types may return an invalid identifier when unsupported.</summary>
        public static long COMPLEX_IEEE_F64BE => H5DLLImporter.Instance.GetHid("H5T_COMPLEX_IEEE_F64BE_g");

        /// <summary>Gets the native COMPLEX_IEEE_F64LE datatype, owned by HDF5. Optional native types may return an invalid identifier when unsupported.</summary>
        public static long COMPLEX_IEEE_F64LE => H5DLLImporter.Instance.GetHid("H5T_COMPLEX_IEEE_F64LE_g");

        /// <summary>Gets the native NATIVE_FLOAT16 datatype, owned by HDF5. Optional native types may return an invalid identifier when unsupported.</summary>
        public static long NATIVE_FLOAT16 => H5DLLImporter.Instance.GetHid("H5T_NATIVE_FLOAT16_g");

        /// <summary>Gets the native NATIVE_FLOAT_COMPLEX datatype, owned by HDF5. Optional native types may return an invalid identifier when unsupported.</summary>
        public static long NATIVE_FLOAT_COMPLEX => H5DLLImporter.Instance.GetHid("H5T_NATIVE_FLOAT_COMPLEX_g");

        /// <summary>Gets the native NATIVE_DOUBLE_COMPLEX datatype, owned by HDF5. Optional native types may return an invalid identifier when unsupported.</summary>
        public static long NATIVE_DOUBLE_COMPLEX => H5DLLImporter.Instance.GetHid("H5T_NATIVE_DOUBLE_COMPLEX_g");

        /// <summary>Gets the native NATIVE_LDOUBLE_COMPLEX datatype, owned by HDF5. Optional native types may return an invalid identifier when unsupported.</summary>
        public static long NATIVE_LDOUBLE_COMPLEX => H5DLLImporter.Instance.GetHid("H5T_NATIVE_LDOUBLE_COMPLEX_g");

    }
}


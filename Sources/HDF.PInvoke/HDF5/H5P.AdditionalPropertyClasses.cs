namespace HDF.PInvoke
{
    /// <summary>Exposes additional predefined native property lists.</summary>
    public unsafe sealed partial class H5P
    {
        /// <summary>Gets the native MAP_CREATE property class, owned by HDF5; unsupported optional classes may have an invalid identifier.</summary>
        public static long MAP_CREATE => H5DLLImporter.Instance.GetHid("H5P_CLS_MAP_CREATE_ID_g");

        /// <summary>Gets the native MAP_CREATE default list, owned by HDF5; unsupported optional classes may have an invalid identifier.</summary>
        public static long MAP_CREATE_DEFAULT => H5DLLImporter.Instance.GetHid("H5P_LST_MAP_CREATE_ID_g");

        /// <summary>Gets the native MAP_ACCESS property class, owned by HDF5; unsupported optional classes may have an invalid identifier.</summary>
        public static long MAP_ACCESS => H5DLLImporter.Instance.GetHid("H5P_CLS_MAP_ACCESS_ID_g");

        /// <summary>Gets the native MAP_ACCESS default list, owned by HDF5; unsupported optional classes may have an invalid identifier.</summary>
        public static long MAP_ACCESS_DEFAULT => H5DLLImporter.Instance.GetHid("H5P_LST_MAP_ACCESS_ID_g");

        /// <summary>Gets the native ATTRIBUTE_ACCESS property class, owned by HDF5; unsupported optional classes may have an invalid identifier.</summary>
        public static long ATTRIBUTE_ACCESS => H5DLLImporter.Instance.GetHid("H5P_CLS_ATTRIBUTE_ACCESS_ID_g");

        /// <summary>Gets the native ATTRIBUTE_ACCESS default list, owned by HDF5; unsupported optional classes may have an invalid identifier.</summary>
        public static long ATTRIBUTE_ACCESS_DEFAULT => H5DLLImporter.Instance.GetHid("H5P_LST_ATTRIBUTE_ACCESS_ID_g");

        /// <summary>Gets the native VOL_INITIALIZE property class, owned by HDF5; unsupported optional classes may have an invalid identifier.</summary>
        public static long VOL_INITIALIZE => H5DLLImporter.Instance.GetHid("H5P_CLS_VOL_INITIALIZE_ID_g");

        /// <summary>Gets the native VOL_INITIALIZE default list, owned by HDF5; unsupported optional classes may have an invalid identifier.</summary>
        public static long VOL_INITIALIZE_DEFAULT => H5DLLImporter.Instance.GetHid("H5P_LST_VOL_INITIALIZE_ID_g");

        /// <summary>Gets the native REFERENCE_ACCESS property class, owned by HDF5; unsupported optional classes may have an invalid identifier.</summary>
        public static long REFERENCE_ACCESS => H5DLLImporter.Instance.GetHid("H5P_CLS_REFERENCE_ACCESS_ID_g");

        /// <summary>Gets the native REFERENCE_ACCESS default list, owned by HDF5; unsupported optional classes may have an invalid identifier.</summary>
        public static long REFERENCE_ACCESS_DEFAULT => H5DLLImporter.Instance.GetHid("H5P_LST_REFERENCE_ACCESS_ID_g");

    }
}


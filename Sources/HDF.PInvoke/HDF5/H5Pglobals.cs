/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * Copyright by The HDF Group.                                               *
 * Copyright by the Board of Trustees of the University of Illinois.         *
 * All rights reserved.                                                      *
 *                                                                           *
 * This file is part of HDF.PInvoke. Copyright notices and redistribution
 * terms for the wrapper and its native dependencies are in LICENSE.txt
 * at the repository and package root.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

using hbool_t = System.Byte;
using herr_t = System.Int32;
using hsize_t = System.UInt64;
using htri_t = System.Int32;
using size_t = System.IntPtr;

using hid_t = System.Int64;

namespace HDF.PInvoke
{
    /// <summary>
    /// Provides native HDF5 property-list operations and predefined property-list identifiers.
    /// </summary>
    public unsafe sealed partial class H5P
    {
        /// <summary>
        /// Caches the exported native identifier <c>H5P_CLS_ROOT_g</c> from the selected HDF5 library.
        /// </summary>
        static readonly hid_t H5P_CLS_ROOT_g = H5DLLImporter.Instance.GetHid("H5P_CLS_ROOT_ID_g");

        /// <summary>
        /// The predefined root property-list identifier (<c>H5P_ROOT</c>), owned by HDF5.
        /// </summary>
        public static hid_t ROOT
        {
            get
            {
                return H5P_CLS_ROOT_g;
            }
        }

        /// <summary>
        /// Caches the exported native identifier <c>H5P_CLS_OBJECT_CREATE_g</c> from the selected HDF5 library.
        /// </summary>
        static readonly hid_t H5P_CLS_OBJECT_CREATE_g = H5DLLImporter.Instance.GetHid("H5P_CLS_OBJECT_CREATE_ID_g");

        /// <summary>
        /// The predefined object create property-list identifier (<c>H5P_OBJECT_CREATE</c>), owned by HDF5.
        /// </summary>
        public static hid_t OBJECT_CREATE
        {
            get
            {
                return H5P_CLS_OBJECT_CREATE_g;
            }
        }

        /// <summary>
        /// Caches the exported native identifier <c>H5P_CLS_FILE_CREATE_g</c> from the selected HDF5 library.
        /// </summary>
        static readonly hid_t H5P_CLS_FILE_CREATE_g = H5DLLImporter.Instance.GetHid("H5P_CLS_FILE_CREATE_ID_g");

        /// <summary>
        /// The predefined file create property-list identifier (<c>H5P_FILE_CREATE</c>), owned by HDF5.
        /// </summary>
        public static hid_t FILE_CREATE
        {
            get
            {
                return H5P_CLS_FILE_CREATE_g;
            }
        }

        /// <summary>
        /// Caches the exported native identifier <c>H5P_CLS_FILE_ACCESS_g</c> from the selected HDF5 library.
        /// </summary>
        static readonly hid_t H5P_CLS_FILE_ACCESS_g = H5DLLImporter.Instance.GetHid("H5P_CLS_FILE_ACCESS_ID_g");

        /// <summary>
        /// The predefined file access property-list identifier (<c>H5P_FILE_ACCESS</c>), owned by HDF5.
        /// </summary>
        public static hid_t FILE_ACCESS
        {
            get
            {
                return H5P_CLS_FILE_ACCESS_g;
            }
        }

        /// <summary>
        /// Caches the exported native identifier <c>H5P_CLS_DATASET_CREATE_g</c> from the selected HDF5 library.
        /// </summary>
        static readonly hid_t H5P_CLS_DATASET_CREATE_g = H5DLLImporter.Instance.GetHid("H5P_CLS_DATASET_CREATE_ID_g");

        /// <summary>
        /// The predefined dataset create property-list identifier (<c>H5P_DATASET_CREATE</c>), owned by HDF5.
        /// </summary>
        public static hid_t DATASET_CREATE
        {
            get
            {
                return H5P_CLS_DATASET_CREATE_g;
            }
        }

        /// <summary>
        /// Caches the exported native identifier <c>H5P_CLS_DATASET_ACCESS_g</c> from the selected HDF5 library.
        /// </summary>
        static readonly hid_t H5P_CLS_DATASET_ACCESS_g = H5DLLImporter.Instance.GetHid("H5P_CLS_DATASET_ACCESS_ID_g");

        /// <summary>
        /// The predefined dataset access property-list identifier (<c>H5P_DATASET_ACCESS</c>), owned by HDF5.
        /// </summary>
        public static hid_t DATASET_ACCESS
        {
            get
            {
                return H5P_CLS_DATASET_ACCESS_g;
            }
        }

        /// <summary>
        /// Caches the exported native identifier <c>H5P_CLS_DATASET_XFER_g</c> from the selected HDF5 library.
        /// </summary>
        static readonly hid_t H5P_CLS_DATASET_XFER_g = H5DLLImporter.Instance.GetHid("H5P_CLS_DATASET_XFER_ID_g");

        /// <summary>
        /// The predefined dataset xfer property-list identifier (<c>H5P_DATASET_XFER</c>), owned by HDF5.
        /// </summary>
        public static hid_t DATASET_XFER
        {
            get
            {
                return H5P_CLS_DATASET_XFER_g;
            }
        }

        /// <summary>
        /// Caches the exported native identifier <c>H5P_CLS_FILE_MOUNT_g</c> from the selected HDF5 library.
        /// </summary>
        static readonly hid_t H5P_CLS_FILE_MOUNT_g = H5DLLImporter.Instance.GetHid("H5P_CLS_FILE_MOUNT_ID_g");

        /// <summary>
        /// The predefined file mount property-list identifier (<c>H5P_FILE_MOUNT</c>), owned by HDF5.
        /// </summary>
        public static hid_t FILE_MOUNT
        {
            get
            {
                return H5P_CLS_FILE_MOUNT_g;
            }
        }

        /// <summary>
        /// Caches the exported native identifier <c>H5P_CLS_GROUP_CREATE_g</c> from the selected HDF5 library.
        /// </summary>
        static readonly hid_t H5P_CLS_GROUP_CREATE_g = H5DLLImporter.Instance.GetHid("H5P_CLS_GROUP_CREATE_ID_g");

        /// <summary>
        /// The predefined group create property-list identifier (<c>H5P_GROUP_CREATE</c>), owned by HDF5.
        /// </summary>
        public static hid_t GROUP_CREATE
        {
            get
            {
                return H5P_CLS_GROUP_CREATE_g;
            }
        }

        /// <summary>
        /// Caches the exported native identifier <c>H5P_CLS_GROUP_ACCESS_g</c> from the selected HDF5 library.
        /// </summary>
        static readonly hid_t H5P_CLS_GROUP_ACCESS_g = H5DLLImporter.Instance.GetHid("H5P_CLS_GROUP_ACCESS_ID_g");

        /// <summary>
        /// The predefined group access property-list identifier (<c>H5P_GROUP_ACCESS</c>), owned by HDF5.
        /// </summary>
        public static hid_t GROUP_ACCESS
        {
            get
            {
                return H5P_CLS_GROUP_ACCESS_g;
            }
        }

        /// <summary>
        /// Caches the exported native identifier <c>H5P_CLS_DATATYPE_CREATE_g</c> from the selected HDF5 library.
        /// </summary>
        static readonly hid_t H5P_CLS_DATATYPE_CREATE_g = H5DLLImporter.Instance.GetHid("H5P_CLS_DATATYPE_CREATE_ID_g");

        /// <summary>
        /// The predefined datatype create property-list identifier (<c>H5P_DATATYPE_CREATE</c>), owned by HDF5.
        /// </summary>
        public static hid_t DATATYPE_CREATE
        {
            get
            {
                return H5P_CLS_DATATYPE_CREATE_g;
            }
        }

        /// <summary>
        /// Caches the exported native identifier <c>H5P_CLS_DATATYPE_ACCESS_g</c> from the selected HDF5 library.
        /// </summary>
        static readonly hid_t H5P_CLS_DATATYPE_ACCESS_g = H5DLLImporter.Instance.GetHid("H5P_CLS_DATATYPE_ACCESS_ID_g");

        /// <summary>
        /// The predefined datatype access property-list identifier (<c>H5P_DATATYPE_ACCESS</c>), owned by HDF5.
        /// </summary>
        public static hid_t DATATYPE_ACCESS
        {
            get
            {
                return H5P_CLS_DATATYPE_ACCESS_g;
            }
        }

        /// <summary>
        /// Caches the exported native identifier <c>H5P_CLS_STRING_CREATE_g</c> from the selected HDF5 library.
        /// </summary>
        static readonly hid_t H5P_CLS_STRING_CREATE_g = H5DLLImporter.Instance.GetHid("H5P_CLS_STRING_CREATE_ID_g");

        /// <summary>
        /// The predefined string create property-list identifier (<c>H5P_STRING_CREATE</c>), owned by HDF5.
        /// </summary>
        public static hid_t STRING_CREATE
        {
            get
            {
                return H5P_CLS_STRING_CREATE_g;
            }
        }

        /// <summary>
        /// Caches the exported native identifier <c>H5P_CLS_ATTRIBUTE_CREATE_g</c> from the selected HDF5 library.
        /// </summary>
        static readonly hid_t H5P_CLS_ATTRIBUTE_CREATE_g = H5DLLImporter.Instance.GetHid("H5P_CLS_ATTRIBUTE_CREATE_ID_g");

        /// <summary>
        /// The predefined attribute create property-list identifier (<c>H5P_ATTRIBUTE_CREATE</c>), owned by HDF5.
        /// </summary>
        public static hid_t ATTRIBUTE_CREATE
        {
            get
            {
                return H5P_CLS_ATTRIBUTE_CREATE_g;
            }
        }

        /// <summary>
        /// Caches the exported native identifier <c>H5P_CLS_OBJECT_COPY_g</c> from the selected HDF5 library.
        /// </summary>
        static readonly hid_t H5P_CLS_OBJECT_COPY_g = H5DLLImporter.Instance.GetHid("H5P_CLS_OBJECT_COPY_ID_g");

        /// <summary>
        /// The predefined object copy property-list identifier (<c>H5P_OBJECT_COPY</c>), owned by HDF5.
        /// </summary>
        public static hid_t OBJECT_COPY
        {
            get
            {
                return H5P_CLS_OBJECT_COPY_g;
            }
        }

        /// <summary>
        /// Caches the exported native identifier <c>H5P_CLS_LINK_CREATE_g</c> from the selected HDF5 library.
        /// </summary>
        static readonly hid_t H5P_CLS_LINK_CREATE_g = H5DLLImporter.Instance.GetHid("H5P_CLS_LINK_CREATE_ID_g");

        /// <summary>
        /// The predefined link create property-list identifier (<c>H5P_LINK_CREATE</c>), owned by HDF5.
        /// </summary>
        public static hid_t LINK_CREATE
        {
            get
            {
                return H5P_CLS_LINK_CREATE_g;
            }
        }

        /// <summary>
        /// Caches the exported native identifier <c>H5P_CLS_LINK_ACCESS_g</c> from the selected HDF5 library.
        /// </summary>
        static readonly hid_t H5P_CLS_LINK_ACCESS_g = H5DLLImporter.Instance.GetHid("H5P_CLS_LINK_ACCESS_ID_g");

        /// <summary>
        /// The predefined link access property-list identifier (<c>H5P_LINK_ACCESS</c>), owned by HDF5.
        /// </summary>
        public static hid_t LINK_ACCESS
        {
            get
            {
                return H5P_CLS_LINK_ACCESS_g;
            }
        }

        /// <summary>
        /// Caches the exported native identifier <c>H5P_LST_FILE_CREATE_g</c> from the selected HDF5 library.
        /// </summary>
        static readonly hid_t H5P_LST_FILE_CREATE_g = H5DLLImporter.Instance.GetHid("H5P_LST_FILE_CREATE_ID_g");

        /// <summary>
        /// The predefined lst file create property-list identifier (<c>H5P_LST_FILE_CREATE</c>), owned by HDF5.
        /// </summary>
        public static hid_t LST_FILE_CREATE
        {
            get
            {
                return H5P_LST_FILE_CREATE_g;
            }
        }

        /// <summary>
        /// Caches the exported native identifier <c>H5P_LST_FILE_ACCESS_g</c> from the selected HDF5 library.
        /// </summary>
        static readonly hid_t H5P_LST_FILE_ACCESS_g = H5DLLImporter.Instance.GetHid("H5P_LST_FILE_ACCESS_ID_g");

        /// <summary>
        /// The predefined lst file access property-list identifier (<c>H5P_LST_FILE_ACCESS</c>), owned by HDF5.
        /// </summary>
        public static hid_t LST_FILE_ACCESS
        {
            get
            {
                return H5P_LST_FILE_ACCESS_g;
            }
        }

        /// <summary>
        /// Caches the exported native identifier <c>H5P_LST_DATASET_CREATE_g</c> from the selected HDF5 library.
        /// </summary>
        static readonly hid_t H5P_LST_DATASET_CREATE_g = H5DLLImporter.Instance.GetHid("H5P_LST_DATASET_CREATE_ID_g");

        /// <summary>
        /// The predefined lst dataset create property-list identifier (<c>H5P_LST_DATASET_CREATE</c>), owned by HDF5.
        /// </summary>
        public static hid_t LST_DATASET_CREATE
        {
            get
            {
                return H5P_LST_DATASET_CREATE_g;
            }
        }

        /// <summary>
        /// Caches the exported native identifier <c>H5P_LST_DATASET_ACCESS_g</c> from the selected HDF5 library.
        /// </summary>
        static readonly hid_t H5P_LST_DATASET_ACCESS_g = H5DLLImporter.Instance.GetHid("H5P_LST_DATASET_ACCESS_ID_g");

        /// <summary>
        /// The predefined lst dataset access property-list identifier (<c>H5P_LST_DATASET_ACCESS</c>), owned by HDF5.
        /// </summary>
        public static hid_t LST_DATASET_ACCESS
        {
            get
            {
                return H5P_LST_DATASET_ACCESS_g;
            }
        }

        /// <summary>
        /// Caches the exported native identifier <c>H5P_LST_DATASET_XFER_g</c> from the selected HDF5 library.
        /// </summary>
        static readonly hid_t H5P_LST_DATASET_XFER_g = H5DLLImporter.Instance.GetHid("H5P_LST_DATASET_XFER_ID_g");

        /// <summary>
        /// The predefined lst dataset xfer property-list identifier (<c>H5P_LST_DATASET_XFER</c>), owned by HDF5.
        /// </summary>
        public static hid_t LST_DATASET_XFER
        {
            get
            {
                return H5P_LST_DATASET_XFER_g;
            }
        }

        /// <summary>
        /// Caches the exported native identifier <c>H5P_LST_FILE_MOUNT_g</c> from the selected HDF5 library.
        /// </summary>
        static readonly hid_t H5P_LST_FILE_MOUNT_g = H5DLLImporter.Instance.GetHid("H5P_LST_FILE_MOUNT_ID_g");

        /// <summary>
        /// The predefined lst file mount property-list identifier (<c>H5P_LST_FILE_MOUNT</c>), owned by HDF5.
        /// </summary>
        public static hid_t LST_FILE_MOUNT
        {
            get
            {
                return H5P_LST_FILE_MOUNT_g;
            }
        }

        /// <summary>
        /// Caches the exported native identifier <c>H5P_LST_GROUP_CREATE_g</c> from the selected HDF5 library.
        /// </summary>
        static readonly hid_t H5P_LST_GROUP_CREATE_g = H5DLLImporter.Instance.GetHid("H5P_LST_GROUP_CREATE_ID_g");

        /// <summary>
        /// The predefined lst group create property-list identifier (<c>H5P_LST_GROUP_CREATE</c>), owned by HDF5.
        /// </summary>
        public static hid_t LST_GROUP_CREATE
        {
            get
            {
                return H5P_LST_GROUP_CREATE_g;
            }
        }

        /// <summary>
        /// Caches the exported native identifier <c>H5P_LST_GROUP_ACCESS_g</c> from the selected HDF5 library.
        /// </summary>
        static readonly hid_t H5P_LST_GROUP_ACCESS_g = H5DLLImporter.Instance.GetHid("H5P_LST_GROUP_ACCESS_ID_g");

        /// <summary>
        /// The predefined lst group access property-list identifier (<c>H5P_LST_GROUP_ACCESS</c>), owned by HDF5.
        /// </summary>
        public static hid_t LST_GROUP_ACCESS
        {
            get
            {
                return H5P_LST_GROUP_ACCESS_g;
            }
        }

        /// <summary>
        /// Caches the exported native identifier <c>H5P_LST_DATATYPE_CREATE_g</c> from the selected HDF5 library.
        /// </summary>
        static readonly hid_t H5P_LST_DATATYPE_CREATE_g = H5DLLImporter.Instance.GetHid("H5P_LST_DATATYPE_CREATE_ID_g");

        /// <summary>
        /// The predefined lst datatype create property-list identifier (<c>H5P_LST_DATATYPE_CREATE</c>), owned by HDF5.
        /// </summary>
        public static hid_t LST_DATATYPE_CREATE
        {
            get
            {
                return H5P_LST_DATATYPE_CREATE_g;
            }
        }

        /// <summary>
        /// Caches the exported native identifier <c>H5P_LST_DATATYPE_ACCESS_g</c> from the selected HDF5 library.
        /// </summary>
        static readonly hid_t H5P_LST_DATATYPE_ACCESS_g = H5DLLImporter.Instance.GetHid("H5P_LST_DATATYPE_ACCESS_ID_g");

        /// <summary>
        /// The predefined lst datatype access property-list identifier (<c>H5P_LST_DATATYPE_ACCESS</c>), owned by HDF5.
        /// </summary>
        public static hid_t LST_DATATYPE_ACCESS
        {
            get
            {
                return H5P_LST_DATATYPE_ACCESS_g;
            }
        }

        /// <summary>
        /// Caches the exported native identifier <c>H5P_LST_ATTRIBUTE_CREATE_g</c> from the selected HDF5 library.
        /// </summary>
        static readonly hid_t H5P_LST_ATTRIBUTE_CREATE_g = H5DLLImporter.Instance.GetHid("H5P_LST_ATTRIBUTE_CREATE_ID_g");

        /// <summary>
        /// The predefined lst attribute create property-list identifier (<c>H5P_LST_ATTRIBUTE_CREATE</c>), owned by HDF5.
        /// </summary>
        public static hid_t LST_ATTRIBUTE_CREATE
        {
            get
            {
                return H5P_LST_ATTRIBUTE_CREATE_g;
            }
        }

        /// <summary>
        /// Caches the exported native identifier <c>H5P_LST_OBJECT_COPY_g</c> from the selected HDF5 library.
        /// </summary>
        static readonly hid_t H5P_LST_OBJECT_COPY_g = H5DLLImporter.Instance.GetHid("H5P_LST_OBJECT_COPY_ID_g");

        /// <summary>
        /// The predefined lst object copy property-list identifier (<c>H5P_LST_OBJECT_COPY</c>), owned by HDF5.
        /// </summary>
        public static hid_t LST_OBJECT_COPY
        {
            get
            {
                return H5P_LST_OBJECT_COPY_g;
            }
        }

        /// <summary>
        /// Caches the exported native identifier <c>H5P_LST_LINK_CREATE_g</c> from the selected HDF5 library.
        /// </summary>
        static readonly hid_t H5P_LST_LINK_CREATE_g = H5DLLImporter.Instance.GetHid("H5P_LST_LINK_CREATE_ID_g");

        /// <summary>
        /// The predefined lst link create property-list identifier (<c>H5P_LST_LINK_CREATE</c>), owned by HDF5.
        /// </summary>
        public static hid_t LST_LINK_CREATE
        {
            get
            {
                return H5P_LST_LINK_CREATE_g;
            }
        }

        /// <summary>
        /// Caches the exported native identifier <c>H5P_LST_LINK_ACCESS_g</c> from the selected HDF5 library.
        /// </summary>
        static readonly hid_t H5P_LST_LINK_ACCESS_g = H5DLLImporter.Instance.GetHid("H5P_LST_LINK_ACCESS_ID_g");

        /// <summary>
        /// The predefined lst link access property-list identifier (<c>H5P_LST_LINK_ACCESS</c>), owned by HDF5.
        /// </summary>
        public static hid_t LST_LINK_ACCESS
        {
            get
            {
                return H5P_LST_LINK_ACCESS_g;
            }
        }
    }
}

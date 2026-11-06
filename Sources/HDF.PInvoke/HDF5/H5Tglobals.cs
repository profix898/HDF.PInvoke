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
    /// Provides native HDF5 datatype operations and predefined datatype identifiers.
    /// </summary>
    public unsafe sealed partial class H5T
    {

        static readonly hid_t H5T_IEEE_F32BE_g = H5DLLImporter.Instance.GetHid("H5T_IEEE_F32BE_g");

        /// <summary>
        /// The predefined 32-bit big-endian IEEE floating-point datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t IEEE_F32BE
        {
            get
            {
                return H5T_IEEE_F32BE_g;
            }
        }

        static readonly hid_t H5T_IEEE_F32LE_g = H5DLLImporter.Instance.GetHid("H5T_IEEE_F32LE_g");

        /// <summary>
        /// The predefined 32-bit little-endian IEEE floating-point datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t IEEE_F32LE
        {
            get
            {
                return H5T_IEEE_F32LE_g;
            }
        }

        static readonly hid_t H5T_IEEE_F64BE_g = H5DLLImporter.Instance.GetHid("H5T_IEEE_F64BE_g");

        /// <summary>
        /// The predefined 64-bit big-endian IEEE floating-point datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t IEEE_F64BE
        {
            get
            {
                return H5T_IEEE_F64BE_g;
            }
        }

        static readonly hid_t H5T_IEEE_F64LE_g = H5DLLImporter.Instance.GetHid("H5T_IEEE_F64LE_g");

        /// <summary>
        /// The predefined 64-bit little-endian IEEE floating-point datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t IEEE_F64LE
        {
            get
            {
                return H5T_IEEE_F64LE_g;
            }
        }

        static readonly hid_t H5T_STD_I8BE_g = H5DLLImporter.Instance.GetHid("H5T_STD_I8BE_g");

        /// <summary>
        /// The predefined 8-bit big-endian signed integer datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t STD_I8BE
        {
            get
            {
                return H5T_STD_I8BE_g;
            }
        }

        static readonly hid_t H5T_STD_I8LE_g = H5DLLImporter.Instance.GetHid("H5T_STD_I8LE_g");

        /// <summary>
        /// The predefined 8-bit little-endian signed integer datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t STD_I8LE
        {
            get
            {
                return H5T_STD_I8LE_g;
            }
        }

        static readonly hid_t H5T_STD_I16BE_g = H5DLLImporter.Instance.GetHid("H5T_STD_I16BE_g");

        /// <summary>
        /// The predefined 16-bit big-endian signed integer datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t STD_I16BE
        {
            get
            {
                return H5T_STD_I16BE_g;
            }
        }

        static readonly hid_t H5T_STD_I16LE_g = H5DLLImporter.Instance.GetHid("H5T_STD_I16LE_g");

        /// <summary>
        /// The predefined 16-bit little-endian signed integer datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t STD_I16LE
        {
            get
            {
                return H5T_STD_I16LE_g;
            }
        }

        static readonly hid_t H5T_STD_I32BE_g = H5DLLImporter.Instance.GetHid("H5T_STD_I32BE_g");

        /// <summary>
        /// The predefined 32-bit big-endian signed integer datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t STD_I32BE
        {
            get
            {
                return H5T_STD_I32BE_g;
            }
        }

        static readonly hid_t H5T_STD_I32LE_g = H5DLLImporter.Instance.GetHid("H5T_STD_I32LE_g");

        /// <summary>
        /// The predefined 32-bit little-endian signed integer datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t STD_I32LE
        {
            get
            {
                return H5T_STD_I32LE_g;
            }
        }

        static readonly hid_t H5T_STD_I64BE_g = H5DLLImporter.Instance.GetHid("H5T_STD_I64BE_g");

        /// <summary>
        /// The predefined 64-bit big-endian signed integer datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t STD_I64BE
        {
            get
            {
                return H5T_STD_I64BE_g;
            }
        }

        static readonly hid_t H5T_STD_I64LE_g = H5DLLImporter.Instance.GetHid("H5T_STD_I64LE_g");

        /// <summary>
        /// The predefined 64-bit little-endian signed integer datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t STD_I64LE
        {
            get
            {
                return H5T_STD_I64LE_g;
            }
        }

        static readonly hid_t H5T_STD_U8BE_g = H5DLLImporter.Instance.GetHid("H5T_STD_U8BE_g");

        /// <summary>
        /// The predefined 8-bit big-endian unsigned integer datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t STD_U8BE
        {
            get
            {
                return H5T_STD_U8BE_g;
            }
        }

        static readonly hid_t H5T_STD_U8LE_g = H5DLLImporter.Instance.GetHid("H5T_STD_U8LE_g");

        /// <summary>
        /// The predefined 8-bit little-endian unsigned integer datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t STD_U8LE
        {
            get
            {
                return H5T_STD_U8LE_g;
            }
        }

        static readonly hid_t H5T_STD_U16BE_g = H5DLLImporter.Instance.GetHid("H5T_STD_U16BE_g");

        /// <summary>
        /// The predefined 16-bit big-endian unsigned integer datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t STD_U16BE
        {
            get
            {
                return H5T_STD_U16BE_g;
            }
        }

        static readonly hid_t H5T_STD_U16LE_g = H5DLLImporter.Instance.GetHid("H5T_STD_U16LE_g");

        /// <summary>
        /// The predefined 16-bit little-endian unsigned integer datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t STD_U16LE
        {
            get
            {
                return H5T_STD_U16LE_g;
            }
        }

        static readonly hid_t H5T_STD_U32BE_g = H5DLLImporter.Instance.GetHid("H5T_STD_U32BE_g");

        /// <summary>
        /// The predefined 32-bit big-endian unsigned integer datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t STD_U32BE
        {
            get
            {
                return H5T_STD_U32BE_g;
            }
        }

        static readonly hid_t H5T_STD_U32LE_g = H5DLLImporter.Instance.GetHid("H5T_STD_U32LE_g");

        /// <summary>
        /// The predefined 32-bit little-endian unsigned integer datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t STD_U32LE
        {
            get
            {
                return H5T_STD_U32LE_g;
            }
        }

        static readonly hid_t H5T_STD_U64BE_g = H5DLLImporter.Instance.GetHid("H5T_STD_U64BE_g");

        /// <summary>
        /// The predefined 64-bit big-endian unsigned integer datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t STD_U64BE
        {
            get
            {
                return H5T_STD_U64BE_g;
            }
        }

        static readonly hid_t H5T_STD_U64LE_g = H5DLLImporter.Instance.GetHid("H5T_STD_U64LE_g");

        /// <summary>
        /// The predefined 64-bit little-endian unsigned integer datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t STD_U64LE
        {
            get
            {
                return H5T_STD_U64LE_g;
            }
        }

        static readonly hid_t H5T_STD_B8BE_g = H5DLLImporter.Instance.GetHid("H5T_STD_B8BE_g");

        /// <summary>
        /// The predefined 8-bit big-endian bitfield datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t STD_B8BE
        {
            get
            {
                return H5T_STD_B8BE_g;
            }
        }

        static readonly hid_t H5T_STD_B8LE_g = H5DLLImporter.Instance.GetHid("H5T_STD_B8LE_g");

        /// <summary>
        /// The predefined 8-bit little-endian bitfield datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t STD_B8LE
        {
            get
            {
                return H5T_STD_B8LE_g;
            }
        }

        static readonly hid_t H5T_STD_B16BE_g = H5DLLImporter.Instance.GetHid("H5T_STD_B16BE_g");

        /// <summary>
        /// The predefined 16-bit big-endian bitfield datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t STD_B16BE
        {
            get
            {
                return H5T_STD_B16BE_g;
            }
        }

        static readonly hid_t H5T_STD_B16LE_g = H5DLLImporter.Instance.GetHid("H5T_STD_B16LE_g");

        /// <summary>
        /// The predefined 16-bit little-endian bitfield datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t STD_B16LE
        {
            get
            {
                return H5T_STD_B16LE_g;
            }
        }

        static readonly hid_t H5T_STD_B32BE_g = H5DLLImporter.Instance.GetHid("H5T_STD_B32BE_g");

        /// <summary>
        /// The predefined 32-bit big-endian bitfield datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t STD_B32BE
        {
            get
            {
                return H5T_STD_B32BE_g;
            }
        }

        static readonly hid_t H5T_STD_B32LE_g = H5DLLImporter.Instance.GetHid("H5T_STD_B32LE_g");

        /// <summary>
        /// The predefined 32-bit little-endian bitfield datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t STD_B32LE
        {
            get
            {
                return H5T_STD_B32LE_g;
            }
        }

        static readonly hid_t H5T_STD_B64BE_g = H5DLLImporter.Instance.GetHid("H5T_STD_B64BE_g");

        /// <summary>
        /// The predefined 64-bit big-endian bitfield datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t STD_B64BE
        {
            get
            {
                return H5T_STD_B64BE_g;
            }
        }

        static readonly hid_t H5T_STD_B64LE_g = H5DLLImporter.Instance.GetHid("H5T_STD_B64LE_g");

        /// <summary>
        /// The predefined 64-bit little-endian bitfield datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t STD_B64LE
        {
            get
            {
                return H5T_STD_B64LE_g;
            }
        }

        static readonly hid_t H5T_STD_REF_OBJ_g = H5DLLImporter.Instance.GetHid("H5T_STD_REF_OBJ_g");

        /// <summary>
        /// The predefined <c>H5T_STD_REF_OBJ</c> datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t STD_REF_OBJ
        {
            get
            {
                return H5T_STD_REF_OBJ_g;
            }
        }

        static readonly hid_t H5T_STD_REF_DSETREG_g = H5DLLImporter.Instance.GetHid("H5T_STD_REF_DSETREG_g");

        /// <summary>
        /// The predefined <c>H5T_STD_REF_DSETREG</c> datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t STD_REF_DSETREG
        {
            get
            {
                return H5T_STD_REF_DSETREG_g;
            }
        }

        static readonly hid_t H5T_UNIX_D32BE_g = H5DLLImporter.Instance.GetHid("H5T_UNIX_D32BE_g");

        /// <summary>
        /// The predefined <c>H5T_UNIX_D32BE</c> datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t UNIX_D32BE
        {
            get
            {
                return H5T_UNIX_D32BE_g;
            }
        }

        static readonly hid_t H5T_UNIX_D32LE_g = H5DLLImporter.Instance.GetHid("H5T_UNIX_D32LE_g");

        /// <summary>
        /// The predefined <c>H5T_UNIX_D32LE</c> datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t UNIX_D32LE
        {
            get
            {
                return H5T_UNIX_D32LE_g;
            }
        }

        static readonly hid_t H5T_UNIX_D64BE_g = H5DLLImporter.Instance.GetHid("H5T_UNIX_D64BE_g");

        /// <summary>
        /// The predefined <c>H5T_UNIX_D64BE</c> datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t UNIX_D64BE
        {
            get
            {
                return H5T_UNIX_D64BE_g;
            }
        }

        static readonly hid_t H5T_UNIX_D64LE_g = H5DLLImporter.Instance.GetHid("H5T_UNIX_D64LE_g");

        /// <summary>
        /// The predefined <c>H5T_UNIX_D64LE</c> datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t UNIX_D64LE
        {
            get
            {
                return H5T_UNIX_D64LE_g;
            }
        }

        static readonly hid_t H5T_C_S1_g = H5DLLImporter.Instance.GetHid("H5T_C_S1_g");

        /// <summary>
        /// The predefined <c>H5T_C_S1</c> datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t C_S1
        {
            get
            {
                return H5T_C_S1_g;
            }
        }

        static readonly hid_t H5T_FORTRAN_S1_g = H5DLLImporter.Instance.GetHid("H5T_FORTRAN_S1_g");

        /// <summary>
        /// The predefined <c>H5T_FORTRAN_S1</c> datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t FORTRAN_S1
        {
            get
            {
                return H5T_FORTRAN_S1_g;
            }
        }

        static readonly hid_t H5T_VAX_F32_g = H5DLLImporter.Instance.GetHid("H5T_VAX_F32_g");

        /// <summary>
        /// The predefined <c>H5T_VAX_F32</c> datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t VAX_F32
        {
            get
            {
                return H5T_VAX_F32_g;
            }
        }

        static readonly hid_t H5T_VAX_F64_g = H5DLLImporter.Instance.GetHid("H5T_VAX_F64_g");

        /// <summary>
        /// The predefined <c>H5T_VAX_F64</c> datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t VAX_F64
        {
            get
            {
                return H5T_VAX_F64_g;
            }
        }

        static readonly hid_t H5T_NATIVE_SCHAR_g = H5DLLImporter.Instance.GetHid("H5T_NATIVE_SCHAR_g");

        /// <summary>
        /// The predefined datatype identifier for the platform's native <c>SCHAR</c> representation, owned by HDF5.
        /// </summary>
        public static hid_t NATIVE_SCHAR
        {
            get
            {
                return H5T_NATIVE_SCHAR_g;
            }
        }

        static readonly hid_t H5T_NATIVE_UCHAR_g = H5DLLImporter.Instance.GetHid("H5T_NATIVE_UCHAR_g");

        /// <summary>
        /// The predefined datatype identifier for the platform's native <c>UCHAR</c> representation, owned by HDF5.
        /// </summary>
        public static hid_t NATIVE_UCHAR
        {
            get
            {
                return H5T_NATIVE_UCHAR_g;
            }
        }

        static readonly hid_t H5T_NATIVE_SHORT_g = H5DLLImporter.Instance.GetHid("H5T_NATIVE_SHORT_g");

        /// <summary>
        /// The predefined datatype identifier for the platform's native <c>SHORT</c> representation, owned by HDF5.
        /// </summary>
        public static hid_t NATIVE_SHORT
        {
            get
            {
                return H5T_NATIVE_SHORT_g;
            }
        }

        static readonly hid_t H5T_NATIVE_USHORT_g = H5DLLImporter.Instance.GetHid("H5T_NATIVE_USHORT_g");

        /// <summary>
        /// The predefined datatype identifier for the platform's native <c>USHORT</c> representation, owned by HDF5.
        /// </summary>
        public static hid_t NATIVE_USHORT
        {
            get
            {
                return H5T_NATIVE_USHORT_g;
            }
        }

        static readonly hid_t H5T_NATIVE_INT_g = H5DLLImporter.Instance.GetHid("H5T_NATIVE_INT_g");

        /// <summary>
        /// The predefined datatype identifier for the platform's native <c>INT</c> representation, owned by HDF5.
        /// </summary>
        public static hid_t NATIVE_INT
        {
            get
            {
                return H5T_NATIVE_INT_g;
            }
        }

        static readonly hid_t H5T_NATIVE_UINT_g = H5DLLImporter.Instance.GetHid("H5T_NATIVE_UINT_g");

        /// <summary>
        /// The predefined datatype identifier for the platform's native <c>UINT</c> representation, owned by HDF5.
        /// </summary>
        public static hid_t NATIVE_UINT
        {
            get
            {
                return H5T_NATIVE_UINT_g;
            }
        }

        static readonly hid_t H5T_NATIVE_LONG_g = H5DLLImporter.Instance.GetHid("H5T_NATIVE_LONG_g");

        /// <summary>
        /// The predefined datatype identifier for the platform's native <c>LONG</c> representation, owned by HDF5.
        /// </summary>
        public static hid_t NATIVE_LONG
        {
            get
            {
                return H5T_NATIVE_LONG_g;
            }
        }

        static readonly hid_t H5T_NATIVE_ULONG_g = H5DLLImporter.Instance.GetHid("H5T_NATIVE_ULONG_g");

        /// <summary>
        /// The predefined datatype identifier for the platform's native <c>ULONG</c> representation, owned by HDF5.
        /// </summary>
        public static hid_t NATIVE_ULONG
        {
            get
            {
                return H5T_NATIVE_ULONG_g;
            }
        }

        static readonly hid_t H5T_NATIVE_LLONG_g = H5DLLImporter.Instance.GetHid("H5T_NATIVE_LLONG_g");

        /// <summary>
        /// The predefined datatype identifier for the platform's native <c>LLONG</c> representation, owned by HDF5.
        /// </summary>
        public static hid_t NATIVE_LLONG
        {
            get
            {
                return H5T_NATIVE_LLONG_g;
            }
        }

        static readonly hid_t H5T_NATIVE_ULLONG_g = H5DLLImporter.Instance.GetHid("H5T_NATIVE_ULLONG_g");

        /// <summary>
        /// The predefined datatype identifier for the platform's native <c>ULLONG</c> representation, owned by HDF5.
        /// </summary>
        public static hid_t NATIVE_ULLONG
        {
            get
            {
                return H5T_NATIVE_ULLONG_g;
            }
        }

        static readonly hid_t H5T_NATIVE_FLOAT_g = H5DLLImporter.Instance.GetHid("H5T_NATIVE_FLOAT_g");

        /// <summary>
        /// The predefined datatype identifier for the platform's native <c>FLOAT</c> representation, owned by HDF5.
        /// </summary>
        public static hid_t NATIVE_FLOAT
        {
            get
            {
                return H5T_NATIVE_FLOAT_g;
            }
        }

        static readonly hid_t H5T_NATIVE_DOUBLE_g = H5DLLImporter.Instance.GetHid("H5T_NATIVE_DOUBLE_g");

        /// <summary>
        /// The predefined datatype identifier for the platform's native <c>DOUBLE</c> representation, owned by HDF5.
        /// </summary>
        public static hid_t NATIVE_DOUBLE
        {
            get
            {
                return H5T_NATIVE_DOUBLE_g;
            }
        }

        static readonly hid_t H5T_NATIVE_LDOUBLE_g = H5DLLImporter.Instance.GetHid("H5T_NATIVE_LDOUBLE_g");

        /// <summary>
        /// The predefined datatype identifier for the platform's native <c>LDOUBLE</c> representation, owned by HDF5.
        /// </summary>
        public static hid_t NATIVE_LDOUBLE
        {
            get
            {
                return H5T_NATIVE_LDOUBLE_g;
            }
        }

        static readonly hid_t H5T_NATIVE_B8_g = H5DLLImporter.Instance.GetHid("H5T_NATIVE_B8_g");

        /// <summary>
        /// The predefined datatype identifier for the platform's native <c>B8</c> representation, owned by HDF5.
        /// </summary>
        public static hid_t NATIVE_B8
        {
            get
            {
                return H5T_NATIVE_B8_g;
            }
        }

        static readonly hid_t H5T_NATIVE_B16_g = H5DLLImporter.Instance.GetHid("H5T_NATIVE_B16_g");

        /// <summary>
        /// The predefined datatype identifier for the platform's native <c>B16</c> representation, owned by HDF5.
        /// </summary>
        public static hid_t NATIVE_B16
        {
            get
            {
                return H5T_NATIVE_B16_g;
            }
        }

        static readonly hid_t H5T_NATIVE_B32_g = H5DLLImporter.Instance.GetHid("H5T_NATIVE_B32_g");

        /// <summary>
        /// The predefined datatype identifier for the platform's native <c>B32</c> representation, owned by HDF5.
        /// </summary>
        public static hid_t NATIVE_B32
        {
            get
            {
                return H5T_NATIVE_B32_g;
            }
        }

        static readonly hid_t H5T_NATIVE_B64_g = H5DLLImporter.Instance.GetHid("H5T_NATIVE_B64_g");

        /// <summary>
        /// The predefined datatype identifier for the platform's native <c>B64</c> representation, owned by HDF5.
        /// </summary>
        public static hid_t NATIVE_B64
        {
            get
            {
                return H5T_NATIVE_B64_g;
            }
        }

        static readonly hid_t H5T_NATIVE_OPAQUE_g = H5DLLImporter.Instance.GetHid("H5T_NATIVE_OPAQUE_g");

        /// <summary>
        /// The predefined datatype identifier for the platform's native <c>OPAQUE</c> representation, owned by HDF5.
        /// </summary>
        public static hid_t NATIVE_OPAQUE
        {
            get
            {
                return H5T_NATIVE_OPAQUE_g;
            }
        }

        static readonly hid_t H5T_NATIVE_HADDR_g = H5DLLImporter.Instance.GetHid("H5T_NATIVE_HADDR_g");

        /// <summary>
        /// The predefined datatype identifier for the platform's native <c>HADDR</c> representation, owned by HDF5.
        /// </summary>
        public static hid_t NATIVE_HADDR
        {
            get
            {
                return H5T_NATIVE_HADDR_g;
            }
        }

        static readonly hid_t H5T_NATIVE_HSIZE_g = H5DLLImporter.Instance.GetHid("H5T_NATIVE_HSIZE_g");

        /// <summary>
        /// The predefined datatype identifier for the platform's native <c>HSIZE</c> representation, owned by HDF5.
        /// </summary>
        public static hid_t NATIVE_HSIZE
        {
            get
            {
                return H5T_NATIVE_HSIZE_g;
            }
        }

        static readonly hid_t H5T_NATIVE_HSSIZE_g = H5DLLImporter.Instance.GetHid("H5T_NATIVE_HSSIZE_g");

        /// <summary>
        /// The predefined datatype identifier for the platform's native <c>HSSIZE</c> representation, owned by HDF5.
        /// </summary>
        public static hid_t NATIVE_HSSIZE
        {
            get
            {
                return H5T_NATIVE_HSSIZE_g;
            }
        }

        static readonly hid_t H5T_NATIVE_HERR_g = H5DLLImporter.Instance.GetHid("H5T_NATIVE_HERR_g");

        /// <summary>
        /// The predefined datatype identifier for the platform's native <c>HERR</c> representation, owned by HDF5.
        /// </summary>
        public static hid_t NATIVE_HERR
        {
            get
            {
                return H5T_NATIVE_HERR_g;
            }
        }

        static readonly hid_t H5T_NATIVE_HBOOL_g = H5DLLImporter.Instance.GetHid("H5T_NATIVE_HBOOL_g");

        /// <summary>
        /// The predefined datatype identifier for the platform's native <c>HBOOL</c> representation, owned by HDF5.
        /// </summary>
        public static hid_t NATIVE_HBOOL
        {
            get
            {
                return H5T_NATIVE_HBOOL_g;
            }
        }

        static readonly hid_t H5T_NATIVE_INT8_g = H5DLLImporter.Instance.GetHid("H5T_NATIVE_INT8_g");

        /// <summary>
        /// The predefined datatype identifier for the platform's native <c>INT8</c> representation, owned by HDF5.
        /// </summary>
        public static hid_t NATIVE_INT8
        {
            get
            {
                return H5T_NATIVE_INT8_g;
            }
        }

        static readonly hid_t H5T_NATIVE_UINT8_g = H5DLLImporter.Instance.GetHid("H5T_NATIVE_UINT8_g");

        /// <summary>
        /// The predefined datatype identifier for the platform's native <c>UINT8</c> representation, owned by HDF5.
        /// </summary>
        public static hid_t NATIVE_UINT8
        {
            get
            {
                return H5T_NATIVE_UINT8_g;
            }
        }

        static readonly hid_t H5T_NATIVE_INT_LEAST8_g = H5DLLImporter.Instance.GetHid("H5T_NATIVE_INT_LEAST8_g");

        /// <summary>
        /// The predefined datatype identifier for the platform's native <c>INT_LEAST8</c> representation, owned by HDF5.
        /// </summary>
        public static hid_t NATIVE_INT_LEAST8
        {
            get
            {
                return H5T_NATIVE_INT_LEAST8_g;
            }
        }

        static readonly hid_t H5T_NATIVE_UINT_LEAST8_g = H5DLLImporter.Instance.GetHid("H5T_NATIVE_UINT_LEAST8_g");

        /// <summary>
        /// The predefined datatype identifier for the platform's native <c>UINT_LEAST8</c> representation, owned by HDF5.
        /// </summary>
        public static hid_t NATIVE_UINT_LEAST8
        {
            get
            {
                return H5T_NATIVE_UINT_LEAST8_g;
            }
        }

        static readonly hid_t H5T_NATIVE_INT_FAST8_g = H5DLLImporter.Instance.GetHid("H5T_NATIVE_INT_FAST8_g");

        /// <summary>
        /// The predefined datatype identifier for the platform's native <c>INT_FAST8</c> representation, owned by HDF5.
        /// </summary>
        public static hid_t NATIVE_INT_FAST8
        {
            get
            {
                return H5T_NATIVE_INT_FAST8_g;
            }
        }

        static readonly hid_t H5T_NATIVE_UINT_FAST8_g = H5DLLImporter.Instance.GetHid("H5T_NATIVE_UINT_FAST8_g");

        /// <summary>
        /// The predefined datatype identifier for the platform's native <c>UINT_FAST8</c> representation, owned by HDF5.
        /// </summary>
        public static hid_t NATIVE_UINT_FAST8
        {
            get
            {
                return H5T_NATIVE_UINT_FAST8_g;
            }
        }

        static readonly hid_t H5T_NATIVE_INT16_g = H5DLLImporter.Instance.GetHid("H5T_NATIVE_INT16_g");

        /// <summary>
        /// The predefined datatype identifier for the platform's native <c>INT16</c> representation, owned by HDF5.
        /// </summary>
        public static hid_t NATIVE_INT16
        {
            get
            {
                return H5T_NATIVE_INT16_g;
            }
        }

        static readonly hid_t H5T_NATIVE_UINT16_g = H5DLLImporter.Instance.GetHid("H5T_NATIVE_UINT16_g");

        /// <summary>
        /// The predefined datatype identifier for the platform's native <c>UINT16</c> representation, owned by HDF5.
        /// </summary>
        public static hid_t NATIVE_UINT16
        {
            get
            {
                return H5T_NATIVE_UINT16_g;
            }
        }

        static readonly hid_t H5T_NATIVE_INT_LEAST16_g = H5DLLImporter.Instance.GetHid("H5T_NATIVE_INT_LEAST16_g");

        /// <summary>
        /// The predefined datatype identifier for the platform's native <c>INT_LEAST16</c> representation, owned by HDF5.
        /// </summary>
        public static hid_t NATIVE_INT_LEAST16
        {
            get
            {
                return H5T_NATIVE_INT_LEAST16_g;
            }
        }

        static readonly hid_t H5T_NATIVE_UINT_LEAST16_g = H5DLLImporter.Instance.GetHid("H5T_NATIVE_UINT_LEAST16_g");

        /// <summary>
        /// The predefined datatype identifier for the platform's native <c>UINT_LEAST16</c> representation, owned by HDF5.
        /// </summary>
        public static hid_t NATIVE_UINT_LEAST16
        {
            get
            {
                return H5T_NATIVE_UINT_LEAST16_g;
            }
        }

        static readonly hid_t H5T_NATIVE_INT_FAST16_g = H5DLLImporter.Instance.GetHid("H5T_NATIVE_INT_FAST16_g");

        /// <summary>
        /// The predefined datatype identifier for the platform's native <c>INT_FAST16</c> representation, owned by HDF5.
        /// </summary>
        public static hid_t NATIVE_INT_FAST16
        {
            get
            {
                return H5T_NATIVE_INT_FAST16_g;
            }
        }

        static readonly hid_t H5T_NATIVE_UINT_FAST16_g = H5DLLImporter.Instance.GetHid("H5T_NATIVE_UINT_FAST16_g");

        /// <summary>
        /// The predefined datatype identifier for the platform's native <c>UINT_FAST16</c> representation, owned by HDF5.
        /// </summary>
        public static hid_t NATIVE_UINT_FAST16
        {
            get
            {
                return H5T_NATIVE_UINT_FAST16_g;
            }
        }

        static readonly hid_t H5T_NATIVE_INT32_g = H5DLLImporter.Instance.GetHid("H5T_NATIVE_INT32_g");

        /// <summary>
        /// The predefined datatype identifier for the platform's native <c>INT32</c> representation, owned by HDF5.
        /// </summary>
        public static hid_t NATIVE_INT32
        {
            get
            {
                return H5T_NATIVE_INT32_g;
            }
        }

        static readonly hid_t H5T_NATIVE_UINT32_g = H5DLLImporter.Instance.GetHid("H5T_NATIVE_UINT32_g");

        /// <summary>
        /// The predefined datatype identifier for the platform's native <c>UINT32</c> representation, owned by HDF5.
        /// </summary>
        public static hid_t NATIVE_UINT32
        {
            get
            {
                return H5T_NATIVE_UINT32_g;
            }
        }

        static readonly hid_t H5T_NATIVE_INT_LEAST32_g = H5DLLImporter.Instance.GetHid("H5T_NATIVE_INT_LEAST32_g");

        /// <summary>
        /// The predefined datatype identifier for the platform's native <c>INT_LEAST32</c> representation, owned by HDF5.
        /// </summary>
        public static hid_t NATIVE_INT_LEAST32
        {
            get
            {
                return H5T_NATIVE_INT_LEAST32_g;
            }
        }

        static readonly hid_t H5T_NATIVE_UINT_LEAST32_g = H5DLLImporter.Instance.GetHid("H5T_NATIVE_UINT_LEAST32_g");

        /// <summary>
        /// The predefined datatype identifier for the platform's native <c>UINT_LEAST32</c> representation, owned by HDF5.
        /// </summary>
        public static hid_t NATIVE_UINT_LEAST32
        {
            get
            {
                return H5T_NATIVE_UINT_LEAST32_g;
            }
        }

        static readonly hid_t H5T_NATIVE_INT_FAST32_g = H5DLLImporter.Instance.GetHid("H5T_NATIVE_INT_FAST32_g");

        /// <summary>
        /// The predefined datatype identifier for the platform's native <c>INT_FAST32</c> representation, owned by HDF5.
        /// </summary>
        public static hid_t NATIVE_INT_FAST32
        {
            get
            {
                return H5T_NATIVE_INT_FAST32_g;
            }
        }

        static readonly hid_t H5T_NATIVE_UINT_FAST32_g = H5DLLImporter.Instance.GetHid("H5T_NATIVE_UINT_FAST32_g");

        /// <summary>
        /// The predefined datatype identifier for the platform's native <c>UINT_FAST32</c> representation, owned by HDF5.
        /// </summary>
        public static hid_t NATIVE_UINT_FAST32
        {
            get
            {
                return H5T_NATIVE_UINT_FAST32_g;
            }
        }

        static readonly hid_t H5T_NATIVE_INT64_g = H5DLLImporter.Instance.GetHid("H5T_NATIVE_INT64_g");

        /// <summary>
        /// The predefined datatype identifier for the platform's native <c>INT64</c> representation, owned by HDF5.
        /// </summary>
        public static hid_t NATIVE_INT64
        {
            get
            {
                return H5T_NATIVE_INT64_g;
            }
        }

        static readonly hid_t H5T_NATIVE_UINT64_g = H5DLLImporter.Instance.GetHid("H5T_NATIVE_UINT64_g");

        /// <summary>
        /// The predefined datatype identifier for the platform's native <c>UINT64</c> representation, owned by HDF5.
        /// </summary>
        public static hid_t NATIVE_UINT64
        {
            get
            {
                return H5T_NATIVE_UINT64_g;
            }
        }

        static readonly hid_t H5T_NATIVE_INT_LEAST64_g = H5DLLImporter.Instance.GetHid("H5T_NATIVE_INT_LEAST64_g");

        /// <summary>
        /// The predefined datatype identifier for the platform's native <c>INT_LEAST64</c> representation, owned by HDF5.
        /// </summary>
        public static hid_t NATIVE_INT_LEAST64
        {
            get
            {
                return H5T_NATIVE_INT_LEAST64_g;
            }
        }

        static readonly hid_t H5T_NATIVE_UINT_LEAST64_g = H5DLLImporter.Instance.GetHid("H5T_NATIVE_UINT_LEAST64_g");

        /// <summary>
        /// The predefined datatype identifier for the platform's native <c>UINT_LEAST64</c> representation, owned by HDF5.
        /// </summary>
        public static hid_t NATIVE_UINT_LEAST64
        {
            get
            {
                return H5T_NATIVE_UINT_LEAST64_g;
            }
        }

        static readonly hid_t H5T_NATIVE_INT_FAST64_g = H5DLLImporter.Instance.GetHid("H5T_NATIVE_INT_FAST64_g");

        /// <summary>
        /// The predefined datatype identifier for the platform's native <c>INT_FAST64</c> representation, owned by HDF5.
        /// </summary>
        public static hid_t NATIVE_INT_FAST64
        {
            get
            {
                return H5T_NATIVE_INT_FAST64_g;
            }
        }

        static readonly hid_t H5T_NATIVE_UINT_FAST64_g = H5DLLImporter.Instance.GetHid("H5T_NATIVE_UINT_FAST64_g");

        /// <summary>
        /// The predefined datatype identifier for the platform's native <c>UINT_FAST64</c> representation, owned by HDF5.
        /// </summary>
        public static hid_t NATIVE_UINT_FAST64
        {
            get
            {
                return H5T_NATIVE_UINT_FAST64_g;
            }
        }

        /// <summary>
        /// The predefined <c>H5T_INTEL_I8</c> datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t INTEL_I8
        {
            get
            {
                return H5T_STD_I8LE_g;
            }
        }

        /// <summary>
        /// The predefined <c>H5T_INTEL_I16</c> datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t INTEL_I16
        {
            get
            {
                return H5T_STD_I16LE_g;
            }
        }

        /// <summary>
        /// The predefined <c>H5T_INTEL_I32</c> datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t INTEL_I32
        {
            get
            {
                return H5T_STD_I32LE_g;
            }
        }

        /// <summary>
        /// The predefined <c>H5T_INTEL_I64</c> datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t INTEL_I64
        {
            get
            {
                return H5T_STD_I64LE_g;
            }
        }

        /// <summary>
        /// The predefined <c>H5T_INTEL_U8</c> datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t INTEL_U8
        {
            get
            {
                return H5T_STD_U8LE_g;
            }
        }

        /// <summary>
        /// The predefined <c>H5T_INTEL_U16</c> datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t INTEL_U16
        {
            get
            {
                return H5T_STD_U16LE_g;
            }
        }

        /// <summary>
        /// The predefined <c>H5T_INTEL_U32</c> datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t INTEL_U32
        {
            get
            {
                return H5T_STD_U32LE_g;
            }
        }

        /// <summary>
        /// The predefined <c>H5T_INTEL_U64</c> datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t INTEL_U64
        {
            get
            {
                return H5T_STD_U64LE_g;
            }
        }

        /// <summary>
        /// The predefined <c>H5T_INTEL_B8</c> datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t INTEL_B8
        {
            get
            {
                return H5T_STD_B8LE_g;
            }
        }

        /// <summary>
        /// The predefined <c>H5T_INTEL_B16</c> datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t INTEL_B16
        {
            get
            {
                return H5T_STD_B16LE_g;
            }
        }

        /// <summary>
        /// The predefined <c>H5T_INTEL_B32</c> datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t INTEL_B32
        {
            get
            {
                return H5T_STD_B32LE_g;
            }
        }

        /// <summary>
        /// The predefined <c>H5T_INTEL_B64</c> datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t INTEL_B64
        {
            get
            {
                return H5T_STD_B64LE_g;
            }
        }

        /// <summary>
        /// The predefined <c>H5T_INTEL_F32</c> datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t INTEL_F32
        {
            get
            {
                return H5T_IEEE_F32LE_g;
            }
        }

        /// <summary>
        /// The predefined <c>H5T_INTEL_F64</c> datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t INTEL_F64
        {
            get
            {
                return H5T_IEEE_F64LE_g;
            }
        }

        /// <summary>
        /// The predefined <c>H5T_ALPHA_I8</c> datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t ALPHA_I8
        {
            get
            {
                return H5T_STD_I8LE_g;
            }
        }

        /// <summary>
        /// The predefined <c>H5T_ALPHA_I16</c> datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t ALPHA_I16
        {
            get
            {
                return H5T_STD_I16LE_g;
            }
        }

        /// <summary>
        /// The predefined <c>H5T_ALPHA_I32</c> datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t ALPHA_I32
        {
            get
            {
                return H5T_STD_I32LE_g;
            }
        }

        /// <summary>
        /// The predefined <c>H5T_ALPHA_I64</c> datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t ALPHA_I64
        {
            get
            {
                return H5T_STD_I64LE_g;
            }
        }

        /// <summary>
        /// The predefined <c>H5T_ALPHA_U8</c> datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t ALPHA_U8
        {
            get
            {
                return H5T_STD_U8LE_g;
            }
        }

        /// <summary>
        /// The predefined <c>H5T_ALPHA_U16</c> datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t ALPHA_U16
        {
            get
            {
                return H5T_STD_U16LE_g;
            }
        }

        /// <summary>
        /// The predefined <c>H5T_ALPHA_U32</c> datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t ALPHA_U32
        {
            get
            {
                return H5T_STD_U32LE_g;
            }
        }

        /// <summary>
        /// The predefined <c>H5T_ALPHA_U64</c> datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t ALPHA_U64
        {
            get
            {
                return H5T_STD_U64LE_g;
            }
        }

        /// <summary>
        /// The predefined <c>H5T_ALPHA_B8</c> datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t ALPHA_B8
        {
            get
            {
                return H5T_STD_B8LE_g;
            }
        }

        /// <summary>
        /// The predefined <c>H5T_ALPHA_B16</c> datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t ALPHA_B16
        {
            get
            {
                return H5T_STD_B16LE_g;
            }
        }

        /// <summary>
        /// The predefined <c>H5T_ALPHA_B32</c> datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t ALPHA_B32
        {
            get
            {
                return H5T_STD_B32LE_g;
            }
        }

        /// <summary>
        /// The predefined <c>H5T_ALPHA_B64</c> datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t ALPHA_B64
        {
            get
            {
                return H5T_STD_B64LE_g;
            }
        }

        /// <summary>
        /// The predefined <c>H5T_ALPHA_F32</c> datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t ALPHA_F32
        {
            get
            {
                return H5T_IEEE_F32LE_g;
            }
        }

        /// <summary>
        /// The predefined <c>H5T_ALPHA_F64</c> datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t ALPHA_F64
        {
            get
            {
                return H5T_IEEE_F64LE_g;
            }
        }

        /// <summary>
        /// The predefined <c>H5T_MIPS_I8</c> datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t MIPS_I8
        {
            get
            {
                return H5T_STD_I8BE_g;
            }
        }

        /// <summary>
        /// The predefined <c>H5T_MIPS_I16</c> datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t MIPS_I16
        {
            get
            {
                return H5T_STD_I16BE_g;
            }
        }

        /// <summary>
        /// The predefined <c>H5T_MIPS_I32</c> datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t MIPS_I32
        {
            get
            {
                return H5T_STD_I32BE_g;
            }
        }

        /// <summary>
        /// The predefined <c>H5T_MIPS_I64</c> datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t MIPS_I64
        {
            get
            {
                return H5T_STD_I64BE_g;
            }
        }

        /// <summary>
        /// The predefined <c>H5T_MIPS_U8</c> datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t MIPS_U8
        {
            get
            {
                return H5T_STD_U8BE_g;
            }
        }

        /// <summary>
        /// The predefined <c>H5T_MIPS_U16</c> datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t MIPS_U16
        {
            get
            {
                return H5T_STD_U16BE_g;
            }
        }

        /// <summary>
        /// The predefined <c>H5T_MIPS_U32</c> datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t MIPS_U32
        {
            get
            {
                return H5T_STD_U32BE_g;
            }
        }

        /// <summary>
        /// The predefined <c>H5T_MIPS_U64</c> datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t MIPS_U64
        {
            get
            {
                return H5T_STD_U64BE_g;
            }
        }

        /// <summary>
        /// The predefined <c>H5T_MIPS_B8</c> datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t MIPS_B8
        {
            get
            {
                return H5T_STD_B8BE_g;
            }
        }

        /// <summary>
        /// The predefined <c>H5T_MIPS_B16</c> datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t MIPS_B16
        {
            get
            {
                return H5T_STD_B16BE_g;
            }
        }

        /// <summary>
        /// The predefined <c>H5T_MIPS_B32</c> datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t MIPS_B32
        {
            get
            {
                return H5T_STD_B32BE_g;
            }
        }

        /// <summary>
        /// The predefined <c>H5T_MIPS_B64</c> datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t MIPS_B64
        {
            get
            {
                return H5T_STD_B64BE_g;
            }
        }

        /// <summary>
        /// The predefined <c>H5T_MIPS_F32</c> datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t MIPS_F32
        {
            get
            {
                return H5T_IEEE_F32BE_g;
            }
        }

        /// <summary>
        /// The predefined <c>H5T_MIPS_F64</c> datatype identifier, owned by HDF5.
        /// </summary>
        public static hid_t MIPS_F64
        {
            get
            {
                return H5T_IEEE_F64BE_g;
            }
        }

        /// <summary>
        /// The predefined datatype identifier for the platform's native <c>CHAR</c> representation, owned by HDF5.
        /// </summary>
        public static hid_t NATIVE_CHAR
        {
            get
            {
                return H5T_NATIVE_SCHAR_g;
            }
        }
    }
}

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
using System.IO;

using HDF.PInvoke;

using hid_t = System.Int64;

namespace UnitTests
{
    /// <summary>
    /// Provides native fixture creation and error-output helpers for integration tests.
    /// </summary>
    class Utilities
    {
        /// <summary>
        /// Suppresses automatic native error output for tests that intentionally exercise failure cases.
        /// </summary>
        public static void DisableErrorPrinting()
        {
            H5E.set_auto(H5E.DEFAULT, null, IntPtr.Zero);
        }

        /// <summary>
        /// Create a temporary HDF5 file IN MEMORY and return a file handle.
        /// </summary>
        public static hid_t H5TempFile(H5F.libver_t version =
            H5F.libver_t.LATEST)
        {
            var fapl = H5P.create(H5P.FILE_ACCESS);

            if (fapl < 0)
            {
                throw new ApplicationException("H5P.create failed.");
            }

            if (H5P.set_libver_bounds(fapl, version) < 0)
            {
                throw new ApplicationException("H5P.set_libver_bounds failed.");
            }

            // use the core VFD, 64K increments, no backing store
            if (H5P.set_fapl_core(fapl, new IntPtr(65536), 0) < 0)
            {
                throw new ApplicationException("H5P.set_fapl_core failed.");
            }

            var fname = Path.GetTempFileName();
            var file = H5F.create(fname, H5F.ACC_TRUNC, H5P.DEFAULT, fapl);

            if (file < 0)
            {
                throw new ApplicationException("H5F.create failed.");
            }

            if (H5P.close(fapl) < 0)
            {
                throw new ApplicationException("H5P.close failed.");
            }

            return file;
        }

        /// <summary>
        /// Create a temporary HDF5 file IN MEMORY and return its name and
        /// a file handle.
        /// </summary>
        public static hid_t H5TempFile(ref string fileName,
            H5F.libver_t version = H5F.libver_t.LATEST,
            bool backing_store = false)
        {
            var fapl = H5P.create(H5P.FILE_ACCESS);

            if (fapl < 0)
            {
                throw new ApplicationException("H5P.create failed.");
            }

            if (H5P.set_libver_bounds(fapl, version) < 0)
            {
                throw new ApplicationException("H5P.set_libver_bounds failed.");
            }

            // use the core VFD, 64K increments, no backing store
            if (H5P.set_fapl_core(fapl, new IntPtr(65536),
                (byte)(backing_store ? 1 : 0)) < 0)
            {
                throw new ApplicationException("H5P.set_fapl_core failed.");
            }

            fileName = Path.GetTempFileName();
            var file = H5F.create(fileName, H5F.ACC_TRUNC, H5P.DEFAULT, fapl);

            if (file < 0)
            {
                throw new ApplicationException("H5F.create failed.");
            }

            if (H5P.close(fapl) < 0)
            {
                throw new ApplicationException("H5P.close failed.");
            }

            return file;
        }

        /// <summary>
        /// Create a temporary HDF5 with SWMR access and return
        /// its name and a file handle.
        /// </summary>
        public static hid_t H5TempFileNoSWMR(ref string fileName)
        {
            var fapl = H5P.create(H5P.FILE_ACCESS);

            if (fapl < 0)
            {
                throw new ApplicationException("H5P.create failed.");
            }

            if (H5P.set_libver_bounds(fapl, H5F.libver_t.LATEST) < 0)
            {
                throw new ApplicationException("H5P.set_libver_bounds failed.");
            }

            if (H5P.set_fclose_degree(fapl, H5F.close_degree_t.STRONG) < 0)
            {
                throw new ApplicationException("H5P.set_fclose_degree failed.");
            }

            fileName = Path.GetTempFileName();
            var file = H5F.create(fileName, H5F.ACC_TRUNC, H5P.DEFAULT, fapl);

            if (file < 0)
            {
                throw new ApplicationException("H5F.create failed.");
            }

            if (H5P.close(fapl) < 0)
            {
                throw new ApplicationException("H5P.close failed.");
            }

            return file;
        }

        /// <summary>
        /// Create a temporary HDF5 with SWMR access and return
        /// its name and a file handle.
        /// </summary>
        public static hid_t H5TempFileSWMR(ref string fileName)
        {
            var fapl = H5P.create(H5P.FILE_ACCESS);

            if (fapl < 0)
            {
                throw new ApplicationException("H5P.create failed.");
            }

            if (H5P.set_libver_bounds(fapl, H5F.libver_t.LATEST) < 0)
            {
                throw new ApplicationException("H5P.set_libver_bounds failed.");
            }

            if (H5P.set_fclose_degree(fapl, H5F.close_degree_t.STRONG) < 0)
            {
                throw new ApplicationException("H5P.set_fclose_degree failed.");
            }

            fileName = Path.GetTempFileName();
            var file = H5F.create(fileName, H5F.ACC_TRUNC | H5F.ACC_SWMR_WRITE,
                H5P.DEFAULT, fapl);

            if (file < 0)
            {
                throw new ApplicationException("H5F.create failed.");
            }

            if (H5P.close(fapl) < 0)
            {
                throw new ApplicationException("H5P.close failed.");
            }

            return file;
        }

        /// <summary>
        /// Return a random INVALID handle.
        /// </summary>
        public static hid_t RandomInvalidHandle()
        {
            var r = new Random();
            return (hid_t)r.Next(Int32.MinValue, -44);
        }
    }
}

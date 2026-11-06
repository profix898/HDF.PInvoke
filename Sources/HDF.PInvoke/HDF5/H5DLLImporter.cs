using System;
using System.Runtime.InteropServices;
#if NETSTANDARD2_0
using NativeLibrary = HDF.PInvoke.LegacyNativeLibrary;
#endif

namespace HDF.PInvoke
{
    /// <summary>
    /// Resolves globals from the same handle used by every core P/Invoke.
    /// </summary>
    internal sealed class H5DLLImporter
    {
        /// <summary>
        /// Provides the shared accessor for identifiers exported by the selected core library.
        /// </summary>
        internal static readonly H5DLLImporter Instance = new H5DLLImporter();

        /// <summary>
        /// Retains the core library handle used to read native globals.
        /// </summary>
        private readonly IntPtr handle;

        // Prevent eager CLR initialization before a caller actually requests a global.
        /// <summary>
        /// Defers access to native globals until the importer is first used and HDF5 is initialized.
        /// </summary>
        static H5DLLImporter()
        {
        }

        /// <summary>
        /// Defers access to native globals until the importer is first used and HDF5 is initialized.
        /// </summary>
        private H5DLLImporter()
        {
            handle = NativeDependencies.GetLibraryHandle(false);

            // HDF5 initializes predefined identifiers during H5open.
            if (H5.open() < 0)
            {
                throw new InvalidOperationException("HDF5 initialization failed.");
            }
        }

        /// <summary>
        /// Gets the address of a required exported native global.
        /// </summary>
        /// <param name="name">The exported symbol name.</param>
        /// <returns>The address of the exported global.</returns>
        public IntPtr GetAddress(string name) => NativeLibrary.GetExport(handle, name);

        /// <summary>
        /// Attempts to get the address of an exported native global.
        /// </summary>
        /// <param name="name">The exported symbol name.</param>
        /// <param name="address">Receives the symbol address when found.</param>
        /// <returns>True if the symbol exists; otherwise false.</returns>
        public bool GetAddress(string name, out IntPtr address) => NativeLibrary.TryGetExport(handle, name, out address);

        /// <summary>
        /// Reads a 64-bit HDF5 identifier from an exported native global.
        /// </summary>
        /// <param name="name">The exported identifier symbol name.</param>
        /// <returns>The native identifier value.</returns>
        public long GetHid(string name) => Marshal.ReadInt64(GetAddress(name));
    }
}

using System;

namespace HDF.PInvoke
{
    // Records the shared import declaration for validation of generated bindings.
    /// <summary>
    /// Records the original library and export for validation of generated legacy bindings.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    internal sealed class LegacyImportAttribute : Attribute
    {
        /// <summary>
        /// Records the shared declaration's native library and entry point.
        /// </summary>
        public LegacyImportAttribute(string library, string entryPoint)
        {
            Library = library;
            EntryPoint = entryPoint;
        }

        /// <summary>
        /// Gets the logical native library name from the original import.
        /// </summary>
        public string Library
        {
            get;
        }

        /// <summary>
        /// Gets the native symbol resolved by the generated forwarder.
        /// </summary>
        public string EntryPoint
        {
            get;
        }
    }
}

using System;

namespace HDF.PInvoke
{
    // Records the shared import declaration for validation of generated bindings.
    [AttributeUsage(AttributeTargets.Method)]
    internal sealed class LegacyImportAttribute : Attribute
    {
        public LegacyImportAttribute(string library, string entryPoint)
        {
            Library = library;
            EntryPoint = entryPoint;
        }

        public string Library
        {
            get;
        }

        public string EntryPoint
        {
            get;
        }
    }
}

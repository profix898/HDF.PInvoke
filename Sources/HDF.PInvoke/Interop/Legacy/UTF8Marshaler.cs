using System;
using System.Runtime.InteropServices;
using System.Text;

namespace HDF.PInvoke
{
    // Forward calls own their temporary buffers; callback parameters borrow native memory.
    /// <summary>
    /// Marshals NUL-terminated UTF-8 strings with explicit owned-input and borrowed-callback lifetimes.
    /// </summary>
    internal sealed class UTF8Marshaler : ICustomMarshaler
    {
        /// <summary>
        /// Allocates and releases temporary UTF-8 buffers for managed-to-native input strings.
        /// </summary>
        private static readonly UTF8Marshaler Owned = new UTF8Marshaler(false);

        /// <summary>
        /// Decodes callback strings without freeing memory owned by HDF5.
        /// </summary>
        private static readonly UTF8Marshaler Borrowed = new UTF8Marshaler(true);

        /// <summary>
        /// Prevents native cleanup from freeing callback-owned memory.
        /// </summary>
        private readonly bool borrowed;

        /// <summary>
        /// Selects whether native cleanup owns the marshalled buffer.
        /// </summary>
        private UTF8Marshaler(bool borrowed)
        {
            this.borrowed = borrowed;
        }

        /// <summary>
        /// Selects the borrowed marshaler for callback cookies and the owned marshaler otherwise.
        /// </summary>
        public static ICustomMarshaler GetInstance(string cookie) => cookie == "borrowed" ? Borrowed : Owned;

        /// <summary>
        /// Leaves managed strings to the garbage collector because no managed cleanup is required.
        /// </summary>
        public void CleanUpManagedData(object value)
        {
        }

        /// <summary>
        /// Frees an owned temporary buffer while leaving borrowed native memory untouched.
        /// </summary>
        public void CleanUpNativeData(IntPtr value)
        {
            if (!borrowed)
            {
                Marshal.FreeHGlobal(value);
            }
        }

        /// <summary>
        /// Reports variable-sized native data as required by the custom-marshaler contract.
        /// </summary>
        public int GetNativeDataSize() => -1;

        /// <summary>
        /// Encodes a string into an owned NUL-terminated UTF-8 allocation, preserving null pointers.
        /// </summary>
        public unsafe IntPtr MarshalManagedToNative(object value)
        {
            if (value == null)
            {
                return IntPtr.Zero;
            }

            if (!(value is string text))
            {
                throw new MarshalDirectiveException("Expected a UTF-8 string.");
            }

            int length = Encoding.UTF8.GetByteCount(text);
            IntPtr buffer = Marshal.AllocHGlobal(checked(length + 1));

            try
            {
                // Encode directly into native memory without a temporary string or byte array.
                fixed (char* characters = text)
                    Encoding.UTF8.GetBytes(characters, text.Length, (byte*)buffer, length);
                Marshal.WriteByte(buffer, length, 0);
                return buffer;
            }
            catch
            {
                Marshal.FreeHGlobal(buffer);
                throw;
            }
        }

        /// <summary>
        /// Decodes a borrowed NUL-terminated UTF-8 pointer without taking ownership of its memory.
        /// </summary>
        public unsafe object MarshalNativeToManaged(IntPtr value)
        {
            if (value == IntPtr.Zero)
            {
                return null;
            }

            byte* bytes = (byte*)value;
            int length = 0;

            while (bytes[length] != 0)
            {
                length = checked(length + 1);
            }

            return Encoding.UTF8.GetString(bytes, length);
        }
    }
}

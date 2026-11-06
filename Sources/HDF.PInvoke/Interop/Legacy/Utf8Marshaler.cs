using System;
using System.Runtime.InteropServices;
using System.Text;

namespace HDF.PInvoke
{
    // Forward calls own their temporary buffers; callback parameters borrow native memory.
    internal sealed class Utf8Marshaler : ICustomMarshaler
    {
        private static readonly Utf8Marshaler Owned = new Utf8Marshaler(false);
        private static readonly Utf8Marshaler Borrowed = new Utf8Marshaler(true);
        private readonly bool borrowed;

        private Utf8Marshaler(bool borrowed)
        {
            this.borrowed = borrowed;
        }

        public static ICustomMarshaler GetInstance(string cookie) => cookie == "borrowed" ? Borrowed : Owned;

        public void CleanUpManagedData(object value)
        {
        }

        public void CleanUpNativeData(IntPtr value)
        {
            if (!borrowed)
            {
                Marshal.FreeHGlobal(value);
            }
        }

        public int GetNativeDataSize() => -1;

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

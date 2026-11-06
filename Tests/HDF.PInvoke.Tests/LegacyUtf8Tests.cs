#if HDF_LEGACY
using System;
using System.Runtime.InteropServices;
using System.Text;
using HDF.PInvoke;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class LegacyUtf8Tests
    {
        [TestMethod]
        public void BorrowedCleanupPreservesTheOuterCallsBuffer()
        {
            var owner = GetMarshaler(null);
            var borrower = GetMarshaler("borrowed");
            const string input = "Grüße 数据 😀 \ud800";
            string expected = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(input));
            IntPtr buffer = owner.MarshalManagedToNative(input);

            try
            {
                Assert.AreEqual(expected, borrower.MarshalNativeToManaged(buffer));
                borrower.CleanUpNativeData(buffer);
                Assert.AreEqual(expected, owner.MarshalNativeToManaged(buffer));
            }
            finally
            {
                owner.CleanUpNativeData(buffer);
            }
        }

        [TestMethod]
        public void NullAndEmptyStringsRetainDistinctNativeRepresentations()
        {
            var marshaler = GetMarshaler(null);
            Assert.AreEqual(IntPtr.Zero, marshaler.MarshalManagedToNative(null));
            Assert.IsNull(marshaler.MarshalNativeToManaged(IntPtr.Zero));
            Assert.ThrowsExactly<MarshalDirectiveException>(() => marshaler.MarshalManagedToNative(42));
            IntPtr empty = marshaler.MarshalManagedToNative(string.Empty);

            try
            {
                Assert.AreNotEqual(IntPtr.Zero, empty);
                Assert.AreEqual((byte)0, Marshal.ReadByte(empty));
                Assert.AreEqual(string.Empty, marshaler.MarshalNativeToManaged(empty));
            }
            finally
            {
                marshaler.CleanUpNativeData(empty);
            }
        }

        private static ICustomMarshaler GetMarshaler(string cookie) =>
            (ICustomMarshaler)typeof(H5).Assembly.GetType("HDF.PInvoke.Utf8Marshaler", true)
                .GetMethod("GetInstance").Invoke(null, new object[] { cookie });
    }
}
#endif

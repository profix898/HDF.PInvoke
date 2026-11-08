#if HDF_LEGACY
using System;
using System.Runtime.InteropServices;
using System.Text;
using HDF.PInvoke;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    /// <summary>
    /// Groups regression checks for Legacy UTF8.
    /// </summary>
    [TestClass]
    public class LegacyUTF8Tests
    {
        /// <summary>
        /// Verifies borrowed Cleanup Preserves The Outer Calls Buffer.
        /// </summary>
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

        /// <summary>
        /// Verifies null And Empty Strings Retain Distinct Native Representations.
        /// </summary>
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

        /// <summary>
        /// Obtains the internal UTF-8 marshaler by reflection to verify ownership without exposing a public test API.
        /// </summary>
        private static ICustomMarshaler GetMarshaler(string cookie) =>
            (ICustomMarshaler)typeof(H5).Assembly.GetType("HDF.PInvoke.UTF8Marshaler", true)
                .GetMethod("GetInstance").Invoke(null, new object[] { cookie });
    }
}
#endif

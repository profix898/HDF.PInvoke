#if NET8_0_OR_GREATER && !HDF_LEGACY
using System;
using System.Reflection;
using System.Text;
using HDF.PInvoke;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    /// <summary>Exercises the internal UTF-8 retry policy using deterministic changing native-length results.</summary>
    [TestClass]
    public class Utf8RetryTests
    {
        /// <summary>Tracks query calls without relying on concurrent access to serial native HDF5.</summary>
        private static int calls;

        /// <summary>Ensures a second growth retries instead of truncating the promised complete string.</summary>
        [TestMethod]
        public void GrowingUtf8ResultIsNotTruncated()
        {
            calls = 0;
            Assert.AreEqual(new string('x', 900), InvokeQuery(nameof(GrowingQuery)));
            Assert.AreEqual(3, calls);
        }

        /// <summary>Rejects a huge retry result before narrowing its native length to int.</summary>
        [TestMethod]
        public void OversizedRetryThrowsOverflow()
        {
            calls = 0;
            var error = Assert.ThrowsExactly<TargetInvocationException>(() => InvokeQuery(nameof(OversizedQuery)));
            Assert.IsInstanceOfType<OverflowException>(error.InnerException);
        }

        /// <summary>Invokes the production helper with its actual internal delegate type.</summary>
        private static string InvokeQuery(string method)
        {
            var helper = typeof(H5).Assembly.GetType("HDF.PInvoke.Utf8String", true);
            var query = helper.GetNestedType("Query`1", BindingFlags.NonPublic).MakeGenericType(typeof(int));
            var callback = Delegate.CreateDelegate(query, typeof(Utf8RetryTests).GetMethod(method, BindingFlags.Static | BindingFlags.NonPublic));
            return (string)helper.GetMethod("Read", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(typeof(int)).Invoke(null, new object[] { 0, callback });
        }

        /// <summary>Simulates two successive growths followed by a stable complete value.</summary>
        private static IntPtr GrowingQuery(int state, Span<byte> buffer)
        {
            calls++;
            var text = new string('x', calls == 1 ? 300 : 900);
            var count = Math.Min(text.Length, buffer.Length - 1);
            Encoding.UTF8.GetBytes(text.AsSpan(0, count), buffer);
            buffer[count] = 0;
            return (IntPtr)text.Length;
        }

        /// <summary>Simulates a length that cannot fit into managed string storage on the retry.</summary>
        private static IntPtr OversizedQuery(int state, Span<byte> buffer) => (IntPtr)(++calls == 1 ? 300L : long.MaxValue);
    }
}
#endif

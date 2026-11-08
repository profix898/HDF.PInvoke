using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using HDF.PInvoke;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    /// <summary>Validates native diagnostic APIs and managed shutdown-callback ownership.</summary>
    [TestClass]
    public class NativeLifecycleTests
    {
        /// <summary>Records native shutdown callback order without throwing through native frames.</summary>
        private static readonly List<int> Calls = new List<int>();

        /// <summary>Observes callbacks without retaining them after native shutdown.</summary>
        private static WeakReference[] callbacks;

        /// <summary>Records whether native caller state was preserved.</summary>
        private static bool invalidState;

        /// <summary>Registers a captured delegate whose only strong owner after return is the wrapper.</summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static WeakReference Register(int index)
        {
            H5.atclose_t callback = state =>
                        {
                            if (state != new IntPtr(index))
                            {
                                invalidState = true;
                            }

                            Calls.Add(index);
                        };
            Assert.IsTrue(H5.atclose(callback, new IntPtr(index)) >= 0);
            return new WeakReference(callback);
        }

        /// <summary>Checks callbacks survive collection until the assembly's explicit H5.close.</summary>
        [TestMethod]
        public void CloseCallbacksRemainRooted()
        {
            callbacks = new[] { Register(1), Register(2) };
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            Assert.IsTrue(callbacks[0].IsAlive && callbacks[1].IsAlive);
            Assert.AreEqual(0, Calls.Count);
        }

        /// <summary>Checks LIFO invocation, user state and release after successful assembly shutdown.</summary>
        internal static void VerifyAfterClose()
        {
            if (callbacks == null)
            {
                return;
            }

            CollectionAssert.AreEqual(new[] { 2, 1 }, Calls);
            Assert.IsFalse(invalidState);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            Assert.IsFalse(callbacks[0].IsAlive || callbacks[1].IsAlive);
        }

        /// <summary>Checks native version validation, driver flags and UTF-8 object comments.</summary>
        [TestMethod]
        public void VersionDriverAndObjectComments()
        {
            uint major = 0, minor = 0, release = 0;
            Assert.IsTrue(H5.get_libversion(ref major, ref minor, ref release) >= 0);
            Assert.IsTrue(H5.check_version(major, minor, release) >= 0);
            var access = H5P.create(H5P.FILE_ACCESS);
            var path = Path.Combine(Path.GetTempPath(), "comments-" + Guid.NewGuid() + ".h5");
            var file = H5F.create(path, H5F.ACC_TRUNC);
            var group = H5G.create(file, "group");

            try
            {
                Assert.IsTrue(H5FD.driver_query(H5P.get_driver(access), out var flags) >= 0);
                Assert.IsTrue(flags != 0);
                Assert.IsTrue(H5O.set_comment(group, "注释") >= 0);
                var text = new StringBuilder(64);
                Assert.IsTrue(H5O.get_comment(group, text, new IntPtr(64)).ToInt64() > 0);
                Assert.AreEqual("注释", text.ToString());
                Assert.IsTrue(H5O.set_comment_by_name(file, "group", "Grüße") >= 0);
                text.Clear();
                Assert.IsTrue(H5O.get_comment(group, text, new IntPtr(64)).ToInt64() > 0);
                Assert.AreEqual("Grüße", text.ToString());
                Assert.IsTrue(H5O.set_comment(group, null) >= 0);
                Assert.AreEqual(0L, H5O.get_comment(group, null, IntPtr.Zero).ToInt64());
            }
            finally
            {
                H5G.close(group);
                H5F.close(file);
                H5P.close(access);
                File.Delete(path);
            }
        }
    }
}


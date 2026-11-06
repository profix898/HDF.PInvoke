using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using HDF.PInvoke;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    /// <summary>
    /// Groups regression checks for Cross Platform.
    /// </summary>
    [TestClass]
    public class CrossPlatformTests
    {
        /// <summary>
        /// Verifies utf8 File And Dataset Round Trip.
        /// </summary>
        [TestMethod]
        public void Utf8FileAndDatasetRoundTrip()
        {
            string path = Path.Combine(Path.GetTempPath(), "hdf5-日本語-" + Guid.NewGuid() + ".h5");
            long file = -1, space = -1, dataset = -1;
            var data = new[] { 10, 20, 30 };
            GCHandle pinned = GCHandle.Alloc(data, GCHandleType.Pinned);

            try
            {
                file = H5F.create(path, H5F.ACC_TRUNC);
                Assert.IsTrue(file >= 0);
                space = H5S.create_simple(1, new ulong[] { 3 }, null);
                Assert.IsTrue(space >= 0);
                dataset = H5D.create(file, "δεδομένα-日本語", H5T.NATIVE_INT, space);
                Assert.IsTrue(dataset >= 0);
                Assert.IsTrue(H5D.write(dataset, H5T.NATIVE_INT, H5S.ALL, H5S.ALL, H5P.DEFAULT, pinned.AddrOfPinnedObject()) >= 0);
                Assert.IsTrue(H5D.close(dataset) >= 0);
                dataset = -1;
                Assert.IsTrue(H5F.close(file) >= 0);
                file = -1;
                file = H5F.open(path, H5F.ACC_RDONLY);
                Assert.IsTrue(file >= 0);
                dataset = H5D.open(file, "δεδομένα-日本語");
                Assert.IsTrue(dataset >= 0);
                var name = new StringBuilder(128);
                Assert.IsTrue(H5I.get_name(dataset, name, new IntPtr(128)).ToInt64() >= 0);
                Assert.AreEqual("/δεδομένα-日本語", name.ToString());
                Array.Clear(data, 0, data.Length);
                Assert.IsTrue(H5D.read(dataset, H5T.NATIVE_INT, H5S.ALL, H5S.ALL, H5P.DEFAULT, pinned.AddrOfPinnedObject()) >= 0);
                CollectionAssert.AreEqual(new[] { 10, 20, 30 }, data);
            }
            finally
            {
                if (dataset >= 0)
                {
                    H5D.close(dataset);
                }

                if (space >= 0)
                {
                    H5S.close(space);
                }

                if (file >= 0)
                {
                    H5F.close(file);
                }

                pinned.Free();
                File.Delete(path);
            }
        }

        /// <summary>
        /// Verifies native Version And Boolean Width.
        /// </summary>
        [TestMethod]
        public void NativeVersionAndBooleanWidth()
        {
            uint major = 0, minor = 0, release = 0;
            Assert.IsTrue(H5.get_libversion(ref major, ref minor, ref release) >= 0);
            Assert.AreEqual(2u, major);
            Assert.AreEqual(2u, minor);
            byte threadSafe = 255;
            Assert.IsTrue(H5.is_library_threadsafe(ref threadSafe) >= 0);
            Assert.AreEqual((byte)0, threadSafe);
        }

        /// <summary>
        /// Verifies error Messages Are Literal Utf8.
        /// </summary>
        [TestMethod]
        public void ErrorMessagesAreLiteralUtf8()
        {
            long stack = H5E.create_stack();
            Assert.IsTrue(stack >= 0);

            try
            {
                const string message = "100% ready: %s 日本語";
                Assert.IsTrue(H5E.push(stack, "file.cs", "test", 1, H5E.ERR_CLS, H5E.NONE_MAJOR, H5E.NONE_MINOR, message) >= 0);
                H5E.walk_t callback = (uint n, ref H5E.error_t error, IntPtr data) =>
                {
                    Assert.AreEqual(message, error.desc);
                    return 0;
                };
                Assert.IsTrue(H5E.walk(stack, H5E.direction_t.H5E_WALK_DOWNWARD, callback, IntPtr.Zero) >= 0);
                GC.KeepAlive(callback);
            }
            finally
            {
                H5E.close_stack(stack);
            }
        }
    }
}

using System;
using System.Runtime.InteropServices;
using HDF.PInvoke;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    /// <summary>
    /// Groups regression checks for Library.
    /// </summary>
    [TestClass]
    public class LibraryTests
    {
        /// <summary>
        /// Verifies native Allocation Can Be Zero Initialized.
        /// </summary>
        [TestMethod]
        public void NativeAllocationCanBeZeroInitialized()
        {
            var pointer = H5.allocate_memory((IntPtr)256, 1);
            Assert.AreNotEqual(IntPtr.Zero, pointer);

            try
            {
                for (var i = 0; i < 256; i++)
                {
                    Assert.AreEqual((byte)0, Marshal.ReadByte(pointer, i));
                }
            }
            finally
            {
                Assert.IsTrue(H5.free_memory(pointer) >= 0);
            }
        }

        /// <summary>
        /// Verifies native Resize Preserves Existing Bytes.
        /// </summary>
        [TestMethod]
        public void NativeResizePreservesExistingBytes()
        {
            var pointer = H5.allocate_memory((IntPtr)256, 0);
            Assert.AreNotEqual(IntPtr.Zero, pointer);

            try
            {
                for (var i = 0; i < 256; i++)
                {
                    Marshal.WriteByte(pointer, i, (byte)i);
                }

                // Preserve the original pointer if native reallocation fails.
                var resized = H5.resize_memory(pointer, (IntPtr)1024);
                Assert.AreNotEqual(IntPtr.Zero, resized);
                pointer = resized;

                for (var i = 0; i < 256; i++)
                {
                    Assert.AreEqual((byte)i, Marshal.ReadByte(pointer, i));
                }
            }
            finally
            {
                Assert.IsTrue(H5.free_memory(pointer) >= 0);
            }
        }

        /// <summary>
        /// Verifies zero Sized Native Allocations Return Null.
        /// </summary>
        [TestMethod]
        public void ZeroSizedNativeAllocationsReturnNull()
        {
            Assert.AreEqual(IntPtr.Zero, H5.allocate_memory(IntPtr.Zero, 0));
            Assert.AreEqual(IntPtr.Zero, H5.resize_memory(IntPtr.Zero, IntPtr.Zero));
            Assert.IsTrue(H5.free_memory(IntPtr.Zero) >= 0);

            var pointer = H5.resize_memory(IntPtr.Zero, (IntPtr)64);
            Assert.AreNotEqual(IntPtr.Zero, pointer);
            pointer = H5.resize_memory(pointer, IntPtr.Zero);
            Assert.AreEqual(IntPtr.Zero, pointer);
        }

        /// <summary>
        /// Verifies native Library Reports The Pinned Release Family.
        /// </summary>
        [TestMethod]
        public void NativeLibraryReportsThePinnedReleaseFamily()
        {
            uint major = 0, minor = 0, release = 0;
            Assert.IsTrue(H5.get_libversion(ref major, ref minor, ref release) >= 0);
            Assert.AreEqual(2u, major);
            Assert.AreEqual(2u, minor);
        }

        /// <summary>
        /// Verifies packaged Library Requires Serialized Calls.
        /// </summary>
        [TestMethod]
        public void PackagedLibraryRequiresSerializedCalls()
        {
            byte threadSafe = 1;
            Assert.IsTrue(H5.is_library_threadsafe(ref threadSafe) >= 0);
            Assert.AreEqual((byte)0, threadSafe);
        }

        /// <summary>
        /// Verifies native Garbage Collection Succeeds.
        /// </summary>
        [TestMethod]
        public void NativeGarbageCollectionSucceeds()
        {
            Assert.IsTrue(H5.garbage_collect() >= 0);
        }
    }
}

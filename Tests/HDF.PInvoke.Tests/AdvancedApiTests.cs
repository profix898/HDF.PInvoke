using System;
using System.Runtime.InteropServices;
using HDF.PInvoke;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    /// <summary>Validates advanced selection, property and native lifecycle APIs.</summary>
    [TestClass]
    public unsafe class AdvancedApiTests
    {
        /// <summary>Checks selection composition, projection, serialization and byte-sequence iteration.</summary>
        [TestMethod]
        public void SelectionOperationsRoundTrip()
        {
            var source = H5S.create_simple(1, new ulong[] { 8 }, null);
            var destination = H5S.copy(source);
            long combined = -1, projected = -1, hyperslab = -1, iterator = -1, decoded = -1;
            try
            {
                Assert.IsTrue(H5S.select_hyperslab(source, H5S.seloper_t.SET, new ulong[] { 2 }, null, new ulong[] { 3 }, null) >= 0);
                Assert.IsTrue(H5S.select_copy(destination, source) >= 0);
                Assert.IsTrue(H5S.select_shape_same(source, destination) > 0);
                combined = H5S.combine_select(source, H5S.seloper_t.OR, destination);
                Assert.IsTrue(combined >= 0);
                Assert.AreEqual(3L, H5S.get_select_npoints(combined));
                Assert.IsTrue(H5S.modify_select(combined, H5S.seloper_t.AND, source) >= 0);
                projected = H5S.select_project_intersection(source, destination, combined);
                Assert.IsTrue(projected >= 0);
                Assert.AreEqual(3L, H5S.get_select_npoints(projected));
                ulong start = 5, count = 1, end = 6;
                hyperslab = H5S.combine_hyperslab(source, H5S.seloper_t.OR, (IntPtr)(&start), IntPtr.Zero, (IntPtr)(&count), IntPtr.Zero);
                Assert.IsTrue(hyperslab >= 0);
                Assert.AreEqual(4L, H5S.get_select_npoints(hyperslab));
                Assert.IsTrue(H5S.select_intersect_block(hyperslab, (IntPtr)(&start), (IntPtr)(&end)) > 0);
                long adjustment = 2;
                Assert.IsTrue(H5S.select_adjust(destination, (IntPtr)(&adjustment)) >= 0);
                Assert.IsTrue(H5S.select_shape_same(source, destination) > 0);
                var size = UIntPtr.Zero;
                Assert.IsTrue(H5S.encode2(source, IntPtr.Zero, ref size) >= 0);
                var encoded = new byte[(int)size.ToUInt64()];
                fixed (byte* p = encoded)
                    Assert.IsTrue(H5S.encode2(source, (IntPtr)p, ref size) >= 0);
                decoded = H5S.decode(encoded);
                Assert.IsTrue(decoded >= 0);
                Assert.IsTrue(H5S.select_shape_same(source, decoded) > 0);
                iterator = H5S.sel_iter_create(source, new UIntPtr(4u), H5S.SEL_ITER_GET_SEQ_LIST_SORTED);
                Assert.IsTrue(iterator >= 0);
                ulong offset = 0;
                UIntPtr length = UIntPtr.Zero;
                Assert.IsTrue(H5S.sel_iter_get_seq_list(iterator, new UIntPtr(1u), new UIntPtr(8u), out var sequences, out var elements, (IntPtr)(&offset), (IntPtr)(&length)) >= 0);
                Assert.AreEqual(1UL, sequences.ToUInt64());
                Assert.AreEqual(3UL, elements.ToUInt64());
                Assert.AreEqual(8UL, offset);
                Assert.AreEqual(12UL, length.ToUInt64());
                Assert.IsTrue(H5S.sel_iter_reset(iterator, destination) >= 0);
                Assert.IsTrue(H5S.sel_iter_get_seq_list(iterator, new UIntPtr(1u), new UIntPtr(8u), out sequences, out elements, (IntPtr)(&offset), (IntPtr)(&length)) >= 0);
                Assert.AreEqual(0UL, offset);
            }
            finally
            {
                if (iterator >= 0) H5S.sel_iter_close(iterator);
                foreach (var id in new[] { decoded, hyperslab, projected, combined, destination, source }) if (id >= 0) H5S.close(id);
            }
        }

        /// <summary>Checks serialized properties and remaining serial driver/virtual-dataset controls.</summary>
        [TestMethod]
        public void PropertySerializationAndDriverControls()
        {
            var access = H5P.create(H5P.FILE_ACCESS);
            var creation = H5P.create(H5P.DATASET_ACCESS);
            var transfer = H5P.create(H5P.DATASET_XFER);
            long decoded = -1;
            try
            {
                Assert.IsTrue(H5P.set_file_locking(access, 0, 1) >= 0);
                var size = UIntPtr.Zero;
                Assert.IsTrue(H5P.encode2(access, IntPtr.Zero, ref size) >= 0);
                var bytes = new byte[(int)size.ToUInt64()];
                fixed (byte* p = bytes)
                {
                    Assert.IsTrue(H5P.encode2(access, (IntPtr)p, ref size) >= 0);
                    decoded = H5P.decode((IntPtr)p);
                }
                Assert.IsTrue(decoded >= 0);
                Assert.IsTrue(H5P.get_file_locking(decoded, out var locking, out var ignore) >= 0);
                Assert.AreEqual((byte)0, locking);
                Assert.AreEqual((byte)1, ignore);
                Assert.IsTrue(H5P.set_relax_file_integrity_checks(access, 0) >= 0);
                Assert.IsTrue(H5P.get_relax_file_integrity_checks(access, out var flags) >= 0);
                Assert.AreEqual(0UL, flags);
                Assert.IsTrue(H5P.set_virtual_spatial_tree(creation, 1) >= 0);
                Assert.IsTrue(H5P.get_virtual_spatial_tree(creation, out var tree) >= 0);
                Assert.AreEqual((byte)1, tree);
                Assert.IsTrue(H5P.set_driver_by_name(access, "sec2", null) >= 0);
                Assert.IsTrue(H5P.set_driver_by_value(access, 0, null) >= 0);
                Assert.IsTrue(H5P.get_driver_config_str(access, IntPtr.Zero, UIntPtr.Zero).ToInt64() >= 0);
                Assert.IsTrue(H5P.set_multi_type(access, H5F.mem_t.DRAW) >= 0);
                Assert.IsTrue(H5P.get_multi_type(access, out var memory) >= 0);
                Assert.AreEqual(H5F.mem_t.DRAW, memory);
                ulong start = 0, count = 2;
                Assert.IsTrue(H5P.set_dataset_io_hyperslab_selection(transfer, 1, H5S.seloper_t.SET, (IntPtr)(&start), IntPtr.Zero, (IntPtr)(&count), IntPtr.Zero) >= 0);
            }
            finally { if (decoded >= 0) H5P.close(decoded); H5P.close(transfer); H5P.close(creation); H5P.close(access); }
        }

        /// <summary>Checks stack pause/resume, stack ownership transfer and library lifecycle queries.</summary>
        [TestMethod]
        public void ErrorStacksAndLibraryState()
        {
            var first = H5E.create_stack();
            var second = H5E.create_stack();
            try
            {
                Assert.IsTrue(H5E.pause_stack(first) >= 0);
                Assert.IsTrue(H5E.is_paused(first, out var paused) >= 0);
                Assert.AreEqual((byte)1, paused);
                Assert.IsTrue(H5E.resume_stack(first) >= 0);
                Assert.IsTrue(H5E.is_paused(first, out paused) >= 0);
                Assert.AreEqual((byte)0, paused);
                Assert.IsTrue(H5E.append_stack(first, second, 1) >= 0);
                second = -1;
                Assert.IsTrue(H5.get_free_list_sizes(out _, out _, out _, out _) >= 0);
                Assert.IsTrue(H5.is_library_terminating(out var terminating) >= 0);
                Assert.AreEqual((byte)0, terminating);
            }
            finally { if (second >= 0) H5E.close_stack(second); H5E.close_stack(first); }
        }

        /// <summary>Checks custom identifier registration, synchronous iteration and final release callback.</summary>
        [TestMethod]
        public void IdentifierIterationAndRelease()
        {
            var freed = 0;
            H5I.free_t release = (IntPtr value, ref IntPtr request) => { if (value == new IntPtr(123)) freed++; return 0; };
            var type = H5I.register_type2(0, release);
            Assert.IsTrue((int)type >= 0);
            try
            {
                var id = H5I.register(type, new IntPtr(123));
                Assert.IsTrue(id >= 0);
                var visits = 0;
                H5I.iterate_t callback = (identifier, state) => { if (identifier != id || state != new IntPtr(42)) return -1; visits++; return 0; };
                Assert.IsTrue(H5I.iterate(type, callback, new IntPtr(42)) >= 0);
                Assert.AreEqual(1, visits);
                Assert.IsTrue(H5I.dec_ref(id) >= 0);
                Assert.AreEqual(1, freed);
                GC.KeepAlive(callback);
            }
            finally { H5I.destroy_type(type); GC.KeepAlive(release); }
        }
    }
}


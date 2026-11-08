using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using HDF.PInvoke;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    /// <summary>Validates event-set ownership and native-connector behavior of asynchronous entry points.</summary>
    [TestClass]
    public unsafe class EventSetApiTests
    {
        /// <summary>Owns unmanaged UTF-8 strings until all operations using them have completed.</summary>
        private sealed class Strings : IDisposable
        {
            /// <summary>Retains each unmanaged allocation for deterministic release.</summary>
            private readonly List<IntPtr> allocations = new List<IntPtr>();

            /// <summary>Allocates a null-terminated UTF-8 value with stable native lifetime.</summary>
            internal IntPtr Add(string text)
            {
                var bytes = Encoding.UTF8.GetBytes(text + "\0");
                var pointer = Marshal.AllocHGlobal(bytes.Length);
                Marshal.Copy(bytes, 0, pointer, bytes.Length);
                allocations.Add(pointer);
                return pointer;
            }

            /// <summary>Releases strings after the event set has drained.</summary>
            public void Dispose()
            {
                foreach (var pointer in allocations)
                {
                    Marshal.FreeHGlobal(pointer);
                }
            }
        }

        /// <summary>Checks every packaged asynchronous function with explicit completion boundaries and native-owned identifiers.</summary>
        [TestMethod]
        public void NativeConnectorAsyncEntryPoints()
        {
            using var strings = new Strings();
            var sourceFile = strings.Add("EventSetApiTests.cs");
            var sourceFunction = strings.Add("NativeConnectorAsyncEntryPoints");
            const uint line = 1;
            var path = Path.Combine(Path.GetTempPath(), "events-" + Guid.NewGuid() + ".h5");
            var filename = strings.Add(path);
            var groupName = strings.Add("组");
            var dataName = strings.Add("数据");
            var fullDataName = strings.Add("/组/数据");
            var dot = strings.Add(".");
            var attrName = strings.Add("attr");
            var attrRenamed = strings.Add("renamed");
            var attrSecond = strings.Add("second");
            var typeName = strings.Add("type");
            var es = H5ES.create();
            Assert.IsTrue(es >= 0);
            var owned = new Dictionary<long, Func<long, int>>();
            void Wait()
            {
                Assert.IsTrue(H5ES.wait(es, H5ES.WAIT_FOREVER, out var pending, out var failed) >= 0);
                Assert.AreEqual(0UL, pending.ToUInt64());
                Assert.AreEqual((byte)0, failed);
            }
            void Check(int result)
            {
                Assert.IsTrue(result >= 0);
                Wait();
            }
            long Own(long id, Func<long, int> close)
            {
                Assert.IsTrue(id >= 0);
                Wait();
                owned.Add(id, close);
                return id;
            }
            void Closed(long id, int result)
            {
                Check(result);
                owned.Remove(id);
            }
            H5ES.event_insert_t inserted = (ref H5ES.op_info_t info, IntPtr state) => 0;
            H5ES.event_complete_t completed = (ref H5ES.op_info_t info, H5ES.status_t status, long stack, IntPtr state) => 0;

            try
            {
                Check(H5ES.register_insert_func(es, inserted, IntPtr.Zero));
                Check(H5ES.register_complete_func(es, completed, IntPtr.Zero));
                var file = Own(H5F.create_async(sourceFile, sourceFunction, line, filename, H5F.ACC_TRUNC, H5P.DEFAULT, H5P.DEFAULT, es), H5F.close);
                var reopened = Own(H5F.reopen_async(sourceFile, sourceFunction, line, file, es), H5F.close);
                Closed(reopened, H5F.close_async(sourceFile, sourceFunction, line, reopened, es));
                var group = Own(H5G.create_async(sourceFile, sourceFunction, line, file, groupName, H5P.DEFAULT, H5P.DEFAULT, H5P.DEFAULT, es), H5G.close);
                var groupOpen = Own(H5G.open_async(sourceFile, sourceFunction, line, file, groupName, H5P.DEFAULT, es), H5G.close);
                Closed(groupOpen, H5G.close_async(sourceFile, sourceFunction, line, groupOpen, es));
                var space = Own(H5S.create_simple(1, new ulong[] { 4 }, new ulong[] { H5S.UNLIMITED }), H5S.close);
                var scalar = Own(H5S.create(H5S.class_t.SCALAR), H5S.close);
                var creation = Own(H5P.create(H5P.DATASET_CREATE), H5P.close);
                Check(H5P.set_chunk(creation, 1, new ulong[] { 4 }));
                var dataset = Own(H5D.create_async(sourceFile, sourceFunction, line, group, dataName, H5T.NATIVE_INT, space, H5P.DEFAULT, creation, H5P.DEFAULT, es), H5D.close);
                var datasetOpen = Own(H5D.open_async(sourceFile, sourceFunction, line, group, dataName, H5P.DEFAULT, es), H5D.close);
                Closed(datasetOpen, H5D.close_async(sourceFile, sourceFunction, line, datasetOpen, es));
                Own(H5D.get_space_async(sourceFile, sourceFunction, line, dataset, es), H5S.close);
                int* values = stackalloc int[4] { 10, 20, 30, 40 };
                int* output = stackalloc int[4];
                Check(H5D.write_async(sourceFile, sourceFunction, line, dataset, H5T.NATIVE_INT, H5S.ALL, H5S.ALL, H5P.DEFAULT, (IntPtr)values, es));
                Check(H5D.read_async(sourceFile, sourceFunction, line, dataset, H5T.NATIVE_INT, H5S.ALL, H5S.ALL, H5P.DEFAULT, (IntPtr)output, es));
                Assert.AreEqual(40, output[3]);
                long datasetId = dataset, typeId = H5T.NATIVE_INT, all = H5S.ALL;
                IntPtr source = (IntPtr)values, destination = (IntPtr)output;
                Check(H5D.write_multi_async(sourceFile, sourceFunction, line, new UIntPtr(1u), (IntPtr)(&datasetId), (IntPtr)(&typeId), (IntPtr)(&all), (IntPtr)(&all), H5P.DEFAULT, (IntPtr)(&source), es));
                Check(H5D.read_multi_async(sourceFile, sourceFunction, line, new UIntPtr(1u), (IntPtr)(&datasetId), (IntPtr)(&typeId), (IntPtr)(&all), (IntPtr)(&all), H5P.DEFAULT, (IntPtr)(&destination), es));
                Assert.AreEqual(30, output[2]);
                ulong extent = 8;
                Check(H5D.set_extent_async(sourceFile, sourceFunction, line, dataset, (IntPtr)(&extent), es));
                var attribute = Own(H5A.create_async(sourceFile, sourceFunction, line, dataset, attrName, H5T.NATIVE_INT, scalar, H5P.DEFAULT, H5P.DEFAULT, es), H5A.close);
                Check(H5A.write_async(sourceFile, sourceFunction, line, attribute, H5T.NATIVE_INT, (IntPtr)values, es));
                Check(H5A.read_async(sourceFile, sourceFunction, line, attribute, H5T.NATIVE_INT, (IntPtr)output, es));
                Assert.AreEqual(10, output[0]);
                byte exists = 0;
                Check(H5A.exists_async(sourceFile, sourceFunction, line, dataset, attrName, (IntPtr)(&exists), es));
                Assert.AreEqual((byte)1, exists);
                Check(H5A.exists_by_name_async(sourceFile, sourceFunction, line, file, fullDataName, attrName, (IntPtr)(&exists), H5P.DEFAULT, es));
                Assert.AreEqual((byte)1, exists);
                var attrOpen = Own(H5A.open_async(sourceFile, sourceFunction, line, dataset, attrName, H5P.DEFAULT, es), H5A.close);
                Closed(attrOpen, H5A.close_async(sourceFile, sourceFunction, line, attrOpen, es));
                Own(H5A.open_by_name_async(sourceFile, sourceFunction, line, file, fullDataName, attrName, H5P.DEFAULT, H5P.DEFAULT, es), H5A.close);
                Own(H5A.open_by_idx_async(sourceFile, sourceFunction, line, file, fullDataName, H5.index_t.NAME, H5.iter_order_t.INC, 0, H5P.DEFAULT, H5P.DEFAULT, es), H5A.close);
                Own(H5A.create_by_name_async(sourceFile, sourceFunction, line, file, fullDataName, attrSecond, H5T.NATIVE_INT, scalar, H5P.DEFAULT, H5P.DEFAULT, H5P.DEFAULT, es), H5A.close);
                Check(H5A.rename_async(sourceFile, sourceFunction, line, dataset, attrName, attrRenamed, es));
                Check(H5A.rename_by_name_async(sourceFile, sourceFunction, line, file, fullDataName, attrRenamed, attrName, H5P.DEFAULT, es));
                var type = Own(H5T.copy(H5T.NATIVE_INT), H5T.close);
                Check(H5T.commit_async(sourceFile, sourceFunction, line, file, typeName, type, H5P.DEFAULT, H5P.DEFAULT, H5P.DEFAULT, es));
                var typeOpen = Own(H5T.open_async(sourceFile, sourceFunction, line, file, typeName, H5P.DEFAULT, es), H5T.close);
                Closed(typeOpen, H5T.close_async(sourceFile, sourceFunction, line, typeOpen, es));
                var obj = Own(H5O.open_async(sourceFile, sourceFunction, line, file, fullDataName, H5P.DEFAULT, es), H5O.close);
                Closed(obj, H5O.close_async(sourceFile, sourceFunction, line, obj, es));
                Own(H5O.open_by_idx_async(sourceFile, sourceFunction, line, group, dot, H5.index_t.NAME, H5.iter_order_t.INC, 0, H5P.DEFAULT, es), H5O.close);
                byte* info = stackalloc byte[72];
                Check(H5O.get_info_by_name_async(sourceFile, sourceFunction, line, file, fullDataName, (IntPtr)info, H5O.INFO_ALL, H5P.DEFAULT, es));
                Assert.AreEqual(H5O.type_t.DATASET, H5O.read_info2((IntPtr)info).type);
                Check(H5O.copy_async(sourceFile, sourceFunction, line, group, dataName, file, strings.Add("copy"), H5P.DEFAULT, H5P.DEFAULT, es));
                Check(H5O.flush_async(sourceFile, sourceFunction, line, dataset, es));
                Check(H5O.refresh_async(sourceFile, sourceFunction, line, dataset, es));
                H5G.info_t groupInfo = default;
                Check(H5G.get_info_async(sourceFile, sourceFunction, line, group, (IntPtr)(&groupInfo), es));
                Check(H5G.get_info_by_name_async(sourceFile, sourceFunction, line, file, groupName, (IntPtr)(&groupInfo), H5P.DEFAULT, es));
                Check(H5G.get_info_by_idx_async(sourceFile, sourceFunction, line, file, dot, H5.index_t.NAME, H5.iter_order_t.INC, 2, (IntPtr)(&groupInfo), H5P.DEFAULT, es));
                var hard = strings.Add("hard");
                var soft = strings.Add("soft");
                Check(H5L.create_hard_async(sourceFile, sourceFunction, line, group, dataName, file, hard, H5P.DEFAULT, H5P.DEFAULT, es));
                Check(H5L.create_soft_async(sourceFile, sourceFunction, line, fullDataName, file, soft, H5P.DEFAULT, H5P.DEFAULT, es));
                Check(H5L.exists_async(sourceFile, sourceFunction, line, file, hard, (IntPtr)(&exists), H5P.DEFAULT, es));
                Assert.AreEqual((byte)1, exists);
                ulong index = 0;
                var visits = 0;
                H5L.iterate2_t visitor = (long id, string name, ref H5L.info2_t details, IntPtr state) =>
                {
                    visits++;
                    return 0;
                };
                Check(H5L.iterate_async(sourceFile, sourceFunction, line, file, H5.index_t.NAME, H5.iter_order_t.INC, (IntPtr)(&index), visitor, IntPtr.Zero, es));
                Assert.IsTrue(visits >= 4);
                GC.KeepAlive(visitor);
                Check(H5L.delete_async(sourceFile, sourceFunction, line, file, hard, H5P.DEFAULT, es));
                var links = Own(H5G.create(file, "links"), H5G.close);
                Check(H5L.create_hard(group, "数据", links, "one"));
                Check(H5L.delete_by_idx_async(sourceFile, sourceFunction, line, links, dot, H5.index_t.NAME, H5.iter_order_t.INC, 0, H5P.DEFAULT, es));
                Check(H5R.create_object(file, "/组/数据", H5P.DEFAULT, out var objectRef));
                Check(H5R.create_region(file, "/组/数据", space, H5P.DEFAULT, out var regionRef));
                Check(H5R.create_attr(file, "/组/数据", "attr", H5P.DEFAULT, out var attrRef));

                try
                {
                    Own(H5R.open_object_async(sourceFile, sourceFunction, line, (IntPtr)(&objectRef), H5P.DEFAULT, H5P.DEFAULT, es), H5O.close);
                    Own(H5R.open_region_async(sourceFile, sourceFunction, line, (IntPtr)(&regionRef), H5P.DEFAULT, H5P.DEFAULT, es), H5S.close);
                    Own(H5R.open_attr_async(sourceFile, sourceFunction, line, (IntPtr)(&attrRef), H5P.DEFAULT, H5P.DEFAULT, es), H5A.close);
                }
                finally
                {
                    H5R.destroy(ref attrRef);
                    H5R.destroy(ref regionRef);
                    H5R.destroy(ref objectRef);
                }

                Check(H5F.flush_async(sourceFile, sourceFunction, line, file, H5F.scope_t.GLOBAL, es));

                foreach (var pair in new Dictionary<long, Func<long, int>>(owned))
                {
                    if (pair.Key != file)
                    {
                        Check(pair.Value(pair.Key));
                        owned.Remove(pair.Key);
                    }
                }

                Closed(file, H5F.close_async(sourceFile, sourceFunction, line, file, es));
                file = Own(H5F.open_async(sourceFile, sourceFunction, line, filename, H5F.ACC_RDONLY, H5P.DEFAULT, es), H5F.close);
                Closed(file, H5F.close_async(sourceFile, sourceFunction, line, file, es));
                Check(H5ES.get_count(es, out var count));
                Assert.AreEqual(0UL, count.ToUInt64());
                Check(H5ES.get_op_counter(es, out _));
                Check(H5ES.get_err_status(es, out var error));
                Assert.AreEqual((byte)0, error);
                Check(H5ES.get_err_count(es, out count));
                Assert.AreEqual(0UL, count.ToUInt64());
                Check(H5ES.cancel(es, out count, out error));
                Assert.AreEqual(0UL, count.ToUInt64());
                H5ES.err_info_t errorInfo = default;
                Check(H5ES.get_err_info(es, new UIntPtr(1u), (IntPtr)(&errorInfo), out count));
                Assert.AreEqual(0UL, count.ToUInt64());
                Assert.IsTrue(H5ES.free_err_info(count, (IntPtr)(&errorInfo)) < 0, "HDF5 rejects an empty error record array.");
            }
            finally
            {
                H5ES.wait(es, H5ES.WAIT_FOREVER, out _, out _);

                foreach (var pair in owned)
                {
                    pair.Value(pair.Key);
                }

                H5ES.close(es);
                GC.KeepAlive(inserted);
                GC.KeepAlive(completed);

                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }
    }
}


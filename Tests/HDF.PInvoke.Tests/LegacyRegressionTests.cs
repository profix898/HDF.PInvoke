using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using HDF.PInvoke;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    /// <summary>
    /// Groups regression checks for Legacy Regression.
    /// </summary>
    [TestClass]
    public class LegacyRegressionTests
    {
        /// <summary>
        /// Verifies binding Backend Matches The Selected Target.
        /// </summary>
        [TestMethod]
        public void BindingBackendMatchesTheSelectedTarget()
        {
            var assembly = typeof(H5).Assembly;
            var framework = assembly.GetCustomAttribute<System.Runtime.Versioning.TargetFrameworkAttribute>().FrameworkName;
            var legacy = framework == ".NETStandard,Version=v2.0";
#if HDF_LEGACY
            Assert.IsTrue(legacy, "This run must exercise the .NET Standard assembly.");
#else
            Assert.IsFalse(legacy, "This run must exercise the modern assembly.");
#endif
            Assert.AreEqual(legacy, assembly.GetType("HDF.PInvoke.LegacyNativeLibrary") != null);
            var methods = assembly.GetTypes().SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static));
            var imports = methods.Count(m => m.GetCustomAttributesData().Any(a => legacy
                ? a.AttributeType.FullName == "HDF.PInvoke.LegacyImportAttribute"
                : a.AttributeType == typeof(DllImportAttribute)));
            Assert.AreEqual(719, imports);

            if (legacy)
            {
                Assert.IsNull(typeof(H5).GetMethod("open").GetCustomAttribute<DllImportAttribute>());
            }
        }

        /// <summary>
        /// Verifies uTF8 Link Class Can Be Registered Replaced And Removed.
        /// </summary>
        [TestMethod]
        public void UTF8LinkClassCanBeRegisteredReplacedAndRemoved()
        {
            var info = new H5L.class_t
            {
                version = H5L.LINK_CLASS_T_VERS,
                id = (H5L.type_t)201,
                comment = "Verknüpfung 数据",
                trav_func = (name, group, data, size, lapl, dxpl) => -1
            };

            try
            {
                Assert.IsTrue(H5L.register(ref info) >= 0);
                Assert.IsTrue(H5L.is_registered(info.id) > 0);
                info.comment = "更新";
                Assert.IsTrue(H5L.register(ref info) >= 0);
            }
            finally
            {
                Assert.IsTrue(H5L.unregister(info.id) >= 0);
                GC.KeepAlive(info);
            }

            Assert.AreEqual(0, H5L.is_registered(info.id));
        }

        /// <summary>
        /// Verifies metadata Cache Fields Survive Native Round Trip.
        /// </summary>
        [TestMethod]
        public void MetadataCacheFieldsSurviveNativeRoundTrip()
        {
            var path = Path.GetTempFileName();
            var file = H5F.create(path, H5F.ACC_TRUNC);
            Assert.IsTrue(file >= 0);

            try
            {
                var before = new H5AC.cache_config_t(H5AC.CURR_CACHE_CONFIG_VERSION);
                Assert.IsTrue(H5F.get_mdc_config(file, ref before) >= 0);
                Assert.IsTrue((long)before.epoch_length.Value > 0);
                before.min_clean_fraction = 0.25;
                Assert.IsTrue(H5F.set_mdc_config(file, ref before) >= 0);
                var after = new H5AC.cache_config_t(H5AC.CURR_CACHE_CONFIG_VERSION);
                Assert.IsTrue(H5F.get_mdc_config(file, ref after) >= 0);
                Assert.AreEqual((long)before.epoch_length.Value, (long)after.epoch_length.Value);
                Assert.AreEqual(before.incr_mode, after.incr_mode);
                Assert.AreEqual(before.lower_hr_threshold, after.lower_hr_threshold);
                Assert.AreEqual(before.metadata_write_strategy, after.metadata_write_strategy);
                Assert.AreEqual(0.25, after.min_clean_fraction);
            }
            finally
            {
                H5F.close(file);
                File.Delete(path);
            }
        }

        /// <summary>
        /// Verifies object Info File Number Is Stable Across Calls.
        /// </summary>
        [TestMethod]
        public void ObjectInfoFileNumberIsStableAcrossCalls()
        {
            var path = Path.GetTempFileName();
            var file = H5F.create(path, H5F.ACC_TRUNC);
            Assert.IsTrue(file >= 0);

            try
            {
                var first = new H5O.info_t();
                var second = new H5O.info_t();
                Assert.IsTrue(H5O.get_info(file, ref first) >= 0);
                Assert.IsTrue(H5O.get_info(file, ref second) >= 0);
                Assert.AreEqual((ulong)first.fileno.Value, (ulong)second.fileno.Value);
                Assert.AreEqual(H5O.type_t.GROUP, first.type);
                Assert.AreEqual(first.addr, second.addr);
            }
            finally
            {
                H5F.close(file);
                File.Delete(path);
            }
        }

        /// <summary>
        /// Verifies repeated UTF8 Error Callbacks Preserve Native Strings.
        /// </summary>
        [TestMethod]
        public void RepeatedUTF8ErrorCallbacksPreserveNativeStrings()
        {
            const string message = "Fehler – 数据 – δοκιμή";
            Assert.IsTrue(H5E.clear(H5E.DEFAULT) >= 0);

            try
            {
                Assert.IsTrue(H5E.push(H5E.DEFAULT, "日期.c", "prüfen", 9, H5E.ERR_CLS, H5E.NONE_MAJOR, H5E.NONE_MINOR, message) >= 0);

                for (var i = 0; i < 3; i++)
                {
                    var calls = 0;
                    H5E.walk_t callback = (uint n, ref H5E.error_t error, IntPtr data) =>
                    {
                        Assert.AreEqual(message, error.desc);
                        Assert.AreEqual("日期.c", error.file_name);
                        Assert.AreEqual("prüfen", error.func_name);
                        calls++;
                        return 0;
                    };
                    Assert.IsTrue(H5E.walk(H5E.DEFAULT, H5E.direction_t.H5E_WALK_DOWNWARD, callback, IntPtr.Zero) >= 0);
                    Assert.AreEqual(1, calls);
                    GC.KeepAlive(callback);
                }
            }
            finally
            {
                H5E.clear(H5E.DEFAULT);
            }
        }
    }
}

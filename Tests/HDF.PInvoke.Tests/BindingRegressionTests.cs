using System;
using System.IO;
using System.Runtime.CompilerServices;
using HDF.PInvoke;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    /// <summary>
    /// Groups regression checks for Binding Regression.
    /// </summary>
    [TestClass]
    public class BindingRegressionTests
    {
        /// <summary>
        /// Verifies loader Rejects Null And Blank Configuration Consistently.
        /// </summary>
        [TestMethod]
        public void LoaderRejectsNullAndBlankConfigurationConsistently()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => NativeDependencies.Configure(null));
            Assert.ThrowsExactly<ArgumentException>(() => NativeDependencies.Configure(" "));
        }

        /// <summary>
        /// Verifies metadata Cache Getter Ignores Unused Input Fields.
        /// </summary>
        [TestMethod]
        public void MetadataCacheGetterIgnoresUnusedInputFields()
        {
            string path = Path.GetTempFileName();
            long file = H5F.create(path, H5F.ACC_TRUNC);
            Assert.IsTrue(file >= 0);

            try
            {
                var config = new H5AC.cache_config_t(H5AC.CURR_CACHE_CONFIG_VERSION);
                config.min_clean_fraction = double.NaN;
#if HDF_LEGACY
                // Only version is an input to the getter, including on Windows with a 32-bit C long.
                config.epoch_length = new CLong(long.MaxValue);
#endif
                Assert.IsTrue(H5F.get_mdc_config(-1, ref config) < 0);
                Assert.IsTrue(double.IsNaN(config.min_clean_fraction));
                Assert.IsTrue(H5F.get_mdc_config(file, ref config) >= 0);
                Assert.IsFalse(double.IsNaN(config.min_clean_fraction));
                Assert.IsTrue((long)config.epoch_length.Value > 0);
            }
            finally
            {
                H5F.close(file);
                File.Delete(path);
            }
        }

        /// <summary>
        /// Verifies registered Link Callback Survives Garbage Collection.
        /// </summary>
        [TestMethod]
        public void RegisteredLinkCallbackSurvivesGarbageCollection()
        {
            string path = Path.GetTempFileName();
            long file = H5F.create(path, H5F.ACC_TRUNC);
            Assert.IsTrue(file >= 0);
            bool registered = false;
            var id = (H5L.type_t)202;

            try
            {
                long group = H5G.create(file, "target");
                Assert.IsTrue(group >= 0);
                Assert.IsTrue(H5G.close(group) >= 0);
                WeakReference callback = RegisterTemporaryCallback(id);
                registered = true;

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                Assert.IsTrue(callback.IsAlive, "Native HDF5 still owns the registration's callback pointer.");

                Assert.IsTrue(H5L.create_ud(file, "custom", id, IntPtr.Zero, IntPtr.Zero) >= 0);
                long target = H5O.open(file, "custom");
                Assert.IsTrue(target >= 0);
                Assert.IsTrue(H5O.close(target) >= 0);
                Assert.IsTrue(H5L.delete(file, "custom") >= 0);
            }
            finally
            {
                if (registered)
                {
                    Assert.IsTrue(H5L.unregister(id) >= 0);
                }

                H5F.close(file);
                File.Delete(path);
            }
        }

        /// <summary>
        /// Registers a callback with captured state so garbage-collection tests can observe its lifetime.
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static WeakReference RegisterTemporaryCallback(H5L.type_t id)
        {
            // Capture a value to avoid the compiler's permanent cache for noncapturing lambdas.
            string target = new string("target".ToCharArray());
            H5L.traverse_func_t callback = (name, group, data, size, lapl, dxpl) => H5G.open(group, target);
            var info = new H5L.class_t { version = H5L.LINK_CLASS_T_VERS, id = id, comment = "Verknüpfung 数据", trav_func = callback };
            Assert.IsTrue(H5L.register(ref info) >= 0);
            return new WeakReference(callback);
        }
    }
}

using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using HDF.PInvoke;

internal static class LoaderScenarios
{
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
    private static WeakReference RegisterShutdownCallback()
    {
        // A captured value prevents the compiler from caching the delegate for process lifetime.
        string marker = Guid.NewGuid().ToString();
        H5L.traverse_func_t callback = (name, group, data, size, lapl, dxpl) => -marker.Length;
        var registration = new H5L.class_t { version = H5L.LINK_CLASS_T_VERS, id = (H5L.type_t)203, comment = "关闭", trav_func = callback };

        if (H5L.register(ref registration) < 0)
        {
            throw new Exception("Cannot register shutdown callback.");
        }

        return new WeakReference(callback);
    }

    internal static bool Run(string[] args)
    {
        if (args.Length == 0)
        {
            return false;
        }

        if (args[0] == "--recover-loader")
        {
            NativeDependencies.Configure(Path.Combine(Path.GetTempPath(), "missing-hdf-" + Guid.NewGuid()));
            bool rejected = false;
            try
            {
                H5.open();
            }
            catch (DllNotFoundException) { rejected = true; }
            if (!rejected)
                throw new Exception("Missing native directory was accepted.");

            string directory = Path.Combine(AppContext.BaseDirectory, "runtimes", NativeDependencies.RuntimeIdentifier, "native");
            if (!Directory.Exists(directory))
                directory = AppContext.BaseDirectory;
            NativeDependencies.Configure(directory);
            if (H5.open() < 0)
                throw new Exception("Corrected native configuration could not initialize.");
            Console.WriteLine("PASS: corrected configuration recovers a failed direct initialization");
            return true;
        }

        if (args[0] == "--registered-close")
        {
            WeakReference callback = RegisterShutdownCallback();

            if (H5.close() < 0)
            {
                throw new Exception("Native shutdown failed.");
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            if (callback.IsAlive)
            {
                throw new Exception("Shutdown retained a registered callback.");
            }

            Console.WriteLine("PASS: library shutdown releases registered callbacks");
            return true;
        }

        if (args[0] == "--configure-first")
        {
            string directory = Path.Combine(AppContext.BaseDirectory, "runtimes", NativeDependencies.RuntimeIdentifier, "native");

            if (!Directory.Exists(directory))
            {
                directory = AppContext.BaseDirectory;
            }

            NativeDependencies.Configure(directory);

            if (H5.open() < 0)
            {
                throw new Exception("Configured first call failed");
            }

            Console.WriteLine("PASS: configuration precedes first native call");
            return true;
        }

        if (args[0] == "--concurrent-first")
        {
            // Serialize HDF5 calls; only loader initialization is raced.
            System.Threading.Tasks.Parallel.For(0, 16, _ => NativeDependencies.ResolvePathToExternalDependencies());

            if (H5.open() < 0)
            {
                throw new Exception("Concurrent loader initialization failed");
            }

            Console.WriteLine("PASS: concurrent loader initialization");
            return true;
        }

        if (args[0] == "--missing-direct")
        {
            NativeDependencies.Configure(args[1]);

            try
            {
                H5.open();
            }
            catch (Exception error)
            {
                while (error is TypeInitializationException && error.InnerException != null)
                {
                    error = error.InnerException;
                }

                if (error is DllNotFoundException && error.Message.Contains(args[1], StringComparison.Ordinal))
                {
                    Console.WriteLine("PASS: direct binding obeys explicit path without system fallback");
                    return true;
                }

                throw;
            }

            throw new Exception("Direct binding bypassed loader validation");
        }

        if (args[0] == "--dont-atexit" || args[0] == "--late-dont-atexit")
        {
            if (args[0] == "--dont-atexit")
            {
                if (H5.dont_atexit() != 0 || H5.dont_atexit() >= 0)
                {
                    throw new Exception("Automatic shutdown opt-out must succeed exactly once before initialization");
                }
            }

            if (H5.open() < 0 || H5.dont_atexit() >= 0 || H5.close() < 0)
            {
                throw new Exception("Late shutdown opt-out or explicit shutdown behaved incorrectly");
            }

            Console.WriteLine("PASS: automatic shutdown opt-out ordering");
            return true;
        }

        if (args[0] == "--missing" || args[0] == "--invalid")
        {
            NativeDependencies.Configure(args[1]);

            try
            {
                NativeDependencies.ResolvePathToExternalDependencies();
            }
            catch (DllNotFoundException e)
            {
                if (!e.Message.Contains(args[1], StringComparison.Ordinal))
                {
                    throw;
                }

                Console.WriteLine("PASS: explicit invalid directory fails without fallback");
                return true;
            }

            throw new Exception("Invalid explicit library location unexpectedly succeeded");
        }

        if (args[0] == "--relative")
        {
            try
            {
                NativeDependencies.Configure("relative/native");
            }
            catch (ArgumentException)
            {
                Console.WriteLine("PASS: relative configuration rejected");
                return true;
            }

            throw new Exception("Relative native directory accepted");
        }

        if (args[0] == "--incompatible")
        {
            NativeDependencies.Configure(args[1]);

            try
            {
                NativeDependencies.ResolvePathToExternalDependencies();
            }
            catch (NotSupportedException e)
            {
                if (!e.Message.Contains("1.10.0", StringComparison.Ordinal))
                {
                    throw;
                }

                Console.WriteLine("PASS: incompatible HDF5 rejected before binding globals");
                return true;
            }

            throw new Exception("An incompatible HDF5 version was accepted");
        }

        if (args[0] == "--context")
        {
            // Load a separate managed binding instance, as a plugin host would.
            var context = new AssemblyLoadContext("HDF5 plugin regression", isCollectible: true);
            Assembly plugin = context.LoadFromAssemblyPath(Path.Combine(AppContext.BaseDirectory, "HDF.PInvoke.dll"));
            Type propertyLists = plugin.GetType("HDF.PInvoke.H5P", throwOnError: true);
            long cls = (long)propertyLists.GetProperty("FILE_ACCESS").GetValue(null);
            long plist = (long)propertyLists.GetMethod("create", new[] { typeof(long) }).Invoke(null, new object[] { cls });

            if (plist < 0)
            {
                throw new Exception("Property list failed in plugin load context");
            }

            int status = (int)propertyLists.GetMethod("close").Invoke(null, new object[] { plist });

            if (status < 0)
            {
                throw new Exception("Property list cleanup failed in plugin load context");
            }

            context.Unload();
            Console.WriteLine("PASS: private managed AssemblyLoadContext resolves functions and globals");
            return true;
        }

        return false;
    }
}

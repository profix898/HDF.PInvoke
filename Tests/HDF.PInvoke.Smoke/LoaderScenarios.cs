using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using HDF.PInvoke;

internal static class LoaderScenarios
{
    internal static bool Run(string[] args)
    {
        if (args.Length == 0)
            return false;
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
                    throw;
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
            catch (ArgumentException) { Console.WriteLine("PASS: relative configuration rejected"); return true; }
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
                    throw;
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
                throw new Exception("Property list failed in plugin load context");
            int status = (int)propertyLists.GetMethod("close").Invoke(null, new object[] { plist });
            if (status < 0)
                throw new Exception("Property list cleanup failed in plugin load context");
            context.Unload();
            Console.WriteLine("PASS: private managed AssemblyLoadContext resolves functions and globals");
            return true;
        }
        return false;
    }
}

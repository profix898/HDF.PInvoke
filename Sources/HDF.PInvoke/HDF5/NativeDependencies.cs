using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace HDF.PInvoke
{
    /// <summary>Loads functions and exported globals from the same native HDF5 instance.</summary>
    public static class NativeDependencies
    {
        /// <summary>
        /// The AppContext key for an absolute directory containing private HDF5 native libraries.
        /// </summary>
        public const string NativePathSetting = "NativeDependenciesAbsolutePath";
        /// <summary>
        /// The environment variable for an absolute directory containing private HDF5 native libraries.
        /// </summary>
        public const string NativePathEnvironmentVariable = "HDF5_NATIVE_PATH";
        private static readonly object Sync = new object();
        private static IntPtr core;
        private static IntPtr highLevel;
        private static string configuredDirectory;
        private static string loadedDirectory;

        // Registration must precede any P/Invoke, including types without a static constructor.
        // Do not load HDF5 here: callers can still configure a private native directory.
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2255", Justification = "Register the native resolver before any public P/Invoke can execute.")]
        [ModuleInitializer]
        internal static void InitializeResolver() =>
            NativeLibrary.SetDllImportResolver(typeof(NativeDependencies).Assembly, Resolve);

        /// <summary>Sets an absolute directory containing matching core and high-level libraries.
        /// Call before the first HDF5 operation. Missing or invalid files never fall back to another installation.</summary>
        /// <param name="directory">The absolute directory containing compatible core and high-level native libraries.</param>
        public static void Configure(string directory)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(directory);
            if (!Path.IsPathFullyQualified(directory))
                throw new ArgumentException("The native library directory must be absolute.", nameof(directory));
            lock (Sync)
            {
                if (core != IntPtr.Zero)
                    throw new InvalidOperationException("HDF5 is already loaded; its library directory cannot be changed.");
                configuredDirectory = Path.GetFullPath(directory);
            }
        }

        /// <summary>Loads and validates the core library. Does not change PATH or the working directory.</summary>
        public static void ResolvePathToExternalDependencies() => GetLibraryHandle(false);

        /// <summary>The runtime identifier selected from the current process architecture.</summary>
        public static string RuntimeIdentifier
        {
            get
            {
                var arch = RuntimeInformation.ProcessArchitecture;
                string os = OperatingSystem.IsWindows() ? "win" : OperatingSystem.IsLinux() ? "linux" :
                    OperatingSystem.IsMacOS() ? "osx" : throw new PlatformNotSupportedException("Unsupported HDF5 operating system.");
                if (os == "linux" && RuntimeInformation.RuntimeIdentifier.StartsWith("linux-musl-", StringComparison.Ordinal))
                    os = "linux-musl";
                if ((arch != Architecture.X64 && arch != Architecture.Arm64) || (os == "osx" && arch != Architecture.Arm64))
                    throw new PlatformNotSupportedException($"HDF5 does not support {os}/{arch} in this package.");
                return os + "-" + arch.ToString().ToLowerInvariant();
            }
        }

        private static IntPtr Resolve(string name, Assembly assembly, DllImportSearchPath? searchPath) => name switch
        {
            Constants.DLLFileName or "hdf5.dll" => GetLibraryHandle(false),
            Constants.HLDLLFileName or "hdf5_hl.dll" => GetLibraryHandle(true),
            _ => IntPtr.Zero
        };

        internal static int DisableAutomaticShutdown()
        {
            lock (Sync)
            {
                if (core != IntPtr.Zero)
                    return -1;
                LoadLibraryHandle(false, true, out int status);
                return status;
            }
        }

        internal static IntPtr GetLibraryHandle(bool high) => LoadLibraryHandle(high, false, out _);

        private static IntPtr LoadLibraryHandle(bool high, bool disableAutomaticShutdown, out int shutdownStatus)
        {
            shutdownStatus = -1;
            lock (Sync)
            {
                string rid = RuntimeIdentifier;
                if (core == IntPtr.Zero)
                {
                    string directory = configuredDirectory ?? AppContext.GetData(NativePathSetting) as string ??
                        Environment.GetEnvironmentVariable(NativePathEnvironmentVariable);
                    IntPtr candidate;
                    if (!string.IsNullOrEmpty(directory))
                    {
                        if (!Path.IsPathFullyQualified(directory))
                            throw new ArgumentException($"{NativePathSetting}/{NativePathEnvironmentVariable} must be an absolute directory.");
                        loadedDirectory = Path.GetFullPath(directory);
                        candidate = LoadFromDirectory(loadedDirectory, false);
                    }
                    else
                    {
                        candidate = IntPtr.Zero;
                        foreach (string probe in ProbeDirectories(rid))
                        {
                            if (!File.Exists(Path.Combine(probe, FileName(false))))
                                continue;
                            // A broken app-local library is an error, not a reason to load a system copy.
                            loadedDirectory = probe;
                            candidate = LoadFromDirectory(probe, false);
                            break;
                        }
                        if (candidate == IntPtr.Zero)
                        {
                            loadedDirectory = null;
                            try
                            {
                                // Honors the assembly's load context and .deps.json native asset probing.
                                // This overload does not recurse into SetDllImportResolver.
                                candidate = NativeLibrary.Load(Constants.DLLFileName, typeof(NativeDependencies).Assembly,
                                    DllImportSearchPath.SafeDirectories | DllImportSearchPath.AssemblyDirectory);
                            }
                            catch (DllNotFoundException e)
                            {
                                throw new DllNotFoundException($"Cannot load HDF5 for {rid}. Deploy the matching runtimes/{rid}/native assets, " +
                                    $"or call NativeDependencies.Configure with their absolute directory. Probed: {string.Join(", ", ProbeDirectories(rid))}", e);
                            }
                        }
                    }
                    try
                    {
                        // This opt-out must precede even the version query, which initializes HDF5.
                        if (disableAutomaticShutdown)
                            shutdownStatus = Marshal.GetDelegateForFunctionPointer<DisableAtExit>(
                                NativeLibrary.GetExport(candidate, "H5dont_atexit"))();
                        ValidateVersion(candidate);
                    }
                    catch { NativeLibrary.Free(candidate); loadedDirectory = null; throw; }
                    // Retain handles for process lifetime: cached P/Invoke stubs and global IDs must remain valid.
                    core = candidate;
                }
                if (!high)
                    return core;
                if (highLevel == IntPtr.Zero)
                    highLevel = loadedDirectory != null ? LoadFromDirectory(loadedDirectory, true) :
                        NativeLibrary.Load(Constants.HLDLLFileName, typeof(NativeDependencies).Assembly,
                            DllImportSearchPath.SafeDirectories | DllImportSearchPath.AssemblyDirectory);
                return highLevel;
            }
        }

        private static IEnumerable<string> ProbeDirectories(string rid)
        {
            string assemblyDirectory = Path.GetDirectoryName(typeof(NativeDependencies).Assembly.Location);
            var seen = new HashSet<string>(OperatingSystem.IsWindows() ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal);
            foreach (string root in new[] { assemblyDirectory, AppContext.BaseDirectory })
            {
                if (string.IsNullOrEmpty(root))
                    continue;
                foreach (string path in new[] { Path.Combine(root, "runtimes", rid, "native"), root })
                    if (seen.Add(path))
                        yield return path;
            }
        }

        internal static T GetOptionalFunction<T>(string symbol) where T : Delegate
        {
            if (!NativeLibrary.TryGetExport(GetLibraryHandle(false), symbol, out IntPtr address))
                throw new PlatformNotSupportedException($"The loaded HDF5 build does not support {symbol} on {RuntimeIdentifier}.");
            return Marshal.GetDelegateForFunctionPointer<T>(address);
        }

        private static string FileName(bool high)
        {
            string name = high ? "hdf5_hl" : "hdf5";
            return OperatingSystem.IsWindows() ? name + ".dll" : "lib" + name + (OperatingSystem.IsMacOS() ? ".dylib" : ".so");
        }

        private static IntPtr LoadFromDirectory(string directory, bool high)
        {
            string file = Path.Combine(directory, FileName(high));
            if (!File.Exists(file) || new FileInfo(file).Length == 0)
                throw new DllNotFoundException($"Missing or empty HDF5 native library: {file}. Deploy real library files, not empty symlink placeholders.");
            try
            {
                return NativeLibrary.Load(file);
            }
            catch (Exception e) when (e is DllNotFoundException || e is BadImageFormatException)
            {
                throw new DllNotFoundException($"Cannot load {file} for {RuntimeIdentifier}. Check binary architecture and its native dependencies. {e.Message}", e);
            }
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int DisableAtExit();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int GetVersion(out uint major, out uint minor, out uint release);

        private static void ValidateVersion(IntPtr handle)
        {
            var getVersion = Marshal.GetDelegateForFunctionPointer<GetVersion>(NativeLibrary.GetExport(handle, "H5get_libversion"));
            if (getVersion(out uint major, out uint minor, out uint release) < 0 || major != 2 || minor != 2)
                throw new NotSupportedException($"This binding requires HDF5 2.2.x; the loaded native library reports {major}.{minor}.{release}.");
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace HDF.PInvoke
{
    /// <summary>
    /// Loads functions and exported globals from the same native HDF5 instance.
    /// </summary>
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

        /// <summary>
        /// Serializes configuration, library selection, and first-load initialization.
        /// </summary>
        private static readonly object Sync = new object();

        /// <summary>
        /// Retains the validated core library handle for the lifetime of the binding.
        /// </summary>
        private static IntPtr core;

        /// <summary>
        /// Retains the high-level library handle associated with the selected core library.
        /// </summary>
        private static IntPtr highLevel;

        /// <summary>
        /// Stores the explicit directory selected before the first native load.
        /// </summary>
        private static string configuredDirectory;

        /// <summary>
        /// Pins subsequent loads to the directory of the successfully loaded core library.
        /// </summary>
        private static string loadedDirectory;

        /// <summary>
        /// Sets an absolute directory containing matching core and high-level libraries.
        /// Call before the first HDF5 operation. Missing or invalid files never fall back to another installation.
        /// </summary>
        /// <param name="directory">The absolute directory containing compatible core and high-level native libraries.</param>
        /// <exception cref="ArgumentNullException">The directory is null.</exception>
        /// <exception cref="ArgumentException">The directory is blank or is not an absolute path.</exception>
        /// <exception cref="InvalidOperationException">The core library has already been loaded.</exception>
        public static void Configure(string directory)
        {
            if (directory == null)
            {
                throw new ArgumentNullException(nameof(directory));
            }

            if (string.IsNullOrWhiteSpace(directory))
            {
                throw new ArgumentException("A native directory is required.", nameof(directory));
            }

            if (!IsAbsolutePath(directory))
            {
                throw new ArgumentException("The native library directory must be absolute.", nameof(directory));
            }

            lock (Sync)
            {
                if (core != IntPtr.Zero)
                {
                    throw new InvalidOperationException("HDF5 is already loaded; its library directory cannot be changed.");
                }

                configuredDirectory = Path.GetFullPath(directory);
            }
        }

        /// <summary>
        /// Loads and validates the core library. Does not change PATH or the working directory.
        /// </summary>
        /// <remarks>This initializes native HDF5. Call <see cref="H5.dont_atexit"/> first if automatic native shutdown must be disabled.</remarks>
        /// <exception cref="DllNotFoundException">The selected core library or one of its dependencies cannot be loaded.</exception>
        /// <exception cref="NotSupportedException">The native library is not from the supported HDF5 release family.</exception>
        public static void ResolvePathToExternalDependencies() => GetLibraryHandle(false);

        /// <summary>
        /// The runtime identifier selected from the current process architecture.
        /// </summary>
        /// <value>The packaged runtime identifier for the current process.</value>
        public static string RuntimeIdentifier
        {
            get
            {
                var arch = RuntimeInformation.ProcessArchitecture;
                var os = LegacyNativeLibrary.IsWindows ? "win" : RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "linux" :
                    LegacyNativeLibrary.IsMacOS ? "osx" : throw new PlatformNotSupportedException("Unsupported HDF5 operating system.");

                if (os == "linux" && LegacyNativeLibrary.IsMusl)
                {
                    os = "linux-musl";
                }

                if ((arch != Architecture.X64 && arch != Architecture.Arm64) || (os == "osx" && arch != Architecture.Arm64))
                {
                    throw new PlatformNotSupportedException($"HDF5 does not support {os}/{arch} in this package.");
                }

                return os + "-" + arch.ToString().ToLowerInvariant();
            }
        }

        /// <summary>
        /// Loads HDF5 without initialization when necessary and requests suppression of its exit handler.
        /// </summary>
        internal static int DisableAutomaticShutdown()
        {
            lock (Sync)
            {
                if (core != IntPtr.Zero)
                {
                    return -1;
                }

                LoadLibraryHandle(false, true, out var status);
                return status;
            }
        }

        /// <summary>
        /// Gets the retained core or high-level handle, loading and validating it on first use.
        /// </summary>
        internal static IntPtr GetLibraryHandle(bool high) => LoadLibraryHandle(high, false, out _);

        /// <summary>
        /// Selects and validates a coherent native library pair under the configuration lock.
        /// </summary>
        private static IntPtr LoadLibraryHandle(bool high, bool disableAutomaticShutdown, out int shutdownStatus)
        {
            shutdownStatus = -1;

            lock (Sync)
            {
                var rid = RuntimeIdentifier;

                if (core == IntPtr.Zero)
                {
                    var directory = configuredDirectory ?? AppContext.GetData(NativePathSetting) as string ??
                        Environment.GetEnvironmentVariable(NativePathEnvironmentVariable);
                    IntPtr candidate;

                    if (!string.IsNullOrEmpty(directory))
                    {
                        if (!IsAbsolutePath(directory))
                        {
                            throw new ArgumentException($"{NativePathSetting}/{NativePathEnvironmentVariable} must be an absolute directory.");
                        }

                        loadedDirectory = Path.GetFullPath(directory);
                        candidate = LoadFromDirectory(loadedDirectory, false);
                    }
                    else
                    {
                        candidate = IntPtr.Zero;

                        foreach (var probe in ProbeDirectories(rid))
                        {
                            if (!File.Exists(Path.Combine(probe, FileName(false))))
                            {
                                continue;
                            }

                            // A broken app-local library is an error, not a reason to load a system copy.
                            loadedDirectory = probe;
                            candidate = LoadFromDirectory(probe, false);
                            break;
                        }

                        if (candidate == IntPtr.Zero)
                        {
                            throw new DllNotFoundException($"Cannot load HDF5 for {rid}. Deploy runtimes/{rid}/native beside the managed assembly, " +
                                $"or configure an absolute native directory. Probed: {string.Join(", ", ProbeDirectories(rid))}");
                        }
                    }

                    try
                    {
                        // This opt-out must precede even the version query, which initializes HDF5.
                        if (disableAutomaticShutdown)
                        {
                            shutdownStatus = Marshal.GetDelegateForFunctionPointer<DisableAtExit>(
                                LegacyNativeLibrary.GetExport(candidate, "H5dont_atexit"))();
                        }

                        ValidateVersion(candidate);
                    }
                    catch
                    {
                        LegacyNativeLibrary.Free(candidate);
                        loadedDirectory = null;
                        throw;
                    }

                    // Retain handles for process lifetime: cached P/Invoke stubs and global IDs must remain valid.
                    core = candidate;
                }

                if (!high)
                {
                    return core;
                }

                if (highLevel == IntPtr.Zero)
                {
                    highLevel = LoadFromDirectory(loadedDirectory, true);
                }

                return highLevel;
            }
        }

        /// <summary>
        /// Enumerates explicit and packaged native directories in deterministic precedence order.
        /// </summary>
        private static IEnumerable<string> ProbeDirectories(string rid)
        {
            var assemblyDirectory = Path.GetDirectoryName(typeof(NativeDependencies).Assembly.Location);
            var seen = new HashSet<string>(LegacyNativeLibrary.IsWindows ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal);

            foreach (var root in new[] { assemblyDirectory, AppContext.BaseDirectory })
            {
                if (string.IsNullOrEmpty(root))
                {
                    continue;
                }

                foreach (var path in new[] { Path.Combine(root, "runtimes", rid, "native"), root })
                {
                    if (seen.Add(path))
                    {
                        yield return path;
                    }
                }
            }
        }

        // Full paths only: Path.IsPathRooted also accepts drive-relative Windows paths.
        /// <summary>
        /// Checks for a fully qualified path, rejecting Windows drive-relative and root-relative paths.
        /// </summary>
        private static bool IsAbsolutePath(string path) => !string.IsNullOrEmpty(path) &&
            Path.IsPathRooted(path) && (!LegacyNativeLibrary.IsWindows ||
                path.StartsWith(@"\\", StringComparison.Ordinal) ||
                (path.Length >= 3 && path[1] == ':' && (path[2] == '\\' || path[2] == '/')));

        /// <summary>
        /// Creates a typed delegate for a required export from the selected native library.
        /// </summary>
        internal static T GetFunction<T>(string library, string symbol) where T : Delegate =>
            Marshal.GetDelegateForFunctionPointer<T>(LegacyNativeLibrary.GetExport(
                GetLibraryHandle(library == Constants.HLDLLFileName), symbol));

        /// <summary>
        /// Returns a typed delegate for an optional core export, throwing when the native build omits it.
        /// </summary>
        internal static T GetOptionalFunction<T>(string symbol) where T : Delegate
        {
            if (!LegacyNativeLibrary.TryGetExport(GetLibraryHandle(false), symbol, out var address))
            {
                throw new PlatformNotSupportedException($"The loaded HDF5 build does not support {symbol} on {RuntimeIdentifier}.");
            }

            return Marshal.GetDelegateForFunctionPointer<T>(address);
        }

        /// <summary>
        /// Selects the platform filename for the core or high-level native library.
        /// </summary>
        private static string FileName(bool high)
        {
            var name = high ? "hdf5_hl" : "hdf5";
            return LegacyNativeLibrary.IsWindows ? name + ".dll" : "lib" + name + (LegacyNativeLibrary.IsMacOS ? ".dylib" : ".so");
        }

        /// <summary>
        /// Loads a nonempty native file and adds architecture and dependency context to loader errors.
        /// </summary>
        private static IntPtr LoadFromDirectory(string directory, bool high)
        {
            var file = Path.Combine(directory, FileName(high));

            if (!File.Exists(file) || new FileInfo(file).Length == 0)
            {
                throw new DllNotFoundException($"Missing or empty HDF5 native library: {file}. Deploy real library files, not empty symlink placeholders.");
            }

            try
            {
                return LegacyNativeLibrary.Load(file);
            }
            catch (Exception e) when (e is DllNotFoundException || e is BadImageFormatException)
            {
                throw new DllNotFoundException($"Cannot load {file} for {RuntimeIdentifier}. Check binary architecture and its native dependencies. {e.Message}", e);
            }
        }

        /// <summary>
        /// Calls H5dont_atexit before any operation that initializes HDF5.
        /// </summary>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int DisableAtExit();

        /// <summary>
        /// Reads the version components exported by the selected core library.
        /// </summary>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int GetVersion(out uint major, out uint minor, out uint release);

        /// <summary>
        /// Rejects native libraries outside the supported HDF5 2.2 ABI series.
        /// </summary>
        private static void ValidateVersion(IntPtr handle)
        {
            var getVersion = Marshal.GetDelegateForFunctionPointer<GetVersion>(LegacyNativeLibrary.GetExport(handle, "H5get_libversion"));

            if (getVersion(out var major, out var minor, out var release) < 0 || major != 2 || minor != 2)
            {
                throw new NotSupportedException($"This binding requires HDF5 2.2.x; the loaded native library reports {major}.{minor}.{release}.");
            }
        }
    }
}

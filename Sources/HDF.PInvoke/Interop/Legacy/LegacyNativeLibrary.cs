using System;
using System.Runtime.InteropServices;

namespace HDF.PInvoke
{
    // Concrete OS calls used only by the netstandard build; no process-wide search-path changes.
    /// <summary>
    /// Provides platform library loading and symbol lookup on runtimes without NativeLibrary.
    /// </summary>
    internal static class LegacyNativeLibrary
    {
        /// <summary>
        /// Gets whether the current process uses Windows library loading and ABI rules.
        /// </summary>
        internal static bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        /// <summary>
        /// Gets whether the current process uses the macOS dynamic loader.
        /// </summary>
        internal static bool IsMacOS => RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

        /// <summary>
        /// Caches libc detection so repeated symbol lookups do not repeat platform probing.
        /// </summary>
        private static readonly Lazy<bool> Musl = new Lazy<bool>(DetectMusl);

        /// <summary>
        /// Gets whether Linux uses musl instead of glibc.
        /// </summary>
        internal static bool IsMusl => Musl.Value;

        /// <summary>
        /// Detects musl from the running process memory mappings without inspecting unrelated installed loaders.
        /// </summary>
        private static bool DetectMusl()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return false;
            }

            // Inspect the running process, not installed loaders: glibc hosts may also have musl installed.
            foreach (string mapping in System.IO.File.ReadLines("/proc/self/maps"))
            {
                if (mapping.IndexOf("ld-musl-", StringComparison.Ordinal) >= 0 || mapping.IndexOf("libc.musl-", StringComparison.Ordinal) >= 0)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Loads an explicit library path through the platform loader and reports native failures.
        /// </summary>
        internal static IntPtr Load(string path)
        {
            IntPtr handle;

            if (IsWindows)
            {
                handle = LoadLibraryEx(path, IntPtr.Zero, 0x00000100 | 0x00001000);
            }
            else if (IsMacOS)
            {
                handle = MacOpen(path, 2);
            }
            else if (IsMusl)
            {
                handle = MuslOpen(path, 2);
            }
            else
            {
                handle = LinuxOpen(path, 2);
            }

            if (handle == IntPtr.Zero)
            {
                throw new DllNotFoundException($"Cannot load native library {path}: {Error()}");
            }

            return handle;
        }

        /// <summary>
        /// Resolves a required export from an existing library handle or throws if it is absent.
        /// </summary>
        internal static IntPtr GetExport(IntPtr handle, string name)
        {
            if (!TryGetExport(handle, name, out IntPtr address))
            {
                throw new EntryPointNotFoundException($"Missing native export: {name}");
            }

            return address;
        }

        /// <summary>
        /// Attempts to resolve an export without throwing when the symbol is absent.
        /// </summary>
        internal static bool TryGetExport(IntPtr handle, string name, out IntPtr address)
        {
            address = IsWindows ? GetProcAddress(handle, name) : IsMacOS ? MacSymbol(handle, name) : IsMusl ? MuslSymbol(handle, name) : LinuxSymbol(handle, name);
            return address != IntPtr.Zero;
        }

        /// <summary>
        /// Releases a loader reference for a handle that has not been retained for active bindings.
        /// </summary>
        internal static void Free(IntPtr handle)
        {
            if (IsWindows)
            {
                FreeLibrary(handle);
            }
            else if (IsMacOS)
            {
                MacClose(handle);
            }
            else if (IsMusl)
            {
                MuslClose(handle);
            }
            else
            {
                LinuxClose(handle);
            }
        }

        /// <summary>
        /// Reads the platform loader's last error message for the failed operation.
        /// </summary>
        private static string Error()
        {
            if (IsWindows)
            {
                return new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error()).Message;
            }

            IntPtr error = IsMacOS ? MacError() : IsMusl ? MuslError() : LinuxError();
            return error == IntPtr.Zero ? "unknown loader error" : Marshal.PtrToStringAnsi(error);
        }

        /// <summary>
        /// Invokes the native <c>LoadLibraryExW</c> entry point using its explicit ABI declaration.
        /// </summary>
        [DllImport("kernel32.dll", EntryPoint = "LoadLibraryExW", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr LoadLibraryEx(string path, IntPtr reserved, uint flags);

        /// <summary>
        /// Invokes the native <c>GetProcAddress</c> entry point using its explicit ABI declaration.
        /// </summary>
        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern IntPtr GetProcAddress(IntPtr handle, string name);

        /// <summary>
        /// Invokes the native <c>FreeLibrary</c> entry point using its explicit ABI declaration.
        /// </summary>
        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool FreeLibrary(IntPtr handle);

        /// <summary>
        /// Invokes the native <c>dlopen</c> entry point using its explicit ABI declaration.
        /// </summary>
        [DllImport("libdl.so.2", EntryPoint = "dlopen")]
        private static extern IntPtr LinuxOpen([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))] string path, int flags);

        /// <summary>
        /// Invokes the native <c>dlsym</c> entry point using its explicit ABI declaration.
        /// </summary>
        [DllImport("libdl.so.2", EntryPoint = "dlsym")]
        private static extern IntPtr LinuxSymbol(IntPtr handle, string name);

        /// <summary>
        /// Invokes the native <c>dlclose</c> entry point using its explicit ABI declaration.
        /// </summary>
        [DllImport("libdl.so.2", EntryPoint = "dlclose")]
        private static extern int LinuxClose(IntPtr handle);

        /// <summary>
        /// Invokes the native <c>dlerror</c> entry point using its explicit ABI declaration.
        /// </summary>
        [DllImport("libdl.so.2", EntryPoint = "dlerror")]
        private static extern IntPtr LinuxError();

        /// <summary>
        /// Invokes the native <c>dlopen</c> entry point using its explicit ABI declaration.
        /// </summary>
        [DllImport("libc", EntryPoint = "dlopen")]
        private static extern IntPtr MuslOpen([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))] string path, int flags);

        /// <summary>
        /// Invokes the native <c>dlsym</c> entry point using its explicit ABI declaration.
        /// </summary>
        [DllImport("libc", EntryPoint = "dlsym")]
        private static extern IntPtr MuslSymbol(IntPtr handle, string name);

        /// <summary>
        /// Invokes the native <c>dlclose</c> entry point using its explicit ABI declaration.
        /// </summary>
        [DllImport("libc", EntryPoint = "dlclose")]
        private static extern int MuslClose(IntPtr handle);

        /// <summary>
        /// Invokes the native <c>dlerror</c> entry point using its explicit ABI declaration.
        /// </summary>
        [DllImport("libc", EntryPoint = "dlerror")]
        private static extern IntPtr MuslError();

        /// <summary>
        /// Invokes the native <c>dlopen</c> entry point using its explicit ABI declaration.
        /// </summary>
        [DllImport("/usr/lib/libSystem.B.dylib", EntryPoint = "dlopen")]
        private static extern IntPtr MacOpen([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))] string path, int flags);

        /// <summary>
        /// Invokes the native <c>dlsym</c> entry point using its explicit ABI declaration.
        /// </summary>
        [DllImport("/usr/lib/libSystem.B.dylib", EntryPoint = "dlsym")]
        private static extern IntPtr MacSymbol(IntPtr handle, string name);

        /// <summary>
        /// Invokes the native <c>dlclose</c> entry point using its explicit ABI declaration.
        /// </summary>
        [DllImport("/usr/lib/libSystem.B.dylib", EntryPoint = "dlclose")]
        private static extern int MacClose(IntPtr handle);

        /// <summary>
        /// Invokes the native <c>dlerror</c> entry point using its explicit ABI declaration.
        /// </summary>
        [DllImport("/usr/lib/libSystem.B.dylib", EntryPoint = "dlerror")]
        private static extern IntPtr MacError();
    }
}

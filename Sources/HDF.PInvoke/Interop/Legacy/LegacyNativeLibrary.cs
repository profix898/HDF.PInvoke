using System;
using System.Runtime.InteropServices;

namespace HDF.PInvoke
{
    // Concrete OS calls used only by the netstandard build; no process-wide search-path changes.
    internal static class LegacyNativeLibrary
    {
        internal static bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        internal static bool IsMacOS => RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

        private static readonly Lazy<bool> Musl = new Lazy<bool>(DetectMusl);

        internal static bool IsMusl => Musl.Value;

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

        internal static IntPtr GetExport(IntPtr handle, string name)
        {
            if (!TryGetExport(handle, name, out IntPtr address))
            {
                throw new EntryPointNotFoundException($"Missing native export: {name}");
            }

            return address;
        }

        internal static bool TryGetExport(IntPtr handle, string name, out IntPtr address)
        {
            address = IsWindows ? GetProcAddress(handle, name) : IsMacOS ? MacSymbol(handle, name) : IsMusl ? MuslSymbol(handle, name) : LinuxSymbol(handle, name);
            return address != IntPtr.Zero;
        }

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

        private static string Error()
        {
            if (IsWindows)
            {
                return new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error()).Message;
            }

            IntPtr error = IsMacOS ? MacError() : IsMusl ? MuslError() : LinuxError();
            return error == IntPtr.Zero ? "unknown loader error" : Marshal.PtrToStringAnsi(error);
        }

        [DllImport("kernel32.dll", EntryPoint = "LoadLibraryExW", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr LoadLibraryEx(string path, IntPtr reserved, uint flags);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern IntPtr GetProcAddress(IntPtr handle, string name);

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool FreeLibrary(IntPtr handle);

        [DllImport("libdl.so.2", EntryPoint = "dlopen")]
        private static extern IntPtr LinuxOpen([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8Marshaler))] string path, int flags);

        [DllImport("libdl.so.2", EntryPoint = "dlsym")]
        private static extern IntPtr LinuxSymbol(IntPtr handle, string name);

        [DllImport("libdl.so.2", EntryPoint = "dlclose")]
        private static extern int LinuxClose(IntPtr handle);

        [DllImport("libdl.so.2", EntryPoint = "dlerror")]
        private static extern IntPtr LinuxError();

        [DllImport("libc", EntryPoint = "dlopen")]
        private static extern IntPtr MuslOpen([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8Marshaler))] string path, int flags);

        [DllImport("libc", EntryPoint = "dlsym")]
        private static extern IntPtr MuslSymbol(IntPtr handle, string name);

        [DllImport("libc", EntryPoint = "dlclose")]
        private static extern int MuslClose(IntPtr handle);

        [DllImport("libc", EntryPoint = "dlerror")]
        private static extern IntPtr MuslError();

        [DllImport("/usr/lib/libSystem.B.dylib", EntryPoint = "dlopen")]
        private static extern IntPtr MacOpen([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8Marshaler))] string path, int flags);

        [DllImport("/usr/lib/libSystem.B.dylib", EntryPoint = "dlsym")]
        private static extern IntPtr MacSymbol(IntPtr handle, string name);

        [DllImport("/usr/lib/libSystem.B.dylib", EntryPoint = "dlclose")]
        private static extern int MacClose(IntPtr handle);

        [DllImport("/usr/lib/libSystem.B.dylib", EntryPoint = "dlerror")]
        private static extern IntPtr MacError();
    }
}

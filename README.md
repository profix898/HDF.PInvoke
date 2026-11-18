# HDF.PInvoke

<img src="https://raw.githubusercontent.com/profix898/HDF.PInvoke/master/HDF5.png" alt="HDF5" width="200" />

[![NuGet](https://img.shields.io/nuget/v/HDF.PInvoke?style=flat-square&logo=nuget&color=blue)](https://www.nuget.org/packages/HDF.PInvoke)

[HDF5](https://www.hdfgroup.org/solutions/hdf5/) is a file format and library for storing multidimensional datasets and their metadata in a hierarchy of groups. HDF.PInvoke lets C# applications read and write these files through the HDF5 C API.

The NuGet package includes the native HDF5 libraries and two sets of managed bindings. The modern bindings are optimized for .NET 8 and later, with source-generated interop and zero-copy span overloads. The .NET Standard 2.0 bindings support older runtimes, including .NET Framework 4.7.2+ and standalone Mono.

## Supported platforms

| Platform     | Runtime identifiers                  | Requirements                                                          |
| ------------ | ------------------------------------ | --------------------------------------------------------------------- |
| Windows      | `win-x64`, `win-arm64`               | Microsoft Visual C++ v14 Redistributable for the process architecture |
| Linux        | `linux-x64`, `linux-arm64`           | Ubuntu 24.04/glibc 2.39 build baseline                                |
| Alpine Linux | `linux-musl-x64`, `linux-musl-arm64` | Alpine 3.24/musl build baseline                                       |
| macOS        | `osx-arm64`                          | macOS 13 or later                                                     |

macOS x64, 32-bit processes, iOS and Android are not supported by this package.

Windows applications require the [Microsoft Visual C++ v14 Redistributable](https://learn.microsoft.com/en-us/cpp/windows/latest-supported-vc-redist/) for the process architecture ([x64](https://aka.ms/vc14/vc_redist.x64.exe) or [ARM64](https://aka.ms/vc14/vc_redist.arm64.exe)).

## Quick start

In a .NET 8 or later project, install the package with:

```sh
dotnet add package HDF.PInvoke --version 2.2.0
```

To read a numeric dataset, open the file read-only, open the dataset by its path, and use its dataspace to size the destination array. This example assumes an existing file named `example.h5` with a dataset at `/temperatures`.

```csharp
using System;
using HDF.PInvoke;

long file = H5F.open("example.h5", H5F.ACC_RDONLY);
if (file < 0)
    throw new InvalidOperationException("Could not open example.h5.");

long dataset = -1, space = -1;
try
{
    dataset = H5D.open(file, "/temperatures");
    if (dataset < 0)
        throw new InvalidOperationException("Could not open /temperatures.");

    space = H5D.get_space(dataset);
    long count = space < 0 ? -1 : H5S.get_simple_extent_npoints(space);
    if (count < 0)
        throw new InvalidOperationException("Could not determine the dataset size.");

    double[] temperatures = new double[checked((int) count)];
    if (H5D.read<double>(dataset, H5T.NATIVE_DOUBLE, H5S.ALL, H5S.ALL, H5P.DEFAULT, temperatures) < 0)
        throw new InvalidOperationException("Could not read /temperatures.");

    Console.WriteLine(string.Join(", ", temperatures));
}
finally
{
    if (space >= 0) H5S.close(space);
    if (dataset >= 0) H5D.close(dataset);
    H5F.close(file);
}
```

The two `H5S.ALL` arguments select the whole dataset and the corresponding positions in memory. `H5T.NATIVE_DOUBLE` tells HDF5 to convert the stored numeric values to the element type of the C# array.

## Modern .NET (.NET 8 and later)

NuGet selects the `net8.0` assembly, and the wrapper loads the bundled native libraries automatically. Dataset and attribute APIs offer `Span<T>` and `ReadOnlySpan<T>` overloads for synchronous I/O without copying the element buffer. UTF-8 string getters are available for names and labels.

The assembly enables `DisableRuntimeMarshalling` and uses source-generated `LibraryImport` declarations and native callback bridges. Applications can use trimming, single-file publishing and NativeAOT (`PublishAot=true`) on all supported platforms.

## Legacy .NET

For .NET Framework 4.7.2+ on Windows x64 and standalone Mono, use the package’s `netstandard2.0` asset. See the [legacy deployment guide](https://github.com/profix898/HDF.PInvoke/blob/master/Docs/LEGACY.md) for setup and native loading. Unity and IL2CPP are not supported.

## Threads, SWMR and compression

The bundled core/high-level library is not thread-safe: serialize all HDF5 calls within a process, including calls involving different files. Close individual resources and call `H5.close()` only after all users of the library have finished. HDF5 can be built with `HDF5_ENABLE_THREADSAFE=ON`, but upstream does not support combining that option with the high-level library; a supported thread-safe variant would be a separate core-only build. The native thread-safe option protects library state and is not a promise of parallel execution.

**Single-writer/multiple-reader (SWMR) is available in this build.** It coordinates one writer process and multiple reader processes accessing the same file. Create groups/datasets before starting SWMR writing, use 1.10-or-newer format bounds, call `H5F.start_swmr_write`, open readers with `H5F.ACC_SWMR_READ`, and use writer flushes/reader refreshes to publish and observe changes. SWMR has operation/datatype restrictions and requires appropriate filesystem write semantics. It does not make concurrent calls within one process safe. See the [upstream SWMR guide](https://portal.hdfgroup.org/documentation/hdf5/latest/_s_w_m_r_t_n.html).

**zlib/Deflate is included**, statically linked into native HDF5. SZIP is a separate optional filter; its open-source implementation is libaec. SZIP/libaec is not bundled, so datasets compressed with that filter need a compatible build or filter implementation before their data can be read. See [HDF5 compression documentation](https://support.hdfgroup.org/documentation/hdf5/latest/_comp_t_s.html).

## Versioning and file compatibility

NuGet package 2.2.0 includes native HDF5 2.2.0. The package major and minor numbers follow the native library. The patch number counts wrapper releases. The [release notes](https://github.com/profix898/HDF.PInvoke/blob/master/Docs/RELEASE_NOTES.md) identify the bundled native version.

**Reading older files.** HDF5 2.2 can read files written by HDF5 1.8 and 1.10 when the required filters and external data are available.

**Writing files for HDF5 1.10 readers.** Set the upper file-format bound to `V110` on a file-access property list and pass it to `H5F.create`. The lower bound `EARLIEST` lets HDF5 use older structures where possible.

```csharp
long fapl = H5P.create(H5P.FILE_ACCESS);
if (fapl < 0) throw new InvalidOperationException("Cannot create file properties.");
try
{
    if (H5P.set_libver_bounds(fapl, H5F.libver_t.EARLIEST, H5F.libver_t.V110) < 0)
        throw new InvalidOperationException("Cannot set format bounds.");

    long file = H5F.create("compatible.h5", H5F.ACC_TRUNC, H5P.DEFAULT, fapl);
    if (file < 0)
        throw new InvalidOperationException("Cannot create file.");
    if (H5F.close(file) < 0)
        throw new InvalidOperationException("Cannot close file.");
}
finally { H5P.close(fapl); }
```

With HDF5 2.2, `H5F.libver_t.LATEST` equals `V200` and permits 2.x format structures. Omitting the upper argument to `H5P.set_libver_bounds` also selects `LATEST`. Use an explicit `V110` upper bound when 1.10 readability is required. Features that need newer structures can fail under that bound. Changing bounds does not convert an existing file. See the [HDF Group's compatibility guidance](https://portal.hdfgroup.org/documentation/hdf5/latest/_accessibility.html).

## Breaking changes since the 1.10.x wrapper series

Version 2.2 is not a guaranteed source-compatible replacement. See the [migration guide](https://github.com/profix898/HDF.PInvoke/blob/master/Docs/MIGRATION.md) for concrete signature, callback and structure changes that can require source edits before recompilation.

- **Recompile consumers.** Version 2 is not a binary replacement for the 1.x assembly. The package provides net8.0 and netstandard2.0 assets; .NET Framework consumers use the latter.
- **Use the supplied HDF5 2.2.x libraries.** The loader rejects other native release families. Old `bin32`/`bin64` deployments and app.config configuration are removed.
- **Remove the old explicit loading call.** `NativeDependencies.ResolvePathToExternalDependencies()` is removed from both targets. Normal loading is automatic; use `Configure(path)` for a private native directory and `H5.open()` for explicit initialization.
- **Review interop declarations in calling code.** Native booleans are `byte` (0 or 1), IDs are 64-bit, and platform-dependent C fields use `System.Runtime.InteropServices.CLong`/`CULong`. Several structures, enums and callback signatures were corrected.
- **Strings are UTF-8 across platforms.** Buffer lengths are byte counts. Raw `byte[]` name arguments remain C buffers and need a NUL terminator, such as `Encoding.UTF8.GetBytes(name + "\0")`. Keep callbacks alive for as long as native code may call them.
- **Optional native features are checked.** Unavailable driver APIs throw `PlatformNotSupportedException`.
- **Platform scope changed.** The seven runtime identifiers listed above are packaged by default. Current application APIs are catalogued in [API_COVERAGE.md](https://github.com/profix898/HDF.PInvoke/blob/master/Docs/API_COVERAGE.md); MPI, map operations and driver/connector-author interfaces are outside the packaged application surface.

For a private native installation, call `NativeDependencies.Configure(absoluteDirectory)` before any HDF5 use. Normal package consumers should leave this unset. See [advanced documentation](https://github.com/profix898/HDF.PInvoke/blob/master/Docs/ADVANCED.md) for configuration, loading and troubleshooting.

## Contributing

[ADVANCED.md](https://github.com/profix898/HDF.PInvoke/blob/master/Docs/ADVANCED.md) covers modern integration, builds, tests and packaging. [LEGACY.md](https://github.com/profix898/HDF.PInvoke/blob/master/Docs/LEGACY.md) covers .NET Standard integration and implementation.

[API_COVERAGE.md](https://github.com/profix898/HDF.PInvoke/blob/master/Docs/API_COVERAGE.md) lists native function and predefined-identifier coverage by API group. [ASYNC.md](https://github.com/profix898/HDF.PInvoke/blob/master/Docs/ASYNC.md) explains event-set ownership and the distinction between async entry points and actual background execution.

## License

HDF.PInvoke is licensed under the terms in [LICENSE.txt](LICENSE.txt), which identifies the [3-clause BSD license](https://opensource.org/license/bsd-3-clause). Preserve the copyright notices, conditions and disclaimers when redistributing source or binaries.

Bundled native HDF5 and zlib libraries retain their respective license terms. The complete HDF.PInvoke, HDF5 and zlib notices are combined in the repository and package root `LICENSE.txt`.

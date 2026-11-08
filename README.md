# HDF.PInvoke

<img src="https://raw.githubusercontent.com/profix898/HDF.PInvoke/master/HDF5.png" alt="HDF5" width="200" />

[![NuGet](https://img.shields.io/nuget/v/HDF.PInvoke?style=flat-square&logo=nuget&color=blue)](https://www.nuget.org/packages/HDF.PInvoke)

Low-level C# bindings for HDF5, with native libraries included for supported desktop platforms. The 2.x wrapper targets **.NET 8+ and .NET Standard 2.0** and **native HDF5 2.2.x**.

CI produces `2.2.0` package artifacts but does not publish them to NuGet.org. Configure a package feed containing the built package before using the installation command below.

## Quick start

Create a .NET 8 or later application, then add the **HDF.PInvoke** package:

```sh
dotnet add package HDF.PInvoke --version 2.2.0
```

Create an HDF5 file:

```csharp
using HDF.PInvoke;

long file = H5F.create("example.h5", H5F.ACC_TRUNC);
if (file < 0)
    throw new InvalidOperationException("Could not create the HDF5 file.");

try
{
    // Create groups, datasets and attributes using the HDF5 APIs.
}
finally
{
    if (H5F.close(file) < 0)
        throw new InvalidOperationException("Could not close the HDF5 file.");
}
```

`ACC_TRUNC` replaces an existing file of the same name. Most HDF5 functions report failure through a negative return value; check results and close the resources you create. This package exposes the native API, rather than an object-oriented data model or automatic resource management.

For .NET Framework, use an SDK-style project with PackageReference, target .NET Framework 4.7.2 or later, select x64, and publish with `-r win-x64`. CI tests a net472 consumer on the installed .NET Framework 4.8 runtime. See [LEGACY.md](https://github.com/profix898/HDF.PInvoke/blob/master/LEGACY.md) for legacy deployment and runtime compatibility. Standalone Mono 6.8 on Linux x64 is locally validated; see the legacy guide for the test procedure and scope. Unity and AOT runtimes are outside the validated configurations.

## Supported platforms

| Platform | Runtime identifiers | Requirements |
| --- | --- | --- |
| Windows | `win-x64`, `win-arm64` | Microsoft Visual C++ v14 Redistributable for the process architecture |
| Linux | `linux-x64`, `linux-arm64` | Ubuntu 24.04/glibc 2.39 build baseline |
| Alpine Linux | `linux-musl-x64`, `linux-musl-arm64` | Alpine 3.24/musl build baseline |
| macOS | `osx-arm64` | macOS 13 or later |

macOS x64, 32-bit processes, iOS and Android are not supported by this package. Alpine/musl x64 and ARM64 are included in the default CI and package matrix. Alpine uses native-architecture containers rather than glibc compatibility packages.

The native build includes the C core, high-level APIs and zlib/Deflate compression. **Calls must be serialized:** this build is not thread-safe. MPI and SZIP/libaec are not included. Close individual resources; avoid calling `H5.close()` while other code still uses the library.

## Threads, SWMR and compression

The bundled core/high-level library is not thread-safe: serialize all HDF5 calls within a process, including calls involving different files. HDF5 can be built with `HDF5_ENABLE_THREADSAFE=ON`, but upstream does not support combining that option with the high-level library; a supported thread-safe variant would be a separate core-only build. The native thread-safe option protects library state and is not a promise of parallel execution.

**Single-writer/multiple-reader (SWMR) is available in this build.** It coordinates one writer process and multiple reader processes accessing the same file. Create groups/datasets before starting SWMR writing, use 1.10-or-newer format bounds, call `H5F.start_swmr_write`, open readers with `H5F.ACC_SWMR_READ`, and use writer flushes/reader refreshes to publish and observe changes. SWMR has operation/datatype restrictions and requires appropriate filesystem write semantics. It does not make concurrent calls within one process safe. See the [upstream SWMR guide](https://portal.hdfgroup.org/documentation/hdf5/latest/_s_w_m_r_t_n.html).

**zlib/Deflate is included**, statically linked into native HDF5. SZIP is a separate optional filter; its open-source implementation is libaec. SZIP/libaec is not currently bundled, so datasets compressed with that filter need a compatible build or filter implementation before their data can be read. See [HDF5 compression documentation](https://support.hdfgroup.org/documentation/hdf5/latest/_comp_t_s.html).

## Versioning and file compatibility

| Version | Meaning |
| --- | --- |
| `2.2.0` | Managed wrapper/package version |
| `2.2.0.0` | Managed assembly version |
| `2.2.0` | Pinned native HDF5 release shipped by this build |
| `net8.0` / `netstandard2.0` | Modern .NET / legacy-compatible managed assets |
| VC++ v14 | Required Windows native runtime: Microsoft Visual C++ v14 Redistributable |

On Windows, install the latest supported [Microsoft Visual C++ v14 Redistributable](https://learn.microsoft.com/en-us/cpp/windows/latest-supported-vc-redist/) for the process architecture ([x64](https://aka.ms/vc14/vc_redist.x64.exe) or [ARM64](https://aka.ms/vc14/vc_redist.arm64.exe)). Its version must be at least as recent as the MSVC toolset used to build the native libraries. Windows builds use Visual Studio 2026 with the dynamic C runtime (`/MD`); the redistributable is a separate prerequisite and is not bundled in the NuGet package.

Starting with 2.2.0, wrapper major/minor numbers match the wrapped native HDF5 major/minor. The patch number counts wrapper releases: 2.2.0, 2.2.1, 2.2.2, and so on. It does not identify the native patch release; the exact native version is recorded separately in Build/HDF5.props. A wrapper version is not a file-format version. The historical wrapper `1.10.11` already shipped HDF5 1.14.3 with compatibility APIs; consult [release notes](https://github.com/profix898/HDF.PInvoke/blob/master/RELEASE_NOTES.md) rather than inferring native versions from package numbers.

HDF5 2.2 can read files produced by HDF5 1.8 and 1.10, provided any required compression filters and external data are available. To create files restricted to 1.10-compatible format features, set an upper format bound on the file-access property list before creating the file:

```csharp
long fapl = H5P.create(H5P.FILE_ACCESS);
if (fapl < 0) throw new InvalidOperationException("Cannot create file properties.");
try
{
    if (H5P.set_libver_bounds(fapl, H5F.libver_t.EARLIEST, H5F.libver_t.V110) < 0)
        throw new InvalidOperationException("Cannot set format bounds.");
    long file = H5F.create("compatible.h5", H5F.ACC_TRUNC, H5P.DEFAULT, fapl);
    if (file < 0) throw new InvalidOperationException("Cannot create file.");
    if (H5F.close(file) < 0) throw new InvalidOperationException("Cannot close file.");
}
finally { H5P.close(fapl); }
```

Bounds constrain new format structures; they do not downgrade an existing file. Features requiring newer formats can fail under these bounds. See the [HDF Group's compatibility guidance](https://portal.hdfgroup.org/documentation/hdf5/latest/_accessibility.html).

## Breaking changes since the 1.10.x wrapper series

- **Recompile consumers.** Version 2 is not a binary replacement for the 1.x assembly. The package provides net8.0 and netstandard2.0 assets; .NET Framework consumers use the latter.
- **Use the supplied HDF5 2.2.x libraries.** The loader rejects other native release families. Old `bin32`/`bin64` deployments and app.config configuration are removed.
- **Review interop declarations in calling code.** Native booleans are `byte` (0 or 1), IDs are 64-bit, and platform-dependent C fields use `CLong`/`CULong`. Several structures, enums and callback signatures were corrected.
- **Strings are UTF-8 across platforms.** Buffer lengths are byte counts. Raw `byte[]` name arguments remain C buffers and need a NUL terminator, such as `Encoding.UTF8.GetBytes(name + "\0")`. Keep callbacks alive for as long as native code may call them.
- **Optional native features are checked.** Unavailable driver APIs throw `PlatformNotSupportedException`.
- **Platform scope changed.** The seven runtime identifiers listed above are packaged by default. Current application APIs are catalogued in [API_COVERAGE.md](API_COVERAGE.md); MPI, map operations and driver/connector-author interfaces are outside the packaged application surface.

For a private native installation, call `NativeDependencies.Configure(absoluteDirectory)` before any HDF5 use. Normal package consumers should leave this unset. See [advanced documentation](https://github.com/profix898/HDF.PInvoke/blob/master/ADVANCED.md) for configuration, loading and troubleshooting.

## Contributing

[ADVANCED.md](https://github.com/profix898/HDF.PInvoke/blob/master/ADVANCED.md) covers modern integration, builds, tests and packaging. [LEGACY.md](https://github.com/profix898/HDF.PInvoke/blob/master/LEGACY.md) covers .NET Standard integration and implementation.

[API_COVERAGE.md](https://github.com/profix898/HDF.PInvoke/blob/master/API_COVERAGE.md) lists native function and predefined-identifier coverage by API group. [ASYNC.md](https://github.com/profix898/HDF.PInvoke/blob/master/ASYNC.md) explains event-set ownership and the distinction between async entry points and actual background execution.

## License

HDF.PInvoke is licensed under the terms in [LICENSE.txt](LICENSE.txt), which identifies the [3-clause BSD license](https://opensource.org/license/bsd-3-clause). Preserve the copyright notices, conditions and disclaimers when redistributing source or binaries.

Bundled native HDF5 and zlib libraries retain their respective license terms. The complete HDF.PInvoke, HDF5 and zlib notices are combined in the repository and package root `LICENSE.txt`.

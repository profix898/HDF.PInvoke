# HDF.PInvoke: integration and contributor guide

This guide covers the modern `net8.0` implementation, native builds and package contributions. Start with [README.md](README.md) for installation and API examples. See [LEGACY.md](LEGACY.md) for .NET Standard and .NET Framework integration.

## Repository layout

| Path | Purpose |
| --- | --- |
| `Sources/HDF.PInvoke` | Signed net8.0 and netstandard2.0 bindings and NuGet package |
| `Tests/HDF.PInvoke.Tests` | Integration tests against real native HDF5 |
| `Tests/HDF.PInvoke.Smoke` | Managed export, ABI, loading and plugin-context checks |
| `Native/hdf5` | Unmodified official source submodule |
| `Native/HDF5Options.cmake` | Native configuration and source-pin verification |
| `Native/Tests` | Header-derived ABI probe and incompatible-version fixture |
| `Tools/BindingGenerator` | Build-time legacy source generation and regression checks |
| `Build/HDF5.props` | Pinned HDF5 and zlib versions/revisions |
| `Build/Native` | Ignored native build, install and runtime outputs |
| `Build/Artifacts` | Ignored test results, packages and consumer deployments |

The root `Build` directory contains versioned build scripts and configuration. Its `Artifacts` and `Native` subdirectories contain generated output and are ignored by Git; they are distinct from the versioned top-level `Native` source directory.

## Managed and native loading

NuGet supplies the managed assembly and platform libraries under `runtimes/<rid>/native`. .NET resolves the managed assembly through its normal dependency/load-context mechanisms. A module initializer registers its native import resolver without eagerly opening HDF5. Call `H5.dont_atexit()` before any other HDF5 operation to opt out of automatic shutdown; the loader invokes the native opt-out before its version query initializes HDF5. The opt-out succeeds only once, and subsequent/late calls return a negative status. Call `H5.close()` explicitly when finished if automatic shutdown was disabled.

Functions imported from logical names `hdf5` and `hdf5_hl`, and exported globals such as `H5P.FILE_ACCESS`, share cached native handles. Globals use `NativeLibrary.GetExport` against the same core handle rather than a separate operating-system loader path. This prevents a successful function call from being followed by a failed global lookup against a fixed Linux path.

Configuration precedence, before first native use:

1. `NativeDependencies.Configure(absoluteDirectory)`.
2. `AppContext.SetData(NativeDependencies.NativePathSetting, absoluteDirectory)`.
3. The `HDF5_NATIVE_PATH` environment variable.

These overrides select a directory, not a filename. Relative paths and reconfiguration after loading are rejected. The old app.config setting and System.Configuration dependency are removed.

Without an override, the resolver checks:

1. The binding assembly's `runtimes/<process-rid>/native` directory, then its own directory.
2. The application base directory's corresponding runtime directory, then the base directory itself.
3. .NET's assembly-aware native probing, including dependency metadata and the assembly load context.

The process architecture selects the runtime, so an emulated x64 process needs x64 assets even on an ARM64 host. A selected file that is invalid or has missing dependencies fails with contextual diagnostics; an explicit location never silently falls back. When a directory supplies the core library, it must also supply the high-level library.

Before binding globals, the loader checks that the native library reports HDF5 2.2.x. Accepted handles remain loaded for process lifetime. Unloading a managed plugin context does not unload HDF5 beneath cached function pointers. Loading does not modify PATH or the working directory and does not create symlinks or write application files.

## Native assets and platform constraints

The package supplies shared core and high-level libraries with statically linked zlib. Canonical names are `hdf5.dll` / `hdf5_hl.dll`, `libhdf5.so` / `libhdf5_hl.so`, and `libhdf5.dylib` / `libhdf5_hl.dylib`. SONAME aliases are materialized as real files before upload and packaging, avoiding broken symlinks or empty placeholders.

Linux artifacts use the Ubuntu 24.04/glibc 2.39 baseline. Alpine 3.24 builds provide `linux-musl-x64` and `linux-musl-arm64`; neither Linux variant is an Android build. macOS ARM64 uses deployment target 13.0; Windows needs the Microsoft Visual C++ v14 Redistributable for the process architecture, at least as recent as the MSVC toolset used for the native build. Visual Studio 2026 builds use the dynamic C runtime (`/MD`); the redistributable is installed separately. See the [version table and runtime downloads](README.md#versioning-and-file-compatibility). Native dependencies must match the process architecture. Mobile targets are outside the supported matrix.

The build intentionally disables MPI, SZIP/libaec, ROS3, HDFS and the C++/Fortran/Java bindings. Thread safety is disabled because upstream does not support the high-level/thread-safe combination without an unsupported-build override. Applications must serialize calls across all users of the library in a process.

## ABI and marshalling rules

`hid_t` is 64-bit; native booleans use one byte. C `long` and `unsigned long` use `CLong` and `CULong` where their width differs between Windows and Unix. Never substitute a C# `long` merely because a C declaration says `long`.

Strings are UTF-8. Managed StringBuilder output overloads use temporary byte buffers and explicit decoding. Native lengths remain byte counts. Raw byte-array overloads require correctly sized, NUL-terminated buffers. Callback signatures, argument widths and lifetimes must match the native declaration; callers must retain delegates while native code can invoke them. User-defined link registration is an exception: the binding retains its comment buffer and callbacks until successful replacement, unregistration or explicit library shutdown. Optional driver functions are resolved only when requested and report unsupported features explicitly.

The ABI probe compares sizes and field offsets from the pinned native headers with managed layouts. The export audit verifies required entry points exist; it does not by itself prove each signature is correct. Integration tests exercise actual calls, buffers, callbacks and data roundtrips.

## Build prerequisites

Install the .NET SDK selected by `global.json`, PowerShell 7.4 or later, Git, CMake and a native C compiler. The native project requires CMake 3.26 or later; the Visual Studio 2026 generator requires CMake 4.2 or later. Unix builds use Ninja. Run each build on the target architecture with native-architecture PowerShell.

```sh
git submodule update --init --recursive
pwsh -File Build/buildNative.ps1
pwsh -File Build/testManaged.ps1
```

For Windows with Visual Studio 2026:

```powershell
./Build/buildNative.ps1 -Rid win-x64 -Generator 'Visual Studio 18 2026' -Jobs 4
./Build/testManaged.ps1 -Rid win-x64
```

Windows ARM64 CI selects the installed stable MSVC 14.44 toolset through Visual Studio 2026 (`-Toolset 'v143,version=14.44'`). MSVC 14.51 has a confirmed [ARM64 compiler crash in HDF5 2.2](https://github.com/actions/runner-images/issues/15052). This toolset selection preserves upstream sources, Release optimization and the full native test suite; the resulting libraries and tests run natively on ARM64. Use a fresh `-BuildDirectory` when changing CMake generators or toolsets.

For Linux under WSL, install the prerequisites inside the distribution and run the Linux scripts there. Prefer a checkout on WSL's Linux filesystem for the native I/O tests; a checkout on `/mnt/c` or `/mnt/e` can be substantially slower. A Windows native build does not validate Linux libraries. When sharing a Windows checkout with WSL, the high-level image test rejects CRLF palette files. The build script normalizes line endings in the generated `earth.pal` and `sepia.pal` test copies on Unix; upstream sources remain unchanged.

When keeping the checkout on a Windows drive, place the native build and upstream test files on the Linux filesystem:

```sh
pwsh -File Build/buildNative.ps1 -Rid linux-x64 -BuildDirectory /tmp/hdf5-pinvoke-native-linux-x64
```

`-BuildDirectory` must be absolute. Installed/staged assets and JUnit summaries still go under the checkout; detailed upstream CTest logs stay in the selected build directory.

## Validation and CI

Native builds default to upstream full mode (level 1), which runs the registered suite with reduced stress repetition counts. Use `-TestExpressLevel 0` for exhaustive stress iterations. Packaging accepts levels 0 and 1, not quick/smoke levels 2 and 3. No individual test exclusions are added.

`buildNative.ps1` configures and builds the pinned upstream source, runs its CTest suite, installs/stages the native assets, and builds/runs the ABI probe. It invalidates the previous build manifest before starting. Only a successful run writes the new manifest with source revisions, feature settings, hashes and the upstream test result.

The upstream suite uses full-suite mode with reduced stress repetition (`HDF_TEST_EXPRESS=1`) for enabled components, including C/API, SWMR and high-level tests. Tests for components not built are excluded. CTest entries may include fixtures and internal feature skips; the entry count is not a count of individual assertions. Managed package consumers separately validate compressed core/high-level data round-trips.

`testManaged.ps1` performs a locked restore, Release build, integration tests and separate-process loading checks. These cover automatic and explicit loading, globals-first initialization, a private AssemblyLoadContext, unrelated working directories, invalid/empty libraries, incompatible native versions and rejected configuration changes. The ABI contract is in `Build/Native/interop/<rid>/abi-layout.txt`.

CI runs native checks and both managed implementations on Windows/Linux x64 and ARM64 and macOS ARM64. Native Linux jobs cover glibc and musl. Package consumers exercise the modern assembly on .NET 8 and .NET 10, plus the .NET Standard assembly; Windows x64 also runs a net472 consumer on the installed Framework runtime. It then collects all runtime assets, packs them and runs clean package consumers on every target. Failed native tests prevent staging and downstream packaging. Logs are uploaded even on failure; upstream JUnit results are in `Build/Artifacts/tests/<rid>/native-upstream.xml`, and detailed CTest logs are under the native build's `Testing/Temporary` directory.

The integration suite exercises actual native calls, Unicode data, callbacks and ABI regressions. Allocator tests use HDF5 allocation, resizing and freeing as a matched family. Library capability tests verify that the supplied native build requires serialized calls. The ABI probe checks structure layouts; the export audit checks every required import. NativeAOT, trimming and single-file deployment are outside the validated configurations.

## Packaging

After collecting verified native assets for all seven default runtime identifiers:

```powershell
./Build/pack.ps1
./Build/testPack.ps1 -Rid win-x64
```

The packaging script checks successful full or exhaustive upstream testing, source pins, file hashes, canonical libraries and licenses. The package includes `README.md` and the combined `LICENSE.txt` at its root; contributor documentation stays in the repository. Native runtime artifacts carry the same combined license once at their root, outside the platform directories. Native builds verify that it contains the full upstream HDF5 and zlib notices. `-RequiredRids` allows an explicitly limited local test package; CI requires all seven targets. Use this script rather than bypassing validation with a direct pack command.

For a local Windows-only build, explicitly select the runtime you have built:

```powershell
./Build/pack.ps1 -RequiredRids win-x64
./Build/testPack.ps1 -Rid win-x64
```

This produces a partial package for local verification, not the all-platform release artifact. If multiple verified runtimes are staged, pass their identifiers as a PowerShell array to `-RequiredRids`.

The package consumer uses an isolated NuGet cache and an actual PackageReference, without a project reference. It publishes and checks ordinary loading and a separate managed load context. Unix consumer checks also run from a read-only publish directory. CI uploads package artifacts but does not publish to a feed.

## Updating dependencies

Update the `Native/hdf5` gitlink and `Build/HDF5.props` together. Update the exact native version required by `Native/Tests/CMakeLists.txt` and review the loader's accepted version family. Re-audit signatures, enums, exported globals and structures against the new headers; a successful compile is insufficient. Review zlib revision changes and bundled license notices as well.

Managed dependency versions live in `Directory.Packages.props`. Regenerate and review lock files after intentional dependency changes. Re-run native, managed and clean package-consumer tests before proposing a release.

## Alpine/musl builds

Musl builds and package-consumer checks run by default on both architectures. The build uses Bash, CMake, jq and .NET; no Python is required.

`Build/Alpine/Alpine.Dockerfile` selects the official .NET 10.0.403 SDK on Alpine 3.24. `Build/Alpine/alpine.sh` runs the same pinned native configuration, upstream suite, ABI/export checks, managed integration tests and loader scenarios. This Bash entry point avoids a PowerShell dependency in the ARM64 Alpine SDK image. Run on the target architecture; CI uses native x64 and ARM64 Ubuntu hosts with Alpine containers.

```sh
docker build --build-arg BUILD_UID="$(id -u)" --build-arg BUILD_GID="$(id -g)" -f Build/Alpine/Alpine.Dockerfile -t hdf-pinvoke-alpine Build/Alpine
docker run --rm --user "$(id -u):$(id -g)" -v "$PWD:/src" hdf-pinvoke-alpine build
# After downloading/building the NuGet package:
docker run --rm --user "$(id -u):$(id -g)" -v "$PWD:/src" hdf-pinvoke-alpine consumer
```

The container builds native sources on its Linux filesystem and copies native assets, test summaries and detailed test logs back into the checkout. The loader uses the .NET runtime's musl identifier together with the process architecture; it does not probe a glibc runtime directory on musl. Publishing for Alpine must use `linux-musl-x64` or `linux-musl-arm64`.

Musl builds set [CMAKE_PLATFORM_NO_VERSIONED_SONAME](https://cmake.org/cmake/help/latest/variable/CMAKE_PLATFORM_NO_VERSIONED_SONAME.html) so the high-level library depends on the same unversioned core filename used by the managed loader. Copying versioned symlink aliases into separate package files otherwise allows musl to load two independent HDF5 cores, causing high-level calls to reject identifiers created by the first core. The package therefore requires neither symlinks nor runtime filesystem changes.

Thread safety and SWMR solve different problems. A supported thread-safe native variant would need to disable the high-level library and adjust the package/API contract accordingly. Do not enable `HDF5_ALLOW_UNSUPPORTED` merely to bypass that restriction. For current applications, serialize calls within each process and use the SWMR protocol for concurrent file access by separate processes. Compression filters do not change those synchronization requirements.

## Formatting and API documentation

The root `.editorconfig` defines whitespace and indentation. Run these commands from the repository root after restoring the solution:

```sh
dotnet format whitespace HDF.PInvoke.slnx --no-restore
dotnet format whitespace HDF.PInvoke.slnx --no-restore --verify-no-changes
dotnet build HDF.PInvoke.slnx -c Release --no-restore
```

Keep formatting and documentation changes separate from signature, marshalling and behavior changes. Preserve the native API names. Document public types, methods, parameters, return values, constants and fields with XML comments, including ownership, encoding and callback contracts where relevant. The library emits XML documentation for IntelliSense and packaging; missing public documentation, malformed XML and mismatched parameter documentation fail its build.

Use short comments to explain interop constraints and non-obvious decisions. Helper scripts expose parameter help through `Get-Help ./Build/<script>.ps1 -Full`. PowerShell scripts can be formatted with PSScriptAnalyzer's `Invoke-Formatter`, Bash with `shfmt -i 4`, and workflow YAML with Prettier. Validate Bash with `bash -n Build/Alpine/alpine.sh` and workflows with actionlint. Do not format or edit the upstream `Native/hdf5` checkout as part of wrapper cleanup.

## Interop and package regression checks

`InteropRegressionTests` covers negative chunk-count status, error-detection settings, identifier-type reference counts, conversion-data pointers, page-buffer outputs, and reverse P/Invoke callback arguments. Append callbacks receive a read-only native pointer to one `ulong` per dataset dimension; filter callbacks receive a pointer to `cd_nelmts` `uint` values. Read those pointers only during the callback and keep the delegate alive while HDF5 can invoke it. External-link group/object names are UTF-8 strings. `H5T.find` returns a native-owned conversion-data pointer; never free it. Page-buffer statistics validate all five arrays before native code can write two counters into each.

`Build/validatePack.ps1` checks the completed archive for the exact requested runtime set, the exact managed framework groups, parseable XML API documentation, matching package metadata and README, native source pins, full-suite test records, native hashes and the combined root license. `Build/testPackValidation.ps1` checks rejection of missing managed/native/license files, altered licenses, duplicate runtime licenses, extra documentation, corrupt API documentation and README, incorrect framework or license metadata, unexpected managed assets, corrupt libraries, extra RIDs, unverified files, wrong source pins and failed test records. A local `-RequiredRids` selection excludes other staged supported RIDs consistently on every operating system. `Build/testPackSelection.ps1` switches repeatedly between partial and combined packages to detect stale incremental-pack output; the project tracks the RID/file selection as a pack input.

Clean consumers map `HDF.PInvoke` exclusively to the local candidate feed and use isolated caches. They reopen and read a compressed UTF-8-named dataset written with `H5DO.append`, exercising both libraries and zlib, then check load contexts and shutdown ordering in separate processes. GitHub Actions keeps `nuget-packages-candidate` separate from the final `nuget-packages` artifact; the final job depends on all seven consumer checks and validates the archive again. No public-feed publishing is performed.

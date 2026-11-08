# Legacy .NET integration

The `netstandard2.0` assembly provides the HDF.PInvoke API to runtimes that cannot use the `net8.0` assembly. NuGet selects the appropriate managed asset; both use the same HDF5 2.2.x native libraries. See [README.md](../README.md) for installation, supported native platforms and API examples, and [ADVANCED.md](ADVANCED.md) for native builds and packaging.

## .NET Framework applications

Use an SDK-style project with PackageReference. The validated Windows configuration targets .NET Framework 4.7.2 and runs on the installed .NET Framework 4.8 runtime. Select x64 and publish with the matching runtime identifier:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net472</TargetFramework>
    <OutputType>Exe</OutputType>
    <PlatformTarget>x64</PlatformTarget>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="HDF.PInvoke" Version="2.2.1" />
  </ItemGroup>
</Project>
```

```sh
dotnet publish -c Release -r win-x64
```

Configure a package source containing the built package. Install the Microsoft Visual C++ v14 Redistributable for x64, as described in [README.md](../README.md#supported-platforms). Deploy the entire publish directory, including both native DLLs. A successful reference to a .NET Standard assembly does not by itself deploy its native dependencies.

Older Framework versions, packages.config and Unity are outside the validated configurations. Standalone Mono validation is described below. NativeAOT, trimming, IL2CPP and single-file deployment are not supported configurations. The native package has no 32-bit or mobile assets.

## Native library selection

Configure an override before any HDF5 operation, using the first applicable setting:

1. `NativeDependencies.Configure(absoluteDirectory)`.
2. `AppContext.SetData(NativeDependencies.NativePathSetting, absoluteDirectory)`.
3. The `HDF5_NATIVE_PATH` environment variable.

Without an override, the loader probes `runtimes/<process-rid>/native` under the managed assembly's directory, then the assembly directory itself, followed by the equivalent locations under the application base directory. Explicit paths must be absolute. Missing, invalid or incompatible files produce an error without falling back to a system HDF5 installation. Configuration cannot change after loading.

Windows uses `LoadLibraryEx` with safe dependency-search flags and `GetProcAddress`. Unix uses `dlopen` and `dlsym`, with entry points for glibc, musl and macOS. Linux libc detection reads `/proc/self/maps`; installing musl alongside glibc does not switch the process's native asset selection. The process architecture determines the RID, including when x64 code runs under emulation on an ARM64 host.

Both function calls and exported globals use the selected core-library handle. The high-level library is loaded from the same directory. Native handles remain valid for process lifetime. The loader does not modify PATH, change the working directory, create symlinks or write deployment files.

Call `H5.dont_atexit()` before any other HDF5 operation to disable native automatic shutdown. The loader performs this call before its version check initializes HDF5. Subsequent or late calls return a negative status. Loading and configuration are synchronized, but the supplied native HDF5 build is not thread-safe: serialize all HDF5 operations across the process.

## Why a separate implementation is necessary

.NET Standard 2.0 describes a managed API surface, not a particular runtime or native-library deployment model. Compiling against it makes the assembly consumable by older runtimes, but does not give those runtimes modern native-loading or marshalling capabilities. There are several independent problems to solve:

| Problem | Why it matters | Implementation |
| --- | --- | --- |
| No modern `NativeLibrary.SetDllImportResolver` API in the target surface | Ordinary imports cannot be redirected through the modern assembly resolver | Replace HDF5 imports at build time with typed delegates resolved from an explicitly loaded handle |
| Functions and exported variables need the same HDF5 instance | A function can succeed while a predefined datatype or property-list identifier comes from a different library | Route function lookup and `H5DLLImporter` through the same cached core handle |
| Old marshalling support differs from modern .NET | UTF-8 parameters, callback strings and string fields cannot all rely on the modern annotations | Use an ownership-aware custom marshaler and explicit native pointer structures |
| Native C types have platform-dependent layouts | Windows C `long` is 32-bit even in a 64-bit process; supported Unix platforms use 64-bit C `long` | Use compatibility value types and a Windows-specific metadata-cache structure bridge |
| CLR initialization can happen earlier than the first visible call | Early loading can prevent configuration or `H5.dont_atexit()` from taking effect | Defer handle and delegate initialization until actual use |
| Native code can retain managed callback pointers | A delegate or temporary comment buffer can disappear while HDF5 still uses it | Retain registrations explicitly and distinguish borrowed memory from owned buffers |

The legacy assembly resolves callable exports and global symbols through one selected library handle. Generated forwarding methods provide the unmanaged calls, with explicit bridges for signatures requiring platform-specific layouts or ownership handling.

## Build-time separation

The [library project](../Sources/HDF.PInvoke/HDF.PInvoke.csproj) includes `Interop/Modern` for `net8.0` and `Interop/Legacy` for `netstandard2.0`. Both directories define the target's implementation of the same `NativeDependencies` type. There is no runtime backend interface, service registration or switch between implementations inside an assembly. NuGet selects the managed assembly; MSBuild has already selected its loader.

The modern assembly keeps its direct HDF5 P/Invoke declarations and native import resolver. The legacy assembly contains generated methods with the same public signatures instead. Small conditional aliases are used where shared helpers need target-specific types: for example, `H5DLLImporter` aliases `NativeLibrary` to `LegacyNativeLibrary` under `NETSTANDARD2_0`. This avoids introducing an abstraction into every native call.

The repository builds with the SDK pinned in `global.json`. The compiler-hosted generator targets .NET Standard 2.0 and references a pinned Roslyn package compatible with that SDK. The generator is a private build dependency: consumers receive compiled wrapper assemblies and do not need the generator or Roslyn at runtime.

### How source generation works

[Tools/BindingGenerator](../Tools/BindingGenerator) implements Roslyn's `IIncrementalGenerator`. It does not parse HDF5 C headers or invent signatures; the declarations in `Sources/HDF.PInvoke/HDF5` remain authoritative. The modern target compiles those files directly. For the legacy target, the project selects them as `AdditionalFiles` during project evaluation and references the generator as an analyzer. The legacy loader and compatibility helpers remain ordinary compilation inputs.

The legacy compilation follows these steps:

1. MSBuild builds the generator through a normal project reference and supplies the selected additional files to the compiler.
2. The generator reads each selected file and the compiler's actual C# parse options, including preprocessor symbols.
3. It validates syntax and string ownership at the original source locations, then replaces supported HDF5 `DllImport` declarations with managed forwarders, typed unmanaged delegates and lazy caches, or selects an explicit bridge for a special signature.
4. It transforms UTF-8 marshalling attributes according to whether the call owns or borrows its string buffers.
5. It adds the generated sources directly to the legacy compilation through Roslyn's `AddSource` API.

Roslyn owns the generated syntax trees and their lifetime within each compilation. Changes to one input do not require transforming unrelated files within a retained generator driver; a fresh compiler process can still regenerate all inputs. Removing an input removes its generated tree from the next compilation. Relative source paths produce stable, collision-resistant generated names across checkout locations, including files with equal basenames in different directories.

Calling conventions, parameter attributes, return marshalling, `ref`/`out` modifiers, optional public arguments and XML documentation are retained. Delegate declarations omit optional argument defaults; the public forwarding methods retain them. The rewriter changes attribute syntax rather than globally replacing strings, so comments and string literals containing names such as `UnmanagedType.LPUTF8Str` remain intact.

The transformation deliberately recognizes a constrained set of import forms. Non-static or non-extern methods, generic imports, unknown library names, unsupported import options, string returns or `ref`/`out` strings without ownership handling, and unknown UTF-8 structure fields fail generation. Parsing validates generated syntax; the subsequent C# compilation validates types. Neither check can prove that a native signature matches the C ABI, which is why header-derived probes and native integration tests remain necessary.

| Diagnostic | Meaning |
| --- | --- |
| `HDFGEN001` | Invalid source syntax or an unsupported binding declaration; points to the original input when available |
| `HDFGEN002` | A selected additional file could not be read |
| `HDFGEN003` | The selected input set contains no HDF5 imports |

Additional-file diagnostics use external-file locations so editors can navigate to the original declaration. Errors detected when compiling the generated code still refer to generated sources. The original legacy inputs are additional files rather than normal compilation trees, so their semantic editing experience can differ from the modern target. Compiler cancellation propagates through parsing and rewriting instead of being reported as a binding failure.

Generated methods carry an internal [LegacyImportAttribute](../Sources/HDF.PInvoke/Interop/Legacy/LegacyImportAttribute.cs) recording the original library and entry-point names. Export validation can therefore inspect the legacy assembly even though these methods no longer carry `DllImport` metadata. The attribute is validation metadata; delegate resolution uses the generated library and symbol arguments directly.

Generated files need not exist on disk. To inspect them, use the compiler's optional output setting:

```sh
dotnet build Sources/HDF.PInvoke -c Release -f netstandard2.0 --no-incremental -p:EmitCompilerGeneratedFiles=true
```

The default inspection directory is `obj/Release/netstandard2.0/generated` beneath the library project. These files are snapshots, not authoritative compilation inputs; change the shared declaration or generator instead. Generator regression tests run separately from generation, through the test project and the Windows/Unix and Alpine validation scripts.

### What a generated call does

A call such as `H5F.open(...)` follows this path in the legacy assembly:

```text
Public forwarding method
  -> cached typed delegate
     -> first use: NativeDependencies.GetFunction<T>(library, entryPoint)
        -> load and validate the selected native library if necessary
        -> GetProcAddress or dlsym
        -> Marshal.GetDelegateForFunctionPointer<T>
     -> invoke the native function with the declared marshalling rules
```

Each generated delegate carries `UnmanagedFunctionPointer` metadata, including the calling convention. The CLR supplies the unmanaged transition; the wrapper does not use reflection invocation or emit IL for each call. Once initialized, the forwarder reuses its typed delegate rather than looking up the symbol again.

The cache uses `LazyInitializer.EnsureInitialized`. A successful lookup is retained; an exception during lookup leaves the cache empty. This allows a failed direct initialization call such as `H5.open()` to be retried after correcting a missing native directory. The cache's explicit static constructor suppresses the CLR's `beforefieldinit` behavior without performing native loading in that constructor. `H5DLLImporter` also has an explicit static constructor to avoid eager global initialization.

This is not a general recovery guarantee after any failed HDF5 use. If loading fails inside another type's static initializer, the CLR can retain that type-initialization failure. Configure deployment before accessing HDF5 types or globals, and use a fresh process when testing initialization failures.

## Native handles, globals and initialization

[NativeDependencies](../Sources/HDF.PInvoke/Interop/Legacy/NativeDependencies.cs) owns configuration, probing, synchronization, release-family validation and the cached handles. [LegacyNativeLibrary](../Sources/HDF.PInvoke/Interop/Legacy/LegacyNativeLibrary.cs) provides the concrete operating-system calls. It uses `LoadLibraryExW` on Windows with DLL-directory/default-directory search flags, `libdl.so.2` on glibc Linux, `libc` on musl, and `/usr/lib/libSystem.B.dylib` on macOS. These are platform loader entry points, not hard-coded HDF5 installation paths.

The loader first determines the process RID and applies the directory-selection rules above. It validates the core library with `H5get_libversion`, requiring the HDF5 2.2 release family before retaining the handle. The high-level library is loaded lazily from the same selected directory. Native dependencies still need to be compatible with the host: selecting the correct RID does not remove the VC++ runtime requirement or lower a Linux binary's glibc baseline.

[H5DLLImporter](../Sources/HDF.PInvoke/HDF5/H5DLLImporter.cs) handles exported data symbols, not managed assembly resolution. Predefined HDF5 identifiers such as datatype IDs are native global variables. The helper gets the same core handle used by the forwarding methods, calls `H5.open()` to initialize predefined identifiers, resolves the global's address and reads its 64-bit value. Function calls and global lookup therefore use the same HDF5 instance, including when the application selects a custom native directory.

Disabling native automatic shutdown needs special ordering. Even the version query initializes HDF5, so `H5.dont_atexit()` must ask the loader to invoke the native opt-out before performing that query. Normal calls initialize through the regular load-and-validate path. Native handles remain loaded for process lifetime because cached delegates and exported addresses depend on them. `H5.close()` shuts down HDF5 state; it does not unload the operating-system library handle.

The loader lock makes configuration and first loading consistent. It does not make arbitrary HDF5 calls thread-safe. Applications must still serialize all calls to the supplied native build, including calls made through different wrapper instances or managed load contexts.

## ABI and marshalling bridges

### C long and Windows structure layouts

The legacy assembly exposes [CLong](../Sources/HDF.PInvoke/Interop/Legacy/CLong.cs) and [CULong](../Sources/HDF.PInvoke/Interop/Legacy/CULong.cs) in the `HDF.PInvoke` namespace. The modern assembly uses the runtime's equivalents. These legacy types are compatibility representations for the structures used here, not a general replacement for platform-sized C integers in arbitrary P/Invoke declarations.

`CLong` stores a managed 64-bit value. Passing the public metadata-cache structure directly on Windows would therefore give native code incorrect field offsets after its 32-bit `long` field. The generator creates `H5AC.WindowsCacheConfig` with the native Windows field widths and fixed trace-file-name buffer, plus conversion methods. The generated `H5F.get_mdc_config` and `H5F.set_mdc_config` forwarders select this bridge on Windows; Unix calls use the shared structure directly.

The getter initializes only the version field expected as input. It copies the native result back only after success, so unrelated incoming values cannot cause narrowing failures and a native error does not overwrite the caller's structure. The setter converts the full input, including a checked conversion of `epoch_length` to 32 bits. An out-of-range value fails instead of silently truncating.

`CULong` serves the unsigned-long field in the padded object-info layout. Its accessor masks the stored value to 32 bits on Windows, excluding adjacent padding that is not part of the native field's value. This relies on the specific enclosing layout, which the ABI probe checks; it should not be extrapolated to every native unsigned-long field.

Use the supplied typed API methods for these conversions. In particular, the raw `IntPtr` overloads of `H5P.get_mdc_config` and `H5P.set_mdc_config` still require a correctly laid-out native buffer. Manually marshalling the public legacy `cache_config_t` on Windows bypasses the bridge and is incorrect.

### UTF-8 strings and memory ownership

[UTF8Marshaler](../Sources/HDF.PInvoke/Interop/Legacy/UTF8Marshaler.cs) implements `ICustomMarshaler`. The generator replaces UTF-8 parameter annotations with this custom marshaler so the legacy path does not depend on modern built-in UTF-8 marshalling behavior. Encoding and decoding operate directly on native buffers without temporary byte arrays.

For a managed-to-native input string, the marshaler allocates a NUL-terminated buffer with `Marshal.AllocHGlobal`, encodes UTF-8 into it, and releases it through `Marshal.FreeHGlobal` when the call's marshalling cleanup runs. A null string becomes a null pointer; an empty string becomes a non-null pointer to a terminator. These temporary input buffers are not suitable for an API that retains a pointer after returning.

For a native-to-managed callback parameter, the native caller owns the string. Generated callback declarations select the same marshaler with `MarshalCookie = "borrowed"`. It decodes the pointer while the callback is active, but its native cleanup does nothing. Without this distinction, callback cleanup could free memory owned by HDF5 or by an outer managed call that is still using it.

Ownership cannot be inferred safely for every possible string signature. The generator rejects string returns and `ref`/`out` strings until an explicit bridge defines who allocates and frees their buffers. Shared `StringBuilder` output helpers already use explicit byte buffers and decoding; raw byte-array APIs retain their documented byte-count and NUL-termination requirements.

### Callback output pointers

Mono 6.8 does not implement by-reference delegate marshalling. Seven HDF5 getters expose callback output pointers through public `ref` delegate parameters. Shared managed forwarding methods in H5E and H5P use private native imports with output IntPtr callback parameters. This also avoids modern .NET retaining an existing delegate instead of copying back the native callback output. After a successful call, the forwarder converts nonzero addresses with `Marshal.GetDelegateForFunctionPointer<T>` and returns null for a zero pointer. A native error leaves the caller's callback reference unchanged.

These are explicitly selected output-only getter contracts, not a general transformation of every `ref` parameter. Existing callback values are not passed into the native getter, and returned addresses remain borrowed. Applications must still retain registered callbacks for as long as native code can invoke them. The public signatures remain unchanged. Both targets use the same forwarding methods; the legacy generator processes the private pointer-based imports normally. Regression tests cover callback invocation, context pointers, null callbacks, failure preservation and the getter returning both allocation and release callbacks.

Raw byte-array string parameters must include their own terminating zero byte. Unlike the managed string overloads, the wrapper does not append a terminator to a byte array. Tests must allocate this byte explicitly; relying on padding beyond an array's end produces runtime-dependent failures and native out-of-bounds reads.

### Error callbacks containing string fields

A custom parameter marshaler does not solve nested string fields in a native structure on older runtimes. [LegacyErrorWalk](../Sources/HDF.PInvoke/Interop/Legacy/LegacyErrorWalk.cs) therefore handles `H5E.walk` through a native-layout `ErrorInfo` structure whose string fields are `IntPtr` values. During the native callback, it reads that structure, decodes its borrowed UTF-8 strings, constructs the public `H5E.error_t` view and invokes the user's callback.

The bridge keeps its delegate alive until the walk returns. It catches managed exceptions inside the callback, returns a negative native status, then rethrows the captured exception with `ExceptionDispatchInfo` after native control has returned. This exception-handling behavior belongs to this bridge; callers must not assume every callback API provides it.

For the known public `error_t` and `class_t` string fields, generated metadata retains the numeric UTF-8 marshalling value `(UnmanagedType)48`. This preserves the declaration without requiring that enum member in the old reference surface. It does not add runtime marshalling support: actual native calls use the explicit pointer-layout bridges. Other UTF-8 structure fields are rejected until their native path is handled explicitly.

### Registrations that outlive a call

HDF5 copies a user-defined link registration but retains its comment and callback pointers. Ordinary call-scoped string marshalling and a temporary delegate reference are insufficient: after the call, native code could dereference freed memory or invoke a collected delegate.

The shared [LinkClassRegistry](../Sources/HDF.PInvoke/HDF5/LinkClassRegistry.cs), used by both targets, owns a native UTF-8 comment buffer and keeps strong references to the registered delegates. Its fixed-size registration table follows HDF5's bounded link-type IDs and reserves storage before native code can retain a pointer. Successful replacement or unregistration releases the old registration; a failed registration releases only the new temporary allocation. Successful explicit `H5.close()` releases the retained registrations after native shutdown has completed.

This is targeted ownership handling for user-defined links. For other callback APIs, retain delegates and associated state for the full lifetime specified by the native API. Serializing loader initialization or storing a function delegate does not automatically retain every user callback passed through it.

## Implementation map and maintenance

| File or directory | Responsibility |
| --- | --- |
| [HDF.PInvoke.csproj](../Sources/HDF.PInvoke/HDF.PInvoke.csproj) | Select target-specific compilation inputs and the legacy analyzer reference |
| [LegacyBindingGenerator.cs](../Tools/BindingGenerator/LegacyBindingGenerator.cs) | Integrate incremental inputs, generated sources, cancellation and source diagnostics with Roslyn |
| [LegacyBindings.cs](../Tools/BindingGenerator/LegacyBindings.cs) | Rewrite imports, create delegate caches and generate the Windows cache-configuration bridge |
| [BindingSource.cs](../Tools/BindingGenerator/BindingSource.cs) | Validate syntax and rewrite UTF-8 attributes with explicit ownership rules |
| [Interop/Legacy](../Sources/HDF.PInvoke/Interop/Legacy) | Load native handles and provide compatibility types, UTF-8 marshalling and the error-walk bridge |
| [H5DLLImporter.cs](../Sources/HDF.PInvoke/HDF5/H5DLLImporter.cs) | Resolve exported native globals through the selected core handle |
| [LinkClassRegistry.cs](../Sources/HDF.PInvoke/HDF5/LinkClassRegistry.cs) | Retain native registration memory and managed callback roots |

Add or correct ordinary signatures in the shared `HDF5` sources. If a declaration introduces new marshalling semantics, update the generator or add a narrowly scoped bridge, and add a regression that exercises the actual native call. Avoid maintaining a second hand-written list of imports. Inspect generated output when debugging, but verify both targets after changing a shared declaration.

The extra runtime work consists of lazy delegate resolution, a cached delegate invocation and any required string or structure conversion. Native symbol lookup occurs on first use of each cache, not on every successful call. There is no blanket performance guarantee relative to modern P/Invoke; profile representative workloads before adding further caching or allocation complexity.

Legacy support is isolated in its target, loader files and generator integration. Shared native declarations and registration lifetime management serve both managed targets.

## Testing changes

After building native libraries, run:

```sh
pwsh -File Build/testLegacy.ps1 -Rid win-x64
dotnet test Tests/BindingGenerator.Tests -c Release -p:RestoreLockedMode=true
```

Validation covers distinct failure classes:

| Check | What it establishes |
| --- | --- |
| [Generator regression checks](../Tests/BindingGenerator.Tests) | Preserve marshalling contracts and metadata; validate source diagnostics, cancellation, input removal, stable output names and incremental caching |
| Shared integration suite with `HdfLegacy=true` | Exercise the actual .NET Standard assembly against native HDF5, including metadata-cache conversions, UTF-8 strings and callback lifetimes |
| Windows net472 execution | Exercise legacy CLR marshalling on the installed .NET Framework 4.8 runtime |
| Header-derived ABI and export checks | Compare native structure sizes/offsets with the layouts actually used by bridges, and verify imported symbols through legacy metadata |
| Separate-process loader scenarios | Check first-use ordering, path failures, retry of direct initialization, version rejection, globals, load contexts and shutdown behavior |
| Clean package consumers | Verify NuGet asset selection, native deployment, compression, core/high-level integration and both managed implementations |

The [legacy test script](../Build/testLegacy.ps1) first runs the generator regression suite, then runs the shared integration suite against `netstandard2.0`, adds a net472 run on Windows x64, and checks loader behavior in separate processes. `-SkipAbi` is available only for local reuse of previously validated native binaries. The native upstream suite validates HDF5 itself; it does not replace tests of managed signatures and marshalling.

CI tests the legacy assembly on every supported native RID. Unix CI legacy tests execute on modern .NET hosts; standalone Mono is checked separately as described below, and Unity compatibility is not established. Clean consumers explicitly force the .NET Standard asset where NuGet would otherwise prefer the modern asset, then verify which implementation was loaded. See [ADVANCED.md](ADVANCED.md#validation-and-ci) for the shared native suite and package-validation process.

## Standalone Mono validation

The legacy assembly is locally validated on Ubuntu 24.04 x64 under WSL using the distribution's Mono 6.8 runtime. This exercises Mono's native marshaller, including UTF-8 strings, callbacks, ABI bridges and exported globals. It does not establish compatibility with Unity's Mono fork, Editor lifecycle, IL2CPP, mobile platforms or other Mono versions. The GitHub matrix uses modern .NET hosts for Unix legacy tests; Mono validation is a separate local check.

Install `mono-devel` from the Linux distribution's package repository. Build or stage the Linux x64 native libraries, and extract the `Microsoft.TestPlatform` NuGet package (validated with version 18.10.1) into a local tools directory. The runner must be its `net462` version: running tests through `dotnet test` would exercise CoreCLR rather than Mono.

From Windows with Mono installed in the default WSL distribution, run:

```powershell
./Build/testMono.ps1 -Wsl -TestPlatformDirectory C:/Tools/Microsoft.TestPlatform
```

On Linux with the repository's required .NET SDK, PowerShell and Mono installed, omit `-Wsl`. The script builds the net472 integration tests against the netstandard2.0 wrapper, invokes VSTest explicitly under Mono, and writes `Build/Artifacts/tests/linux-x64-mono/mono.trx`. `-SkipBuild` reuses an already-built test assembly. Run the package consumer separately when validating NuGet deployment; this script validates the project build against staged native libraries.

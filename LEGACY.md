# Legacy .NET integration

The `netstandard2.0` assembly provides the HDF.PInvoke API to runtimes that cannot use the `net8.0` assembly. NuGet selects the appropriate managed asset; both use the same HDF5 2.2.x native libraries. See [README.md](README.md) for installation, supported native platforms and API examples, and [ADVANCED.md](ADVANCED.md) for native builds and packaging.

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
    <PackageReference Include="HDF.PInvoke" Version="2.2.0" />
  </ItemGroup>
</Project>
```

```sh
dotnet publish -c Release -r win-x64
```

Configure a package source containing the built package. Install the Microsoft Visual C++ v14 Redistributable for x64, as described in [README.md](README.md#supported-platforms). Deploy the entire publish directory, including both native DLLs. A successful reference to a .NET Standard assembly does not by itself deploy its native dependencies.

Older Framework versions, packages.config, Mono and Unity are outside the validated configurations. NativeAOT, trimming, IL2CPP and single-file deployment are not supported configurations. The native package has no 32-bit or mobile assets.

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

Changing DLL names with `#if` would address only part of the first problem. It would still leave the runtime in charge of resolving each import independently, without a portable way to bind those calls to the exact handle used for globals. The implementation therefore combines build-time selection with generated forwarding methods and a small set of explicit ABI bridges.

## Build-time separation

The [library project](Sources/HDF.PInvoke/HDF.PInvoke.csproj) includes `Interop/Modern` for `net8.0` and `Interop/Legacy` for `netstandard2.0`. Both directories define the target's implementation of the same `NativeDependencies` type. There is no runtime backend interface, service registration or switch between implementations inside an assembly. NuGet selects the managed assembly; MSBuild has already selected its loader.

The modern assembly keeps its direct HDF5 P/Invoke declarations and native import resolver. The legacy assembly contains generated methods with the same public signatures instead. Small conditional aliases are used where shared helpers need target-specific types: for example, `H5DLLImporter` aliases `NativeLibrary` to `LegacyNativeLibrary` under `NETSTANDARD2_0`. This avoids introducing an abstraction into every native call.

The repository builds with the SDK pinned in `global.json`. The generator itself targets .NET 10 and uses Roslyn assemblies shipped with that SDK. This is a contributor/build requirement: consumers receive compiled assemblies and do not need the generator, Roslyn or the repository's SDK to run the legacy assembly.

### How the source transformation works

[Tools/BindingGenerator](Tools/BindingGenerator) is a standalone console tool invoked by an MSBuild target before `CoreCompile`. It uses Roslyn syntax trees, but is not a compiler-hosted `ISourceGenerator` or `IIncrementalGenerator`. It does not parse HDF5 C headers or invent signatures; the declarations in `Sources/HDF.PInvoke/HDF5` remain authoritative.

The legacy compilation follows these steps:

1. Build the generator and execute its regression checks.
2. Parse the shared C# files using the legacy compilation's actual `DefineConstants`.
3. Replace each supported HDF5 `DllImport` method with a managed forwarder, a typed unmanaged delegate and a lazy delegate cache, or select an explicit bridge for a special signature.
4. Transform UTF-8 marshalling attributes according to whether the call owns or borrows its string buffers.
5. Write generated C# files below `obj/<configuration>/netstandard2.0/LegacyBindings` and compile these in place of the original `HDF5/**/*.cs` files.

Calling conventions, parameter attributes, return marshalling, `ref`/`out` modifiers, optional public arguments and XML documentation are retained. Delegate declarations omit optional argument defaults; the public forwarding methods retain them. The rewriter changes attribute syntax rather than globally replacing strings, so comments and string literals containing names such as `UnmanagedType.LPUTF8Str` remain intact.

The transformation deliberately recognizes a constrained set of import forms. Unknown library names, unsupported import options, string returns or `ref`/`out` strings without ownership handling, and unknown UTF-8 structure fields cause generation to fail. Parsing validates generated syntax; the subsequent C# compilation validates types. Neither check can prove that a native signature matches the C ABI, which is why header-derived probes and native integration tests remain necessary.

Generated methods carry an internal [LegacyImportAttribute](Sources/HDF.PInvoke/Interop/Legacy/LegacyImportAttribute.cs) recording the original library and entry-point names. Export validation can therefore inspect the legacy assembly even though these methods no longer carry `DllImport` metadata. The attribute is validation metadata; delegate resolution uses the generated library and symbol arguments directly.

Generation processes files in deterministic order and avoids rewriting unchanged output. It removes obsolete files only when they carry its own generated-file marker. Generated files are build products: inspect them to debug a transformation, but change the shared declaration or generator rather than editing the generated copy.

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

[NativeDependencies](Sources/HDF.PInvoke/Interop/Legacy/NativeDependencies.cs) owns configuration, probing, synchronization, release-family validation and the cached handles. [LegacyNativeLibrary](Sources/HDF.PInvoke/Interop/Legacy/LegacyNativeLibrary.cs) provides the concrete operating-system calls. It uses `LoadLibraryExW` on Windows with DLL-directory/default-directory search flags, `libdl.so.2` on glibc Linux, `libc` on musl, and `/usr/lib/libSystem.B.dylib` on macOS. These are platform loader entry points, not hard-coded HDF5 installation paths.

The loader first determines the process RID and applies the directory-selection rules above. It validates the core library with `H5get_libversion`, requiring the HDF5 2.2 release family before retaining the handle. The high-level library is loaded lazily from the same selected directory. Native dependencies still need to be compatible with the host: selecting the correct RID does not remove the VC++ runtime requirement or lower a Linux binary's glibc baseline.

[H5DLLImporter](Sources/HDF.PInvoke/HDF5/H5DLLImporter.cs) handles exported data symbols, not managed assembly resolution. Predefined HDF5 identifiers such as datatype IDs are native global variables. The helper gets the same core handle used by the forwarding methods, calls `H5.open()` to initialize predefined identifiers, resolves the global's address and reads its 64-bit value. This prevents the old failure pattern in which function imports find one library while global lookup attempts a different fixed Linux path.

Disabling native automatic shutdown needs special ordering. Even the version query initializes HDF5, so `H5.dont_atexit()` must ask the loader to invoke the native opt-out before performing that query. Normal calls initialize through the regular load-and-validate path. Native handles remain loaded for process lifetime because cached delegates and exported addresses depend on them. `H5.close()` shuts down HDF5 state; it does not unload the operating-system library handle.

The loader lock makes configuration and first loading consistent. It does not make arbitrary HDF5 calls thread-safe. Applications must still serialize all calls to the supplied native build, including calls made through different wrapper instances or managed load contexts.

## ABI and marshalling bridges

### C long and Windows structure layouts

The legacy assembly exposes [CLong](Sources/HDF.PInvoke/Interop/Legacy/CLong.cs) and [CULong](Sources/HDF.PInvoke/Interop/Legacy/CULong.cs) in the `HDF.PInvoke` namespace. The modern assembly uses the runtime's equivalents. These legacy types are compatibility representations for the structures used here, not a general replacement for platform-sized C integers in arbitrary P/Invoke declarations.

`CLong` stores a managed 64-bit value. Passing the public metadata-cache structure directly on Windows would therefore give native code incorrect field offsets after its 32-bit `long` field. The generator creates `H5AC.WindowsCacheConfig` with the native Windows field widths and fixed trace-file-name buffer, plus conversion methods. The generated `H5F.get_mdc_config` and `H5F.set_mdc_config` forwarders select this bridge on Windows; Unix calls use the shared structure directly.

The getter initializes only the version field expected as input. It copies the native result back only after success, so unrelated incoming values cannot cause narrowing failures and a native error does not overwrite the caller's structure. The setter converts the full input, including a checked conversion of `epoch_length` to 32 bits. An out-of-range value fails instead of silently truncating.

`CULong` serves the unsigned-long field in the padded object-info layout. Its accessor masks the stored value to 32 bits on Windows, excluding adjacent padding that is not part of the native field's value. This relies on the specific enclosing layout, which the ABI probe checks; it should not be extrapolated to every native unsigned-long field.

Use the supplied typed API methods for these conversions. In particular, the raw `IntPtr` overloads of `H5P.get_mdc_config` and `H5P.set_mdc_config` still require a correctly laid-out native buffer. Manually marshalling the public legacy `cache_config_t` on Windows bypasses the bridge and is incorrect.

### UTF-8 strings and memory ownership

[Utf8Marshaler](Sources/HDF.PInvoke/Interop/Legacy/Utf8Marshaler.cs) implements `ICustomMarshaler`. The generator replaces UTF-8 parameter annotations with this custom marshaler so the legacy path does not depend on modern built-in UTF-8 marshalling behavior. Encoding and decoding operate directly on native buffers without temporary byte arrays.

For a managed-to-native input string, the marshaler allocates a NUL-terminated buffer with `Marshal.AllocHGlobal`, encodes UTF-8 into it, and releases it through `Marshal.FreeHGlobal` when the call's marshalling cleanup runs. A null string becomes a null pointer; an empty string becomes a non-null pointer to a terminator. These temporary input buffers are not suitable for an API that retains a pointer after returning.

For a native-to-managed callback parameter, the native caller owns the string. Generated callback declarations select the same marshaler with `MarshalCookie = "borrowed"`. It decodes the pointer while the callback is active, but its native cleanup does nothing. Without this distinction, callback cleanup could free memory owned by HDF5 or by an outer managed call that is still using it.

Ownership cannot be inferred safely for every possible string signature. The generator rejects string returns and `ref`/`out` strings until an explicit bridge defines who allocates and frees their buffers. Shared `StringBuilder` output helpers already use explicit byte buffers and decoding; raw byte-array APIs retain their documented byte-count and NUL-termination requirements.

### Error callbacks containing string fields

A custom parameter marshaler does not solve nested string fields in a native structure on older runtimes. [LegacyErrorWalk](Sources/HDF.PInvoke/Interop/Legacy/LegacyErrorWalk.cs) therefore handles `H5E.walk` through a native-layout `ErrorInfo` structure whose string fields are `IntPtr` values. During the native callback, it reads that structure, decodes its borrowed UTF-8 strings, constructs the public `H5E.error_t` view and invokes the user's callback.

The bridge keeps its delegate alive until the walk returns. It catches managed exceptions inside the callback, returns a negative native status, then rethrows the captured exception with `ExceptionDispatchInfo` after native control has returned. This exception-handling behavior belongs to this bridge; callers must not assume every callback API provides it.

For the known public `error_t` and `class_t` string fields, generated metadata retains the numeric UTF-8 marshalling value `(UnmanagedType)48`. This preserves the declaration without requiring that enum member in the old reference surface. It does not add runtime marshalling support: actual native calls use the explicit pointer-layout bridges. Other UTF-8 structure fields are rejected until their native path is handled explicitly.

### Registrations that outlive a call

HDF5 copies a user-defined link registration but retains its comment and callback pointers. Ordinary call-scoped string marshalling and a temporary delegate reference are insufficient: after the call, native code could dereference freed memory or invoke a collected delegate.

The shared [LinkClassRegistry](Sources/HDF.PInvoke/HDF5/LinkClassRegistry.cs), used by both targets, owns a native UTF-8 comment buffer and keeps strong references to the registered delegates. Its fixed-size registration table follows HDF5's bounded link-type IDs and reserves storage before native code can retain a pointer. Successful replacement or unregistration releases the old registration; a failed registration releases only the new temporary allocation. Successful explicit `H5.close()` releases the retained registrations after native shutdown has completed.

This is targeted ownership handling for user-defined links. For other callback APIs, retain delegates and associated state for the full lifetime specified by the native API. Serializing loader initialization or storing a function delegate does not automatically retain every user callback passed through it.

## Implementation map and maintenance

| File or directory | Responsibility |
| --- | --- |
| [HDF.PInvoke.csproj](Sources/HDF.PInvoke/HDF.PInvoke.csproj) | Select target-specific files and invoke generation before legacy compilation |
| [Program.cs](Tools/BindingGenerator/Program.cs) | Manage generator inputs, compilation symbols, deterministic output and stale generated files |
| [LegacyBindings.cs](Tools/BindingGenerator/LegacyBindings.cs) | Rewrite imports, create delegate caches and generate the Windows cache-configuration bridge |
| [BindingSource.cs](Tools/BindingGenerator/BindingSource.cs) | Validate syntax and rewrite UTF-8 attributes with explicit ownership rules |
| [Interop/Legacy](Sources/HDF.PInvoke/Interop/Legacy) | Load native handles and provide compatibility types, UTF-8 marshalling and the error-walk bridge |
| [H5DLLImporter.cs](Sources/HDF.PInvoke/HDF5/H5DLLImporter.cs) | Resolve exported native globals through the selected core handle |
| [LinkClassRegistry.cs](Sources/HDF.PInvoke/HDF5/LinkClassRegistry.cs) | Retain native registration memory and managed callback roots |

Add or correct ordinary signatures in the shared `HDF5` sources. If a declaration introduces new marshalling semantics, update the generator or add a narrowly scoped bridge, and add a regression that exercises the actual native call. Avoid maintaining a second hand-written list of imports. Inspect generated output when debugging, but verify both targets after changing a shared declaration.

The extra runtime work consists of lazy delegate resolution, a cached delegate invocation and any required string or structure conversion. Native symbol lookup occurs on first use of each cache, not on every successful call. There is no blanket performance guarantee relative to modern P/Invoke; profile representative workloads before adding further caching or allocation complexity.

Legacy support is isolated in its target, loader files and generator integration. Retiring it would also remove legacy-only tests and package-validation cases. The shared native signatures and modern resolver can remain in place; the shared registration-lifetime fix is useful independently of legacy support.

## Testing changes

After building native libraries, run:

```sh
pwsh -File Build/test-legacy.ps1 -Rid win-x64
dotnet run --project Tools/BindingGenerator -c Release -- --self-test
```

Validation covers distinct failure classes:

| Check | What it establishes |
| --- | --- |
| Generator regression checks | Preserve attributes, XML documentation and compilation symbols; reject unsupported signatures; select the intended bridges |
| Shared integration suite with `HdfLegacy=true` | Exercise the actual .NET Standard assembly against native HDF5, including metadata-cache conversions, UTF-8 strings and callback lifetimes |
| Windows net472 execution | Exercise legacy CLR marshalling on the installed .NET Framework 4.8 runtime |
| Header-derived ABI and export checks | Compare native structure sizes/offsets with the layouts actually used by bridges, and verify imported symbols through legacy metadata |
| Separate-process loader scenarios | Check first-use ordering, path failures, retry of direct initialization, version rejection, globals, load contexts and shutdown behavior |
| Clean package consumers | Verify NuGet asset selection, native deployment, compression, core/high-level integration and both managed implementations |

The [legacy test script](Build/test-legacy.ps1) runs the shared integration suite against `netstandard2.0`, adds a net472 run on Windows x64, and checks loader behavior in separate processes. `-SkipAbi` is available only for local reuse of previously validated native binaries. The native upstream suite validates HDF5 itself; it does not replace tests of managed signatures and marshalling.

CI tests the legacy assembly on every supported native RID. Unix legacy tests execute on modern .NET hosts; they do not establish Mono or Unity compatibility. Clean consumers explicitly force the .NET Standard asset where NuGet would otherwise prefer the modern asset, then verify which implementation was loaded. See [ADVANCED.md](ADVANCED.md#validation-and-ci) for the shared native suite and package-validation process.

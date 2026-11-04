# Modernization review

This document records the assessment and decisions for the unreleased 2.2.0 migration. It describes the implementation, compatibility boundaries and validation requirements for reviewers.

## Previous-state assessment

The repository combined legacy project and solution formats, Paket/FAKE dependency and build tooling, AppVeyor configuration, bundled native binaries and conditional source branches for old HDF5 releases. Native builds were not reproducible from pinned upstream sources across the intended platform matrix. The managed loading paths and ABI declarations also contained platform assumptions that compilation alone could not detect.

### Native loading

A Linux reproduction showed that a .NET DllImport resolver fixed `H5.open()` but not `H5P.FILE_ACCESS`: the old global importer bypassed the resolver and called dlopen with `/usr/lib/libhdf5.so`. It also imported `libdl.so`, assumed Windows filenames, and had an `hdf5_hd.dll` typo in high-level name mapping. Separate code mutated PATH with Windows separators and relied on bin32/bin64 and System.Configuration.

Deployment also exposed a separate packaging defect: empty unversioned/SONAME files cannot be loaded as native libraries. Native assets must retain their actual contents when transferred through archives and package tooling.

## Loader changes

The .NET 10 module initializer registers one resolver for the binding assembly without eagerly loading native code. Functions and globals share cached NativeLibrary handles; globals use GetExport instead of a second dlopen/GetProcAddress path. Selection uses the process architecture. Explicit absolute-directory configuration, AppContext and environment overrides are supported before first use. App-local RID assets and .NET's assembly-aware probing support ordinary apps and plugin AssemblyLoadContexts. A selected local directory also supplies the high-level library.

No process-wide PATH/CWD mutation, filesystem writes or runtime symlinks are required. Missing/empty/wrong-format files produce contextual errors; explicit locations cannot silently fall back. Runtime version validation rejects native libraries outside 2.2.x. Native handles intentionally remain loaded for process lifetime; unloading a managed plugin does not unload HDF5 beneath cached native pointers.

Native builds materialize symlinks and retain library SONAME aliases as actual files. Packaging validates source revisions, hashes, canonical filenames and licenses. Clean package tests verify NuGet runtime asset selection separately from source-project deployments.

## ABI and marshalling findings

- HDF5 2.2 `hbool_t`/bool is one byte, not the old UInt32 representation. This affected parameters and structures.
- H5AC's trace filename was a UTF-16 fixed char buffer rather than a C byte buffer; epoch_length used a fixed-width long despite Windows/Unix C-long differences; dirty_bytes_threshold was int rather than size_t.
- H5O's file number uses C unsigned long; timestamps use signed 64-bit time_t on the supported platforms.
- H5I type IDs and H5F format bounds had stale numeric values. H5I free callbacks and H5L traversal callbacks had outdated argument lists; traversal also returned the wrong width.
- Corrected H5Dread_chunk1, H5Tdecode1, H5Iregister_type1, H5Fformat_convert, and the misspelled H5Pset_gc_references export. Optional Direct-I/O and Windows VFD methods now check availability explicitly.
- String arguments and reverse-P/Invoke strings are explicitly UTF-8. StringBuilder outputs use byte buffers and explicit UTF-8 decoding instead of the Windows ANSI code page. An output-only get_filter2 argument no longer uses immutable string storage. Error push messages escape percent signs so literal messages are not interpreted as missing varargs.

The native probe covers 17 exposed structures and 147 size/field-offset checks. The managed smoke test checks all required P/Invoke declarations against the loaded libraries and initializes exported global classes. Compatibility exports remain intentionally enabled; complete bindings for new async/VOL/token APIs are outside this migration.

## Versioning and framework decision

Wrapper 2.2.0 / assembly 2.2.0.0 is an unreleased major bump from the repository's 1.x history. The wrapper major/minor tracks native HDF5 2.2. Its patch increments for each wrapper release (2.2.0, 2.2.1, etc.), independently of the pinned native patch version. .NET 10 is the sole target. Multi-targeting .NET Framework would require a second loader implementation because NativeLibrary and per-assembly import resolvers are unavailable there; the migration therefore uses one .NET 10 loader implementation.

Consumers must recompile against v2 and review byte-sized booleans, CLong/CULong fields and callback changes. Removing System.Configuration eliminates an unnecessary managed deployment dependency. Version 2 does not claim binary compatibility with HDF.PInvoke 1.x or compatibility with every downstream application.

## Build and platform decisions

Paket/FAKE, AppVeyor, legacy project configurations and checked-in HDF5 1.8/1.10 binaries were removed. The repository now separates managed sources, tests, native inputs and build scripts into Sources/Tests/Native/Build. SDK projects, an XML solution, central NuGet versions and lock files replace the legacy build setup. The official source remains pinned to 49df1b4e5ca4108d56e80f06321a749da1535268; zlib is pinned separately.

Default RIDs are win-x64, win-arm64, linux-x64, linux-arm64, linux-musl-x64, linux-musl-arm64 and osx-arm64. Alpine builds use native-architecture containers, the same pinned HDF5 configuration, and native/managed/package validation. Musl native builds use unversioned SONAMEs to keep the managed loader and high-level library on one core instance. Materializing versioned symlink aliases as separate package files otherwise caused two managed high-level append tests to fail because musl loaded duplicate cores. The native core/high-level build is deliberately not thread-safe; consumers must serialize calls. CI runs native tests, ABI checks, managed integration and loader tests, packs all runtime assets, then verifies a clean published package consumer on each RID. It does not publish packages to a feed.

## Validation scope

Every supported runtime has a native-architecture CI job that builds the pinned source, runs the upstream CTest suite and ABI probe, and exercises the managed integration and loader tests. Native test entries include setup/cleanup fixtures; individual tests may internally skip unavailable features. Full or exhaustive upstream success is required before staging and packaging. WSL native build and test outputs should use the Linux filesystem to avoid slow mounted-Windows storage tests.

Managed validation covers native calls, Unicode round-trips, literal UTF-8 error messages, callback marshalling, negative error returns and output-buffer validation. Loader scenarios exercise function-first/global-first initialization, explicit private directories, unrelated working directories, invalid/empty/incompatible libraries, late/relative configuration rejection, shutdown ordering and a separate managed load context. Legacy disabled allocator/threading tests are excluded from coverage.

Clean consumers restore the actual NuGet archive from an isolated local feed and cache, publish for their target RID, and exercise compressed core/high-level I/O, loading and shutdown. Unix consumers execute from read-only publish directories. Archive validation checks the exact selected RID set, source pins, native hashes, license files and managed assets. The final seven-RID NuGet artifact is available only after all platform consumers pass. Local partial packages are explicitly selected through `-RequiredRids` and are not release packages.

## Native test-suite decision

Native builds default to upstream full mode (level 1), which runs the registered suite with reduced stress repetition counts. Use `-TestExpressLevel 0` for exhaustive stress iterations. Packaging accepts levels 0 and 1, not quick/smoke levels 2 and 3. No individual test exclusions are added.

The standalone C roundtrip smoke test has been removed. Native builds now enable the upstream C/API, SWMR and high-level tests for the components shipped, with `HDF_TEST_EXPRESS=1` (upstream full mode). CTest must succeed before installation and staging. An earlier manifest is invalidated at build start; a successful manifest records the upstream result, and the packaging script rejects runtimes without it. CI preserves JUnit results and CTest logs, including on failures.

The header-derived ABI probe and deliberately incompatible test library remain because they validate the managed/native boundary. Managed loader and clean NuGet-consumer tests remain because upstream tests do not validate .NET deployment. Components not shipped (MPI, C++, Fortran, Java and tools) are outside this suite configuration. Platform-specific skips within upstream tests must be distinguished from passing coverage.

## Scope exclusions

- A dedicated compatibility corpus generated by older HDF5 releases, including reading newly generated V110-bounded files with an old reader, is not included.
- Android/iOS require distinct toolchains, packaging and managed interop validation and are not supported targets.
- NativeAOT, trimming, single-file publishing and complete coverage of current HDF5 APIs are outside this migration.

## References

- [Official HDF5 2.2.0 release](https://github.com/HDFGroup/hdf5/releases/tag/2.2.0)
- [Native library loading in .NET](https://learn.microsoft.com/en-us/dotnet/standard/native-interop/native-library-loading)
- [NativeLibrary.Load and load contexts](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.nativelibrary.load?view=net-10.0)
- [GitHub-hosted runners](https://docs.github.com/en/actions/reference/runners/github-hosted-runners)

## Formatting and documentation conventions

The wrapper and tests use a shared `.editorconfig` and `dotnet format whitespace`. Public wrapper members now have XML documentation, including native constants, enum values, structure fields and callback contracts. Parameter documentation follows managed names and UTF-8 marshalling where applicable. The library produces an XML documentation file and treats missing or invalid public documentation as build errors. Strategic comments explain native initialization and UTF-8 buffer handling.

Helper scripts provide usage and parameter documentation and use consistent formatting and syntax validation. CI comments explain runtime collection, archive validation and clean package-consumer checks.

## Follow-up interop and packaging corrections

The implementation review found declarations that could resolve valid symbols while still returning incorrect data: `H5P.get_edc_check` and `H5I.H5Iget_type_ref` imported the wrong functions; `H5D.get_num_chunks` used a 64-bit return for a 32-bit signed status; `H5T.find` marshalled a pointer-to-pointer as an inline structure. These are corrected. Page-buffer statistics now expose five unsigned two-counter arrays and reject null or undersized outputs before calling native code. The free callback has a void return. Callback dimension/client-data arrays are native pointers rather than implicit one-element arrays, and external-link names are decoded as complete UTF-8 strings. These declaration corrections are intentional source-breaking changes in 2.2.0.

The old eager initialization made `H5.dont_atexit()` fail even as the first user call. The managed opt-out now loads the core and calls the native opt-out before querying its version. Default loading remains lazy and version-checked; fresh-process tests verify early, repeated and late calls.

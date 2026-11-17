# Release notes

## 2.2.0

- Wrapped native HDF5 2.2.0 with statically linked zlib/Deflate across `win-x64`, `win-arm64`, `linux-x64`, `linux-arm64`, `linux-musl-x64`, `linux-musl-arm64` and `osx-arm64`; removed Paket, legacy `bin32`/`bin64` deployments, `app.config` probing and HDF5 1.8 conditional compilation.
- Multi-targeted `net8.0` and `netstandard2.0` from shared `[LibraryImport]` declarations, supporting modern .NET (`[assembly: DisableRuntimeMarshalling]`, source-generated P/Invoke stubs, stack-buffered UTF-8 marshalling, zero-cleared pooled `StringBuilder` decoding, zero-copy `Span<T>`/`ReadOnlySpan<T>` overloads, truncation-free UTF-8 `string` getters, trimming, single-file deployment and `PublishAot=true` NativeAOT), .NET Framework 4.7.2+ and standalone Mono.
- Unified native loading across function imports and exported globals (`H5P`, `H5T`, `H5E`) with automatic RID probing, unversioned Unix/macOS shared-library SONAMEs, `NativeDependencies.Configure(path)` for private installations, and `H5.dont_atexit()` ordering before native initialization; removed `NativeDependencies.ResolvePathToExternalDependencies()`.
- Expanded HDF5 2.2 application API coverage across multi-dataset and direct-chunk I/O, modern owning references, token-aware object/link APIs, advanced selection utilities, VOL connector discovery, event sets (`H5ES`) and asynchronous entry points, rooted `H5.atclose` callbacks, and the `H5DO`, `H5DS`, `H5IM`, `H5LD`, `H5LT`, `H5PL`, `H5PT` and `H5TB` high-level families.
- Corrected ABI and marshalling contracts relative to 1.10.x: 1-byte `hbool_t` values, UTF-8 string marshalling, `System.Runtime.InteropServices.CLong`/`CULong` C integer fields (type-forwarded on `net8.0` and polyfilled in `netstandard2.0`), typed `ref H5AC.cache_config_t` and `ref H5AC.cache_image_config_t` parameters on `H5P.get/set_mdc_config` and `H5P.get/set_mdc_image_config`, managed `string` filter names on `H5Z.class_t`, `out IntPtr` on `H5T.find`, blittable pointer-based `H5D.append_cb_t`/`H5I.free_t`/`H5L.*_func_t`/`H5L.elink_traverse_t`/`H5P.prp_cb*_t`/`H5T.conv_t`/`H5Z.func_t` callbacks (removing unused `H5L.*_ascii_t` delegates), `void`-returning `H5MM.free_t`, validated five-array `H5F.get_page_buffering_stats` outputs, and `int` status on `H5D.get_num_chunks`.
- Added blittable bridges and lifetime management for synchronous callbacks and registration structures (`H5E.walk`, `H5D.iterate`, `H5D.scatter`, `H5A.iterate*`, `H5L.iterate*`/`visit*`, `H5O.visit*`, `H5P.iterate`, `H5L.register`/`unregister`, `H5Z.register`/`unregister` and `H5P.get/set_file_image_callbacks`), capturing managed exceptions across synchronous callbacks and retaining registered link and filter strings and delegates until replacement, unregistration or `H5.close()`.
- Added header-derived ABI layout probes, compiler-hosted legacy binding generation with diagnostics (`HDFGEN001`–`HDFGEN003`), package archive validation, and clean JIT, NativeAOT and .NET Framework package-consumer verification.

## 1.10.11

- HDF5 1.10.x is no longer supported
- This release includes 1.14.3 binaries with 1.10 API compatibility
- Remove globals that are no longer available
- Version APIs for H5L and H5S calls
- Solution and project updates for VS 2022
- Updates paket version

## 1.10.6.1

- Fixed the documentation link.

## 1.10.6.0

- Updated native dependencies (HDF5 1.10.6)

## 1.10.5.2

- Bugfix for https://github.com/HDFGroup/HDF.PInvoke/issues/161.

## 1.10.5.1

- Bugfix for https://github.com/HDFGroup/HDF.PInvoke/issues/154.

## 1.8.21.1

- Bugfix for https://github.com/HDFGroup/HDF.PInvoke/issues/154.

## 1.8.21.0

- Updated native dependencies (HDF5 1.8.21)

## 1.10.5.0

- Updated native dependencies (HDF5 1.10.5)
- New chunk info functions H5Dget_chunk[info,info_by_coord], H5Dget_num_chunks
- New object header minimization functions H5[F,P][g,s]et_dset_no_attrs_hint

## 1.10.4.0

- Updated native dependencies (HDF5 1.10.4)

## 1.10.3.0

- Updated native dependencies (HDF5 1.10.3)
- API versioning for H5O[get_info*,visit*]
- Moved H5DO[read,write]\_chunk to H5D[read,write]\_chunk
- General performance improvements (native library)

## 1.10.2.0

- Updated native dependencies (HDF5 1.10.2)
- VDS H5P.[g,s]et_virtual_prefix
- Fixed H5F.get_file_image() declaration (thanks polivbr)

## 1.8.20.0

- Updated native dependencies (HDF5 1.8.20)

## 1.10.1.0

- Updated native dependencies (HDF5 1.10.1)
- New functions for metadata cache image handling
- MDC H5P.set_evict_on_close
- New functions for file space management (page buffering, paged aggr.)

## 1.8.19.0

- Updated native dependencies (HDF5 1.8.19)
- New H5PL functions
- H5DOread_chunk
- H5Dget_chunk_storage_size

## 1.8.18.0

- Updated native dependencies (HDF5 1.8.18)

## 1.10.0.4

- Added missing predefined datatypes (INTEL, MIPS, VAX)
- Added the CMake configuration files for building the native dependnecies
- The native dependencies included were built as RelWithDebugInfo (PDBs are included)

## 1.8.17.8

- Added missing predefined datatypes (INTEL, MIPS, VAX)
- Added the CMake configuration files for building the native dependnecies
- The native dependencies included were built as RelWithDebugInfo (PDBs are included)

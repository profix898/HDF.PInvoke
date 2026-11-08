# Release notes

## 2.2.0

- Managed targets are net8.0 and netstandard2.0, with separate build-selected modern and legacy loaders and shared binding declarations. Legacy validation includes net472 on .NET Framework 4.8 and Linux x64 Mono 6.8. Unity is not yet validated.
- Added multi-dataset I/O, bounded chunk reads and datatype decoding, complex datatypes, live file format bounds, file locking and selection-I/O controls.
- Added modern object/region/attribute references, the STD_REF datatype, token serialization and current object/link information and traversal. Modern references own resources: duplicate them with H5R.copy and release each owner with H5R.destroy, or reclaim buffers with H5T.reclaim.
- Corrected H5R.MAXTYPE to 5, H5O.NTYPES to 4 and H5T.NCLASSES to 12; added missing reference, map and complex enum values. Existing legacy reference enum aliases retain their numeric values.
- Unified native loading for function imports and exported globals, with process-RID assets and explicit private-directory configuration.
- HDF5 2.2.x is required. Updated native boolean widths, C long fields, callbacks, enums and versioned entry points.
- Windows/Linux x64 and ARM64 plus macOS ARM64.
- Removed Paket, old native binaries and HDF5 1.8 conditional compilation.
- Breaking interop corrections: `H5T.find` returns conversion data through `out IntPtr`; append/filter callbacks expose native arrays as `IntPtr`; external-link callbacks use UTF-8 strings; `H5MM.free_t` returns `void`; page-buffer statistics take five `uint[]` outputs of at least two elements; chunk-count status returns `int`.
- Retained registered link comments and callbacks for their native lifetime, and preserved Windows legacy metadata-cache getter inputs on failure.
- Corrected error-detection and identifier-type reference-count entry points, and allowed `H5.dont_atexit()` before initialization.
- Added wrapper regressions, exact runtime selection, finished-archive validation, and core/high-level compressed data round-trips in clean package consumers. CI exposes the final NuGet artifact only after all seven consumers pass, including glibc and musl Linux.

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

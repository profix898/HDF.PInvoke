# Migrating from HDF.PInvoke 1.10.11 to 2.2.0

Recompile applications and libraries that reference HDF.PInvoke. Most applications using ordinary file, group, dataset and attribute operations on x64 should need few source edits. Allow more time for custom native loading, callbacks, and code that marshals HDF5 structures itself.

The comparison uses the DLL from the [published 1.10.11 package](https://www.nuget.org/packages/HDF.PInvoke/1.10.11) and both managed DLLs in 2.2.0. Of 886 public methods and property accessors declared on top-level types in 1.10.11, 857 retain their parameter and return types on both targets. The same 29 signatures changed or disappeared on each. Delegates, fields, constants and optional argument defaults are assessed separately below. The 1.10.11 wrapper bundled native HDF5 1.14.3 with 1.10 API compatibility.

## Estimated migration effort

| Use in the application | Estimated impact | What to do |
|---|---|---|
| Ordinary dataset I/O with package-provided native libraries on x64 | Low | Recompile and exercise a representative read/write round trip. Test non-ASCII names and any file-format compatibility requirement. |
| Native boolean flags, filter queries or statistics | Low to moderate | Update flag variables and supply correctly sized output buffers. |
| Cache configuration, native struct access or custom installers | Moderate | Update field and parameter types, review manual marshalling and test native loading from the installed application. |
| Custom filters, links, property callbacks or datatype converters | High for users of these extension points | Adapt callback signatures and test native invocation, pointer indirection and ownership. |
| A required 32-bit Windows process | Blocking with the supplied binaries | Move the host to a supported 64-bit architecture or retain the older deployment. |

## Source changes

| API or type | Migration |
|---|---|
| Native `hbool_t` parameters and fields | Use `byte` instead of `uint`, with 0 for false and 1 for true. Update matching `ref` variables and callback parameters. |
| `H5I.type_t.REFERENCE` | Remove uses of this identifier category. For a reference-typed dataset, inspect its datatype class using `H5T.get_class` and `H5T.class_t.REFERENCE`. |
| `H5T.find` | Replace `ref H5T.cdata_t` with `out IntPtr`. The returned conversion-data pointer is borrowed; marshal its contents if needed without freeing native-owned storage. |
| `H5T.conv_t` | Replace `ref H5T.cdata_t` with `IntPtr` (`H5T_cdata_t*`) for the borrowed native conversion-state structure. |
| `H5F.get_page_buffering_stats` | Supply five `uint[]` output buffers, each with at least two elements, instead of scalar integers. |
| `H5P.get_filter2` | Supply a writable UTF-8 `byte[]` name buffer instead of an immutable `string`. |
| `H5P.prp_cb1_t` / `H5P.prp_cb2_t` | Receive the borrowed UTF-8 property name as `IntPtr` instead of `string`. (`H5P.iterate_t` continues to receive a decoded `string`.) |
| `H5D.append_cb_t` | Read current dimensions through `IntPtr` instead of a marshalled `ulong[]`; the number of elements is the dataset rank. |
| `H5Z.func_t` | Receive `cd_values` (`const uint*`), `buf_size` (`size_t*`) and `buf` (`void**`) as `IntPtr` addresses instead of `uint[]` and `ref` parameters; read or update capacity and chunk buffer pointers through those addresses. |
| `H5L.create_func_t` / `move_func_t` / `copy_func_t` / `delete_func_t` / `query_func_t` | Receive borrowed UTF-8 link names as `IntPtr` instead of `byte[]`; the unused `H5L.*_ascii_t` delegate variants are removed. |
| `H5L.traverse_func_t` | Return a 64-bit object identifier (`long`), accept the extra transfer-property-list identifier (`dxpl_id`), and receive the borrowed UTF-8 link name as `IntPtr`; `H5L.traverse_func_ascii_t` is removed. |
| `H5L.elink_traverse_t` | Receive the four borrowed UTF-8 file/group/object names and the mutable `uint` access-flags storage (`unsigned*`) as `IntPtr` values; `H5L.elink_traverse_ascii_t` is removed. |
| `H5I.free_t` | Add the `IntPtr request` argument required by the native callback. A zero address means synchronous execution with no output storage. Only write through a nonzero address using `Marshal.WriteIntPtr`; do not treat it as an unconditional `ref` parameter. |
| `H5MM.free_t` | Return `void`, not `IntPtr`. |
| `H5D.get_num_chunks` | The status result is `int`, not `long`; widening assignment still compiles, but matching delegate signatures must change. |
| `H5P.get_mdc_config` / `H5P.set_mdc_config` | Pass `ref H5AC.cache_config_t` instead of a raw `IntPtr` buffer. |
| `H5P.get_mdc_image_config` / `H5P.set_mdc_image_config` | Pass `ref H5AC.cache_image_config_t` instead of a raw `IntPtr` buffer. |
| `H5AC.cache_config_t` | The trace filename is a fixed UTF-8 byte buffer, `epoch_length` is `System.Runtime.InteropServices.CLong`, `dirty_bytes_threshold` is `IntPtr`, and native boolean fields are bytes. |
| `H5O.info_t` | The file number is `System.Runtime.InteropServices.CULong` and timestamps are signed `long`. |
| `H5Z.class_t` | `name` is a UTF-8 `string` instead of `byte*`, and `H5Z.CLASS_T_VERS` defines the structure version. |
| Explicit native-loading setup | Old `bin32`/`bin64` path constants and `ResolvePathToExternalDependencies()` are removed from both targets. Normal package loading is automatic. Use `NativeDependencies.Configure(absoluteDirectory)` for a private installation before first use, and `H5.open()` when explicit initialization is needed. `NativePathSetting` remains an AppContext key rather than an app.config lookup. |

The boolean correction affects `H5.allocate_memory`, `H5.is_library_threadsafe`, `H5I.clear_type`, `H5O.are_mdc_flushes_disabled`, the `H5F` dataset-attribute hints and metadata-cache logging status, and the corresponding flags in `H5P` core-driver settings, write tracking, file-space strategy, object timestamps, eviction, hints and logging. It also affects fields such as `H5A.info_t.corder_valid`, `H5L.info_t.corder_valid`, `H5G.info_t.mounted`, `H5T.cdata_t.recalc` and the cache configuration flags. Integer literals 0 and 1 often still compile, while a matching by-reference variable must have the exact new type.

## Behavior changes even when recompilation succeeds

| Affected code | What changes | Migration or verification |
|---|---|---|
| Calls to `H5P.set_libver_bounds` that omit `high` or pass `H5F.libver_t.LATEST` | Recompiling changes the upper bound from 2 (`V110`) to 5 (`V200`). This permits newer on-disk structures. | If HDF5 1.10 readers must open newly written files, specify `V110` as the upper bound and open a representative file with the oldest supported reader. Setting bounds does not downgrade existing files. |
| Persisted enum values, numeric comparisons and lookup tables | The old `H5I.type_t.REFERENCE` value 7 now denotes `ATTR`. Value 6 changed from `ATTR` to `MAP`. Identifier, object, reference, datatype and library-version count sentinels also changed. | Translate stored values using the producing version. Use named members and update tables indexed by ordinal. |
| User-defined link registration | `H5L.LINK_CLASS_T_VERS` changed from 0 to 1. A hard-coded 0 still compiles. | Set the structure version from the named constant and update the traversal callback signature. |
| Native filenames, configuration and deployment scripts | `Constants.DLLFileName` and `HLDLLFileName` now hold the logical names `hdf5` and `hdf5_hl`. Old app.config settings and directory layouts no longer select the native runtime. | Use the package runtime assets or configure a private directory before first use. Remove old native binaries from the deployment and test from the installed location. |
| Non-ASCII strings and output buffers | Strings use UTF-8. A character count can underestimate the required byte capacity. Raw byte-array input names require a NUL terminator. | Size buffers in bytes and allow room for the terminator where required. Exercise non-ASCII names and callback data. |
| Modern span overloads and sparse selections | Undersized slices can throw before native I/O. A sparse memory selection can address beyond the number of selected elements. | Size the span for the native memory datatype and selected addresses, or use `H5S.BLOCK` for packed memory with an explicit file selection. |
| Raw struct copies and callback pointers | Updated field widths change native layout. Borrowed callback arguments are valid only for their documented lifetime. | Update mirrored structs and serializers. Copy any callback data that must survive the call. |

The loader requires compatible native HDF5 2.2.x libraries. Optional driver methods can throw `PlatformNotSupportedException` when the packaged serial build does not export them.

## Target and deployment requirements

The package contains `net8.0` and `netstandard2.0` managed assets. Supported .NET Framework deployment uses .NET Framework 4.7.2 or later with the x64 native libraries. On .NET 8 and later, trimming, single-file deployment and NativeAOT (`PublishAot=true`) are supported and validated across all seven packaged runtime identifiers. Unity and IL2CPP are not validated configurations.

See [README.md](../README.md) for the platform matrix and file-compatibility example, [LEGACY.md](LEGACY.md) for legacy deployment, and [API_COVERAGE.md](API_COVERAGE.md) for current and superseded native entry points.

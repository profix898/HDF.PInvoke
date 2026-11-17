# Migrating from the 1.10.x wrapper

HDF.PInvoke 2.2 is not a guaranteed source-compatible or binary-compatible replacement for the 1.10.x wrapper. Ordinary file, dataset, group and attribute operations retain their familiar entry points. Recompile consumers and review the corrected interop declarations below. This comparison uses the repository's 1.10.11-era declarations; applications upgrading from earlier releases may encounter additional historical changes.

## Source changes

| API or type | Migration |
|---|---|
| Native `hbool_t` parameters and fields | Use `byte` instead of `uint`, with 0 for false and 1 for true. Update matching `ref` variables and callback parameters. |
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

The ABI corrections prevent incorrect memory access and make the same declarations work on Windows and Unix. Existing compatibility entry points remain where supported; the newly added token-aware object APIs and modern owning references do not require applications to replace their existing reference operations immediately.

## Changes that can compile without preserving behavior

Strings consistently use UTF-8. Buffer capacities are byte counts, not character counts. Raw byte-array input names must be NUL-terminated. Callback pointers and buffers are borrowed unless their documentation explicitly transfers ownership; never retain them beyond their native lifetime.

Enum values and structure layouts match HDF5 2.2. Do not persist raw enum ordinals, assume old native type counts, or reinterpret structure bytes using a historical layout. `H5F.libver_t.LATEST` permits the newest supported file format. Select an explicit `V110` upper bound when new files must remain readable by HDF5 1.10; bounds do not downgrade existing files or enable newer features in an older format.

The loader requires the supplied HDF5 2.2.x native libraries. Updating the managed package while retaining old native DLLs or shared objects is unsupported. Optional driver methods may throw `PlatformNotSupportedException` when the packaged native build does not export them.

## Target and deployment requirements

The package contains `net8.0` and `netstandard2.0` managed assets. Supported .NET Framework deployment uses .NET Framework 4.7.2 or later with the x64 native libraries. On .NET 8 and later, trimming, single-file deployment and NativeAOT (`PublishAot=true`) are supported and validated across all seven packaged runtime identifiers. Unity and IL2CPP are not validated configurations.

See [README.md](../README.md) for the platform matrix and file-compatibility example, [LEGACY.md](LEGACY.md) for legacy deployment, and [API_COVERAGE.md](API_COVERAGE.md) for current and superseded native entry points.

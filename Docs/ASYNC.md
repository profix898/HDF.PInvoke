# Native event sets and asynchronous entry points

The wrapper exposes HDF5 event sets (`H5ES`) and the native `*_async` entry points on both managed targets. These are low-level native bindings, not .NET `Task` APIs. The packaged native VOL connector completes these operations synchronously; creating an event set does not introduce background I/O. Deferred execution through a third-party async connector, its threading requirements and its own native dependencies are outside the validated package configuration.

## Resource lifetime

Asynchronous imports deliberately accept `IntPtr` for strings, arrays, data buffers and output values. Automatic string or array marshalling would normally release temporary storage when the import returns, which is too early for a connector that retains those pointers.

| Argument or resource | Caller responsibility |
|---|---|
| Source filename, source function and object names | Supply null-terminated UTF-8 storage and keep it allocated through completion |
| Data buffers and coordinate arrays | Allocate or pin sufficient storage; keep it fixed through completion |
| Output booleans, dimensions and structures | Allocate native-width storage and read it only after completion; C booleans occupy one byte |
| Multi-dataset arrays | Supply native arrays of 64-bit identifiers and pointer-sized buffer addresses, each with at least `count` entries |
| Callback delegates and user state | Keep strong managed roots until operations complete and callbacks are unregistered or their event set closes |
| Object, datatype, dataspace and property-list identifiers | Keep required identifiers alive until the operation completes |
| Modern reference buffers | Preserve the 64-byte reference storage and its owned resources until the reference operation completes |

Use `H5ES.wait` and check both the return status and its `inProgress` and `errorOccurred` outputs. A successful wait call does not necessarily mean every operation completed: a finite timeout can leave operations pending. `H5ES.cancel` is a request and reports operations it could not cancel. Do not free buffers merely because cancellation was requested. `H5ES.NONE` explicitly requests synchronous execution.

`H5O.get_info_by_name_async` writes native object information, whose file-number layout differs between Windows and Unix. Allocate at least 72 bytes on supported platforms and call `H5O.read_info2` after completion to obtain the portable managed representation. Other structure outputs must use the corresponding native layout; the native ABI probe verifies event metadata, reference storage and token-aware link/object layouts.

## Errors and completion callbacks

Inspect `H5ES.get_err_status` and `H5ES.get_err_count` after a failed operation. `H5ES.get_err_info` transfers up to the requested number of records into caller-owned array storage. For a positive returned count, call `H5ES.free_err_info` to release the records' strings and error-stack identifiers, then free the array storage. HDF5 rejects a zero count in `free_err_info`.

Insertion and completion callbacks receive borrowed operation metadata. Copy any text needed after the callback returns. Do not free these callback strings, close borrowed identifiers, or allow managed exceptions to cross native callback frames. A connector can impose additional restrictions on callback execution and reentrancy; consult its documentation before using it.

## Validation boundary

The integration suite invokes every asynchronous entry point exported by the packaged serial build, checks explicit event-set completion boundaries, and exercises UTF-8 names, dataset I/O, attributes, groups, links, datatype commits and modern references. It runs on modern .NET, .NET Framework and Mono. This validates signatures and the native connector's synchronous path. It does not establish delayed buffer retention, actual background execution, cancellation races or callback delivery from an external async connector. Those require a separately configured connector and tests that force operations to remain pending.

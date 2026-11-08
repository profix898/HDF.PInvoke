# API expansion priorities

The wrapper exposes the same public API through its modern direct imports and legacy source-generated delegates. Additions belong in the shared declarations and must work with both implementations. [API_COVERAGE.md](API_COVERAGE.md) provides the reproducible function inventory against the pinned HDF5 2.2.0 headers.

## Priority and implementation sequence

| Stage | API areas | Why this matters | Required behavioral validation |
|---|---|---|---|
| P1 | H5D multi-dataset I/O and bounded raw-chunk reads; H5F accessibility/deletion and format bounds; H5P locking and selection-I/O controls; H5T bounded decoding, complex types and reclamation | Common scientific data access, deployment controls, safer buffer handling and native 2.x datatypes | Round-trip multiple datasets, chunk contents and capacities, UTF-8 filenames, property round-trips, datatype encoding and complex layout |
| P2 | H5R modern object/region/attribute references; H5O tokens and current object information; H5L token-aware traversal | Correct access to modern references and VOL-compatible object identity | Create/copy/open/destroy references, token round-trips, traversal callbacks and header-derived structure layouts |
| P3 | H5S selection composition/iteration; H5E stack management; H5P serialization and remaining serial controls; H5LT and H5DS high-level utilities | Less universal but useful advanced queries and simpler application code | Selection algebra/sequence iteration, error-stack lifecycle, serialized property round-trips, high-level datasets and dimension scales |
| P4 | H5ES/async, VOL discovery/configuration, remaining high-level families and extension-oriented APIs | Specialized workflows requiring explicit ownership, callback and capability contracts | Capability-aware lifecycle tests; native unsupported-operation results where the default connector lacks the feature |

Each stage receives an unsigned commit describing its API additions. Validation includes a clean build of both targets, generator tests, full modern and legacy integration suites, native export checks, .NET Framework on Windows, and Linux legacy execution on Mono. Windows x64, WSL Linux x64 and Alpine/musl x64 are the locally executable native platforms. Existing native binaries can be reused because binding additions do not change the HDF5 build; new structure layouts require extending and rerunning the native ABI probe.

## API and ABI rules

Use exact native widths: identifiers are signed 64-bit, dimensions and addresses unsigned 64-bit, `size_t` is pointer-sized, C `bool` occupies one byte, and C `long` differs between Windows and Unix. Strings passed as managed strings use UTF-8 on both paths. Buffer lengths and ownership must be documented; functions accepting opaque reference storage must explain destruction and copying. Keep callbacks rooted until native code can no longer invoke them. Prefer managed bounds validation around native calls where array length is known.

Existing method names and deprecated overloads remain available. Add explicit versioned methods where a newer native ABI differs rather than silently changing the old structure. Numeric enums and required exported globals are reviewed alongside functions, since a function inventory alone cannot establish complete API coverage.

## Deliberate scope boundaries

New bindings to obsolete 1.x compatibility entry points are omitted when a supported replacement is already exposed. MPI functions cannot be validated or used with the shipped serial HDF5 build and require a separately designed parallel distribution. Map functions and map iteration hints are not exported by the packaged build, although map property classes exist. Asynchronous operations require event-set ownership and buffer/callback lifetimes extending through completion; see [ASYNC.md](ASYNC.md). Tests exercise the native connector's synchronous completion through the async entry points; pending operations, cancellation races and failure records from third-party asynchronous connectors require connector-specific validation. Driver-specific and connector-author interfaces are outside the application-header inventory. High-level specialist families are included: H5PT, H5IM, H5TB and H5LD complement H5LT and H5DS.

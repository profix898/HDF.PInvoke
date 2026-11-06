#!/usr/bin/env bash
# Alpine build and package checks; shares pinned CMake inputs with desktop builds.
# Usage: Build/Alpine/alpine.sh [build|consumer]; JOBS controls native parallelism (default 4).
set -euo pipefail
root=$(cd "$(dirname "$0")/../.." && pwd)
cd "$root"
case "$(uname -m)" in x86_64) arch=x64 ;; aarch64) arch=arm64 ;; *)
    echo 'Unsupported architecture' >&2
    exit 1
    ;;
esac
[[ -f /etc/alpine-release ]] || {
    echo 'Run inside the Alpine image.' >&2
    exit 1
}
rid="linux-musl-$arch"
phase=${1:-build}
jobs=${JOBS:-4}
[[ "$jobs" =~ ^[1-9][0-9]*$ ]] || {
    echo 'JOBS must be positive' >&2
    exit 1
}
unset HDF5_NATIVE_PATH
export HDF5_EXPECTED_RID="$rid"
work=$(mktemp -d /tmp/hdf5-musl.XXXXXX)
mkdir -p "$work/tmp"
export TMPDIR="$work/tmp"
results="$root/Build/Artifacts/tests/$rid"
mkdir -p "$results"
cleanup() {
    if [[ -d "$work/build/Testing/Temporary" ]]; then
        mkdir -p "$results/native-logs"
        cp -a "$work/build/Testing/Temporary/." "$results/native-logs/"
    fi
    # work is created by mktemp above, never supplied by the caller.
    chmod -R u+w "$work"
    rm -rf -- "$work"
}
trap cleanup EXIT
prop() { sed -n "s:.*<$1>\(.*\)</$1>.*:\1:p" "$2" | tr -d '\r'; }
if [[ "$phase" == consumer ]]; then
    version=$(prop Version Sources/HDF.PInvoke/HDF.PInvoke.csproj)
    for variant in net8.0 net10.0 legacy; do
        framework=$variant
        reference="<PackageReference Include=\"HDF.PInvoke\" Version=\"$version\" />"
        export HDF5_EXPECTED_BINDING=modern
        self_contained=false
        if [[ "$variant" == net8.0 ]]; then self_contained=true; fi
        if [[ "$variant" == legacy ]]; then
            framework=net10.0
            export HDF5_EXPECTED_BINDING=legacy
            reference="<PackageReference Include=\"HDF.PInvoke\" Version=\"$version\" GeneratePathProperty=\"true\" ExcludeAssets=\"compile;runtime\" /><Reference Include=\"HDF.PInvoke\"><HintPath>\$(PkgHDF_PInvoke)/lib/netstandard2.0/HDF.PInvoke.dll</HintPath></Reference>"
        fi
        cat >"$work/Consumer.csproj" <<EOF
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <AssemblySearchPaths>{HintPathFromItem};{RawFileName}</AssemblySearchPaths>
    <TargetFramework>$framework</TargetFramework>
    <SelfContained>$self_contained</SelfContained>
    <OutputType>Exe</OutputType>
    <UseAppHost>$self_contained</UseAppHost>
  </PropertyGroup>
  <ItemGroup>$reference</ItemGroup>
</Project>
EOF
        cp "$root"/Tests/HDF.PInvoke.Smoke/{Program.cs,LoaderScenarios.cs} "$work/"
        # Container mount is fixed at /src, so no arbitrary XML path interpolation.
        cat >"$work/NuGet.Config" <<'EOF'
<configuration><packageSources><clear /><add key="local" value="/src/Build/Artifacts/packages" /><add key="nuget" value="https://api.nuget.org/v3/index.json" /></packageSources><packageSourceMapping><packageSource key="local"><package pattern="HDF.PInvoke" /></packageSource><packageSource key="nuget"><package pattern="*" /></packageSource></packageSourceMapping></configuration>
EOF
        cd "$work"
        dotnet restore Consumer.csproj -r "$rid" --packages "$work/packages"
        dotnet publish Consumer.csproj -c Release -r "$rid" --self-contained "$self_contained" --no-restore -o "$work/$variant"
        chmod -R a-w "$work/$variant"
        cd /tmp
        for mode in --auto --recover-loader --registered-close --context --dont-atexit --late-dont-atexit --configure-first --concurrent-first; do
            dotnet "$work/$variant/Consumer.dll" "$mode"
        done
    done
    exit 0
fi
[[ "$phase" == build ]] || {
    echo 'Usage: alpine.sh [build|consumer]' >&2
    exit 1
}
build="$work/build"
install="$work/install"
stage="$root/Build/Native/runtimes/$rid/native"
interop="$root/Build/Native/interop/$rid"
mkdir -p "$stage"
rm -f "$stage/build-info.json"
# Use one core filename so musl cannot load separate copies through SONAME aliases.
cmake -S Native/hdf5 -C Native/HDF5Options.cmake -B "$build" -G Ninja \
    -DCMAKE_BUILD_TYPE=Release -DHDF_TEST_EXPRESS=1 -DCMAKE_INSTALL_PREFIX="$install" \
    -DCMAKE_PLATFORM_NO_VERSIONED_SONAME=ON
cmake --build "$build" --parallel "$jobs"
for palette in earth.pal sepia.pal; do
    sed -i 's/\r$//' "$build/hl/test/testfiles/$palette"
done
ctest --test-dir "$build" -C Release --parallel "$jobs" --output-on-failure --no-tests=error \
    --output-junit "$results/native-upstream.xml"
cmake --install "$build" --config Release
# This exact generated RID directory is derived above, not a command-line path.
rm -rf -- "$stage"
mkdir -p "$stage"
cp -L "$install"/lib/*hdf5*.so* "$stage/"
[[ -s "$stage/libhdf5.so" && -s "$stage/libhdf5_hl.so" ]]
cmake -S Native/Tests -B "$interop" -G Ninja -DCMAKE_BUILD_TYPE=Release -DCMAKE_PREFIX_PATH="$install"
cmake --build "$interop" --parallel "$jobs"
ctest --test-dir "$interop" --output-on-failure --no-tests=error
# Keep upstream notices intact in the single redistribution license.
combined=$(tr -d '\r' <LICENSE.txt)
for license in Native/hdf5/LICENSE "$build/_deps/hdf5_zlib-src/LICENSE"; do
    notice=$(tr -d '\r' <"$license")
    [[ "$combined" == *"$notice"* ]] || {
        echo "Combined LICENSE.txt is missing the full notice from $license" >&2
        exit 1
    }
done
cp LICENSE.txt "$root/Build/Native/runtimes/LICENSE.txt"
files='[]'
for library in "$stage"/*.so*; do
    hash=$(sha256sum "$library" | cut -d' ' -f1)
    files=$(jq --arg name "${library##*/}" --arg hash "$hash" '. + [{name:$name,sha256:$hash}]' <<<"$files")
done
jq -n --arg rid "$rid" --arg version "$(prop Hdf5Version Build/HDF5.props)" \
    --arg revision "$(prop Hdf5Revision Build/HDF5.props)" --arg zlib "$(prop ZlibRevision Build/HDF5.props)" \
    --argjson files "$files" \
    '{rid:$rid,hdf5Version:$version,hdf5Revision:$revision,zlibRevision:$zlib,configuration:"Release",
      upstreamTests:{passed:true,expressLevel:1},
      features:{highLevel:true,deflate:true,szip:false,threadSafe:false,mpi:false},files:$files}' >"$stage/build-info.json"
for legacy in false true; do
    # Assert that each test process uses the intended managed asset.
    if [[ "$legacy" == true ]]; then export HDF5_EXPECTED_BINDING=legacy; else export HDF5_EXPECTED_BINDING=modern; fi
    cd "$root"
    dotnet restore HDF.PInvoke.slnx --locked-mode -p:HdfLegacy="$legacy"
    dotnet build HDF.PInvoke.slnx -c Release --no-restore -p:HdfLegacy="$legacy" -p:RequiredNativeRids="$rid"
    HDF5_NATIVE_PATH="$stage" dotnet test Tests/HDF.PInvoke.Tests -c Release --no-build --no-restore -p:HdfLegacy="$legacy" \
        --logger "trx;LogFileName=managed-legacy-$legacy.trx" --results-directory "$results"
    publish="$root/Build/Artifacts/loader/$rid/$legacy"
    dotnet publish Tests/HDF.PInvoke.Smoke -c Release --no-restore -p:HdfLegacy="$legacy" -p:UseAppHost=false -p:RequiredNativeRids="$rid" -o "$publish"
    smoke="$publish/HDF.PInvoke.Smoke.dll"
    abi="$interop/abi-layout.txt"
    cd "$work"
    for mode in --auto --globals-first "$stage"; do dotnet "$smoke" "$mode" "$abi"; done
    for mode in --recover-loader --registered-close --context --relative --dont-atexit --late-dont-atexit --configure-first --concurrent-first; do dotnet "$smoke" "$mode"; done
    mkdir -p missing invalid empty
    printf 'invalid ELF' >invalid/libhdf5.so
    : >empty/libhdf5.so
    dotnet "$smoke" --missing "$work/missing"
    dotnet "$smoke" --missing-direct "$work/missing"
    dotnet "$smoke" --invalid "$work/invalid"
    dotnet "$smoke" --invalid "$work/empty"
    dotnet "$smoke" --incompatible "$interop/incompatible/Release"
done

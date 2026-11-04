#!/usr/bin/env bash
# Alpine build and package checks; shares pinned CMake inputs with desktop builds.
# Usage: Build/alpine.sh [build|consumer]; JOBS controls native parallelism (default 4).
set -euo pipefail
root=$(cd "$(dirname "$0")/.." && pwd)
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
    cat >"$work/Consumer.csproj" <<EOF
<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup>
<TargetFramework>net10.0</TargetFramework><OutputType>Exe</OutputType><UseAppHost>false</UseAppHost>
</PropertyGroup><ItemGroup><PackageReference Include="HDF.PInvoke" Version="$version" /></ItemGroup></Project>
EOF
    cp Tests/HDF.PInvoke.Smoke/{Program.cs,LoaderScenarios.cs} "$work/"
    # Container mount is fixed at /src, so no arbitrary XML path interpolation.
    cat >"$work/NuGet.Config" <<'EOF'
<configuration><packageSources><clear /><add key="local" value="/src/Build/Artifacts/packages" /><add key="nuget" value="https://api.nuget.org/v3/index.json" /></packageSources></configuration>
EOF
    cd "$work"
    dotnet restore Consumer.csproj -r "$rid" --packages "$work/packages"
    dotnet publish Consumer.csproj -c Release -r "$rid" --self-contained false --no-restore -o publish
    chmod -R a-w publish
    cd /tmp
    dotnet "$work/publish/Consumer.dll" --auto
    dotnet "$work/publish/Consumer.dll" --context
    dotnet "$work/publish/Consumer.dll" --dont-atexit
    dotnet "$work/publish/Consumer.dll" --late-dont-atexit
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
mkdir -p "$stage/Licenses"
cp -L "$install"/lib/*hdf5*.so* "$stage/"
[[ -s "$stage/libhdf5.so" && -s "$stage/libhdf5_hl.so" ]]
cmake -S Native/Tests -B "$interop" -G Ninja -DCMAKE_BUILD_TYPE=Release -DCMAKE_PREFIX_PATH="$install"
cmake --build "$interop" --parallel "$jobs"
ctest --test-dir "$interop" --output-on-failure --no-tests=error
cp Native/hdf5/LICENSE "$stage/Licenses/HDF5.txt"
cp "$build/_deps/hdf5_zlib-src/LICENSE" "$stage/Licenses/zlib.txt"
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
dotnet restore HDF.PInvoke.slnx --locked-mode
dotnet build HDF.PInvoke.slnx -c Release --no-restore -p:RequiredNativeRids="$rid"
HDF5_NATIVE_PATH="$stage" dotnet test Tests/HDF.PInvoke.Tests -c Release --no-build --no-restore \
    --logger 'trx;LogFileName=managed.trx' --results-directory "$results"
publish="$root/Build/Artifacts/loader/$rid"
dotnet publish Tests/HDF.PInvoke.Smoke -c Release --no-restore -p:UseAppHost=false -p:RequiredNativeRids="$rid" -o "$publish"
smoke="$publish/HDF.PInvoke.Smoke.dll"
abi="$interop/abi-layout.txt"
cd "$work"
for mode in --auto --globals-first "$stage"; do dotnet "$smoke" "$mode" "$abi"; done
for mode in --context --relative --dont-atexit --late-dont-atexit; do dotnet "$smoke" "$mode"; done
mkdir missing invalid empty
printf 'invalid ELF' >invalid/libhdf5.so
: >empty/libhdf5.so
dotnet "$smoke" --missing "$work/missing"
dotnet "$smoke" --invalid "$work/invalid"
dotnet "$smoke" --invalid "$work/empty"
dotnet "$smoke" --incompatible "$interop/incompatible/Release"

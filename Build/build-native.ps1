#requires -Version 7.0

<#
.SYNOPSIS
Builds and verifies the pinned native HDF5 libraries for the current host.
.DESCRIPTION
Runs the upstream suite and ABI probe before staging libraries, licenses and a
hash manifest. Use native-architecture tooling; this script does not cross-compile.
.PARAMETER Rid
Target runtime identifier. Defaults to the current host OS and architecture.
.PARAMETER Generator
CMake generator override. Unix builds default to Ninja.
.PARAMETER BuildDirectory
Absolute native build directory. Prefer a Linux filesystem when running under WSL.
.PARAMETER TestExpressLevel
Upstream test iteration mode: 1 runs the full suite; 0 uses exhaustive iterations.
.PARAMETER Jobs
Maximum parallel build and test jobs (default 4).
#>
param(
    [ValidateSet('win-x64', 'win-arm64', 'linux-x64', 'linux-arm64', 'linux-musl-x64', 'linux-musl-arm64', 'osx-arm64')]
    [string]$Rid,
    [string]$Generator = '',
    [string]$BuildDirectory = '',
    [ValidateSet(0, 1)][int]$TestExpressLevel = 1,
    [ValidateRange(1, 128)][int]$Jobs = 4
)
$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
function Run([string]$Tool, [string[]]$Arguments) {
    & $Tool @Arguments
    if ($LASTEXITCODE -ne 0) { throw "$Tool failed with exit code $LASTEXITCODE" }
}
$architecture = [Runtime.InteropServices.RuntimeInformation]::OSArchitecture.ToString().ToLowerInvariant()
$os = if ($IsWindows) { 'win' } elseif ($IsLinux) { 'linux' } elseif ($IsMacOS) { 'osx' } else { throw 'Unsupported OS' }
if ($IsLinux -and [Runtime.InteropServices.RuntimeInformation]::RuntimeIdentifier.StartsWith("linux-musl-")) { $os = "linux-musl" }
$hostRid = "$os-$architecture"
if (-not $Rid) { $Rid = $hostRid }
if ($Rid -ne $hostRid -or [Runtime.InteropServices.RuntimeInformation]::ProcessArchitecture.ToString().ToLowerInvariant() -ne $architecture) {
    throw "Build and test with native-architecture PowerShell on a $Rid host (current host: $hostRid)."
}
if ($BuildDirectory -and -not [IO.Path]::IsPathFullyQualified($BuildDirectory)) { throw 'BuildDirectory must be absolute.' }
$build = if ($BuildDirectory) { [IO.Path]::GetFullPath($BuildDirectory) } else { Join-Path $PSScriptRoot "Native/artifacts/$Rid/standalone" }
$install = Join-Path $PSScriptRoot "Native/install/$Rid"
$stage = Join-Path $PSScriptRoot "Native/runtimes/$Rid/native"
$interop = Join-Path $PSScriptRoot "Native/interop/$Rid"
$configure = @('-S', "$root/Native/hdf5", '-C', "$root/Native/HDF5Options.cmake", '-B', $build, '-DCMAKE_BUILD_TYPE=Release', "-DHDF_TEST_EXPRESS=$TestExpressLevel", "-DCMAKE_INSTALL_PREFIX=$install")
# Match the loader filename on musl to avoid loading a second core through a SONAME alias.
if ($os -eq 'linux-musl') { $configure += '-DCMAKE_PLATFORM_NO_VERSIONED_SONAME=ON' }
$consumer = @('-S', "$root/Native/Tests", '-B', $interop, '-DCMAKE_BUILD_TYPE=Release', "-DCMAKE_PREFIX_PATH=$install")
if ($IsWindows) {
    $cmakeArch = if ($architecture -eq 'arm64') { 'ARM64' } else { 'x64' }
    $configure += @('-A', $cmakeArch)
    $consumer += @('-A', $cmakeArch)
}
else {
    if (-not $Generator) { $Generator = 'Ninja' }
    if ($IsMacOS) {
        $macArch = 'arm64'
        $configure += @("-DCMAKE_OSX_ARCHITECTURES=$macArch", '-DCMAKE_OSX_DEPLOYMENT_TARGET=13.0')
        $consumer += @("-DCMAKE_OSX_ARCHITECTURES=$macArch", '-DCMAKE_OSX_DEPLOYMENT_TARGET=13.0')
    }
}
if ($Generator) {
    $configure += @('-G', $Generator)
    $consumer += @('-G', $Generator)
}
# Invalidate an earlier successful manifest before rebuilding or testing.
$manifestPath = Join-Path $stage 'build-info.json'
if (Test-Path -LiteralPath $manifestPath) { Remove-Item -LiteralPath $manifestPath -Force }
Run cmake $configure
Run cmake @('--build', $build, '--config', 'Release', '--parallel', "$Jobs")
# Windows checkouts may convert these text fixtures to CRLF. The upstream Unix
# palette parser counts the CR as data. Normalize only generated test copies.
if (-not $IsWindows) {
    foreach ($name in @('earth.pal', 'sepia.pal')) {
        $palette = Join-Path $build "hl/test/testfiles/$name"
        $text = [IO.File]::ReadAllText($palette)
        [IO.File]::WriteAllText($palette, $text.Replace("`r`n", "`n"), [Text.UTF8Encoding]::new($false))
    }
}
$testResults = Join-Path $PSScriptRoot "Artifacts/tests/$Rid"
New-Item -ItemType Directory -Force $testResults | Out-Null
Run ctest @('--test-dir', $build, '-C', 'Release', '--parallel', "$Jobs", '--output-on-failure', '--no-tests=error', '--output-junit', "$testResults/native-upstream.xml")
# Remove only generated install/staging trees to prevent stale libraries surviving upgrades.
foreach ($directory in @($install, $stage)) {
    $resolved = [IO.Path]::GetFullPath($directory)
    $allowed = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot 'Native')) + [IO.Path]::DirectorySeparatorChar
    if (-not $resolved.StartsWith($allowed, [StringComparison]::OrdinalIgnoreCase)) { throw "Unsafe output path: $resolved" }
    if (Test-Path -LiteralPath $resolved) { Remove-Item -LiteralPath $resolved -Recurse -Force }
    New-Item -ItemType Directory -Force $resolved | Out-Null
}
Run cmake @('--install', $build, '--config', 'Release')
$pattern = if ($IsWindows) { 'bin/*hdf5*.dll' } elseif ($IsMacOS) { 'lib/*hdf5*.dylib' } else { 'lib/*hdf5*.so*' }
$libraries = @(Get-ChildItem -Path (Join-Path $install $pattern) -File)
if ($libraries.Count -lt 2) { throw 'Expected both HDF5 core and high-level shared libraries.' }
foreach ($library in $libraries) {
    # Materialize symlinks: artifact upload and NuGet must carry each SONAME alias as a file.
    [IO.File]::Copy($library.FullName, (Join-Path $stage $library.Name), $true)
}
Run cmake $consumer
Run cmake @('--build', $interop, '--config', 'Release', '--parallel', "$Jobs")
Run ctest @('--test-dir', $interop, '-C', 'Release', '--output-on-failure', '--no-tests=error')
[xml]$pins = Get-Content -LiteralPath "$PSScriptRoot/HDF5.props"
$notices = Join-Path $stage 'Licenses'
New-Item -ItemType Directory -Force $notices | Out-Null
Copy-Item -LiteralPath "$root/Native/hdf5/LICENSE" -Destination "$notices/HDF5.txt"
$zlibLicense = Get-ChildItem -Path "$build/_deps/hdf5_zlib-src/LICENSE*" -File | Select-Object -First 1
if (-not $zlibLicense) { throw 'Missing zlib redistribution license' }
Copy-Item -LiteralPath $zlibLicense.FullName -Destination "$notices/zlib.txt"
$manifest = [ordered]@{
    rid           = $Rid
    hdf5Version   = $pins.Project.PropertyGroup.Hdf5Version
    hdf5Revision  = $pins.Project.PropertyGroup.Hdf5Revision
    zlibRevision  = $pins.Project.PropertyGroup.ZlibRevision
    configuration = 'Release'
    upstreamTests = @{ passed = $true; expressLevel = $TestExpressLevel }
    features      = @{ highLevel = $true; deflate = $true; szip = $false; threadSafe = $false; mpi = $false }
    files         = @($libraries | ForEach-Object { @{ name = $_.Name; sha256 = (Get-FileHash -LiteralPath (Join-Path $stage $_.Name) -Algorithm SHA256).Hash.ToLowerInvariant() } })
}
$manifest | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath "$stage/build-info.json" -Encoding utf8
Write-Host "Verified native runtime: $stage"

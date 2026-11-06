#requires -Version 7.4

<#
.SYNOPSIS
Runs managed integration tests and isolated native-loader scenarios.
.DESCRIPTION
Requires staged native libraries and ABI results from buildNative.ps1. Loader
scenarios use separate processes because native handles live for the process lifetime.
.PARAMETER Rid
Runtime identifier for the current process architecture; defaults to the host.
.PARAMETER SkipBuild
Uses previously built tests and published loader binaries.
#>
param(
    [ValidateSet('win-x64', 'win-arm64', 'linux-x64', 'linux-arm64', 'linux-musl-x64', 'linux-musl-arm64', 'osx-arm64')][string]$Rid,
    [switch]$SkipBuild
)
$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
function Run([string]$Tool, [string[]]$Arguments) {
    & $Tool @Arguments
    if ($LASTEXITCODE -ne 0) { throw "$Tool failed with exit code $LASTEXITCODE" }
}
$arch = [Runtime.InteropServices.RuntimeInformation]::ProcessArchitecture.ToString().ToLowerInvariant()
$os = if ($IsWindows) { 'win' } elseif ($IsMacOS) { 'osx' } else { 'linux' }
if ($IsLinux -and [Runtime.InteropServices.RuntimeInformation]::RuntimeIdentifier.StartsWith("linux-musl-")) { $os = "linux-musl" }
if (-not $Rid) { $Rid = "$os-$arch" }
if ($Rid -ne "$os-$arch") { throw 'Managed integration tests must run on the target architecture.' }
$native = Join-Path $PSScriptRoot "Native/runtimes/$Rid/native"
$abi = Join-Path $PSScriptRoot "Native/interop/$Rid/abi-layout.txt"
if (-not (Test-Path "$native/build-info.json") -or -not (Test-Path $abi)) { throw 'Run Build/buildNative.ps1 first.' }
if (-not $SkipBuild) {
    Run dotnet @('restore', "$root/HDF.PInvoke.slnx", '--locked-mode')
    Run dotnet @('build', "$root/HDF.PInvoke.slnx", '-c', 'Release', '--no-restore')
    Run dotnet @('publish', "$root/Tests/HDF.PInvoke.Smoke", '-c', 'Release', '--no-restore', '-p:UseAppHost=false', '-o', "$PSScriptRoot/Artifacts/loader/$Rid")
}
$smoke = Join-Path $PSScriptRoot "Artifacts/loader/$Rid/HDF.PInvoke.Smoke.dll"
$working = Join-Path $PSScriptRoot "Artifacts/loader-work/$Rid"
$missing = Join-Path $working 'missing'
$invalid = Join-Path $working 'invalid'
$empty = Join-Path $working 'empty'
New-Item -ItemType Directory -Force $working, $missing, $invalid, $empty | Out-Null
$filename = if ($IsWindows) { 'hdf5.dll' } elseif ($IsMacOS) { 'libhdf5.dylib' } else { 'libhdf5.so' }
[IO.File]::WriteAllText((Join-Path $invalid $filename), 'not a native library')
[IO.File]::WriteAllBytes((Join-Path $empty $filename), [byte[]]::new(0))
$savedOverride = $env:HDF5_NATIVE_PATH
$savedBinding = $env:HDF5_EXPECTED_BINDING
$env:HDF5_EXPECTED_BINDING = 'modern'
try {
    $env:HDF5_NATIVE_PATH = $native
    Run dotnet @('test', "$root/Tests/HDF.PInvoke.Tests", '-c', 'Release', '--no-build', '--no-restore',
        '--logger', 'trx;LogFileName=managed.trx', '--results-directory', "$PSScriptRoot/Artifacts/tests/$Rid")
    $env:HDF5_NATIVE_PATH = $null
    Push-Location $working
    try {
        Run dotnet @($smoke, '--auto', $abi)
        Run dotnet @($smoke, '--globals-first', $abi)
        Run dotnet @($smoke, $native, $abi)
        Run dotnet @($smoke, '--recover-loader')
        Run dotnet @($smoke, '--registered-close')
        Run dotnet @($smoke, '--context')
        Run dotnet @($smoke, '--dont-atexit')
        Run dotnet @($smoke, '--late-dont-atexit')
        Run dotnet @($smoke, '--missing', $missing)
        Run dotnet @($smoke, '--invalid', $invalid)
        Run dotnet @($smoke, '--invalid', $empty)
        Run dotnet @($smoke, '--incompatible', "$PSScriptRoot/Native/interop/$Rid/incompatible/Release")
        Run dotnet @($smoke, '--relative')
        Run dotnet @($smoke, '--configure-first')
        Run dotnet @($smoke, '--concurrent-first')
        Run dotnet @($smoke, '--missing-direct', $missing)
    }
    finally { Pop-Location }
}
finally { $env:HDF5_NATIVE_PATH = $savedOverride; $env:HDF5_EXPECTED_BINDING = $savedBinding }

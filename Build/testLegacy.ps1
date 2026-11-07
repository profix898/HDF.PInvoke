#requires -Version 7.4
<#
.SYNOPSIS
Runs shared tests and isolated loader scenarios against the .NET Standard assembly.
.PARAMETER Rid
The native runtime identifier for this host.
.PARAMETER SkipAbi
Skip header-derived layout checks when reusing previously validated native artifacts locally.
#>
param([Parameter(Mandatory)][string]$Rid, [switch]$SkipAbi)
$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
function Run([string]$Tool, [string[]]$Arguments) {
    & $Tool @Arguments
    if ($LASTEXITCODE -ne 0) { throw "$Tool failed with exit code $LASTEXITCODE" }
}
$arch = [Runtime.InteropServices.RuntimeInformation]::ProcessArchitecture.ToString().ToLowerInvariant()
$os = if ($IsWindows) { 'win' } elseif ($IsMacOS) { 'osx' } else { 'linux' }
if ($IsLinux -and [Runtime.InteropServices.RuntimeInformation]::RuntimeIdentifier.StartsWith('linux-musl-')) { $os = 'linux-musl' }
if ($Rid -ne "$os-$arch") { throw 'Legacy integration tests must run on the target architecture.' }
$native = Join-Path $PSScriptRoot "Native/runtimes/$Rid/native"
$publish = Join-Path $PSScriptRoot "Artifacts/legacy-loader/$Rid"
$abi = Join-Path $PSScriptRoot "Native/interop/$Rid/abi-layout.txt"
if (-not $SkipAbi -and -not (Test-Path $abi)) { throw 'Build the ABI probe first, or explicitly use -SkipAbi for local checks.' }
# Test the compiler integration independently of the native runtime and wrapper tests.
Run dotnet @('test', "$root/Tests/BindingGenerator.Tests", '-c', 'Release', '-p:RestoreLockedMode=true',
    '--logger', 'trx;LogFileName=generator.trx', '--results-directory', "$PSScriptRoot/Artifacts/tests/$Rid")
$saved = $env:HDF5_NATIVE_PATH
$savedBinding = $env:HDF5_EXPECTED_BINDING
$env:HDF5_EXPECTED_BINDING = 'legacy'
try {
    $env:HDF5_NATIVE_PATH = $native
    Run dotnet @('test', "$root/Tests/HDF.PInvoke.Tests", '-c', 'Release', '-p:HdfLegacy=true', '-p:RestoreLockedMode=true',
        '--logger', 'trx;LogFileName=legacy.trx', '--results-directory', "$PSScriptRoot/Artifacts/tests/$Rid")
    if ($IsWindows -and $Rid -eq 'win-x64') {
        Run dotnet @('test', "$root/Tests/HDF.PInvoke.Tests", '-c', 'Release', '-p:HdfLegacy=true', '-p:HdfTestFramework=net472', '-p:RestoreLockedMode=true',
            '--logger', 'trx;LogFileName=framework.trx', '--results-directory', "$PSScriptRoot/Artifacts/tests/$Rid")
    }
    Run dotnet @('publish', "$root/Tests/HDF.PInvoke.Smoke", '-c', 'Release', '-p:HdfLegacy=true', '-p:RestoreLockedMode=true', '-p:UseAppHost=false', '-o', $publish)
    $env:HDF5_NATIVE_PATH = $null
    $app = Join-Path $publish 'HDF.PInvoke.Smoke.dll'
    Push-Location ([IO.Path]::GetTempPath())
    try {
        foreach ($mode in @('--auto', '--globals-first', $native)) {
            $arguments = @($app, $mode)
            if (-not $SkipAbi) { $arguments += $abi }
            Run dotnet $arguments
        }
        foreach ($mode in @('--recover-loader', '--registered-close', '--context', '--dont-atexit', '--late-dont-atexit', '--relative', '--configure-first', '--concurrent-first')) {
            Run dotnet @($app, $mode)
        }
        $missing = Join-Path $publish 'missing'
        New-Item -ItemType Directory -Force $missing | Out-Null
        Run dotnet @($app, '--missing', $missing)
        Run dotnet @($app, '--missing-direct', $missing)
        $filename = if ($IsWindows) { 'hdf5.dll' } elseif ($IsMacOS) { 'libhdf5.dylib' } else { 'libhdf5.so' }
        foreach ($kind in @('invalid', 'empty')) {
            $directory = Join-Path $publish $kind
            New-Item -ItemType Directory -Force $directory | Out-Null
            [IO.File]::WriteAllText((Join-Path $directory $filename), $(if ($kind -eq 'invalid') { 'invalid library' } else { '' }))
            Run dotnet @($app, '--invalid', $directory)
        }
        if (-not $SkipAbi) { Run dotnet @($app, '--incompatible', "$PSScriptRoot/Native/interop/$Rid/incompatible/Release") }
    }
    finally { Pop-Location }
}
finally { $env:HDF5_NATIVE_PATH = $saved; $env:HDF5_EXPECTED_BINDING = $savedBinding }

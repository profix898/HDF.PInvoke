#requires -Version 7.4

<#
.SYNOPSIS
Runs the .NET Framework-targeted integration suite on Linux Mono against the legacy wrapper.
.PARAMETER TestPlatformDirectory
Extracted Microsoft.TestPlatform NuGet package containing the net462 vstest.console.exe runner.
.PARAMETER Wsl
Build on Windows and execute the tests in the default WSL distribution.
.PARAMETER SkipBuild
Reuse the Release net472 test assembly from a previous legacy build.
#>
param(
    [Parameter(Mandatory)][string]$TestPlatformDirectory,
    [switch]$Wsl,
    [switch]$SkipBuild
)
$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent

function Run([string]$Tool, [string[]]$Arguments) {
    & $Tool @Arguments
    if ($LASTEXITCODE -ne 0) { throw "$Tool failed with exit code $LASTEXITCODE" }
}

function LinuxPath([string]$Path) {
    $absolute = [IO.Path]::GetFullPath($Path)
    if (-not $Wsl) { return $absolute }
    $converted = & wsl --exec wslpath -a $absolute
    if ($LASTEXITCODE -ne 0) { throw "Cannot translate WSL path: $absolute" }
    return $converted.Trim()
}

if ($Wsl -and -not $IsWindows) { throw '-Wsl requires a Windows host.' }
if (-not $Wsl -and -not $IsLinux) { throw 'Run on Linux or use -Wsl from Windows.' }
$architecture = if ($Wsl) { & wsl --exec uname -m } else { & uname -m }
if ($LASTEXITCODE -ne 0 -or $architecture.Trim() -ne 'x86_64') { throw 'This validation currently targets Linux x64 Mono.' }
$runner = @(Get-ChildItem -LiteralPath $TestPlatformDirectory -Filter vstest.console.exe -Recurse |
    Where-Object { $_.FullName -match '[/\\]net462[/\\]' })
if ($runner.Count -ne 1) { throw 'Expected exactly one net462 vstest.console.exe in the extracted test-platform package.' }
$native = Join-Path $PSScriptRoot 'Native/runtimes/linux-x64/native'
if (-not (Test-Path "$native/libhdf5.so") -or -not (Test-Path "$native/libhdf5_hl.so")) { throw 'Stage Linux x64 native libraries first.' }
if (-not $SkipBuild) {
    Run dotnet @('build', "$root/Tests/HDF.PInvoke.Tests", '-c', 'Release', '-p:HdfLegacy=true', '-p:HdfTestFramework=net472', '-p:RestoreLockedMode=true')
}
$assembly = Join-Path $root 'Tests/HDF.PInvoke.Tests/bin/Release/net472/HDF.PInvoke.Tests.dll'
if (-not (Test-Path $assembly)) { throw 'The net472 test assembly is missing.' }
$results = Join-Path $PSScriptRoot 'Artifacts/tests/linux-x64-mono'
New-Item -ItemType Directory -Force $results | Out-Null
# Launch Mono explicitly: a CoreCLR test host would not exercise Mono's marshalling behavior.
$arguments = @("HDF5_NATIVE_PATH=$(LinuxPath $native)", 'HDF5_EXPECTED_BINDING=legacy', 'mono', (LinuxPath $runner[0].FullName),
    (LinuxPath $assembly), '/Logger:trx;LogFileName=mono.trx', "/ResultsDirectory:$(LinuxPath $results)")
if ($Wsl) {
    Run wsl @('--exec', 'mono', '--version')
    Run wsl (@('--cd', '/tmp', '--exec', 'env') + $arguments)
} else {
    Run mono @('--version')
    Push-Location ([IO.Path]::GetTempPath())
    try { Run env $arguments }
    finally { Pop-Location }
}

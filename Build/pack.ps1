#requires -Version 7.0

<#
.SYNOPSIS
Validates native artifacts and creates the HDF.PInvoke NuGet package.
.DESCRIPTION
Checks source pins, successful upstream test records, hashes and licenses before
packing. Release packages require every supported runtime by default.
.PARAMETER RequiredRids
Required runtime identifiers. Override only for local, partial-package validation.
#>
param([string[]]$RequiredRids = @('win-x64', 'win-arm64', 'linux-x64', 'linux-arm64', 'linux-musl-x64', 'linux-musl-arm64', 'osx-arm64'))
$ErrorActionPreference = 'Stop'
if ($RequiredRids.Count -eq 0 -or @($RequiredRids | Select-Object -Unique).Count -ne $RequiredRids.Count) { throw 'Expected a nonempty, unique RID set.' }
$root = Split-Path $PSScriptRoot -Parent
[xml]$pins = Get-Content "$PSScriptRoot/HDF5.props"
foreach ($rid in $RequiredRids) {
    if ($rid -notin @('win-x64', 'win-arm64', 'linux-x64', 'linux-arm64', 'linux-musl-x64', 'linux-musl-arm64', 'osx-arm64')) { throw "Unsupported RID: $rid" }
    $dir = Join-Path $PSScriptRoot "Native/runtimes/$rid/native"
    $manifest = Get-Content "$dir/build-info.json" -Raw | ConvertFrom-Json
    if ($manifest.rid -ne $rid -or $manifest.hdf5Revision -ne $pins.Project.PropertyGroup.Hdf5Revision -or
        $manifest.zlibRevision -ne $pins.Project.PropertyGroup.ZlibRevision) { throw "Wrong native source revision for $rid" }
    if ($manifest.upstreamTests.passed -ne $true -or $manifest.upstreamTests.expressLevel -notin @(0, 1)) {
        throw "Native upstream suite has not passed in full or exhaustive mode for $rid; rebuild the runtime."
    }
    $names = if ($rid.StartsWith('win-')) { @('hdf5.dll', 'hdf5_hl.dll') }
    elseif ($rid.StartsWith('linux-')) { @('libhdf5.so', 'libhdf5_hl.so') }
    else { @('libhdf5.dylib', 'libhdf5_hl.dylib') }
    foreach ($name in $names) {
        if ($name -notin $manifest.files.name -or (Get-Item "$dir/$name").Length -eq 0) { throw "Missing or empty $rid/$name" }
    }
    foreach ($file in $manifest.files) {
        if ((Get-FileHash "$dir/$($file.name)" -Algorithm SHA256).Hash -ne $file.sha256) { throw "Native hash mismatch: $rid/$($file.name)" }
    }
}
$ridProperty = $RequiredRids -join '%3B'
dotnet pack "$root/Sources/HDF.PInvoke/HDF.PInvoke.csproj" -c Release -o "$PSScriptRoot/Artifacts/packages" '-p:RestoreLockedMode=true' "-p:RequiredNativeRids=$ridProperty"
if ($LASTEXITCODE -ne 0) { throw 'NuGet packing failed' }

[xml]$project = Get-Content "$root/Sources/HDF.PInvoke/HDF.PInvoke.csproj"
$package = "$PSScriptRoot/Artifacts/packages/HDF.PInvoke.$($project.Project.PropertyGroup.Version).nupkg"
& "$PSScriptRoot/validate-package.ps1" -Package $package -RequiredRids $RequiredRids

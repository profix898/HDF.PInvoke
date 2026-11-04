#requires -Version 7.0

<#
.SYNOPSIS
Validates the contents of a finished NuGet archive, without relying on staging files.
.PARAMETER Package
The NuGet archive to validate.
.PARAMETER RequiredRids
The exact runtime set expected in the archive. Defaults to all supported platforms.
#>
param(
    [Parameter(Mandatory)][string]$Package,
    [string[]]$RequiredRids = @('win-x64', 'win-arm64', 'linux-x64', 'linux-arm64', 'linux-musl-x64', 'linux-musl-arm64', 'osx-arm64')
)
$ErrorActionPreference = 'Stop'
$supported = @('win-x64', 'win-arm64', 'linux-x64', 'linux-arm64', 'linux-musl-x64', 'linux-musl-arm64', 'osx-arm64')
if ($RequiredRids.Count -eq 0 -or @($RequiredRids | Select-Object -Unique).Count -ne $RequiredRids.Count) { throw 'Expected a nonempty, unique RID set.' }
foreach ($rid in $RequiredRids) { if ($rid -notin $supported) { throw "Unsupported RID: $rid" } }
[xml]$pins = Get-Content "$PSScriptRoot/HDF5.props"
$archive = [IO.Compression.ZipFile]::OpenRead((Resolve-Path $Package))
try {
    $entries = @{}
    foreach ($entry in $archive.Entries) {
        $name = $entry.FullName
        if ($name.EndsWith('/')) { continue }
        if ($entries.ContainsKey($name) -or $name.Contains('\') -or $name -match '(^|/)\.\.(/|$)') { throw "Invalid or duplicate archive path: $name" }
        $entries[$name] = $entry
    }
    function RequireEntry([string]$Name) {
        if (-not $entries.ContainsKey($Name) -or $entries[$Name].Length -eq 0) { throw "Missing or empty package entry: $Name" }
        return $entries[$Name]
    }
    function ReadEntry([string]$Name) {
        $reader = [IO.StreamReader]::new((RequireEntry $Name).Open())
        try { return $reader.ReadToEnd() } finally { $reader.Dispose() }
    }
    foreach ($name in @('lib/net10.0/HDF.PInvoke.dll', 'lib/net10.0/HDF.PInvoke.xml', 'LICENSE.txt', 'README.md', 'HDF.PInvoke.nuspec')) {
        $null = RequireEntry $name
    }
    $expectedLicense = (Get-Content "$PSScriptRoot/../LICENSE.txt" -Raw).Replace("`r", '').TrimEnd()
    if ((ReadEntry 'LICENSE.txt').Replace("`r", '').TrimEnd() -cne $expectedLicense) { throw 'Package license differs from the complete repository license.' }
    foreach ($name in $entries.Keys | Where-Object { $_ -match '^[^/]+\.md$' -and $_ -ne 'README.md' }) { throw "Unexpected package documentation: $name" }
    $actualRids = @($entries.Keys | Where-Object { $_.StartsWith('runtimes/') } | ForEach-Object { $_.Split('/')[1] } | Sort-Object -Unique)
    if (@(Compare-Object $RequiredRids $actualRids).Count -ne 0) { throw "Package runtime set differs: expected $RequiredRids; found $actualRids" }
    foreach ($rid in $RequiredRids) {
        $prefix = "runtimes/$rid/native/"
        $manifest = ReadEntry "${prefix}build-info.json" | ConvertFrom-Json
        if ($manifest.rid -ne $rid -or $manifest.hdf5Version -ne $pins.Project.PropertyGroup.Hdf5Version -or
            $manifest.hdf5Revision -ne $pins.Project.PropertyGroup.Hdf5Revision -or $manifest.zlibRevision -ne $pins.Project.PropertyGroup.ZlibRevision) { throw "Wrong source pin: $rid" }
        if ($manifest.upstreamTests.passed -ne $true -or $manifest.upstreamTests.expressLevel -notin @(0, 1)) { throw "Unverified native suite: $rid" }
        if ($manifest.features.highLevel -ne $true -or $manifest.features.deflate -ne $true) { throw "Missing required native features: $rid" }
        $libraries = if ($rid.StartsWith('win-')) { @('hdf5.dll', 'hdf5_hl.dll') }
        elseif ($rid.StartsWith('linux-')) { @('libhdf5.so', 'libhdf5_hl.so') }
        else { @('libhdf5.dylib', 'libhdf5_hl.dylib') }
        $allowed = @("${prefix}build-info.json")
        foreach ($name in $allowed) { $null = RequireEntry $name }
        foreach ($name in $libraries) { if ($name -notin $manifest.files.name) { throw "Required library absent from manifest: $rid/$name" } }
        $seen = @{}
        foreach ($file in $manifest.files) {
            if ($file.name -notmatch '^[A-Za-z0-9_.-]+$' -or $seen.ContainsKey($file.name)) { throw "Invalid native filename: $rid/$($file.name)" }
            $seen[$file.name] = $true
            $name = $prefix + $file.name
            $stream = (RequireEntry $name).Open()
            try { $hash = [Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($stream)) } finally { $stream.Dispose() }
            if ($hash -ne $file.sha256) { throw "Archive native hash mismatch: $rid/$($file.name)" }
            $allowed += $name
        }
        foreach ($name in $entries.Keys | Where-Object { $_.StartsWith("runtimes/$rid/") }) {
            if ($name -notin $allowed) { throw "Unverified runtime entry: $name" }
        }
    }
    Write-Host "PASS: NuGet archive contains verified managed assets and exactly $($RequiredRids.Count) native runtimes"
}
finally { $archive.Dispose() }

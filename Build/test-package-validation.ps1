#requires -Version 7.0

<#
.SYNOPSIS
Regression tests for the archive validator using disposable damaged copies.
.PARAMETER Package
A valid archive used as the baseline.
.PARAMETER RequiredRids
The exact runtime set in the baseline archive.
#>
param(
    [Parameter(Mandatory)][string]$Package,
    [string[]]$RequiredRids = @('win-x64', 'win-arm64', 'linux-x64', 'linux-arm64', 'linux-musl-x64', 'linux-musl-arm64', 'osx-arm64')
)
$ErrorActionPreference = 'Stop'
& "$PSScriptRoot/validate-package.ps1" -Package $Package -RequiredRids $RequiredRids
$work = Join-Path ([IO.Path]::GetTempPath()) "hdf-package-tests-$([Guid]::NewGuid().ToString('N'))"
$null = New-Item -ItemType Directory $work
try {
    $prefix = "runtimes/$($RequiredRids[0])/native/"
    $cases = @('missing-managed', 'missing-library', 'missing-license', 'corrupt-library', 'extra-rid', 'extra-runtime-file', 'wrong-pin', 'unverified-suite')
    foreach ($case in $cases) {
        $copy = Join-Path $work "$case.nupkg"
        Copy-Item $Package $copy
        $zip = [IO.Compression.ZipFile]::Open($copy, [IO.Compression.ZipArchiveMode]::Update)
        try {
            $manifestEntry = $zip.GetEntry("${prefix}build-info.json")
            $reader = [IO.StreamReader]::new($manifestEntry.Open())
            try { $manifest = $reader.ReadToEnd() | ConvertFrom-Json } finally { $reader.Dispose() }
            $library = $prefix + $manifest.files[0].name
            $target = switch ($case) {
                'missing-managed' { 'lib/net10.0/HDF.PInvoke.dll' }
                'missing-library' { $library }
                'missing-license' { "${prefix}Licenses/zlib.txt" }
                'corrupt-library' { $library }
                'extra-rid' { 'runtimes/unexpected/native/fake.so' }
                'extra-runtime-file' { "${prefix}unverified.so" }
                default { "${prefix}build-info.json" }
            }
            $entry = $zip.GetEntry($target)
            if ($entry) { $entry.Delete() }
            if (-not $case.StartsWith('missing-')) {
                $content = 'damaged'
                if ($case -eq 'wrong-pin') { $manifest.hdf5Revision = 'wrong'; $content = $manifest | ConvertTo-Json -Depth 10 }
                if ($case -eq 'unverified-suite') { $manifest.upstreamTests.passed = $false; $content = $manifest | ConvertTo-Json -Depth 10 }
                $writer = [IO.StreamWriter]::new($zip.CreateEntry($target).Open())
                try { $writer.Write($content) } finally { $writer.Dispose() }
            }
        }
        finally { $zip.Dispose() }
        $rejected = $false
        try { & "$PSScriptRoot/validate-package.ps1" -Package $copy -RequiredRids $RequiredRids } catch { $rejected = $true }
        if (-not $rejected) { throw "Validator accepted $case" }
        Write-Host "PASS: rejected $case"
    }
}
finally {
    # This directory is created above, never taken from user input.
    $resolved = (Resolve-Path -LiteralPath $work).Path
    $expected = [IO.Path]::GetFullPath($work)
    if ($resolved -ne $expected -or -not $resolved.StartsWith([IO.Path]::GetFullPath([IO.Path]::GetTempPath()), [StringComparison]::OrdinalIgnoreCase)) {
        throw 'Refusing to remove an unexpected test directory.'
    }
    Remove-Item -LiteralPath $resolved -Recurse -Force
}

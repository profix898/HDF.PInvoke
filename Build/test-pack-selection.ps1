#requires -Version 7.0

<#
.SYNOPSIS
Checks that changing the RID selection regenerates a previously built package.
.DESCRIPTION
Rebuilds partial and combined archives in the standard package output directory.
Finishes with the combined archive. Each pack validates the archive's exact RID set.
.PARAMETER RequiredRids
The combined runtime set to restore after testing a partial package.
#>
param([string[]]$RequiredRids = @('win-x64', 'win-arm64', 'linux-x64', 'linux-arm64', 'linux-musl-x64', 'linux-musl-arm64', 'osx-arm64'))
$ErrorActionPreference = 'Stop'
if ($RequiredRids.Count -lt 2) { throw 'At least two staged runtimes are needed for selection testing.' }
for ($iteration = 0; $iteration -lt 2; $iteration++) {
    & "$PSScriptRoot/pack.ps1" -RequiredRids @($RequiredRids[0])
    & "$PSScriptRoot/pack.ps1" -RequiredRids $RequiredRids
}
Write-Host 'PASS: changing RID selection regenerates the archive without stale native assets'

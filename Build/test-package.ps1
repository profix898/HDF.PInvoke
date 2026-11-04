#requires -Version 7.0

<#
.SYNOPSIS
Verifies the built NuGet package through a clean consumer application.
.DESCRIPTION
Uses an isolated package cache and tests published native loading and assembly
load contexts. Unix publish directories are read-only during execution.
.PARAMETER Rid
Runtime identifier for the published consumer.
.PARAMETER BuildOnly
Restores and publishes without executing the consumer on this host.
#>
param(
    [Parameter(Mandatory)][ValidateSet('win-x64', 'win-arm64', 'linux-x64', 'linux-arm64', 'linux-musl-x64', 'linux-musl-arm64', 'osx-arm64')][string]$Rid,
    [switch]$BuildOnly
)
$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
function Run([string]$Tool, [string[]]$Arguments) {
    & $Tool @Arguments
    if ($LASTEXITCODE -ne 0) { throw "$Tool failed with exit code $LASTEXITCODE" }
}
[xml]$project = Get-Content "$root/Sources/HDF.PInvoke/HDF.PInvoke.csproj"
$version = $project.Project.PropertyGroup.Version
$work = Join-Path $PSScriptRoot "Artifacts/package-consumer/$Rid/$([Guid]::NewGuid().ToString('N'))"
New-Item -ItemType Directory -Force $work | Out-Null
$xml = @"
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework><OutputType>Exe</OutputType><UseAppHost>false</UseAppHost>
    <ManagePackageVersionsCentrally>false</ManagePackageVersionsCentrally><RestorePackagesWithLockFile>false</RestorePackagesWithLockFile>
    <RestorePackagesPath>$work/packages</RestorePackagesPath>
  </PropertyGroup>
  <ItemGroup><PackageReference Include="HDF.PInvoke" Version="$version" /></ItemGroup>
</Project>
"@
[IO.File]::WriteAllText("$work/Consumer.csproj", $xml)
Copy-Item "$root/Tests/HDF.PInvoke.Smoke/Program.cs", "$root/Tests/HDF.PInvoke.Smoke/LoaderScenarios.cs" -Destination $work
[IO.File]::WriteAllText("$work/NuGet.Config", "<configuration><packageSources><clear /><add key='local' value='$PSScriptRoot/Artifacts/packages' /><add key='nuget.org' value='https://api.nuget.org/v3/index.json' /></packageSources><packageSourceMapping><packageSource key='local'><package pattern='HDF.PInvoke' /></packageSource><packageSource key='nuget.org'><package pattern='*' /></packageSource></packageSourceMapping></configuration>")
Run dotnet @('restore', "$work/Consumer.csproj", '-r', $Rid, '--configfile', "$work/NuGet.Config")
Run dotnet @('publish', "$work/Consumer.csproj", '-c', 'Release', '-r', $Rid, '--self-contained', 'false', '--no-restore', '-o', "$work/publish")
if (-not $BuildOnly) {
    if (-not $IsWindows) { Run chmod @('-R', 'a-w', "$work/publish") }
    $savedOverride = $env:HDF5_NATIVE_PATH
    $env:HDF5_NATIVE_PATH = $null
    Push-Location ([IO.Path]::GetTempPath())
    try {
        Run dotnet @("$work/publish/Consumer.dll", '--auto')
        Run dotnet @("$work/publish/Consumer.dll", '--context')
        Run dotnet @("$work/publish/Consumer.dll", '--dont-atexit')
        Run dotnet @("$work/publish/Consumer.dll", '--late-dont-atexit')
    }
    finally { Pop-Location; $env:HDF5_NATIVE_PATH = $savedOverride }
}
[IO.File]::WriteAllText("$PSScriptRoot/Artifacts/package-consumer/$Rid/latest.txt", "$work/publish/Consumer.dll")
Write-Host "Package consumer: $work/publish/Consumer.dll"

#requires -Version 7.4

<#
.SYNOPSIS
Verifies the built NuGet package through a clean consumer application.
.DESCRIPTION
Uses an isolated package cache and tests published native loading, assembly
load contexts, transitive .NET Standard type forwarding, and NativeAOT publishing.
Unix publish directories are read-only during execution.
.PARAMETER Rid
Runtime identifier for the published consumer.
.PARAMETER Framework
Consumer framework. .NET 8 is published self-contained so its actual runtime is tested.
.PARAMETER Legacy
Force the .NET Standard asset in a modern consumer to test legacy loading on Unix.
.PARAMETER Aot
Publish a reflection-free NativeAOT consumer and a transitive .NET Standard 2.0 library.
.PARAMETER BuildOnly
Restores and publishes without executing the consumer on this host.
#>
param(
    [Parameter(Mandatory)][ValidateSet('win-x64', 'win-arm64', 'linux-x64', 'linux-arm64', 'linux-musl-x64', 'linux-musl-arm64', 'osx-arm64')][string]$Rid,
    [ValidateSet('net8.0', 'net10.0', 'net472')][string]$Framework = 'net8.0',
    [switch]$Legacy,
    [switch]$Aot,
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
if ($Framework -eq 'net472' -and $Rid -ne 'win-x64') { throw 'Framework package validation requires Windows x64.' }
if ($Aot -and ($Legacy -or $Framework -eq 'net472')) { throw 'NativeAOT package validation requires a modern .NET consumer.' }
if (-not $BuildOnly) {
    $arch = [Runtime.InteropServices.RuntimeInformation]::ProcessArchitecture.ToString().ToLowerInvariant()
    $os = if ($IsWindows) { 'win' } elseif ($IsMacOS) { 'osx' } else { 'linux' }
    if ($IsLinux -and [Runtime.InteropServices.RuntimeInformation]::RuntimeIdentifier.StartsWith('linux-musl-')) { $os = 'linux-musl' }
    if ($Rid -ne "$os-$arch") { throw 'Run consumers on the target architecture, or use -BuildOnly.' }
}
$selfContained = if ($Aot -or $Framework -eq 'net8.0') { 'true' } else { 'false' }
$reference = "<PackageReference Include='HDF.PInvoke' Version='$version' />"
if ($Legacy -and $Framework -ne 'net472') {
    $reference = @"
<PackageReference Include="HDF.PInvoke" Version="$version" GeneratePathProperty="true" ExcludeAssets="compile;runtime" />
    <Reference Include="HDF.PInvoke">
      <HintPath>`$(PkgHDF_PInvoke)/lib/netstandard2.0/HDF.PInvoke.dll</HintPath>
    </Reference>
"@
}
if ($Framework -eq 'net472') { $reference += "`n    <PackageReference Include='Microsoft.NETFramework.ReferenceAssemblies' Version='1.0.3' PrivateAssets='all' />" }
$work = Join-Path $PSScriptRoot "Artifacts/package-consumer/$Rid/$([Guid]::NewGuid().ToString('N'))"
New-Item -ItemType Directory -Force $work | Out-Null
$xmlWork = [Security.SecurityElement]::Escape($work)
$xmlFeed = [Security.SecurityElement]::Escape("$PSScriptRoot/Artifacts/packages")
if ($Aot) {
    New-Item -ItemType Directory -Force "$work/BridgeLib" | Out-Null
    $bridgeXml = @"
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>netstandard2.0</TargetFramework>
    <LangVersion>latest</LangVersion>
    <ManagePackageVersionsCentrally>false</ManagePackageVersionsCentrally>
    <RestorePackagesWithLockFile>false</RestorePackagesWithLockFile>
    <RestorePackagesPath>$xmlWork/packages</RestorePackagesPath>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="HDF.PInvoke" Version="$version" />
  </ItemGroup>
</Project>
"@
    [IO.File]::WriteAllText("$work/BridgeLib/BridgeLib.csproj", $bridgeXml)
    Copy-Item "$root/Tests/PackageConsumer/TransitiveBridge.cs" "$work/BridgeLib/TransitiveBridge.cs"
    $reference += "`n    <Compile Remove='BridgeLib/**/*.cs' />`n    <ProjectReference Include='BridgeLib/BridgeLib.csproj' />"
}
$searchPaths = if ($Legacy -and $Framework -ne 'net472') { '<AssemblySearchPaths>{HintPathFromItem};{RawFileName}</AssemblySearchPaths>' } else { '' }
$aotProps = if ($Aot) { "<PublishAot>true</PublishAot>`n    <AllowUnsafeBlocks>true</AllowUnsafeBlocks>`n    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>" } else { "<SelfContained>$selfContained</SelfContained>`n    <UseAppHost>$selfContained</UseAppHost>" }
# Prioritize the explicit legacy HintPath over NuGet's preferred modern asset during validation.
$xml = @"
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>$Framework</TargetFramework>
    <PlatformTarget>$(if ($Rid -eq 'win-x64' -and -not $Aot) { 'x64' } else { 'AnyCPU' })</PlatformTarget>
    <OutputType>Exe</OutputType>
    $aotProps
    $searchPaths
    <ManagePackageVersionsCentrally>false</ManagePackageVersionsCentrally>
    <RestorePackagesWithLockFile>false</RestorePackagesWithLockFile>
    <RestorePackagesPath>$xmlWork/packages</RestorePackagesPath>
  </PropertyGroup>
  <ItemGroup>
    $reference
  </ItemGroup>
</Project>
"@
[IO.File]::WriteAllText("$work/Consumer.csproj", $xml)
if ($Aot) { Copy-Item "$root/Tests/PackageConsumer/AotProgram.cs" "$work/Program.cs" }
elseif ($Framework -eq 'net472') { Copy-Item "$root/Tests/PackageConsumer/FrameworkProgram.cs" "$work/Program.cs" }
else { Copy-Item "$root/Tests/HDF.PInvoke.Smoke/Program.cs", "$root/Tests/HDF.PInvoke.Smoke/LoaderScenarios.cs" -Destination $work }
$nuget = @"
<configuration>
  <packageSources>
    <clear />
    <add key="local" value="$xmlFeed" />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
  <packageSourceMapping>
    <packageSource key="local"><package pattern="HDF.PInvoke" /></packageSource>
    <packageSource key="nuget.org"><package pattern="*" /></packageSource>
  </packageSourceMapping>
</configuration>
"@
[IO.File]::WriteAllText("$work/NuGet.Config", $nuget)
Run dotnet @('restore', "$work/Consumer.csproj", '-r', $Rid, '--configfile', "$work/NuGet.Config")
$publishArgs = @('publish', "$work/Consumer.csproj", '-c', 'Release', '-r', $Rid, '--no-restore', '-o', "$work/publish")
if (-not $Aot) { $publishArgs += @('--self-contained', $selfContained) }
Run dotnet $publishArgs
if (Get-ChildItem "$work/publish" -Recurse -Filter 'build-info.json' -ErrorAction SilentlyContinue) {
    throw 'Package manifest leaked into publish output.'
}
if (-not $BuildOnly) {
    if (-not $IsWindows) { Run chmod @('-R', 'a-w', "$work/publish") }
    $savedOverride = $env:HDF5_NATIVE_PATH
    $savedBinding = $env:HDF5_EXPECTED_BINDING
    $env:HDF5_NATIVE_PATH = $null
    Push-Location ([IO.Path]::GetTempPath())
    try {
        if ($Aot) {
            $exe = if ($IsWindows) { "$work/publish/Consumer.exe" } else { "$work/publish/Consumer" }
            Run $exe @()
            Run $exe @('--dont-atexit')
            Run $exe @('--recover-loader')
            Run $exe @('--configure-first')
            Run $exe @('--missing-direct')
            Run $exe @('--registered-close')
        }
        elseif ($Framework -eq 'net472') {
            Run "$work/publish/Consumer.exe" @()
            Run "$work/publish/Consumer.exe" @('--dont-atexit')
            Run "$work/publish/Consumer.exe" @('--recover-loader')
            Run "$work/publish/Consumer.exe" @('--configure-first')
            Run "$work/publish/Consumer.exe" @('--missing-direct')
        }
        else {
            $env:HDF5_EXPECTED_BINDING = if ($Legacy) { 'legacy' } else { 'modern' }
            foreach ($mode in @('--auto', '--recover-loader', '--registered-close', '--context', '--dont-atexit', '--late-dont-atexit', '--configure-first', '--concurrent-first')) {
                Run dotnet @("$work/publish/Consumer.dll", $mode)
            }
        }
    }
    finally { Pop-Location; $env:HDF5_NATIVE_PATH = $savedOverride; $env:HDF5_EXPECTED_BINDING = $savedBinding }
}
$consumer = if ($Aot) { if ($IsWindows) { "$work/publish/Consumer.exe" } else { "$work/publish/Consumer" } }
elseif ($Framework -eq 'net472') { "$work/publish/Consumer.exe" }
else { "$work/publish/Consumer.dll" }
[IO.File]::WriteAllText("$PSScriptRoot/Artifacts/package-consumer/$Rid/latest.txt", $consumer)
Write-Host "Package consumer: $consumer"

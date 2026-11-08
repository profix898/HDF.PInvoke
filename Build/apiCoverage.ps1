#requires -Version 7.4
<#
.SYNOPSIS
Regenerates the function-level coverage tables from the pinned HDF5 headers and shared bindings.
.DESCRIPTION
Counts exported declarations, not macros, globals or overloads. Conditional declarations remain visible;
this inventory does not imply that every declaration is exported by the packaged serial build.
#>
param([string]$Output = (Join-Path $PSScriptRoot '../API_COVERAGE.md'))
$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
$bindingText = (Get-ChildItem "$root/Sources/HDF.PInvoke" -Recurse -Filter '*.cs' | Where-Object { $_.FullName -notmatch '[/\\](obj|bin)[/\\]' } | Get-Content -Raw) -join "`n"
$covered = @{}
[regex]::Matches($bindingText, 'EntryPoint\s*=\s*"(H5\w+)"') | ForEach-Object { $covered[$_.Groups[1].Value] = $true }
# These public methods deliberately route through managed loader logic rather than a direct declaration.
$covered['H5dont_atexit'] = $true
$groups = @()
$headers = @(Get-ChildItem "$root/Native/hdf5/src/*public.h") + @(Get-ChildItem "$root/Native/hdf5/hl/src/*public.h")
foreach ($header in $headers | Sort-Object Name) {
    $source = [IO.File]::ReadAllText($header.FullName) -replace '(?s)/\*.*?\*/', ''
    $names = @([regex]::Matches($source, '(?s)H5(?:HL)?_DLL\s+[^;{}]*?\b(H5\w+)\s*\([^;{}]*\)\s*;') | ForEach-Object { $_.Groups[1].Value } | Sort-Object -Unique)
    if ($names.Count) { $groups += [pscustomobject]@{ Name = $header.BaseName.Replace('public',''); Header = $header.Name; Names = $names } }
}
$lines = [Collections.Generic.List[string]]::new()
$lines.AddRange([string[]]@(
    '# HDF5 2.2 API coverage', '',
    'This inventory compares the pinned HDF5 2.2.0 core and high-level `*public.h` function declarations with the shared C# bindings used by both managed targets. Regenerate it with `Build/apiCoverage.ps1`. Each native symbol is counted once; managed overloads do not increase coverage. A binding means a declaration exists, not that every combination of arguments has been tested.', '',
    'Macros, exported datatype/property globals, VFD-specific headers and VOL connector-author interfaces are outside these function counts. Conditional and deprecated declarations are included, even when the packaged serial native build does not export them. See [API_PLAN.md](API_PLAN.md) for priorities, scope and validation requirements.', '',
    '| Group | Native functions | Bound | Gaps |', '|---|---:|---:|---:|'))
foreach ($group in $groups) {
    $count = @($group.Names | Where-Object { $covered.ContainsKey($_) }).Count
    $lines.Add("| $($group.Name) | $($group.Names.Count) | $count | $($group.Names.Count - $count) |")
}
foreach ($group in $groups) {
    $lines.AddRange([string[]]@('', "## $($group.Name)", '', "Native header: ``$($group.Header)``.", '', '| Native function | C# coverage |', '|---|---|'))
    foreach ($name in $group.Names) {
        $status = if ($covered.ContainsKey($name)) { 'Bound' } else { 'Gap' }
        $lines.Add("| ``$name`` | $status |")
    }
}
[IO.File]::WriteAllText([IO.Path]::GetFullPath($Output), ($lines -join "`n") + "`n")

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
$replacements = @{
    H5Acreate1 = 'H5Acreate2'
    H5Aget_num_attrs = 'H5Oget_info3'
    H5Aiterate1 = 'H5Aiterate2'
    H5Aopen_idx = 'H5Aopen_by_idx'
    H5Aopen_name = 'H5Aopen'
    H5Dcreate1 = 'H5Dcreate2'
    H5Dextend = 'H5Dset_extent'
    H5Dopen1 = 'H5Dopen2'
    H5Eclear1 = 'H5Eclear2'
    H5Eget_auto1 = 'H5Eget_auto2'
    H5Eget_major = 'H5Eget_msg'
    H5Eget_minor = 'H5Eget_msg'
    H5Eprint1 = 'H5Eprint2'
    H5Epush1 = 'H5Epush2'
    H5Eset_auto1 = 'H5Eset_auto2'
    H5Ewalk1 = 'H5Ewalk2'
    H5Fget_info1 = 'H5Fget_info2'
    H5Fset_latest_format = 'H5Fset_libver_bounds'
    H5Gcreate1 = 'H5Gcreate2'
    H5Gget_comment = 'H5Oget_comment'
    H5Gget_linkval = 'H5Lget_val'
    H5Gget_num_objs = 'H5Gget_info'
    H5Gget_objinfo = 'H5Oget_info3'
    H5Gget_objname_by_idx = 'H5Lget_name_by_idx'
    H5Gget_objtype_by_idx = 'H5Oget_info_by_idx3'
    H5Giterate = 'H5Literate2'
    H5Glink = 'H5Lcreate_hard / H5Lcreate_soft'
    H5Glink2 = 'H5Lcreate_hard / H5Lcreate_soft'
    H5Gmove = 'H5Lmove'
    H5Gmove2 = 'H5Lmove'
    H5Gopen1 = 'H5Gopen2'
    H5Gset_comment = 'H5Oset_comment'
    H5Gunlink = 'H5Ldelete'
    H5Oget_info_by_idx2 = 'H5Oget_info_by_idx3'
    H5Oget_info_by_name2 = 'H5Oget_info_by_name3'
    H5Oget_info2 = 'H5Oget_info3'
    H5Ovisit_by_name2 = 'H5Ovisit_by_name3'
    H5Ovisit2 = 'H5Ovisit3'
    H5Pencode1 = 'H5Pencode2'
    H5Pget_filter_by_id1 = 'H5Pget_filter_by_id2'
    H5Pget_filter1 = 'H5Pget_filter2'
    H5Pget_preserve = 'automatic native compound-field preservation'
    H5Pset_preserve = 'automatic native compound-field preservation'
    H5Pinsert1 = 'H5Pinsert2'
    H5Pregister1 = 'H5Pregister2'
    H5Rdereference1 = 'H5Rdereference2'
    H5Rget_obj_type1 = 'H5Rget_obj_type2'
    H5Tarray_create1 = 'H5Tarray_create2'
    H5Tcommit1 = 'H5Tcommit2'
    H5Tget_array_dims1 = 'H5Tget_array_dims2'
    H5Topen1 = 'H5Topen2'
}
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
    'Every unbound function is classified below as superseded or unavailable in the packaged build; an unclassified addition appears as `Gap`. Predefined datatype and property identifiers have separate tables after the function inventory.', '',
    '| Group | Native functions | Bound | Not bound |', '|---|---:|---:|---:|'))
foreach ($group in $groups) {
    $count = @($group.Names | Where-Object { $covered.ContainsKey($_) }).Count
    $lines.Add("| $($group.Name) | $($group.Names.Count) | $count | $($group.Names.Count - $count) |")
}
foreach ($group in $groups) {
    $lines.AddRange([string[]]@('', "## $($group.Name)", '', "Native header: ``$($group.Header)``.", '', '| Native function | C# coverage |', '|---|---|'))
    foreach ($name in $group.Names) {
        $status = if ($covered.ContainsKey($name)) { 'Bound' } else { 'Gap' }
        if ($status -eq 'Gap' -and ($name.StartsWith('H5M') -or $name -match 'map_iterate_hints|mpi|coll_metadata')) {
            $status = 'Unavailable in packaged serial build'
        }
        if ($status -eq 'Gap' -and $replacements.ContainsKey($name)) {
            $status = 'Not added: superseded by ' + $replacements[$name]
        }
        $lines.Add("| ``$name`` | $status |")
    }
}
foreach ($group in 'H5P', 'H5T') {
    $header = [IO.File]::ReadAllText("$root/Native/hdf5/src/${group}public.h")
    $globals = @([regex]::Matches($header, 'H5_DLLVAR\s+hid_t\s+(H5\w+_g)\s*;') | ForEach-Object { $_.Groups[1].Value } | Sort-Object -Unique)
    $lines.AddRange([string[]]@('', "## $group predefined identifiers", '', 'These identifiers remain owned by HDF5. Optional native datatypes may have negative values when the native compiler lacks support. Map property classes exist even though map operations are not compiled into this package.', '', '| Native global | C# coverage |', '|---|---|'))
    foreach ($global in $globals) {
        $status = if ($bindingText.Contains('"' + $global + '"')) { 'Bound' } else { 'Gap' }
        $lines.Add("| ``$global`` | $status |")
    }
}
[IO.File]::WriteAllText([IO.Path]::GetFullPath($Output), ($lines -join "`n") + "`n")

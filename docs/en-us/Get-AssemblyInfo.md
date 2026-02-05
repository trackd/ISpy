---
external help file: ISpy.dll-Help.xml
Module Name: ISpy
online version: https://github.com/trackd/ISpy/blob/main/docs/en-US
schema: 2.0.0
---

# Get-AssemblyInfo

## SYNOPSIS

Retrieves comprehensive metadata and information about a .NET assembly.

## SYNTAX

```powershell
Get-AssemblyInfo [-Path] <String> [<CommonParameters>]
```

## DESCRIPTION

The `Get-AssemblyInfo` cmdlet analyzes a .NET assembly file and returns detailed metadata including assembly name, version, culture, public key token, target framework, type count, module count, entry point information, processor architecture, and other assembly characteristics.

## EXAMPLES

### Example 1: Retrieve metadata for a local assembly

```powershell
PS C:\> Get-AssemblyInfo -Path "$PSHOME/System.Linq.dll"
```

This command returns rich metadata about `System.Linq.dll`, including `FullName`, `Version`, and `TargetFramework`.

### Example 2: Pipeline usage with Get-ChildItem

```powershell
PS C:\> Get-ChildItem -Path .\lib -Filter '*.dll' | Get-AssemblyInfo
```

This command enumerates all DLLs in `./lib` and returns assembly metadata for each file via the pipeline.

### Example 3: Show only selected fields

```powershell
PS C:\> Get-AssemblyInfo -Path "./lib/MyLibrary.dll" | Select-Object FullName, TargetFramework, TypeCount
```

This command selects and displays only the `FullName`, `TargetFramework`, and `TypeCount` properties for concise output.

## PARAMETERS

### -Path

Specifies the path to the .NET assembly file to analyze.

```yaml
Type: String
Required: True
Position: 0
Accept pipeline input: True (ByPropertyName, ByValue)
```

## INPUTS

System.String — accepts assembly file paths from the pipeline.

## OUTPUTS

### ISpy.Models.ISpyAssemblyInfo

Returns an `ISpyAssemblyInfo` object containing assembly metadata.

## NOTES

- The cmdlet returns rich metadata including name, version, culture, public key token, target framework, and more.
- Use with Select-Object to filter properties for concise output.

## RELATED LINKS

[Get-Dependency](Get-Dependency.md)
[Get-Type](Get-Type.md)

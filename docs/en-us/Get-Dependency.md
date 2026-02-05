---
external help file: ISpy.dll-Help.xml
Module Name: ISpy
online version: https://github.com/trackd/ISpy/blob/main/docs/en-US
schema: 2.0.0
---

# Get-Dependency

## SYNOPSIS

Analyzes and lists assembly dependencies and references.

## SYNTAX

```powershell
Get-Dependency [-Path] <String> [-IncludeVersionDetails] [-ExternalOnly] [<CommonParameters>]
```

## DESCRIPTION

The `Get-Dependency` cmdlet analyzes a .NET assembly and returns information about its dependencies and referenced assemblies.

## EXAMPLES

### Example 1: Show dependencies for a system assembly

```powershell
PS C:\> Get-Dependency -Path "$PSHOME/Humanizer.dll"
```

This command lists assemblies referenced by the specified system library.  

### Example 2: Show external-only dependencies for a local assembly

```powershell
PS C:\> Get-Dependency -Path './lib/MyLibrary.dll' -ExternalOnly
```

This command lists only external references (skips self-references) for `MyLibrary.dll`.  

### Example 3: Include version details for each referenced assembly

```powershell
PS C:\> Get-Dependency -Path './lib/MyLibrary.dll' -IncludeVersionDetails
```

This command includes version and full assembly name details for each referenced assembly.  

## PARAMETERS

### -Path

Specifies the path to the .NET assembly file to analyze.

```yaml
Type: String
Required: True
Position: 0
Accept pipeline input: True (ByPropertyName, ByValue)
```

### -IncludeVersionDetails

Include version and full name details for each referenced assembly.

```yaml
Type: SwitchParameter
Required: False
Position: Named
```

### -ExternalOnly

Only return external dependencies (skip self-references).

```yaml
Type: SwitchParameter
Required: False
Position: Named
```

## INPUTS

System.String — accepts assembly file paths from the pipeline.

## OUTPUTS

### ISpy.Models.ISpyDependencyInfo

## NOTES

- Use `-ExternalOnly` to filter out self-references.
- Use `-IncludeVersionDetails` for full assembly name and version info.

## RELATED LINKS

[Get-AssemblyInfo](Get-AssemblyInfo.md)
[Get-Type](Get-Type.md)

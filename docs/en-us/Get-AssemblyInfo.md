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
Get-AssemblyInfo [-AssemblyPath] <String> [<CommonParameters>]
```

## DESCRIPTION

The `Get-AssemblyInfo` cmdlet analyzes a .NET assembly file and returns detailed metadata including assembly name, version, culture, public key token, target framework, type count, module count, entry point information, processor architecture, and other assembly characteristics.

## OUTPUTS

### ISpy.Models.ISpyAssemblyInfo

Returns an `ISpyAssemblyInfo` object containing assembly metadata.

## RELATED LINKS

[Get-Type](Get-Type.md)
[Get-Dependency](Get-Dependency.md)
[Get-DecompiledSource](Get-DecompiledSource.md)

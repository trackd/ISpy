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
Get-Dependency [-AssemblyPath] <String> [-IncludeVersionDetails] [-ExternalOnly] [<CommonParameters>]
```

## DESCRIPTION

The `Get-Dependency` cmdlet analyzes a .NET assembly and returns information about its dependencies and referenced assemblies.

## OUTPUTS

### ISpy.Models.ISpyDependencyInfo

## RELATED LINKS

[Get-AssemblyInfo](Get-AssemblyInfo.md)

---
external help file: ISpy.dll-Help.xml
Module Name: ISpy
online version: https://github.com/trackd/ISpy/blob/main/docs/en-US
schema: 2.0.0
---

# Find-Type

## SYNOPSIS

Searches for types matching a pattern across multiple .NET assemblies with advanced filtering options.

## SYNTAX

```powershell
Find-Type [-Pattern] <String> [[-AssemblyPaths] <String[]>] [-Directory <String>] [-Recurse]
 [-IncludeNamespace] [-CaseSensitive] [<CommonParameters>]
```

## DESCRIPTION

The `Find-Type` cmdlet provides powerful cross-assembly type search capabilities. It can search across multiple assemblies using wildcard patterns, making it invaluable for discovering types across entire projects, frameworks, or solution structures.

## INPUTS

### System.String[]

## OUTPUTS

### ISpy.Models.ISpyFoundTypeInfo

Returns `ISpyFoundTypeInfo` objects containing details about each matched type.

## RELATED LINKS

[Get-Type](Get-Type.md)
[Get-DecompiledSource](Get-DecompiledSource.md)
[Get-AssemblyInfo](Get-AssemblyInfo.md)

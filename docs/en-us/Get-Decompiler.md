---
external help file: ISpy.dll-Help.xml
Module Name: ISpy
online version: https://github.com/trackd/ISpy/blob/main/docs/en-US
schema: 2.0.0
---

# Get-Decompiler

## SYNOPSIS

Creates a configured CSharpDecompiler instance for advanced decompilation scenarios.

## SYNTAX

```powershell
Get-Decompiler [-AssemblyPath] <String> [-LanguageVersion <LanguageVersion>] [-RemoveDeadStores] 
 [-RemoveDeadCode] [-PDBFilePath <String>] [<CommonParameters>]
```

## DESCRIPTION

The `Get-Decompiler` cmdlet creates and configures a `CSharpDecompiler` instance that can be used for advanced decompilation scenarios. This cmdlet is useful when you need fine-grained control over the decompilation process or want to reuse a decompiler instance across multiple operations.

## OUTPUTS

### ICSharpCode.Decompiler.CSharp.CSharpDecompiler

Returns a configured `CSharpDecompiler` instance.

## RELATED LINKS

[Get-DecompiledSource](Get-DecompiledSource.md)
[Get-DecompilerVersion](Get-DecompilerVersion.md)

````

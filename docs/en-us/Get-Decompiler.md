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
Get-Decompiler [-Path] <String> [-LanguageVersion <LanguageVersion>] [-RemoveDeadStores] 
 [-RemoveDeadCode] [-PDBFilePath <String>] [<CommonParameters>]
```

## DESCRIPTION

The `Get-Decompiler` cmdlet creates and configures a `CSharpDecompiler` instance that can be used for advanced decompilation scenarios. This cmdlet is useful when you need fine-grained control over the decompilation process or want to reuse a decompiler instance across multiple operations.

## EXAMPLES

### Example 1: Create a decompiler for an assembly

```powershell
PS C:\> Get-Decompiler -Path './output/lib/ICSharpCode.Decompiler.dll'
```

This command creates a CSharpDecompiler instance for the specified assembly.  

### Example 2: Specify a language version

```powershell
PS C:\> Get-Decompiler -Path './output/lib/ICSharpCode.Decompiler.dll' -LanguageVersion CSharp10
```

This command creates a decompiler using C# 10 syntax.  

### Example 3: Remove dead code and stores

```powershell
PS C:\> Get-Decompiler -Path './output/lib/ICSharpCode.Decompiler.dll' -RemoveDeadCode -RemoveDeadStores
```

This command configures the decompiler to remove dead code and stores.  

## PARAMETERS

### -Path

Specifies the path to the .NET assembly file to decompile.

```yaml
Type: String
Required: True
Position: 0
Accept pipeline input: True (ByPropertyName, ByValue)
```

### -LanguageVersion

Specifies the C# language version for the decompiler.

```yaml
Type: LanguageVersion
Required: False
Position: Named
```

### -RemoveDeadStores

Removes dead stores from the decompiled output.

```yaml
Type: SwitchParameter
Required: False
Position: Named
```

### -RemoveDeadCode

Removes dead code from the decompiled output.

```yaml
Type: SwitchParameter
Required: False
Position: Named
```

### -PDBFilePath

Specifies the path to a PDB file for debug information.

```yaml
Type: String
Required: False
Position: Named
```

## INPUTS

System.String — accepts assembly file paths from the pipeline.

## OUTPUTS

### ICSharpCode.Decompiler.CSharp.CSharpDecompiler

Returns a configured `CSharpDecompiler` instance.

## NOTES

- Use with Get-Framework or Get-DecompiledSource for advanced scenarios.

## RELATED LINKS

[Get-DecompiledSource](Get-DecompiledSource.md)
[Get-DecompilerVersion](Get-DecompilerVersion.md)

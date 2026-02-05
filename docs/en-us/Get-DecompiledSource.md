---
external help file: ISpy.dll-Help.xml
Module Name: ISpy
online version: https://github.com/trackd/ISpy/blob/main/docs/en-US/
schema: 2.0.0
---

# Get-DecompiledSource

## SYNOPSIS

Decompiles .NET assemblies or specific types to readable C# source code using the decompiler engine.

## SYNTAX

```powershell
Get-DecompiledSource [-AssemblyPath] <String> [[-TypeName] <String>] [-OutputPath <String>]
 [-Settings <DecompilerSettings>] [<CommonParameters>]
```

## DESCRIPTION

The `Get-DecompiledSource` cmdlet converts compiled .NET assemblies (DLLs, EXEs) into readable C# source code. You can decompile entire assemblies, focus on specific types, or extract individual methods. The output can be displayed in the console or saved directly to files for further analysis or documentation purposes.

## EXAMPLES

```powershell
Get-DecompiledSource -AssemblyPath "MyLibrary.dll"
```

## PARAMETERS

### -AssemblyPath

Specifies the path to the .NET assembly file to decompile.

```yaml
Type: String
Parameter Sets: (All)
Aliases: PSPath, Path

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByPropertyName, ByValue)
Accept wildcard characters: False
```

### -TypeName

Full name of a specific type to decompile.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: 1
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -OutputPath

File path where the decompiled source will be saved.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

## INPUTS

### System.String

## OUTPUTS

### ISpy.Models.ISpyDecompilationResult

Returns an `ISpyDecompilationResult` containing `AssemblyPath`, `TypeName`, `Source`, `Success`, and optional `FilePath`.

## RELATED LINKS

[Export-DecompiledSource](Export-DecompiledSource.md)
[Get-Type](Get-Type.md)
[Get-Method](Get-Method.md)
[Find-Type](Find-Type.md)

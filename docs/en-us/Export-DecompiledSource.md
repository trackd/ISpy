---
external help file: ISpy.dll-Help.xml
Module Name: ISpy
online version: https://github.com/trackd/ISpy/blob/main/docs/en-US/
schema: 2.0.0
---

# Export-DecompiledSource

## SYNOPSIS

Exports decompiled C# source code from .NET assemblies to organized file structures with advanced bulk processing capabilities.

## SYNTAX

```powershell
Export-DecompiledSource [-AssemblyPath] <String> [-OutputDirectory] <String> [-TypeNames <String[]>]
 [-Namespace <String>] [-CreateNamespaceDirectories] [-Settings <DecompilerSettings>] [-Force]
 [<CommonParameters>]
```

## DESCRIPTION

The `Export-DecompiledSource` cmdlet provides powerful bulk decompilation capabilities for .NET assemblies. It efficiently processes large assemblies by decompiling types individually and organizing the resulting C# source code into well-structured directory hierarchies.

## EXAMPLES

```powershell
Export-DecompiledSource -AssemblyPath "MyLibrary.dll" -OutputDirectory ".\Decompiled"
```

## PARAMETERS

### -AssemblyPath

Specifies the path to the .NET assembly file to decompile and export. This can be a DLL, EXE, or any valid .NET assembly format.

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

### -OutputDirectory

Specifies the root directory where decompiled source files will be exported. The directory will be created if it doesn't exist.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: True
Position: 1
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -TypeNames

Specifies an array of specific type names to export. If not provided, all types in the assembly will be exported. Use full type names including namespace.

```yaml
Type: String[]
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: All types in assembly
Accept pipeline input: False
Accept wildcard characters: False
```

## INPUTS

### System.String

## OUTPUTS

### ISpy.Models.ISpyExportResult

Returns an `ISpyExportResult` object containing information about exported files.

## RELATED LINKS

[Get-DecompiledSource](Get-DecompiledSource.md)
[Get-Type](Get-Type.md)
[Find-Type](Find-Type.md)
[Get-AssemblyInfo](Get-AssemblyInfo.md)

---
external help file: ISpy.dll-Help.xml
Module Name: ISpy
online version: https://github.com/trackd/ISpy/blob/main/docs/en-US
schema: 2.0.0
---

# Get-Type

## SYNOPSIS

Retrieves comprehensive type information from .NET assemblies with advanced filtering and classification options.

## SYNTAX

```powershell
Get-Type [-AssemblyPath] <String> [-Namespace <String>] [-NamePattern <String>] [-PublicOnly] [-Detailed] [-TypeKinds <String[]>]
 [<CommonParameters>]
```

## DESCRIPTION

The `Get-Type` cmdlet provides detailed analysis of all types (classes, interfaces, enums, structs, delegates) contained within a .NET assembly. It offers sophisticated filtering options by namespace, name patterns, visibility, and type characteristics, making it an essential tool for assembly exploration and analysis.

## EXAMPLES

```powershell
Get-Type -AssemblyPath "MyLibrary.dll"
```

## PARAMETERS

### -AssemblyPath

Path to the assembly file to analyze

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

### -Detailed

Include detailed type information

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -NamePattern

Filter types by name pattern (supports wildcards)

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

### -Namespace

Filter types by namespace

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

### -PublicOnly

Only return public types

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -TypeKinds

Filter by type kinds. Use shorthand: 'c' (class), 'i' (interface), 's' (struct), 'e' (enum), 'd' (delegate). Can combine: 'ci' for classes and interfaces, or use full names: 'class', 'interface', etc.

```yaml
Type: String[]
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

### ISpy.Models.ISpyTypeInfo

Returns `ISpyTypeInfo` objects containing detailed information about each type including name, namespace, accessibility, inheritance information, and member counts.

## RELATED LINKS

[Get-Method](Get-Method.md)
[Find-Type](Find-Type.md)

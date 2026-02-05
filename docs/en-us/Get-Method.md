---
external help file: ISpy.dll-Help.xml
Module Name: ISpy
online version: https://github.com/trackd/ISpy/blob/main/docs/en-US
schema: 2.0.0
---

# Get-Method

## SYNOPSIS

Analyzes and retrieves comprehensive method information from specific types in .NET assemblies with advanced filtering capabilities.

## SYNTAX

```powershell
Get-Method [-AssemblyPath] <String> [-TypeName] <String> [-NamePattern <String>] [-PublicOnly]
 [-IncludeStatic] [-IncludeConstructors] [-IncludeProperties] [<CommonParameters>]
```

## DESCRIPTION

The `Get-Method` cmdlet provides detailed analysis of methods, constructors, and properties within a specified type. It offers sophisticated filtering options to focus on specific method categories, visibility levels, and naming patterns.

## EXAMPLES

```powershell
Get-Method -AssemblyPath "System.Console.dll" -TypeName "System.Console" -PublicOnly
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

### -IncludeConstructors

Include constructors

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

### -IncludeProperties

Include property getters and setters

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

### -IncludeStatic

Include static methods

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

Filter methods by name pattern (supports wildcards)

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

Only return public methods

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

### -TypeName

Full name of the type to analyze

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

## INPUTS

### System.String

## OUTPUTS

### ISpy.Models.ISpyMethodInfo

## RELATED LINKS

[Get-Type](Get-Type.md)

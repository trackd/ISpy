---
external help file: ISpy.dll-Help.xml
Module Name: ISpy
online version: https://github.com/trackd/ISpy/blob/main/docs/en-US
schema: 2.0.0
---

# Get-Type

## SYNOPSIS

Retrieves detailed type metadata from a .NET assembly with flexible filtering and classification.

## SYNTAX

```powershell
Get-Type [-Path] <String> [-Namespace <String>] [-NamePattern <String>] [-PublicOnly] [-TypeKinds <TypeKind[]>] [-IncludeCompilerGenerated] [<CommonParameters>]
```

## DESCRIPTION

`Get-Type` analyzes every type defined in the specified assembly and streams back `ISpyTypeInfo`
objects. The cmdlet supports namespace, name-pattern, visibility, and type-kind filters while keeping
compiler-generated definitions out of the default view.

## EXAMPLES

### Example 1: List public types in a namespace

```powershell
PS C:\> Get-Type -Path "MyLibrary.dll" -Namespace "MyCompany.Core" -PublicOnly
```

This command returns `ISpyTypeInfo` objects for public types in the `MyCompany.Core` namespace.

### Example 2: Find types matching a wildcard

```powershell
PS C:\> Get-Type -Path "MyLibrary.dll" -NamePattern '*Manager'
```

This command finds types whose names end with `Manager` (includes non-public types unless `-PublicOnly` is specified).

### Example 3: Stream and filter enums

```powershell
PS C:\> Get-Type -Path "MyLibrary.dll" -PublicOnly -TypeKind Enum | Select-Object FullName, Kind
```

This command streams public enum types and selects `FullName` and `Kind` for concise output.

## PARAMETERS

### -Path

Path to the assembly file to analyze.

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

### -Namespace

Limit the results to a specific namespace.

```yaml
Type: String
Position: Named
Required: False
Accept pipeline input: False
```

### -NamePattern

Filter types by a wildcard-friendly name pattern.

```yaml
Type: String
Position: Named
Required: False
Accept pipeline input: False
```

### -PublicOnly

Only return public types.

```yaml
Type: SwitchParameter
Position: Named
Required: False
Accept pipeline input: False
```

### -TypeKinds

Filter by one or more type kinds.
Other, Class, Interface, Struct, Delegate, Enum, Void, Unknown, Null, None, Dynamic, UnboundTypeArgument, TypeParameter, Array, Pointer, ByReference, Intersection, ArgList, Tuple, ModOpt, ModReq, NInt, NUInt, FunctionPointer

```yaml
Type: TypeKind[]
Position: Named
Required: False
Accept pipeline input: False
```

### -IncludeCompilerGenerated

Include compiler-generated types

```yaml
Type: SwitchParameter
Position: Named
Required: False
Accept pipeline input: False
```

## INPUTS

### System.String

## OUTPUTS

### ISpy.Models.ISpyTypeInfo

Each returned object contains the type's name, namespace, accessibility flags, `Kind`, and optionally member counts. `IsCompilerGenerated` flags synthetic definitions and helps downstream plumbing avoid noise.

## NOTES

- Use with Get-AssemblyInfo to correlate type counts and metadata.
- Supports filtering by namespace, name pattern, visibility, and type kind.

## RELATED LINKS

[Get-AssemblyInfo](Get-AssemblyInfo.md)
[Get-Dependency](Get-Dependency.md)
[Show-Type](Show-Type.md)

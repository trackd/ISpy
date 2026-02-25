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
Get-Type [-Path] <String> [-Namespace <String>] [-NamePattern <String>] [-PublicOnly] [-Typekind <TypeKind[]>] [-IncludeCompilerGenerated] [-Settings <DecompilerSettings>] [-Decompiler <CSharpDecompiler>] [<CommonParameters>]

Get-Type [[-InputObject] <Object>] [-Namespace <String>] [-NamePattern <String>] [-PublicOnly] [-Typekind <TypeKind[]>] [-IncludeCompilerGenerated] [-Settings <DecompilerSettings>] [-Decompiler <CSharpDecompiler>] [<CommonParameters>]

Get-Type [-TypeName] <String> [-Namespace <String>] [-NamePattern <String>] [-PublicOnly] [-Typekind <TypeKind[]>] [-IncludeCompilerGenerated] [<CommonParameters>]
```

## DESCRIPTION

`Get-Type` analyzes every type defined in the specified assembly and streams back `ISpyTypeInfo`
objects. The cmdlet supports namespace, name-pattern, visibility, and type-kind filters while keeping
compiler-generated definitions out of the default view.

When `-TypeName` is provided, `Get-Type` resolves matching types from currently loaded AppDomain assemblies,
which enables pathless discovery workflows. You can also pipe a runtime `System.Type` directly to `Get-Type`.

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

### Example 4: Reuse a custom decompiler

```powershell
PS C:\> $decompiler = New-Decompiler -Path "MyLibrary.dll"
PS C:\> Get-Type -Path "MyLibrary.dll" -Decompiler $decompiler -PublicOnly
```

This command reuses a pre-created decompiler instance.

### Example 5: Resolve a loaded type by name without a file path

```powershell
PS C:\> Get-Type -TypeName System.Management.Automation.LanguagePrimitives
```

This command resolves the type from assemblies currently loaded in the PowerShell process.

### Example 6: Pipe a runtime type into Get-Type

```powershell
PS C:\> [System.Management.Automation.LanguagePrimitives] | Get-Type
```

This command converts the runtime type into `ISpyTypeInfo` output.

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

### -TypeName

Resolve type metadata from already-loaded assemblies by full type name or simple type name.

```yaml
Type: String
Parameter Sets: ByTypeName
Required: True
Position: 0
Accept pipeline input: False
```

### -InputObject

Pipeline input object. Supports assembly path strings and runtime `System.Type` values.

```yaml
Type: Object
Parameter Sets: ByInputObject
Required: False
Position: 0
Accept pipeline input: True (ByValue)
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

### -Typekind

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

### -Settings

Custom decompiler settings used when constructing a decompiler.

```yaml
Type: DecompilerSettings
Position: Named
Required: False
Accept pipeline input: False
```

### -Decompiler

Custom `CSharpDecompiler` instance to use directly.

```yaml
Type: CSharpDecompiler
Position: Named
Required: False
Accept pipeline input: False
```

## INPUTS

### System.String

### System.Type

## OUTPUTS

### ISpy.Models.ISpyTypeInfo

Each returned object contains the type's name, namespace, accessibility flags, `Kind`, and optionally member counts. `IsCompilerGenerated` flags synthetic definitions and helps downstream plumbing avoid noise.

## NOTES

- Use with Get-AssemblyInfo to correlate type counts and metadata.
- Supports filtering by namespace, name pattern, visibility, and type kind.

## RELATED LINKS

[Get-AssemblyInfo](Get-AssemblyInfo.md)
[Get-Dependency](Get-Dependency.md)
[Expand-Type](Expand-Type.md)

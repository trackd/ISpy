---
external help file: ISpy.dll-Help.xml
Module Name: ISpy
online version: https://github.com/trackd/ISpy/blob/main/docs/en-US
schema: 2.0.0
---

# Expand-Type

## SYNOPSIS

Resolves methods, cmdlets, and types from pipeline input and streams decompiled C# source for the resolved members.

## SYNTAX

```powershell
Expand-Type [[-InputObject] <PSObject>] [-Path <String>] [-TypeName <String>] [-MethodName <String>] [-Metadata] [<CommonParameters>]
```

## DESCRIPTION

`Expand-Type` accepts a variety of inputs from the pipeline and resolves them to types or method targets for decompilation. Supported input forms include `System.Reflection.MethodBase`, `PSMethod` (method group adapters), `CommandInfo` (cmdlet/alias), `Type`, delegates, and collections of these objects. When method targets are identified the cmdlet decompiles them (grouping by assembly when appropriate) and emits the resulting C# source.

This cmdlet is useful for interactively inspecting implementations discovered via tab-completion, `Get-Command`, `Get-ChildItem`, or other pipeline-producing commands.

## EXAMPLES

### Example 1: Decompile a method group

```powershell
[math]::Round | Expand-Type
```

Resolves the `Round` overloads on `System.Math` and prints the decompiled C# bodies for the overloads.

### Example 2: Decompile a cmdlet implementation

```powershell
Get-Command Get-ChildItem | Expand-Type
```

Resolves the implementing type for the cmdlet and decompiles the relevant methods.

### Example 3: Decompile a specific type from an assembly

```powershell
Expand-Type -Path "C:\libs\MyLib.dll" -TypeName "MyNamespace.MyClass"
```

Decompiles the specified type from the provided assembly.

### Example 4: Emit metadata only

```powershell
[math]::Round | Expand-Type -Metadata
```

Emits a metadata object describing the resolved assembly path, declaring type, method names and metadata tokens, and does not print the source when `-Metadata` is specified.

## PARAMETERS

### -InputObject

The input object to resolve. Accepts `PSObject`, `MethodBase`, `PSMethod`, `CommandInfo`, `Type`, delegates, and collections of these. Accepts pipeline input by value.

```yaml
Type: PSObject
Required: False
Position: 0
Accept pipeline input: True (ByValue)
```

### -Path

Path to the assembly that contains the target type. When provided the cmdlet will resolve types and methods against the specified assembly path. Alias: `PSPath`, `Path`.

```yaml
Type: String
Required: False
Position: Named
Accept pipeline input: True (ByPropertyName)
```

### -TypeName

Full name of the type that contains the target method (for explicit lookups) or the type to decompile when `-MethodName` is not specified.

```yaml
Type: String
Required: False
Position: Named
```

### -MethodName

Method name to filter overloads by. If omitted, the cmdlet will decompile the resolved type (when available) or all resolved method targets.

```yaml
Type: String
Required: False
Position: Named
```

### -Metadata

When present, emits a metadata object (assembly path, declaring type, method names, metadata tokens) alongside the source text.

```yaml
Type: SwitchParameter
Required: False
```

## INPUTS

System.Object (various) — accepts method, type, command, and PS adapter objects from the pipeline.

## OUTPUTS

- Default: `System.String` — When called without `-Metadata` the cmdlet emits the decompiled C# source as plain text so it can be piped to formatters or renderers.
- Metadata: `ISpy.Models.ISpyDecompilationResult` — When `-Metadata` is used the cmdlet emits an `ISpyDecompilationResult` object containing `AssemblyPath`, `TypeName`, `MethodNames`, `MetadataTokens`, `Source` (if present), and other metadata.

## NOTES

- The cmdlet attempts ergonomic resolution of common PowerShell pipeline objects (`PSMethod`, `CommandInfo`, `MethodBase`, `Type`).
- When multiple method targets are resolved from the input the cmdlet groups them by assembly and performs a single decompiler pass per assembly for efficient output and to avoid repeated using directives.

## RELATED LINKS

[Get-DecompiledSource](Get-DecompiledSource.md)

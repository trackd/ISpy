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
Get-DecompiledSource [-Path] <String> [[-TypeName] <String>] [-OutputPath <String>]
 [-Settings <DecompilerSettings>] [-Decompiler <CSharpDecompiler>] [<CommonParameters>]

Get-DecompiledSource [-TypeName] <String> [-Settings <DecompilerSettings>] [-Decompiler <CSharpDecompiler>] [<CommonParameters>]
```

## DESCRIPTION

The `Get-DecompiledSource` cmdlet converts compiled .NET assemblies (DLLs, EXEs) into readable C# source code. You can decompile entire assemblies, focus on specific types, or extract individual methods. The output can be displayed in the console or saved directly to files for further analysis or documentation purposes.

## EXAMPLES

### Example 1: Decompile an assembly to the console

```powershell
PS C:\> Get-DecompiledSource -Path "MyLibrary.dll"
```

This command decompiles `MyLibrary.dll` and writes human-readable C# source for the discovered types to the console.  

### Example 2: Decompile a single type and write to a file

```powershell
PS C:\> Get-DecompiledSource -Path "MyLibrary.dll" -TypeName 'MyCompany.Core.Service' -OutputPath '.\Service.cs'
```

This command extracts the decompiled source for the `MyCompany.Core.Service` type and saves it to `Service.cs`.  

### Example 3: Decompile all types into a folder

```powershell
PS C:\> Get-DecompiledSource -Path "$PSHOME/Humanizer.dll"
```

This command decompiles every type in `Humanizer.dll`

### Example 4: Reuse a custom decompiler

```powershell
PS C:\> $decompiler = New-Decompiler -Path "MyLibrary.dll"
PS C:\> Get-DecompiledSource -Path "MyLibrary.dll" -TypeName 'MyCompany.Core.Service' -Decompiler $decompiler
```

This command uses a pre-created decompiler instance.

### Example 5: Decompile a loaded type without specifying a path

```powershell
PS C:\> Get-DecompiledSource -TypeName System.Management.Automation.LanguagePrimitives
```

This command resolves the type from loaded assemblies, discovers its assembly path automatically, and decompiles that type.

## PARAMETERS

### -Path

Specifies the path to the .NET assembly file to decompile.

```yaml
Type: String
Required: True
Position: 0
Accept pipeline input: True (ByPropertyName, ByValue)
```

### -TypeName

Full name of a specific type to decompile.

```yaml
Type: String
Required: False
Position: 1
Accept pipeline input: False
```

When `-Path` is omitted, `-TypeName` is resolved against loaded AppDomain assemblies to infer the assembly path.

### -Settings

Custom decompiler settings used when creating a decompiler.

```yaml
Type: DecompilerSettings
Required: False
Position: Named
Accept pipeline input: False
```

### -Decompiler

Custom `CSharpDecompiler` instance to use directly.

```yaml
Type: CSharpDecompiler
Required: False
Position: Named
Accept pipeline input: False
```

## INPUTS

System.String — accepts assembly file paths from the pipeline.

## OUTPUTS

### ISpy.Models.ISpyDecompilationResult

Returns an `ISpyDecompilationResult` containing `AssemblyPath`, `TypeName`, `Source`, `Success`, and optional `FilePath`.

## NOTES

- Use `-TypeName` to decompile a specific type, or omit to decompile the whole assembly.
- Output can be piped to file or formatting cmdlets for further processing.

## RELATED LINKS

[Export-DecompiledSource](Export-DecompiledSource.md)
[Expand-Type](Expand-Type.md)

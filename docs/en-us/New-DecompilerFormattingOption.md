---
external help file: ISpy.dll-Help.xml
Module Name: ISpy
online version: https://github.com/trackd/ISpy/blob/main/docs/en-US/
schema: 2.0.0
---

# New-DecompilerFormattingOption

## SYNOPSIS

Creates a configurable `CSharpFormattingOptions` object for decompiler output formatting.

## SYNTAX

```powershell
New-DecompilerFormattingOption [<CommonParameters>]
```

## DESCRIPTION

`New-DecompilerFormattingOption` creates a `CSharpFormattingOptions` instance and dynamically exposes writable boolean formatting switches. The result can be passed to `New-DecompilerSetting` to control emitted C# style.

## EXAMPLES

### Example 1: Create default formatting options

```powershell
PS C:\> New-DecompilerFormattingOption
```

Creates default formatting options.

### Example 2: Enable selected formatting switches

```powershell
PS C:\> New-DecompilerFormattingOption -IndentSwitchBody -SpaceBeforeMethodCallParentheses
```

Creates formatting options with selected style switches enabled.

### Example 3: Pipe into decompiler settings creation

```powershell
PS C:\> $fmt = New-DecompilerFormattingOption -IndentSwitchBody
PS C:\> New-DecompilerSetting -CSharpFormattingOptions $fmt
```

Creates decompiler settings that carry the custom formatting options.

## PARAMETERS

This cmdlet defines dynamic parameters for writable boolean properties on `CSharpFormattingOptions`.

## INPUTS

None.

## OUTPUTS

### ICSharpCode.Decompiler.CSharp.OutputVisitor.CSharpFormattingOptions

Returns a configured formatting options object.

## NOTES

- Parameter availability can change with ILSpy versions as formatting properties evolve.

## RELATED LINKS

[New-DecompilerSetting](New-DecompilerSetting.md)
[Get-Decompiler](Get-Decompiler.md)

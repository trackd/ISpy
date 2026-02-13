---
external help file: ISpy.dll-Help.xml
Module Name: ISpy
online version: https://github.com/trackd/ISpy/blob/main/docs/en-US/
schema: 2.0.0
---

# New-DecompilerSetting

## SYNOPSIS

Creates a configurable `DecompilerSettings` instance for reuse across decompilation cmdlets.

## SYNTAX

```powershell
New-DecompilerSetting [-LanguageVersion <LanguageVersion>] [-CSharpFormattingOptions <CSharpFormattingOptions>] [<CommonParameters>]
```

## DESCRIPTION

`New-DecompilerSetting` builds a `DecompilerSettings` object for ILSpy-based cmdlets. It supports language version selection, accepts `CSharpFormattingOptions`, and dynamically exposes writable boolean `DecompilerSettings` switches so you can quickly shape decompilation behavior.

## EXAMPLES

### Example 1: Create default settings

```powershell
PS C:\> New-DecompilerSetting
```

Creates a default `DecompilerSettings` object.

### Example 2: Set language version and dead-code cleanup

```powershell
PS C:\> New-DecompilerSetting -LanguageVersion CSharp11 -RemoveDeadCode -RemoveDeadStores
```

Creates settings that target C# 11 and remove dead code/stores.

### Example 3: Attach formatting options

```powershell
PS C:\> $fmt = New-CSharpFormattingOption -IndentSwitchBody
PS C:\> $settings = New-DecompilerSetting -CSharpFormattingOptions $fmt -UsingDeclarations
```

Creates settings that include custom C# formatting preferences.

## PARAMETERS

### -LanguageVersion

C# language version used by the decompiler.

```yaml
Type: LanguageVersion
Required: False
Position: Named
```

### -CSharpFormattingOptions

Formatting options object to apply to `DecompilerSettings.CSharpFormattingOptions`.

```yaml
Type: CSharpFormattingOptions
Required: False
Position: Named
```

## INPUTS

None.

## OUTPUTS

### ICSharpCode.Decompiler.DecompilerSettings

Returns a configured `DecompilerSettings` instance.

## NOTES

- This cmdlet includes dynamic switch parameters mapped from writable boolean properties on `DecompilerSettings`.

## RELATED LINKS

[New-CSharpFormattingOption](New-CSharpFormattingOption.md)
[Get-Decompiler](Get-Decompiler.md)
[Get-DecompiledSource](Get-DecompiledSource.md)

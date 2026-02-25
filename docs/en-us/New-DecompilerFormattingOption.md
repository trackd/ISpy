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

`New-DecompilerFormattingOption` creates a `CSharpFormattingOptions` instance and dynamically exposes writable formatting properties from ILSpy. Boolean properties are exposed as switches, and non-boolean properties (for example `int`, `string`, and enum-based options) are exposed as typed dynamic parameters.

The result can be passed to `New-DecompilerSetting` to control emitted C# style.

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

### Example 2b: Set non-boolean formatting options

```powershell
PS C:\> New-DecompilerFormattingOption -MinimumBlankLinesBetweenUsingGroups 2 -ClassBraceStyle EndOfLine
```

Creates formatting options by setting typed dynamic parameters (an `int` and an enum value).

### Example 3: Pipe into decompiler settings creation

```powershell
PS C:\> $fmt = New-DecompilerFormattingOption -IndentSwitchBody
PS C:\> New-DecompilerSetting -CSharpFormattingOptions $fmt
```

Creates decompiler settings that carry the custom formatting options.

## PARAMETERS

### -AlignElseInIfStatements

Set CSharpFormattingOptions.AlignElseInIfStatements to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -AlignEmbeddedStatements

Set CSharpFormattingOptions.AlignEmbeddedStatements to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -AlignToFirstIndexerArgument

Set CSharpFormattingOptions.AlignToFirstIndexerArgument to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -AlignToFirstIndexerDeclarationParameter

Set CSharpFormattingOptions.AlignToFirstIndexerDeclarationParameter to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -AlignToFirstMethodCallArgument

Set CSharpFormattingOptions.AlignToFirstMethodCallArgument to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -AlignToFirstMethodDeclarationParameter

Set CSharpFormattingOptions.AlignToFirstMethodDeclarationParameter to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -AlignToMemberReferenceDot

Set CSharpFormattingOptions.AlignToMemberReferenceDot to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -AllowEventAddBlockInline

Set CSharpFormattingOptions.AllowEventAddBlockInline to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -AllowEventRemoveBlockInline

Set CSharpFormattingOptions.AllowEventRemoveBlockInline to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -AllowIfBlockInline

Set CSharpFormattingOptions.AllowIfBlockInline to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -AllowOneLinedArrayInitialziers

Set CSharpFormattingOptions.AllowOneLinedArrayInitialziers to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -AnonymousMethodBraceStyle

Set CSharpFormattingOptions.AnonymousMethodBraceStyle (BraceStyle).  
Accepted values: EndOfLine, EndOfLineWithoutSpace, NextLine, NextLineShifted, NextLineShifted2, BannerStyle.

```yaml
Type: BraceStyle
Required: False
Position: Named
Accept pipeline input: False
```

### -ArrayInitializerBraceStyle

Set CSharpFormattingOptions.ArrayInitializerBraceStyle (BraceStyle).  
Accepted values: EndOfLine, EndOfLineWithoutSpace, NextLine, NextLineShifted, NextLineShifted2, BannerStyle.

```yaml
Type: BraceStyle
Required: False
Position: Named
Accept pipeline input: False
```

### -ArrayInitializerWrapping

Set CSharpFormattingOptions.ArrayInitializerWrapping (Wrapping).  
Accepted values: DoNotWrap, WrapAlways, WrapIfTooLong.

```yaml
Type: Wrapping
Required: False
Position: Named
Accept pipeline input: False
```

### -AutoPropertyFormatting

Set CSharpFormattingOptions.AutoPropertyFormatting (PropertyFormatting).  
Accepted values: SingleLine, MultipleLines.

```yaml
Type: PropertyFormatting
Required: False
Position: Named
Accept pipeline input: False
```

### -CatchNewLinePlacement

Set CSharpFormattingOptions.CatchNewLinePlacement (NewLinePlacement).  
Accepted values: DoNotCare, NewLine, SameLine.

```yaml
Type: NewLinePlacement
Required: False
Position: Named
Accept pipeline input: False
```

### -ChainedMethodCallWrapping

Set CSharpFormattingOptions.ChainedMethodCallWrapping (Wrapping).  
Accepted values: DoNotWrap, WrapAlways, WrapIfTooLong.

```yaml
Type: Wrapping
Required: False
Position: Named
Accept pipeline input: False
```

### -ClassBraceStyle

Set CSharpFormattingOptions.ClassBraceStyle (BraceStyle).  
Accepted values: EndOfLine, EndOfLineWithoutSpace, NextLine, NextLineShifted, NextLineShifted2, BannerStyle.

```yaml
Type: BraceStyle
Required: False
Position: Named
Accept pipeline input: False
```

### -ConstructorBraceStyle

Set CSharpFormattingOptions.ConstructorBraceStyle (BraceStyle).  
Accepted values: EndOfLine, EndOfLineWithoutSpace, NextLine, NextLineShifted, NextLineShifted2, BannerStyle.

```yaml
Type: BraceStyle
Required: False
Position: Named
Accept pipeline input: False
```

### -DestructorBraceStyle

Set CSharpFormattingOptions.DestructorBraceStyle (BraceStyle).  
Accepted values: EndOfLine, EndOfLineWithoutSpace, NextLine, NextLineShifted, NextLineShifted2, BannerStyle.

```yaml
Type: BraceStyle
Required: False
Position: Named
Accept pipeline input: False
```

### -ElseIfNewLinePlacement

Set CSharpFormattingOptions.ElseIfNewLinePlacement (NewLinePlacement).  
Accepted values: DoNotCare, NewLine, SameLine.

```yaml
Type: NewLinePlacement
Required: False
Position: Named
Accept pipeline input: False
```

### -ElseNewLinePlacement

Set CSharpFormattingOptions.ElseNewLinePlacement (NewLinePlacement).  
Accepted values: DoNotCare, NewLine, SameLine.

```yaml
Type: NewLinePlacement
Required: False
Position: Named
Accept pipeline input: False
```

### -EmbeddedStatementPlacement

Set CSharpFormattingOptions.EmbeddedStatementPlacement (NewLinePlacement).  
Accepted values: DoNotCare, NewLine, SameLine.

```yaml
Type: NewLinePlacement
Required: False
Position: Named
Accept pipeline input: False
```

### -EmptyLineFormatting

Set CSharpFormattingOptions.EmptyLineFormatting (EmptyLineFormatting).  
Accepted values: DoNotChange, Indent, DoNotIndent.

```yaml
Type: EmptyLineFormatting
Required: False
Position: Named
Accept pipeline input: False
```

### -EnumBraceStyle

Set CSharpFormattingOptions.EnumBraceStyle (BraceStyle).  
Accepted values: EndOfLine, EndOfLineWithoutSpace, NextLine, NextLineShifted, NextLineShifted2, BannerStyle.

```yaml
Type: BraceStyle
Required: False
Position: Named
Accept pipeline input: False
```

### -EventAddBraceStyle

Set CSharpFormattingOptions.EventAddBraceStyle (BraceStyle).  
Accepted values: EndOfLine, EndOfLineWithoutSpace, NextLine, NextLineShifted, NextLineShifted2, BannerStyle.

```yaml
Type: BraceStyle
Required: False
Position: Named
Accept pipeline input: False
```

### -EventBraceStyle

Set CSharpFormattingOptions.EventBraceStyle (BraceStyle).  
Accepted values: EndOfLine, EndOfLineWithoutSpace, NextLine, NextLineShifted, NextLineShifted2, BannerStyle.

```yaml
Type: BraceStyle
Required: False
Position: Named
Accept pipeline input: False
```

### -EventRemoveBraceStyle

Set CSharpFormattingOptions.EventRemoveBraceStyle (BraceStyle).  
Accepted values: EndOfLine, EndOfLineWithoutSpace, NextLine, NextLineShifted, NextLineShifted2, BannerStyle.

```yaml
Type: BraceStyle
Required: False
Position: Named
Accept pipeline input: False
```

### -FinallyNewLinePlacement

Set CSharpFormattingOptions.FinallyNewLinePlacement (NewLinePlacement).  
Accepted values: DoNotCare, NewLine, SameLine.

```yaml
Type: NewLinePlacement
Required: False
Position: Named
Accept pipeline input: False
```

### -IndentationString

Set CSharpFormattingOptions.IndentationString (String).  
Provide a string value.

```yaml
Type: String
Required: False
Position: Named
Accept pipeline input: False
```

### -IndentBlocks

Set CSharpFormattingOptions.IndentBlocks to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -IndentBlocksInsideExpressions

Set CSharpFormattingOptions.IndentBlocksInsideExpressions to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -IndentBreakStatements

Set CSharpFormattingOptions.IndentBreakStatements to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -IndentCaseBody

Set CSharpFormattingOptions.IndentCaseBody to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -IndentClassBody

Set CSharpFormattingOptions.IndentClassBody to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -IndentEnumBody

Set CSharpFormattingOptions.IndentEnumBody to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -IndentEventBody

Set CSharpFormattingOptions.IndentEventBody to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -IndentInterfaceBody

Set CSharpFormattingOptions.IndentInterfaceBody to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -IndentMethodBody

Set CSharpFormattingOptions.IndentMethodBody to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -IndentNamespaceBody

Set CSharpFormattingOptions.IndentNamespaceBody to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -IndentPreprocessorDirectives

Set CSharpFormattingOptions.IndentPreprocessorDirectives to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -IndentPropertyBody

Set CSharpFormattingOptions.IndentPropertyBody to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -IndentStructBody

Set CSharpFormattingOptions.IndentStructBody to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -IndentSwitchBody

Set CSharpFormattingOptions.IndentSwitchBody to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -IndexerArgumentWrapping

Set CSharpFormattingOptions.IndexerArgumentWrapping (Wrapping).  
Accepted values: DoNotWrap, WrapAlways, WrapIfTooLong.

```yaml
Type: Wrapping
Required: False
Position: Named
Accept pipeline input: False
```

### -IndexerClosingBracketOnNewLine

Set CSharpFormattingOptions.IndexerClosingBracketOnNewLine (NewLinePlacement).  
Accepted values: DoNotCare, NewLine, SameLine.

```yaml
Type: NewLinePlacement
Required: False
Position: Named
Accept pipeline input: False
```

### -IndexerDeclarationClosingBracketOnNewLine

Set CSharpFormattingOptions.IndexerDeclarationClosingBracketOnNewLine (NewLinePlacement).  
Accepted values: DoNotCare, NewLine, SameLine.

```yaml
Type: NewLinePlacement
Required: False
Position: Named
Accept pipeline input: False
```

### -IndexerDeclarationParameterWrapping

Set CSharpFormattingOptions.IndexerDeclarationParameterWrapping (Wrapping).  
Accepted values: DoNotWrap, WrapAlways, WrapIfTooLong.

```yaml
Type: Wrapping
Required: False
Position: Named
Accept pipeline input: False
```

### -InterfaceBraceStyle

Set CSharpFormattingOptions.InterfaceBraceStyle (BraceStyle).  
Accepted values: EndOfLine, EndOfLineWithoutSpace, NextLine, NextLineShifted, NextLineShifted2, BannerStyle.

```yaml
Type: BraceStyle
Required: False
Position: Named
Accept pipeline input: False
```

### -IsBuiltIn

Set CSharpFormattingOptions.IsBuiltIn to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -KeepCommentsAtFirstColumn

Set CSharpFormattingOptions.KeepCommentsAtFirstColumn to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -MethodBraceStyle

Set CSharpFormattingOptions.MethodBraceStyle (BraceStyle).  
Accepted values: EndOfLine, EndOfLineWithoutSpace, NextLine, NextLineShifted, NextLineShifted2, BannerStyle.

```yaml
Type: BraceStyle
Required: False
Position: Named
Accept pipeline input: False
```

### -MethodCallArgumentWrapping

Set CSharpFormattingOptions.MethodCallArgumentWrapping (Wrapping).  
Accepted values: DoNotWrap, WrapAlways, WrapIfTooLong.

```yaml
Type: Wrapping
Required: False
Position: Named
Accept pipeline input: False
```

### -MethodCallClosingParenthesesOnNewLine

Set CSharpFormattingOptions.MethodCallClosingParenthesesOnNewLine (NewLinePlacement).  
Accepted values: DoNotCare, NewLine, SameLine.

```yaml
Type: NewLinePlacement
Required: False
Position: Named
Accept pipeline input: False
```

### -MethodDeclarationClosingParenthesesOnNewLine

Set CSharpFormattingOptions.MethodDeclarationClosingParenthesesOnNewLine (NewLinePlacement).  
Accepted values: DoNotCare, NewLine, SameLine.

```yaml
Type: NewLinePlacement
Required: False
Position: Named
Accept pipeline input: False
```

### -MethodDeclarationParameterWrapping

Set CSharpFormattingOptions.MethodDeclarationParameterWrapping (Wrapping).  
Accepted values: DoNotWrap, WrapAlways, WrapIfTooLong.

```yaml
Type: Wrapping
Required: False
Position: Named
Accept pipeline input: False
```

### -MinimumBlankLinesAfterUsings

Set CSharpFormattingOptions.MinimumBlankLinesAfterUsings (Int32).  
Provide a numeric value.

```yaml
Type: Int32
Required: False
Position: Named
Accept pipeline input: False
```

### -MinimumBlankLinesAroundRegion

Set CSharpFormattingOptions.MinimumBlankLinesAroundRegion (Int32).  
Provide a numeric value.

```yaml
Type: Int32
Required: False
Position: Named
Accept pipeline input: False
```

### -MinimumBlankLinesBeforeFirstDeclaration

Set CSharpFormattingOptions.MinimumBlankLinesBeforeFirstDeclaration (Int32).  
Provide a numeric value.

```yaml
Type: Int32
Required: False
Position: Named
Accept pipeline input: False
```

### -MinimumBlankLinesBeforeUsings

Set CSharpFormattingOptions.MinimumBlankLinesBeforeUsings (Int32).  
Provide a numeric value.

```yaml
Type: Int32
Required: False
Position: Named
Accept pipeline input: False
```

### -MinimumBlankLinesBetweenEventFields

Set CSharpFormattingOptions.MinimumBlankLinesBetweenEventFields (Int32).  
Provide a numeric value.

```yaml
Type: Int32
Required: False
Position: Named
Accept pipeline input: False
```

### -MinimumBlankLinesBetweenFields

Set CSharpFormattingOptions.MinimumBlankLinesBetweenFields (Int32).  
Provide a numeric value.

```yaml
Type: Int32
Required: False
Position: Named
Accept pipeline input: False
```

### -MinimumBlankLinesBetweenMembers

Set CSharpFormattingOptions.MinimumBlankLinesBetweenMembers (Int32).  
Provide a numeric value.

```yaml
Type: Int32
Required: False
Position: Named
Accept pipeline input: False
```

### -MinimumBlankLinesBetweenTypes

Set CSharpFormattingOptions.MinimumBlankLinesBetweenTypes (Int32).  
Provide a numeric value.

```yaml
Type: Int32
Required: False
Position: Named
Accept pipeline input: False
```

### -MinimumBlankLinesInsideRegion

Set CSharpFormattingOptions.MinimumBlankLinesInsideRegion (Int32).  
Provide a numeric value.

```yaml
Type: Int32
Required: False
Position: Named
Accept pipeline input: False
```

### -Name

Set CSharpFormattingOptions.Name (String).  
Provide a string value.

```yaml
Type: String
Required: False
Position: Named
Accept pipeline input: False
```

### -NamespaceBraceStyle

Set CSharpFormattingOptions.NamespaceBraceStyle (BraceStyle).  
Accepted values: EndOfLine, EndOfLineWithoutSpace, NextLine, NextLineShifted, NextLineShifted2, BannerStyle.

```yaml
Type: BraceStyle
Required: False
Position: Named
Accept pipeline input: False
```

### -NewLineAferIndexerDeclarationOpenBracket

Set CSharpFormattingOptions.NewLineAferIndexerDeclarationOpenBracket (NewLinePlacement).  
Accepted values: DoNotCare, NewLine, SameLine.

```yaml
Type: NewLinePlacement
Required: False
Position: Named
Accept pipeline input: False
```

### -NewLineAferIndexerOpenBracket

Set CSharpFormattingOptions.NewLineAferIndexerOpenBracket (NewLinePlacement).  
Accepted values: DoNotCare, NewLine, SameLine.

```yaml
Type: NewLinePlacement
Required: False
Position: Named
Accept pipeline input: False
```

### -NewLineAferMethodCallOpenParentheses

Set CSharpFormattingOptions.NewLineAferMethodCallOpenParentheses (NewLinePlacement).  
Accepted values: DoNotCare, NewLine, SameLine.

```yaml
Type: NewLinePlacement
Required: False
Position: Named
Accept pipeline input: False
```

### -NewLineAferMethodDeclarationOpenParentheses

Set CSharpFormattingOptions.NewLineAferMethodDeclarationOpenParentheses (NewLinePlacement).  
Accepted values: DoNotCare, NewLine, SameLine.

```yaml
Type: NewLinePlacement
Required: False
Position: Named
Accept pipeline input: False
```

### -NewLineAfterConstructorInitializerColon

Set CSharpFormattingOptions.NewLineAfterConstructorInitializerColon (NewLinePlacement).  
Accepted values: DoNotCare, NewLine, SameLine.

```yaml
Type: NewLinePlacement
Required: False
Position: Named
Accept pipeline input: False
```

### -NewLineBeforeConstructorInitializerColon

Set CSharpFormattingOptions.NewLineBeforeConstructorInitializerColon (NewLinePlacement).  
Accepted values: DoNotCare, NewLine, SameLine.

```yaml
Type: NewLinePlacement
Required: False
Position: Named
Accept pipeline input: False
```

### -NewLineBeforeNewQueryClause

Set CSharpFormattingOptions.NewLineBeforeNewQueryClause (NewLinePlacement).  
Accepted values: DoNotCare, NewLine, SameLine.

```yaml
Type: NewLinePlacement
Required: False
Position: Named
Accept pipeline input: False
```

### -PropertyBraceStyle

Set CSharpFormattingOptions.PropertyBraceStyle (BraceStyle).  
Accepted values: EndOfLine, EndOfLineWithoutSpace, NextLine, NextLineShifted, NextLineShifted2, BannerStyle.

```yaml
Type: BraceStyle
Required: False
Position: Named
Accept pipeline input: False
```

### -PropertyGetBraceStyle

Set CSharpFormattingOptions.PropertyGetBraceStyle (BraceStyle).  
Accepted values: EndOfLine, EndOfLineWithoutSpace, NextLine, NextLineShifted, NextLineShifted2, BannerStyle.

```yaml
Type: BraceStyle
Required: False
Position: Named
Accept pipeline input: False
```

### -PropertySetBraceStyle

Set CSharpFormattingOptions.PropertySetBraceStyle (BraceStyle).  
Accepted values: EndOfLine, EndOfLineWithoutSpace, NextLine, NextLineShifted, NextLineShifted2, BannerStyle.

```yaml
Type: BraceStyle
Required: False
Position: Named
Accept pipeline input: False
```

### -RemoveEndOfLineWhiteSpace

Set CSharpFormattingOptions.RemoveEndOfLineWhiteSpace to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SimpleGetBlockFormatting

Set CSharpFormattingOptions.SimpleGetBlockFormatting (PropertyFormatting).  
Accepted values: SingleLine, MultipleLines.

```yaml
Type: PropertyFormatting
Required: False
Position: Named
Accept pipeline input: False
```

### -SimplePropertyFormatting

Set CSharpFormattingOptions.SimplePropertyFormatting (PropertyFormatting).  
Accepted values: SingleLine, MultipleLines.

```yaml
Type: PropertyFormatting
Required: False
Position: Named
Accept pipeline input: False
```

### -SimpleSetBlockFormatting

Set CSharpFormattingOptions.SimpleSetBlockFormatting (PropertyFormatting).  
Accepted values: SingleLine, MultipleLines.

```yaml
Type: PropertyFormatting
Required: False
Position: Named
Accept pipeline input: False
```

### -SpaceAfterBracketComma

Set CSharpFormattingOptions.SpaceAfterBracketComma to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpaceAfterConditionalOperatorCondition

Set CSharpFormattingOptions.SpaceAfterConditionalOperatorCondition to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpaceAfterConditionalOperatorSeparator

Set CSharpFormattingOptions.SpaceAfterConditionalOperatorSeparator to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpaceAfterConstructorDeclarationParameterComma

Set CSharpFormattingOptions.SpaceAfterConstructorDeclarationParameterComma to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpaceAfterDelegateDeclarationParameterComma

Set CSharpFormattingOptions.SpaceAfterDelegateDeclarationParameterComma to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpaceAfterFieldDeclarationComma

Set CSharpFormattingOptions.SpaceAfterFieldDeclarationComma to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpaceAfterForSemicolon

Set CSharpFormattingOptions.SpaceAfterForSemicolon to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpaceAfterIndexerDeclarationParameterComma

Set CSharpFormattingOptions.SpaceAfterIndexerDeclarationParameterComma to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpaceAfterLocalVariableDeclarationComma

Set CSharpFormattingOptions.SpaceAfterLocalVariableDeclarationComma to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpaceAfterMethodCallParameterComma

Set CSharpFormattingOptions.SpaceAfterMethodCallParameterComma to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpaceAfterMethodDeclarationParameterComma

Set CSharpFormattingOptions.SpaceAfterMethodDeclarationParameterComma to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpaceAfterNewParameterComma

Set CSharpFormattingOptions.SpaceAfterNewParameterComma to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpaceAfterTypecast

Set CSharpFormattingOptions.SpaceAfterTypecast to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpaceAfterUnsafeAddressOfOperator

Set CSharpFormattingOptions.SpaceAfterUnsafeAddressOfOperator to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpaceAfterUnsafeAsteriskOfOperator

Set CSharpFormattingOptions.SpaceAfterUnsafeAsteriskOfOperator to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpaceAroundAdditiveOperator

Set CSharpFormattingOptions.SpaceAroundAdditiveOperator to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpaceAroundAssignment

Set CSharpFormattingOptions.SpaceAroundAssignment to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpaceAroundBitwiseOperator

Set CSharpFormattingOptions.SpaceAroundBitwiseOperator to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpaceAroundEqualityOperator

Set CSharpFormattingOptions.SpaceAroundEqualityOperator to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpaceAroundLogicalOperator

Set CSharpFormattingOptions.SpaceAroundLogicalOperator to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpaceAroundMultiplicativeOperator

Set CSharpFormattingOptions.SpaceAroundMultiplicativeOperator to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpaceAroundNullCoalescingOperator

Set CSharpFormattingOptions.SpaceAroundNullCoalescingOperator to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpaceAroundRelationalOperator

Set CSharpFormattingOptions.SpaceAroundRelationalOperator to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpaceAroundShiftOperator

Set CSharpFormattingOptions.SpaceAroundShiftOperator to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpaceAroundUnsafeArrowOperator

Set CSharpFormattingOptions.SpaceAroundUnsafeArrowOperator to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpaceBeforeAnonymousMethodParentheses

Set CSharpFormattingOptions.SpaceBeforeAnonymousMethodParentheses to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpaceBeforeArrayDeclarationBrackets

Set CSharpFormattingOptions.SpaceBeforeArrayDeclarationBrackets to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpaceBeforeBracketComma

Set CSharpFormattingOptions.SpaceBeforeBracketComma to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpaceBeforeCatchParentheses

Set CSharpFormattingOptions.SpaceBeforeCatchParentheses to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpaceBeforeConditionalOperatorCondition

Set CSharpFormattingOptions.SpaceBeforeConditionalOperatorCondition to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpaceBeforeConditionalOperatorSeparator

Set CSharpFormattingOptions.SpaceBeforeConditionalOperatorSeparator to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpaceBeforeConstructorDeclarationParameterComma

Set CSharpFormattingOptions.SpaceBeforeConstructorDeclarationParameterComma to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpaceBeforeConstructorDeclarationParentheses

Set CSharpFormattingOptions.SpaceBeforeConstructorDeclarationParentheses to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpaceBeforeDelegateDeclarationParameterComma

Set CSharpFormattingOptions.SpaceBeforeDelegateDeclarationParameterComma to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpaceBeforeDelegateDeclarationParentheses

Set CSharpFormattingOptions.SpaceBeforeDelegateDeclarationParentheses to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpaceBeforeFieldDeclarationComma

Set CSharpFormattingOptions.SpaceBeforeFieldDeclarationComma to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpaceBeforeForeachParentheses

Set CSharpFormattingOptions.SpaceBeforeForeachParentheses to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpaceBeforeForParentheses

Set CSharpFormattingOptions.SpaceBeforeForParentheses to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpaceBeforeForSemicolon

Set CSharpFormattingOptions.SpaceBeforeForSemicolon to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpaceBeforeIfParentheses

Set CSharpFormattingOptions.SpaceBeforeIfParentheses to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpaceBeforeIndexerDeclarationBracket

Set CSharpFormattingOptions.SpaceBeforeIndexerDeclarationBracket to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpaceBeforeIndexerDeclarationParameterComma

Set CSharpFormattingOptions.SpaceBeforeIndexerDeclarationParameterComma to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpaceBeforeLocalVariableDeclarationComma

Set CSharpFormattingOptions.SpaceBeforeLocalVariableDeclarationComma to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpaceBeforeLockParentheses

Set CSharpFormattingOptions.SpaceBeforeLockParentheses to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpaceBeforeMethodCallParameterComma

Set CSharpFormattingOptions.SpaceBeforeMethodCallParameterComma to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpaceBeforeMethodCallParentheses

Set CSharpFormattingOptions.SpaceBeforeMethodCallParentheses to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpaceBeforeMethodDeclarationParameterComma

Set CSharpFormattingOptions.SpaceBeforeMethodDeclarationParameterComma to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpaceBeforeMethodDeclarationParentheses

Set CSharpFormattingOptions.SpaceBeforeMethodDeclarationParentheses to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpaceBeforeNewParameterComma

Set CSharpFormattingOptions.SpaceBeforeNewParameterComma to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpaceBeforeNewParentheses

Set CSharpFormattingOptions.SpaceBeforeNewParentheses to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpaceBeforeSemicolon

Set CSharpFormattingOptions.SpaceBeforeSemicolon to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpaceBeforeSizeOfParentheses

Set CSharpFormattingOptions.SpaceBeforeSizeOfParentheses to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpaceBeforeSwitchParentheses

Set CSharpFormattingOptions.SpaceBeforeSwitchParentheses to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpaceBeforeTypeOfParentheses

Set CSharpFormattingOptions.SpaceBeforeTypeOfParentheses to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpaceBeforeUsingParentheses

Set CSharpFormattingOptions.SpaceBeforeUsingParentheses to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpaceBeforeWhileParentheses

Set CSharpFormattingOptions.SpaceBeforeWhileParentheses to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpaceBetweenEmptyConstructorDeclarationParentheses

Set CSharpFormattingOptions.SpaceBetweenEmptyConstructorDeclarationParentheses to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpaceBetweenEmptyDelegateDeclarationParentheses

Set CSharpFormattingOptions.SpaceBetweenEmptyDelegateDeclarationParentheses to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpaceBetweenEmptyMethodCallParentheses

Set CSharpFormattingOptions.SpaceBetweenEmptyMethodCallParentheses to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpaceBetweenEmptyMethodDeclarationParentheses

Set CSharpFormattingOptions.SpaceBetweenEmptyMethodDeclarationParentheses to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpaceBetweenParameterAttributeSections

Set CSharpFormattingOptions.SpaceBetweenParameterAttributeSections to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpaceInNamedArgumentAfterDoubleColon

Set CSharpFormattingOptions.SpaceInNamedArgumentAfterDoubleColon to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpacesBeforeBrackets

Set CSharpFormattingOptions.SpacesBeforeBrackets to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpacesBetweenEmptyNewParentheses

Set CSharpFormattingOptions.SpacesBetweenEmptyNewParentheses to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpacesWithinBrackets

Set CSharpFormattingOptions.SpacesWithinBrackets to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpacesWithinCastParentheses

Set CSharpFormattingOptions.SpacesWithinCastParentheses to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpacesWithinCatchParentheses

Set CSharpFormattingOptions.SpacesWithinCatchParentheses to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpacesWithinCheckedExpressionParantheses

Set CSharpFormattingOptions.SpacesWithinCheckedExpressionParantheses to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpacesWithinForeachParentheses

Set CSharpFormattingOptions.SpacesWithinForeachParentheses to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpacesWithinForParentheses

Set CSharpFormattingOptions.SpacesWithinForParentheses to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpacesWithinIfParentheses

Set CSharpFormattingOptions.SpacesWithinIfParentheses to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpacesWithinLockParentheses

Set CSharpFormattingOptions.SpacesWithinLockParentheses to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpacesWithinNewParentheses

Set CSharpFormattingOptions.SpacesWithinNewParentheses to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpacesWithinParentheses

Set CSharpFormattingOptions.SpacesWithinParentheses to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpacesWithinSizeOfParentheses

Set CSharpFormattingOptions.SpacesWithinSizeOfParentheses to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpacesWithinSwitchParentheses

Set CSharpFormattingOptions.SpacesWithinSwitchParentheses to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpacesWithinTypeOfParentheses

Set CSharpFormattingOptions.SpacesWithinTypeOfParentheses to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpacesWithinUsingParentheses

Set CSharpFormattingOptions.SpacesWithinUsingParentheses to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpacesWithinWhileParentheses

Set CSharpFormattingOptions.SpacesWithinWhileParentheses to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpaceWithinAnonymousMethodParentheses

Set CSharpFormattingOptions.SpaceWithinAnonymousMethodParentheses to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpaceWithinConstructorDeclarationParentheses

Set CSharpFormattingOptions.SpaceWithinConstructorDeclarationParentheses to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpaceWithinDelegateDeclarationParentheses

Set CSharpFormattingOptions.SpaceWithinDelegateDeclarationParentheses to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpaceWithinIndexerDeclarationBracket

Set CSharpFormattingOptions.SpaceWithinIndexerDeclarationBracket to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpaceWithinMethodCallParentheses

Set CSharpFormattingOptions.SpaceWithinMethodCallParentheses to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SpaceWithinMethodDeclarationParentheses

Set CSharpFormattingOptions.SpaceWithinMethodDeclarationParentheses to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -StatementBraceStyle

Set CSharpFormattingOptions.StatementBraceStyle (BraceStyle).  
Accepted values: EndOfLine, EndOfLineWithoutSpace, NextLine, NextLineShifted, NextLineShifted2, BannerStyle.

```yaml
Type: BraceStyle
Required: False
Position: Named
Accept pipeline input: False
```

### -StructBraceStyle

Set CSharpFormattingOptions.StructBraceStyle (BraceStyle).  
Accepted values: EndOfLine, EndOfLineWithoutSpace, NextLine, NextLineShifted, NextLineShifted2, BannerStyle.

```yaml
Type: BraceStyle
Required: False
Position: Named
Accept pipeline input: False
```

### -UsingPlacement

Set CSharpFormattingOptions.UsingPlacement (UsingPlacement).  
Accepted values: TopOfFile, InsideNamespace.

```yaml
Type: UsingPlacement
Required: False
Position: Named
Accept pipeline input: False
```

### -WhileNewLinePlacement

Set CSharpFormattingOptions.WhileNewLinePlacement (NewLinePlacement).  
Accepted values: DoNotCare, NewLine, SameLine.

```yaml
Type: NewLinePlacement
Required: False
Position: Named
Accept pipeline input: False
```

## INPUTS

None.

## OUTPUTS

### ICSharpCode.Decompiler.CSharp.OutputVisitor.CSharpFormattingOptions

Returns a configured formatting options object.

## NOTES

- Parameter availability can change with ILSpy versions as formatting properties evolve.

## RELATED LINKS

[New-DecompilerSetting](New-DecompilerSetting.md)
[New-Decompiler](New-Decompiler.md)

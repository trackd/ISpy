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

Most writable `DecompilerSettings` properties are boolean toggles, so this cmdlet primarily surfaces switch-style dynamic parameters. For richer formatting controls (`int`, `string`, enums), use `New-DecompilerFormattingOption` and pass the result to `-CSharpFormattingOptions`.

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
PS C:\> $fmt = New-DecompilerFormattingOption -IndentSwitchBody
PS C:\> $settings = New-DecompilerSetting -CSharpFormattingOptions $fmt -UsingDeclarations
```

Creates settings that include custom C# formatting preferences.

## PARAMETERS

### -AggressiveInlining

Set DecompilerSettings.AggressiveInlining to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -AggressiveScalarReplacementOfAggregates

Set DecompilerSettings.AggressiveScalarReplacementOfAggregates to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -AlwaysCastTargetsOfExplicitInterfaceImplementationCalls

Set DecompilerSettings.AlwaysCastTargetsOfExplicitInterfaceImplementationCalls to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -AlwaysQualifyMemberReferences

Set DecompilerSettings.AlwaysQualifyMemberReferences to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -AlwaysShowEnumMemberValues

Set DecompilerSettings.AlwaysShowEnumMemberValues to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -AlwaysUseBraces

Set DecompilerSettings.AlwaysUseBraces to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -AlwaysUseGlobal

Set DecompilerSettings.AlwaysUseGlobal to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -AnonymousMethods

Set DecompilerSettings.AnonymousMethods to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -AnonymousTypes

Set DecompilerSettings.AnonymousTypes to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -ApplyWindowsRuntimeProjections

Set DecompilerSettings.ApplyWindowsRuntimeProjections to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -ArrayInitializers

Set DecompilerSettings.ArrayInitializers to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -AssumeArrayLengthFitsIntoInt32

Set DecompilerSettings.AssumeArrayLengthFitsIntoInt32 to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -AsyncAwait

Set DecompilerSettings.AsyncAwait to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -AsyncEnumerator

Set DecompilerSettings.AsyncEnumerator to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -AsyncUsingAndForEachStatement

Set DecompilerSettings.AsyncUsingAndForEachStatement to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -AutoLoadAssemblyReferences

Set DecompilerSettings.AutoLoadAssemblyReferences to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -AutomaticEvents

Set DecompilerSettings.AutomaticEvents to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -AutomaticProperties

Set DecompilerSettings.AutomaticProperties to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -AwaitInCatchFinally

Set DecompilerSettings.AwaitInCatchFinally to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -CheckedOperators

Set DecompilerSettings.CheckedOperators to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -CovariantReturns

Set DecompilerSettings.CovariantReturns to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -DecimalConstants

Set DecompilerSettings.DecimalConstants to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -DecompileMemberBodies

Set DecompilerSettings.DecompileMemberBodies to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -DecompilerFormattingOption

Use New-DecompilerFormattingOption to create a formatting options object with specific settings.

```yaml
Type: CSharpFormattingOptions
Aliases: CSharpFormattingOptions
Required: False
Position: Named
Accept pipeline input: False
```

### -Deconstruction

Set DecompilerSettings.Deconstruction to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -DictionaryInitializers

Set DecompilerSettings.DictionaryInitializers to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -Discards

Set DecompilerSettings.Discards to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -DoWhileStatement

Set DecompilerSettings.DoWhileStatement to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -Dynamic

Set DecompilerSettings.Dynamic to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -ExpandMemberDefinitions

Set DecompilerSettings.ExpandMemberDefinitions to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -ExpandUsingDeclarations

Set DecompilerSettings.ExpandUsingDeclarations to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -ExpressionTrees

Set DecompilerSettings.ExpressionTrees to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -ExtensionMethods

Set DecompilerSettings.ExtensionMethods to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -ExtensionMethodsInCollectionInitializers

Set DecompilerSettings.ExtensionMethodsInCollectionInitializers to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -FileScopedNamespaces

Set DecompilerSettings.FileScopedNamespaces to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -FixedBuffers

Set DecompilerSettings.FixedBuffers to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -FoldBraces

Set DecompilerSettings.FoldBraces to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -ForEachStatement

Set DecompilerSettings.ForEachStatement to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -ForEachWithGetEnumeratorExtension

Set DecompilerSettings.ForEachWithGetEnumeratorExtension to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -ForStatement

Set DecompilerSettings.ForStatement to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -FunctionPointers

Set DecompilerSettings.FunctionPointers to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -GetterOnlyAutomaticProperties

Set DecompilerSettings.GetterOnlyAutomaticProperties to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -InitAccessors

Set DecompilerSettings.InitAccessors to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -IntroduceIncrementAndDecrement

Set DecompilerSettings.IntroduceIncrementAndDecrement to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -IntroducePrivateProtectedAccessibility

Set DecompilerSettings.IntroducePrivateProtectedAccessibility to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -IntroduceReadonlyAndInModifiers

Set DecompilerSettings.IntroduceReadonlyAndInModifiers to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -IntroduceRefModifiersOnStructs

Set DecompilerSettings.IntroduceRefModifiersOnStructs to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -IntroduceUnmanagedConstraint

Set DecompilerSettings.IntroduceUnmanagedConstraint to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -LanguageVersion

C# Language version to be used by the decompiler  
Accepted values: CSharp1, CSharp2, CSharp3, CSharp4, CSharp5, CSharp6, CSharp7, CSharp7_1, CSharp7_2, CSharp7_3, CSharp8_0, CSharp9_0, CSharp10_0, CSharp11_0, Preview, CSharp12_0, Latest.

```yaml
Type: LanguageVersion
Required: False
Position: Named
Accept pipeline input: False
```

### -LifetimeAnnotations

Set DecompilerSettings.LifetimeAnnotations to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -LiftNullables

Set DecompilerSettings.LiftNullables to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -LoadInMemory

Set DecompilerSettings.LoadInMemory to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -LocalFunctions

Set DecompilerSettings.LocalFunctions to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -LockStatement

Set DecompilerSettings.LockStatement to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -MakeAssignmentExpressions

Set DecompilerSettings.MakeAssignmentExpressions to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -NamedArguments

Set DecompilerSettings.NamedArguments to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -NativeIntegers

Set DecompilerSettings.NativeIntegers to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -NonTrailingNamedArguments

Set DecompilerSettings.NonTrailingNamedArguments to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -NullableReferenceTypes

Set DecompilerSettings.NullableReferenceTypes to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -NullPropagation

Set DecompilerSettings.NullPropagation to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -NumericIntPtr

Set DecompilerSettings.NumericIntPtr to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -ObjectOrCollectionInitializers

Set DecompilerSettings.ObjectOrCollectionInitializers to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -OptionalArguments

Set DecompilerSettings.OptionalArguments to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -OutVariables

Set DecompilerSettings.OutVariables to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -PatternBasedFixedStatement

Set DecompilerSettings.PatternBasedFixedStatement to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -PatternCombinators

Set DecompilerSettings.PatternCombinators to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -PatternMatching

Set DecompilerSettings.PatternMatching to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -QueryExpressions

Set DecompilerSettings.QueryExpressions to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -Ranges

Set DecompilerSettings.Ranges to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -ReadOnlyMethods

Set DecompilerSettings.ReadOnlyMethods to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -RecordClasses

Set DecompilerSettings.RecordClasses to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -RecordStructs

Set DecompilerSettings.RecordStructs to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -RecursivePatternMatching

Set DecompilerSettings.RecursivePatternMatching to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -RefExtensionMethods

Set DecompilerSettings.RefExtensionMethods to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -RefReadOnlyParameters

Set DecompilerSettings.RefReadOnlyParameters to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -RelationalPatterns

Set DecompilerSettings.RelationalPatterns to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -RemoveDeadCode

Set DecompilerSettings.RemoveDeadCode to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -RemoveDeadStores

Set DecompilerSettings.RemoveDeadStores to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -RequiredMembers

Set DecompilerSettings.RequiredMembers to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -ScopedRef

Set DecompilerSettings.ScopedRef to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SeparateLocalVariableDeclarations

Set DecompilerSettings.SeparateLocalVariableDeclarations to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -ShowDebugInfo

Set DecompilerSettings.ShowDebugInfo to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -ShowXmlDocumentation

Set DecompilerSettings.ShowXmlDocumentation to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SortCustomAttributes

Set DecompilerSettings.SortCustomAttributes to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SparseIntegerSwitch

Set DecompilerSettings.SparseIntegerSwitch to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -StackAllocInitializers

Set DecompilerSettings.StackAllocInitializers to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -StaticLocalFunctions

Set DecompilerSettings.StaticLocalFunctions to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -StringConcat

Set DecompilerSettings.StringConcat to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -StringInterpolation

Set DecompilerSettings.StringInterpolation to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SwitchExpressions

Set DecompilerSettings.SwitchExpressions to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SwitchOnReadOnlySpanChar

Set DecompilerSettings.SwitchOnReadOnlySpanChar to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -SwitchStatementOnString

Set DecompilerSettings.SwitchStatementOnString to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -ThrowExpressions

Set DecompilerSettings.ThrowExpressions to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -ThrowOnAssemblyResolveErrors

Set DecompilerSettings.ThrowOnAssemblyResolveErrors to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -TupleComparisons

Set DecompilerSettings.TupleComparisons to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -TupleConversions

Set DecompilerSettings.TupleConversions to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -TupleTypes

Set DecompilerSettings.TupleTypes to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -UnsignedRightShift

Set DecompilerSettings.UnsignedRightShift to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -UseDebugSymbols

Set DecompilerSettings.UseDebugSymbols to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -UseEnhancedUsing

Set DecompilerSettings.UseEnhancedUsing to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -UseExpressionBodyForCalculatedGetterOnlyProperties

Set DecompilerSettings.UseExpressionBodyForCalculatedGetterOnlyProperties to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -UseImplicitMethodGroupConversion

Set DecompilerSettings.UseImplicitMethodGroupConversion to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -UseLambdaSyntax

Set DecompilerSettings.UseLambdaSyntax to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -UseNestedDirectoriesForNamespaces

Set DecompilerSettings.UseNestedDirectoriesForNamespaces to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -UsePrimaryConstructorSyntax

Set DecompilerSettings.UsePrimaryConstructorSyntax to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -UsePrimaryConstructorSyntaxForNonRecordTypes

Set DecompilerSettings.UsePrimaryConstructorSyntaxForNonRecordTypes to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -UseRefLocalsForAccurateOrderOfEvaluation

Set DecompilerSettings.UseRefLocalsForAccurateOrderOfEvaluation to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -UseSdkStyleProjectFormat

Set DecompilerSettings.UseSdkStyleProjectFormat to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -UsingDeclarations

Set DecompilerSettings.UsingDeclarations to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -UsingStatement

Set DecompilerSettings.UsingStatement to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -Utf8StringLiterals

Set DecompilerSettings.Utf8StringLiterals to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -WithExpressions

Set DecompilerSettings.WithExpressions to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
```

### -YieldReturn

Set DecompilerSettings.YieldReturn to $true.

```yaml
Type: SwitchParameter
Required: False
Position: Named
Accept pipeline input: False
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
[New-Decompiler](New-Decompiler.md)
[Get-DecompiledSource](Get-DecompiledSource.md)

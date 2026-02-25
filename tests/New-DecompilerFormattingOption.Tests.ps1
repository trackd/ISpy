BeforeAll {
    if (-not (Get-Module ISpy)) {
        Import-Module (Join-Path $PSScriptRoot '..' 'output' 'ISpy.psd1')
    }
}

Describe 'New-DecompilerFormattingOption cmdlet' {
    It 'New-DecompilerFormattingOption_Default_ReturnsFormattingOptions' {
        $options = New-DecompilerFormattingOption

        $options | Should -Not -BeNull
        $options.GetType().FullName | Should -Be 'ICSharpCode.Decompiler.CSharp.OutputVisitor.CSharpFormattingOptions'
    }

    It 'New-DecompilerFormattingOption_DynamicSwitch_SetsBooleanProperty' {
        $options = New-DecompilerFormattingOption -IndentSwitchBody

        $options.IndentSwitchBody | Should -BeTrue
    }

    It 'New-DecompilerFormattingOption_DynamicIntParameter_SetsValue' {
        $formattingType = [ICSharpCode.Decompiler.CSharp.OutputVisitor.CSharpFormattingOptions]
        $intProperty = $formattingType.GetProperties([System.Reflection.BindingFlags]'Public,Instance') |
            Where-Object { $_.CanWrite -and $null -ne $_.SetMethod -and $_.PropertyType -eq [int] } |
            Select-Object -First 1 -ExpandProperty Name

        $intProperty | Should -Not -BeNullOrEmpty
        $params = @{ $intProperty = 123 }
        $formattingOptions = New-DecompilerFormattingOption @params

        $formattingOptions.$intProperty | Should -Be 123
    }

    It 'New-DecompilerFormattingOption_DynamicEnumParameter_SetsValue' {
        $formattingType = [ICSharpCode.Decompiler.CSharp.OutputVisitor.CSharpFormattingOptions]
        $enumProperty = $formattingType.GetProperties([System.Reflection.BindingFlags]'Public,Instance') |
            Where-Object { $_.CanWrite -and $null -ne $_.SetMethod -and $_.PropertyType.IsEnum } |
            Select-Object -First 1

        $enumProperty | Should -Not -BeNull
        $enumValue = [System.Enum]::GetNames($enumProperty.PropertyType) | Select-Object -First 1
        $params = @{ $($enumProperty.Name) = $enumValue }
        $formattingOptions = New-DecompilerFormattingOption @params

        $formattingOptions.$($enumProperty.Name).ToString() | Should -Be $enumValue
    }

    It 'New-DecompilerFormattingOption_CanBeUsedForDecompilerSettings' {
        $options = New-DecompilerFormattingOption -IndentSwitchBody
        $settings = New-DecompilerSetting -CSharpFormattingOptions $options

        $settings.CSharpFormattingOptions.IndentSwitchBody | Should -BeTrue
    }

    It 'Should have Help and examples' {
        $help = Get-Help New-DecompilerFormattingOption -Full
        $help.Synopsis | Should -Not -BeNullOrEmpty
        $help.examples.example.Count | Should -BeGreaterThan 1
    }
}

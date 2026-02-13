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

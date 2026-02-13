BeforeAll {
    if (-not (Get-Module ISpy)) {
        Import-Module (Join-Path $PSScriptRoot '..' 'output' 'ISpy.psd1')
    }
}

Describe 'New-DecompilerSetting cmdlet' {
    It 'New-DecompilerSetting_Default_ReturnsSettings' {
        $settings = New-DecompilerSetting

        $settings | Should -Not -BeNull
        $settings.GetType().FullName | Should -Be 'ICSharpCode.Decompiler.DecompilerSettings'
    }

    It 'New-DecompilerSetting_DynamicSwitch_SetsBooleanProperty' {
        $settings = New-DecompilerSetting -RemoveDeadCode

        $settings.RemoveDeadCode | Should -BeTrue
    }

    It 'New-DecompilerSetting_AcceptsFormattingOptions' {
        $fmt = New-DecompilerFormattingOption
        $settings = New-DecompilerSetting -CSharpFormattingOptions $fmt

        $settings.CSharpFormattingOptions | Should -Not -BeNull
    }

    It 'Should have Help and examples' {
        $help = Get-Help New-DecompilerSetting -Full
        $help.Synopsis | Should -Not -BeNullOrEmpty
        $help.examples.example.Count | Should -BeGreaterThan 1
    }
}

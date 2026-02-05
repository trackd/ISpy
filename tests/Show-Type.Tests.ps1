BeforeAll {
    $ModulePath = "$PSScriptRoot\..\output\PwshIlSpy.psd1"
    Import-Module $ModulePath -Force
}

Describe "Show-Type cmdlet" {
    It "Should decompile method group and return string by default" {
        $output = [math]::Round | Show-Type
        $output | Should -Not -BeNullOrEmpty
        # default output is string source
        $output | Should -BeOfType System.String
        $output | Should -Match "Round"
    }

    It "Should emit DecompilationResult when EmitMetadata is used" {
        $result = [math]::Round | Show-Type -EmitMetadata
        $result | Should -Not -BeNull
        $result | Should -BeOfType PwshIlSpy.Models.DecompilationResult
        $result.MethodNames | Should -Contain "Round"
    }

    It "Should emit metadata only when MetadataOnly is used" {
        $result = [math]::Round | Show-Type -EmitMetadata -MetadataOnly
        $result | Should -Not -BeNull
        $result | Should -BeOfType PwshIlSpy.Models.DecompilationResult
        $result.Source | Should -BeNull
    }
}

BeforeAll {
    if (-not (Get-Module ISpy)) {
        Import-Module (Join-Path $PSScriptRoot '..' 'output' 'ISpy.psd1')
    }
    function helloworld {
        <#
        .DESCRIPTION
        A simple function that returns "Hello, World!" and a question.
        #>
        param($noop)
        'Hello, World!'
        'does this work?'
    }
}

Describe 'Expand-Type cmdlet' {
    It "Expand-Type_DefaultInput_ReturnsCSharpSource" {
        $output = [math]::Truncate | Expand-Type
        $output | Should -Not -BeNullOrEmpty
        $output | Should -BeOfType System.String
        $output | Should -Match "Truncate"
    }

    It "Expand-Type_MetadataSwitch_EmitsDecompilationResult" {
        $result = [math]::Truncate | Expand-Type -Metadata
        $result | Should -Not -BeNull
        $result | Should -BeOfType ISpy.Models.ISpyDecompilationResult
        $result.MethodNames | Should -Contain "Truncate"
    }
    It "Decompile CommandInfo" {
        $up = Get-Command New-Decompiler | Expand-Type
        $up | Should -Not -BeNull
        $up | Should -BeOfType System.String
        $up | Should -Match "NewDecompilerCmdlet"
        $up.PSChildName | Should -Be "NewDecompilerCmdlet.cs"
    }
    It "Decompile Locally Defined Function" {
        $f = Expand-Type 'helloworld'
        $f | Should -Not -BeNull
        $f | Should -BeOfType System.String
        $f | Should -Match "helloworld"
        $f.PSChildName | Should -Be "helloworld.ps1"
    }
    It "Resolves powershell alias and decompiles cmdlets" {
        $s = Expand-Type 'scb'
        $s | Should -Not -BeNull
        $s | Should -BeOfType System.String
        $s | Should -Match "SetClipboard"
        $s.PSChildName | Should -Be "SetClipboardCommand.cs"
    }
    It 'Should have Help and examples' {
        $help = Get-Help Expand-Type -Examples
        $help.Synopsis | Should -Not -BeNullOrEmpty
        $help.examples.example.Count | Should -BeGreaterThan 1
    }
}

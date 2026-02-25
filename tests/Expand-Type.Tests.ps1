BeforeAll {
    if (-not (Get-Module ISpy)) {
        Import-Module (Join-Path $PSScriptRoot '..' 'output' 'ISpy.psd1')
    }
    # assembly used for type-based pipeline tests
    $Script:TestAssembly = [System.Math].Assembly.Location
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

    It "Expand-Type_FromGetType_Pipeline_TypeInfo_Metadata" {
        $result = Get-Type -Path $Script:TestAssembly -NamePattern 'Math' |
            Where-Object { $_.Name -eq 'Math' } |
            Expand-Type -Metadata

        $result | Should -Not -BeNull
        $result | Should -BeOfType ISpy.Models.ISpyDecompilationResult
        $result.TypeName | Should -Match 'System.Math'
        $result.MethodNames | Should -Contain 'Truncate'
    }

    It "Expand-Type_FromDecompilationResult_Pipeline_ReturnsSource" {
        $dec = Get-DecompiledSource -Path $Script:TestAssembly -TypeName 'System.Math'
        $output = $dec | Expand-Type
        $output | Should -Not -BeNullOrEmpty
        $output | Should -BeOfType System.String
        $output | Should -Match 'Math'
    }

    It "Expand-Type_ExplicitParameters_Decompiles" {
        $output = Expand-Type -Path $Script:TestAssembly -TypeName 'System.Math'
        $output | Should -Not -BeNullOrEmpty
        $output | Should -BeOfType System.String
        $output | Should -Match 'Math'
    }

    It "Expand-Type_CustomDecompiler_UsesProvidedDecompiler" {
        $decompiler = New-Decompiler -Path $Script:TestAssembly
        $output = Expand-Type -Path $Script:TestAssembly -TypeName 'System.Math' -Decompiler $decompiler

        $output | Should -Not -BeNullOrEmpty
        $output | Should -BeOfType System.String
        $output | Should -Match 'Math'
    }

    It "Expand-Type_CustomSettings_UsesProvidedSettings" {
        $settings = New-DecompilerSetting -UsingDeclarations
        $output = Expand-Type -Path $Script:TestAssembly -TypeName 'System.Math' -Settings $settings

        $output | Should -Not -BeNullOrEmpty
        $output | Should -BeOfType System.String
        $output | Should -Match 'Math'
    }

    It "Expand-Type_CustomSettings_DoesNotPostProcessOutput" {
        $settings = New-DecompilerSetting -UsingDeclarations
        $output = [System.Management.Automation.Host.PSHostUserInterface]::GetFormatStyleString | Expand-Type -IncludeXml -Settings $settings

        $output | Should -Not -BeNullOrEmpty
        $output | Should -BeOfType System.String
        $output | Should -Match 'GetFormatStyleString'
        $output | Should -Not -Match '(?m)^\s*//\s*System\.Management\.Automation\.Host\.PSHostUserInterface'
    }

    It "Expand-Type_TypeNameWithoutPath_ResolvesLoadedType" {
        $output = Expand-Type -TypeName 'System.Math'
        $output | Should -Not -BeNullOrEmpty
        $output | Should -BeOfType System.String
        $output | Should -Match 'Math'
    }

    It "Expand-Type_Default_OmitsXmlComments" {
        $output = [System.Management.Automation.Host.PSHostUserInterface]::GetFormatStyleString | Expand-Type
        $output | Should -Not -BeNullOrEmpty
        $output | Should -BeOfType System.String
        $output | Should -Not -Match '///\s*<summary>'
    }

    It "Expand-Type_IncludeXml_IncludesXmlAndRemovesUsingDirectives" {
        $output = [System.Management.Automation.Host.PSHostUserInterface]::GetFormatStyleString | Expand-Type -IncludeXml
        $output | Should -Not -BeNullOrEmpty
        $output | Should -BeOfType System.String
        $output | Should -Match '///\s*<summary>'
        $output | Should -Not -Match '(?m)^\s*using\s+'
        $output | Should -Not -Match 'System\.Management\.Automation\.PSStyle'
        $output | Should -Not -Match 'System\.Management\.Automation\.OutputRendering'
        $output | Should -Match '\bPSStyle\b'
        $output | Should -Match '\bOutputRendering\b'
    }
}

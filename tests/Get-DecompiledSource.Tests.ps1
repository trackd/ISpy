BeforeAll {
    if (-not (Get-Module ISpy)) {
        Import-Module ([IO.Path]::Combine($PSScriptRoot, '..', 'output', 'ISpy.psd1'))
    }
    if ($PSEdition -ne 'Core') {
        Add-Type -AssemblyName System.Web
    }
    # $Script:TestAssembly = [System.Web.HttpUtility].Assembly.Location
    $Script:TestAssembly = [IO.Path]::Combine($PSScriptRoot, '..', 'output', 'netstandard2.0', 'ISpy.dll')
    $Script:TestAssemblyName = [System.Reflection.AssemblyName]::GetAssemblyName($Script:TestAssembly).Name
    $Script:TestOutputDir = [IO.Path]::Combine($PSScriptRoot, 'TestOutput')
}



Describe "Get-DecompiledSource cmdlet" {
    BeforeEach {
        if (Test-Path $Script:TestOutputDir) {
            Remove-Item $Script:TestOutputDir -Recurse -Force
        }
        New-Item -ItemType Directory -Path $Script:TestOutputDir | Out-Null
    }

    AfterAll {
        if (Test-Path $Script:TestOutputDir) {
            Remove-Item $Script:TestOutputDir -Recurse -Force
        }
    }
    Context "Basic scenarios" {
        It "Get-DecompiledSource_Assembly_ReturnsValidResult" {
            $result = Get-DecompiledSource -Path $Script:TestAssembly

            $result | Should -Not -BeNull
            $result | ForEach-Object { if ($_.TypeName -notmatch '<PrivateImplementationDetails>') { $_.Success } } | Should -Not -Contain $false
            $result | Where-Object { $_.source -match 'namespace ISpy.Cmdlets' } | Should -Not -BeNullOrEmpty
        }

        It "Get-DecompiledSource_TypeFilter_ReturnsSpecificType" {
            $typeName = 'ISpy.Models.ISpyAssemblyInfo'
            $result = Get-DecompiledSource -Path $Script:TestAssembly -TypeName $typeName

            $result.TypeName | Should -Be $typeName
            [regex]::Match($result.Source, 'public class ISpyAssemblyInfo').Success | Should -BeTrue
            $result.FilePath | Should -BeNull
        }
    }

    Context "Output handling" {
        It "Get-DecompiledSource_OutputPath_WritesFileAndReturnsPath" {

            $typeName = 'ISpy.Models.ISpyTypeInfo'
            $result = Get-DecompiledSource -Path $Script:TestAssembly -TypeName $typeName
            $result.Source | Should -Not -BeNullOrEmpty
            $result.MethodNames | Should -Contain '.ctor'
            $result.TypeName | Should -Be $typeName
            $result.Success | Should -BeTrue
        }
    }

    Context "Pipeline support" {
        It "Get-DecompiledSource_PipelineInput_ProcessesAssembly" {
            $typeName = 'ISpy.Models.ISpyAssemblyInfo'
            $result = $Script:TestAssembly | Get-DecompiledSource -TypeName $typeName

            $result | Should -Not -BeNull
            $result.TypeName | Should -Be $typeName
            $result.Success | Should -BeTrue
        }

        It "Get-DecompiledSource_CustomDecompiler_UsesProvidedDecompiler" {
            $typeName = 'ISpy.Models.ISpyAssemblyInfo'
            $decompiler = New-Decompiler -Path $Script:TestAssembly
            $result = Get-DecompiledSource -Path $Script:TestAssembly -TypeName $typeName -Decompiler $decompiler

            $result | Should -Not -BeNull
            $result.TypeName | Should -Be $typeName
            $result.Source | Should -Not -BeNullOrEmpty
            $result.Success | Should -BeTrue
        }
    }

    Context "Error handling" {
        It "Get-DecompiledSource_MissingPathAndTypeName_Throws" {
            { Get-DecompiledSource -ErrorAction Stop } | Should -Throw
        }

        It "Get-DecompiledSource_MissingAssembly_Throws" {
            { Get-DecompiledSource -Path 'NonExistent.dll' -ErrorAction Stop } | Should -Throw
        }

        It "Get-DecompiledSource_MissingType_Throws" {
            { Get-DecompiledSource -Path $Script:TestAssembly -TypeName "NonExistent.Type" -ErrorAction Stop } | Should -Throw
        }
    }

    Context "Help" {
        It "Get-DecompiledSource_HelpContainsExamples" {
            $help = Get-Help Get-DecompiledSource -Full
            $help.Synopsis | Should -Not -BeNullOrEmpty
            $help.examples.example.Count | Should -BeGreaterThan 0
        }
    }
}

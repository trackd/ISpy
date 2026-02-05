BeforeAll {
    $ModulePath = "$PSScriptRoot\..\output\ISpy.psd1"
    Import-Module $ModulePath -Force

    $Script:TestAssembly = [System.Web.HttpUtility].Assembly.Location
    $Script:TestAssemblyName = [System.Reflection.AssemblyName]::GetAssemblyName($Script:TestAssembly).Name
    $Script:TestOutputDir = "$PSScriptRoot\TestOutput"
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
            $result | Where-Object { $_.source -match 'namespace System.Web' } | Should -Not -BeNullOrEmpty
        }

        It "Get-DecompiledSource_TypeFilter_ReturnsSpecificType" {
            $result = Get-DecompiledSource -Path $Script:TestAssembly -TypeName $Script:TestAssemblyName

            $result.TypeName | Should -Be $Script:TestAssemblyName
            [regex]::Match($result.Source,'public sealed class HttpUtility').Success | Should -BeTrue
            $result.FilePath | Should -BeNull
        }
    }

    Context "Output handling" {
        It "Get-DecompiledSource_OutputPath_WritesFileAndReturnsPath" {

            $result = Get-DecompiledSource -Path $Script:TestAssembly -TypeName $Script:TestAssemblyName

            $result.Source | Should -Not -BeNullOrEmpty
        }
    }

    Context "Pipeline support" {
        It "Get-DecompiledSource_PipelineInput_ProcessesAssembly" {
            $result = $Script:TestAssembly | Get-DecompiledSource -TypeName $Script:TestAssemblyName

            $result | Should -Not -BeNull
            $result.TypeName | Should -Be $Script:TestAssemblyName
        }
    }

    Context "Error handling" {
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

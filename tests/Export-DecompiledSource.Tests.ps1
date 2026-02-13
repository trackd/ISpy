BeforeAll {
    if (-not (Get-Module ISpy)) {
        Import-Module (Join-Path $PSScriptRoot '..' 'output' 'ISpy.psd1')
    }
    # Use a small local assembly (ICSharpCode.Decompiler) to keep decompilation fast
    $Script:TestAssembly = "$PSScriptRoot\..\output\lib\ICSharpCode.Decompiler.dll"
    $Script:TestAssemblyName = [System.Reflection.AssemblyName]::GetAssemblyName($Script:TestAssembly).Name
    $Script:TestOutputDir = "$PSScriptRoot\TestOutput"
    $script:typeNames = @('ISpy.Models.ISpyAssemblyInfo', 'ISpy.Models.ISpyTypeInfo')

}

AfterAll {
    if (Test-Path $Script:TestOutputDir) {
        Remove-Item $Script:TestOutputDir -Recurse -Force
    }
}

Describe 'ExportDecompiledSource cmdlet' {
    BeforeEach {
        if (Test-Path $Script:TestOutputDir) {
            Remove-Item $Script:TestOutputDir -Recurse -Force
        }
        New-Item -ItemType Directory -Path $Script:TestOutputDir | Out-Null
    }

    Context "Basic export" {
        It "Export-DecompiledSource_TypeNames_ProducesFiles" {
            $typeName1 = 'ICSharpCode.Decompiler.CSharp.CSharpDecompiler'

            $result = Export-DecompiledSource -Path $script:TestAssembly -OutputPath $Script:TestOutputDir -TypeNames $typeName1

            $result | Should -Not -BeNull
            $count = ($result | Where-Object { $_.Success -eq $true }).Count
            $count | Should -BeGreaterThan 0
            (Get-ChildItem $Script:TestOutputDir -Filter '*.cs' -Recurse -File).Count | Should -Be $count
        }
    }

    Context "Filtering and directories" {
        It "Export-DecompiledSource_Namespace_CreatesSubdirectories" {
            Export-DecompiledSource -Path $script:TestAssembly -OutputPath $Script:TestOutputDir -Namespace 'ICSharpCode.Decompiler' -CreateNamespaceDirectories

            $namespaceDir = Join-Path $Script:TestOutputDir 'ICSharpCode\Decompiler'
            Test-Path $namespaceDir | Should -BeTrue
        }

        It "Export-DecompiledSource_Force_OverwritesFiles" {
            $typeName1 = 'ICSharpCode.Decompiler.CSharp.CSharpDecompiler'

            $first = Export-DecompiledSource -Path $script:TestAssembly -OutputPath $Script:TestOutputDir -TypeNames $typeName1
            $firstCount = ($first | Where-Object { $_.Success -eq $true }).Count
            $firstCount | Should -BeGreaterThan 0
            $firstFiles = Get-ChildItem $Script:TestOutputDir -Filter '*.cs' -File -Recurse
            $firstFiles.Count | Should -Be $firstCount

            $secondResult = Export-DecompiledSource -Path $script:TestAssembly -OutputPath $Script:TestOutputDir -TypeNames $typeName1 -Force
            $secondCount = ($secondResult | Where-Object { $_.Success -eq $true }).Count
            $secondCount | Should -Be $firstCount
            $secondFiles = Get-ChildItem $Script:TestOutputDir -Filter '*.cs' -File -Recurse
            $secondFiles.Count | Should -Be $secondCount

        }

        It "Export-DecompiledSource_CustomDecompiler_UsesProvidedDecompiler" {
            $typeName1 = 'ICSharpCode.Decompiler.CSharp.CSharpDecompiler'
            $decompiler = New-Decompiler -Path $Script:TestAssembly

            $result = Export-DecompiledSource -Path $script:TestAssembly -OutputPath $Script:TestOutputDir -TypeNames $typeName1 -Decompiler $decompiler

            $result | Should -Not -BeNull
            ($result | Where-Object Success -eq $true).Count | Should -BeGreaterThan 0
        }
    }

    Context "Help" {
        It "Export-DecompiledSource_HelpProvidesExamples" {
            $help = Get-Help Export-DecompiledSource -Full
            $help.Synopsis | Should -Not -BeNullOrEmpty
            $help.examples.example.Count | Should -BeGreaterThan 1
        }
    }
}

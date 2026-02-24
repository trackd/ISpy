BeforeAll {
    if (-not (Get-Module ISpy)) {
        Import-Module (Join-Path $PSScriptRoot '..' 'output' 'ISpy.psd1')
    }
    $Script:TestAssembly = [System.Web.HttpUtility].Assembly.Location
    $Script:TestAssemblyName = [System.Reflection.AssemblyName]::GetAssemblyName($Script:TestAssembly).Name

}

Describe "Get-Type cmdlet" {
    It "Get-Type_PublicOnly_ReturnsOnlyPublicTypes" {
        $results = Get-Type -Path $Script:TestAssembly -PublicOnly -Namespace 'System.Web'

        $results | Should -Not -BeNullOrEmpty
        $results | ForEach-Object { $_.IsPublic | Should -BeTrue }
    }

    It "Get-Type_TypeKindInterfaces_ReturnsInterfaces" {
        $results = Get-Type -Path $Script:TestAssembly -TypeKind 'Interface'

        $results | Should -Not -BeNullOrEmpty
        $results | ForEach-Object { $_.Kind | Should -Be 'Interface' }
    }

    It "Get-Type_NamePattern_FilterMatches" {
        $results = Get-Type -Path $Script:TestAssembly -NamePattern '*Web*'

        $results | Should -Not -BeNullOrEmpty
        $results | ForEach-Object { $_.Namespace | Should -Match 'Web' }
    }

    It "Get-Type_PipelineInput_ProcessesAssembly" {
        $results = $Script:TestAssembly | Get-Type -PublicOnly -TypeKind 'Class'

        $results | Should -Not -BeNullOrEmpty
    }

    It "Get-Type_CustomDecompiler_UsesProvidedDecompiler" {
        $decompiler = New-Decompiler -Path $Script:TestAssembly
        $results = Get-Type -Path $Script:TestAssembly -Decompiler $decompiler -PublicOnly -TypeKind 'Class'

        $results | Should -Not -BeNullOrEmpty
    }

    It 'Should have Help and examples' {
        $help = Get-Help Get-Type -Full
        $help.Synopsis | Should -Not -BeNullOrEmpty
        $help.examples.example.Count | Should -BeGreaterThan 1
    }

    It "Get-Type_PipesTo_ExpandType_MetadataBinding_Works" {
        $result = Get-Type -Path $Script:TestAssembly -NamePattern '*HttpUtility*' |
            Where-Object { $_.Name -match 'HttpUtility' } |
            Expand-Type -Metadata

        $result | Should -Not -BeNullOrEmpty
        $result | ForEach-Object { $_ | Should -BeOfType ISpy.Models.ISpyDecompilationResult }
        ($result | Select-Object -ExpandProperty TypeName) | Should -Contain 'System.Web.HttpUtility'
    }
}

BeforeAll {
    if (-not (Get-Module ISpy)) {
        Import-Module (Join-Path $PSScriptRoot '..' 'output' 'ISpy.psd1')
    }
    $Script:TestAssembly = [System.Web.HttpUtility].Assembly.Location
    $Script:TestAssemblyName = [System.Reflection.AssemblyName]::GetAssemblyName($Script:TestAssembly).Name
}

Describe "Get-Dependency cmdlet" {
    It "Get-Dependency_MissingPathAndTypeName_Throws" {
        { Get-Dependency -ErrorAction Stop } | Should -Throw
    }

    It "Get-Dependency_Default_ReturnsReferencedAssemblies" {
        $results = Get-Dependency -Path $Script:TestAssembly

        $results | Should -Not -BeNullOrEmpty
        $results | ForEach-Object { $_.FullName | Should -Match 'Version=' }
    }

    It "Get-Dependency_ExternalOnly_SkipsSelfReference" {
        $all = Get-Dependency -Path $Script:TestAssembly
        $external = Get-Dependency -Path $Script:TestAssembly -ExternalOnly

        $external.Count | Should -BeLessOrEqual $all.Count
        ($external | Where-Object { $_.Name -eq $Script:TestAssemblyName }) | Should -BeNullOrEmpty
    }

    It "Get-Dependency_TypeNameOnly_ResolvesLoadedAssembly" {
        $results = Get-Dependency -TypeName 'System.Web.HttpUtility'

        $results | Should -Not -BeNullOrEmpty
        $results | ForEach-Object { $_.FullName | Should -Match 'Version=' }
    }

    It 'Should have Help and examples' {
        $help = Get-Help Get-Dependency -Full
        $help.Synopsis | Should -Not -BeNullOrEmpty
        $help.examples.example.Count | Should -BeGreaterThan 1
    }
}

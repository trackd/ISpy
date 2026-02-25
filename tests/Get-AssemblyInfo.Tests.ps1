BeforeAll {
    if (-not (Get-Module ISpy)) {
        Import-Module (Join-Path $PSScriptRoot '..' 'output' 'ISpy.psd1')
    }
    $Script:TestAssembly = [System.Web.HttpUtility].Assembly.Location
    $Script:TestAssemblyName = [System.Reflection.AssemblyName]::GetAssemblyName($Script:TestAssembly).Name
}

Describe "Get-AssemblyInfo cmdlet" {
    It "Get-AssemblyInfo_MissingPathAndTypeName_Throws" {
        { Get-AssemblyInfo -ErrorAction Stop } | Should -Throw
    }

    It "Get-AssemblyInfo_GivenPath_ReturnsMetadata" {
        $result = Get-AssemblyInfo -Path $Script:TestAssembly

        $result | Should -Not -BeNull
        $result.FullName | Should -Not -BeNullOrEmpty
        $result.FilePath | Should -Be $Script:TestAssembly
        $result.Types | Should -BeGreaterThan 0
    }

    It "Get-AssemblyInfo_PipelineInput_ReturnsSameFilePath" {
        $result = $Script:TestAssembly | Get-AssemblyInfo

        $result.FilePath | Should -Be $Script:TestAssembly
    }

    It "Get-AssemblyInfo_TypeNameOnly_ResolvesLoadedAssembly" {
        $result = Get-AssemblyInfo -TypeName 'System.Web.HttpUtility'

        $result | Should -Not -BeNull
        $result.FilePath | Should -Be $Script:TestAssembly
        $result.Name | Should -Be $Script:TestAssemblyName
    }

    It "Should have Help and examples" {
        $help = Get-Help Get-AssemblyInfo -Full
        $help.Synopsis | Should -Not -BeNullOrEmpty
        $help.examples.example.Count | Should -BeGreaterThan 0
    }
}

BeforeAll {
    $ModulePath = "$PSScriptRoot\..\output\ISpy.psd1"
    Import-Module $ModulePath -Force

    $Script:TestAssembly = [System.Web.HttpUtility].Assembly.Location
    $Script:TestAssemblyName = [System.Reflection.AssemblyName]::GetAssemblyName($Script:TestAssembly).Name
}

Describe "Get-AssemblyInfo cmdlet" {
    It "Get-AssemblyInfo_GivenPath_ReturnsMetadata" {
        $result = Get-AssemblyInfo -Path $Script:TestAssembly

        $result | Should -Not -BeNull
        $result.FullName | Should -Not -BeNullOrEmpty
        $result.FilePath | Should -Be $Script:TestAssembly
        $result.TypeCount | Should -BeGreaterThan 0
    }

    It "Get-AssemblyInfo_PipelineInput_ReturnsSameFilePath" {
        $result = $Script:TestAssembly | Get-AssemblyInfo

        $result.FilePath | Should -Be $Script:TestAssembly
    }
    It "Should have Help and examples" {
        $help = Get-Help Get-AssemblyInfo -Full
        $help.Synopsis | Should -Not -BeNullOrEmpty
        $help.examples.example.Count | Should -BeGreaterThan 0
    }
}

BeforeAll {
	$ModulePath = "$PSScriptRoot\..\output\ISpy.psd1"
	Import-Module $ModulePath -Force

	# Use a small local assembly for fast decompiler creation
	$Script:TestAssembly = "$PSScriptRoot\..\output\lib\ICSharpCode.Decompiler.dll"
}

Describe 'Get-Decompiler cmdlet' {
	It 'Get-Decompiler_GivenPath_ReturnsDecompilerInstance' {
		$decompiler = Get-Decompiler -Path $Script:TestAssembly

		$decompiler | Should -Not -BeNull
		$decompiler.GetType().FullName | Should -Match 'CSharpDecompiler'
	}

	It 'Get-Decompiler_InvalidPath_Throws' {
		{ Get-Decompiler -Path 'NonExistent-DoesNotExist.dll' -ErrorAction Stop } | Should -Throw
	}
    It "Should have Help and examples" {
        $help = Get-Help Get-Decompiler -Full
        $help.Synopsis | Should -Not -BeNullOrEmpty
        $help.examples.example.Count | Should -BeGreaterThan 1
    }
}

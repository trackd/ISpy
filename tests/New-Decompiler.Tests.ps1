BeforeAll {
	if (-not (Get-Module ISpy)) {
		Import-Module (Join-Path $PSScriptRoot '..' 'output' 'ISpy.psd1')
	}
	# Use a small local assembly for fast decompiler creation
	$Script:TestAssembly = "$PSScriptRoot\..\output\lib\ICSharpCode.Decompiler.dll"
}

Describe 'New-Decompiler cmdlet' {
	It 'New-Decompiler_GivenPath_ReturnsDecompilerInstance' {
		$decompiler = New-Decompiler -Path $Script:TestAssembly

		$decompiler | Should -Not -BeNull
		$decompiler.GetType().FullName | Should -Match 'CSharpDecompiler'
	}

	It 'New-Decompiler_InvalidPath_Throws' {
		{ New-Decompiler -Path 'NonExistent-DoesNotExist.dll' -ErrorAction Stop } | Should -Throw
	}
    It "Should have Help and examples" {
        $help = Get-Help New-Decompiler -Full
        $help.Synopsis | Should -Not -BeNullOrEmpty
        $help.examples.example.Count | Should -BeGreaterThan 1
    }
}

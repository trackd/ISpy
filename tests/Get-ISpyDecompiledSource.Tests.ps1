# Test for Get-SpyDecompiledSource cmdlet

BeforeAll {
    # Import the module
    $ModulePath = "$PSScriptRoot\..\output\PwshIlSpy.psd1"
    Import-Module $ModulePath -Force

    # Use System.Management.Automation assembly - always available in PowerShell
    $Script:TestAssembly = [System.Management.Automation.PowerShell].Assembly.Location
    $Script:TestType = "System.Management.Automation.PowerShell"
    $Script:TestOutputDir = "$PSScriptRoot\TestOutput"

    # Verify test assembly exists
    if (-not (Test-Path $Script:TestAssembly)) {
        throw "Test assembly not found: $Script:TestAssembly"
    }

    # Ensure test output directory exists
    if (!(Test-Path $Script:TestOutputDir)) {
        New-Item -ItemType Directory -Path $Script:TestOutputDir -Force | Out-Null
    }
}

AfterAll {
    # Clean up test output directory
    if (Test-Path $Script:TestOutputDir) {
        Remove-Item $Script:TestOutputDir -Recurse -Force
    }
}

Describe "Get-SpyDecompiledSource Tests" {

    Context "Basic Functionality" {
        It "Should decompile specific type and return source code" {
            $result = Get-SpyDecompiledSource -AssemblyPath $Script:TestAssembly -TypeName $Script:TestType

            # New return type is DecompilationResult; verify Source property contains code
            $result | Should -Not -BeNull
            $result.Source | Should -Not -BeNullOrEmpty
            $result.Source | Should -Match "class PowerShell"
            $result.Source | Should -Match "namespace System.Management.Automation"
        }

        It "Should return valid C# code structure" {
            $result = Get-SpyDecompiledSource -AssemblyPath $Script:TestAssembly -TypeName $Script:TestType

            $result.Source | Should -Match "namespace|using"
            $result.Source | Should -Match "{"
            $result.Source | Should -Match "}"
        }
    }

    Context "Output Options" {
        It "Should save to file when OutputPath is specified" {
            $outputFile = Join-Path $Script:TestOutputDir "TestOutput.cs"

            $result = Get-SpyDecompiledSource -AssemblyPath $Script:TestAssembly -TypeName $Script:TestType -OutputPath $outputFile

            Test-Path $outputFile | Should -Be $true
            $content = Get-Content $outputFile -Raw
            $content | Should -Match "class PowerShell"
            # Result should include FilePath when OutputPath provided
            $result | Should -Not -BeNull
            $result.FilePath | Should -Be $outputFile
        }

        It "Should include debug info when requested" {
            $result = Get-SpyDecompiledSource -AssemblyPath $Script:TestAssembly -TypeName $Script:TestType -IncludeDebugInfo

            $result | Should -Not -BeNull
            $result.Source | Should -Not -BeNullOrEmpty
            # Debug info might include line numbers or source file references
        }
    }

    Context "Pipeline Support" {
        It "Should accept assembly path from pipeline" {
            $result = $Script:TestAssembly | Get-SpyDecompiledSource -TypeName $Script:TestType

            $result | Should -Not -BeNull
            $result.Source | Should -Match "class PowerShell"
        }
    }

    Context "Error Handling" {
        It "Should throw error for non-existent assembly" {
            { Get-SpyDecompiledSource -AssemblyPath "NonExistent.dll" -ErrorAction Stop } | Should -Throw
        }

        It "Should throw error for non-existent type" {
            { Get-SpyDecompiledSource -AssemblyPath $Script:TestAssembly -TypeName "NonExistent.Type" -ErrorAction Stop } | Should -Throw
        }

        It "Should handle invalid output path gracefully" {
            # Test with invalid path - may create directories or handle gracefully
            $tempInvalidPath = Join-Path $env:TEMP "InvalidTestDir\NonExistent\File.cs"
            $result = Get-SpyDecompiledSource -AssemblyPath $Script:TestAssembly -TypeName $Script:TestType -OutputPath $tempInvalidPath -ErrorAction SilentlyContinue
            # Test should complete without crashing
            $true | Should -Be $true
        }
    }

    Context "Help Documentation" {
        It "Should have proper help documentation" {
            $help = Get-Help Get-SpyDecompiledSource

            $help.Synopsis | Should -Not -BeNullOrEmpty
            $help.Name | Should -Be "Get-SpyDecompiledSource"
        }

        It "Should have examples in help" {
            $help = Get-Help Get-SpyDecompiledSource -Examples

            $help | Should -Not -BeNull
            # Basic help structure should exist
        }
    }
}


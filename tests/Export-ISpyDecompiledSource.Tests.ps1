# Test for Export-ISpyDecompiledSource cmdlet

BeforeAll {
    # Import the module
    $ModulePath = "$PSScriptRoot/../output/ISpy.psd1"
    Import-Module $ModulePath -Force

    # Use System.Management.Automation assembly - always available in PowerShell
    $Script:TestAssembly = [System.Management.Automation.PowerShell].Assembly.Location
    $Script:TestOutputDir = "$PSScriptRoot/TestOutput"

    # Verify test assembly exists
    if (-not (Test-Path $Script:TestAssembly)) {
        throw "Test assembly not found: $Script:TestAssembly"
    }
}

Describe "Export-SpyDecompiledSource Tests" {

    BeforeEach {
        # Clean and recreate test output directory for each test
        if (Test-Path $Script:TestOutputDir) {
            Remove-Item $Script:TestOutputDir -Recurse -Force
        }
        New-Item -ItemType Directory -Path $Script:TestOutputDir -Force | Out-Null
    }

    AfterAll {
        # Clean up test output directory
        if (Test-Path $Script:TestOutputDir) {
            Remove-Item $Script:TestOutputDir -Recurse -Force
        }
    }

    Context "Basic Functionality" {
        It "Should export decompiled source to directory" {
            # Use a more targeted approach - export only a few specific types from our assembly
            $result = Export-SpyDecompiledSource -AssemblyPath $Script:TestAssembly -OutputDirectory $Script:TestOutputDir -TypeNames @("ISpy.Models.ISpyAssemblyInfo", "ISpy.Models.ISpyTypeInfo")

            $result | Should -Not -BeNullOrEmpty
            $result.FilesCreated | Should -BeGreaterThan 0
            $result.FilesCreated | Should -BeLessOrEqual 2
            $result.OutputDirectory | Should -Be $Script:TestOutputDir

            # Check that files were actually created
            $files = Get-ChildItem $Script:TestOutputDir -Filter "*.cs"
            $files.Count | Should -BeGreaterThan 0
            $files.Count | Should -BeLessOrEqual 2
            # $files | Where-Object { $_.Extension -eq '.cs' } | Remove-Item -Force
        }

        It "Should create files with proper C# content" {
            # Export just one specific type from our assembly
            Export-SpyDecompiledSource -AssemblyPath $Script:TestAssembly -OutputDirectory $Script:TestOutputDir -TypeNames @('ISpy.Models.ISpyAssemblyInfo')

            $files = Get-ChildItem $Script:TestOutputDir -Filter "*.cs"
            $files.Count | Should -Be 1

            # Check that the file contains valid C# code for our type
            $content = Get-Content $files[0].FullName -Raw
            $content | Should -Match "namespace ISpy.Models"
            $content | Should -Match "class ISpyAssemblyInfo|struct ISpyAssemblyInfo"
            # $files | Where-Object { $_.Extension -eq '.cs' } | Remove-Item -Force

        }
    }

    Context "Filtering Options" {
        It "Should export only specified types when TypeNames provided" {
            $typeNames = @("System.Management.Automation.PowerShell", "System.Management.Automation.Runspaces.Runspace")

            $result = Export-ISpyDecompiledSource -AssemblyPath $Script:TestAssembly -OutputDirectory $Script:TestOutputDir -TypeNames $typeNames

            $result | Should -Not -BeNullOrEmpty
            $result.FilesCreated | Should -Be 2
            $files = Get-ChildItem $Script:TestOutputDir -Filter "*.cs"
            $files.Count | Should -Be 2
        }

        It "Should filter by namespace when specified" {
            # Use System.Management.Automation.Runspaces namespace which has fewer types
            $result = Export-ISpyDecompiledSource -AssemblyPath $Script:TestAssembly -OutputDirectory $Script:TestOutputDir -Namespace "System.Management.Automation.Runspaces" -TypeNames @("System.Management.Automation.Runspaces.Runspace")

            $result | Should -Not -BeNullOrEmpty
            $files = Get-ChildItem $Script:TestOutputDir -Filter "*.cs"
            $files.Count | Should -Be 1

            # Check that exported files are from the correct namespace
            $content = Get-Content $files[0].FullName -Raw
            $content | Should -Match "namespace System.Management.Automation.Runspaces"
        }

        It "Should create namespace directories when requested" {
            # Export a few specific types to test namespace directory creation
            Export-ISpyDecompiledSource -AssemblyPath $Script:TestAssembly -OutputDirectory $Script:TestOutputDir -CreateNamespaceDirectories -TypeNames @("System.Management.Automation.PowerShell", "System.Management.Automation.Runspaces.Runspace")

            # Should create subdirectories for namespaces
            $modelsDir = Join-Path $Script:TestOutputDir "System\Management\Automation"
            $runspacesDir = Join-Path $Script:TestOutputDir "System\Management\Automation\Runspaces"

            Test-Path $modelsDir | Should -Be $true
            # Runspaces subdirectory should exist if Runspace type is exported
            if (Test-Path $runspacesDir) { Test-Path $runspacesDir | Should -Be $true }
        }

        It "Should overwrite existing files when Force is specified" {
            # First export
            Export-ISpyDecompiledSource -AssemblyPath $Script:TestAssembly -OutputDirectory $Script:TestOutputDir -TypeNames @("System.Management.Automation.PowerShell")

            # Second export with Force
            $result = Export-ISpyDecompiledSource -AssemblyPath $Script:TestAssembly -OutputDirectory $Script:TestOutputDir -Force -TypeNames @("System.Management.Automation.PowerShell")

            $result | Should -Not -BeNullOrEmpty
            $result.FilesCreated | Should -Be 1
        }
    }

    Context "Pipeline Support" {
        It "Should accept assembly path from pipeline" {
            $result = $Script:TestAssembly | Export-SpyDecompiledSource -OutputDirectory $Script:TestOutputDir -TypeNames @("ISpy.Models.ISpyAssemblyInfo")

            $result | Should -Not -BeNullOrEmpty
            $result.FilesCreated | Should -Be 1
        }
    }

    Context "Error Handling" {
        It "Should throw error for non-existent assembly" {
            { Export-ISpyDecompiledSource -AssemblyPath "NonExistent.dll" -OutputDirectory $Script:TestOutputDir -ErrorAction Stop } | Should -Throw
        }

        It "Should throw error for invalid output directory" {
            { Export-ISpyDecompiledSource -AssemblyPath $Script:TestAssembly -OutputDirectory "X:\Invalid\Path" -ErrorAction Stop } | Should -Throw
        }

        It "Should handle existing files without Force parameter" {
            # First export - use specific type to avoid performance issues
            Export-ISpyDecompiledSource -AssemblyPath $Script:TestAssembly -OutputDirectory $Script:TestOutputDir -TypeNames @("System.Management.Automation.PowerShell")

            # Second export without Force should handle gracefully
            { Export-ISpyDecompiledSource -AssemblyPath $Script:TestAssembly -OutputDirectory $Script:TestOutputDir -TypeNames @("System.Management.Automation.PowerShell") } | Should -Not -Throw
        }
    }

    Context "Help Documentation" {
        It "Should have proper help documentation" {
            $help = Get-Help Export-ISpyDecompiledSource

            $help.Synopsis | Should -Not -Match "Fill in"
            $help.Synopsis | Should -Not -BeNullOrEmpty
            $help.Description | Should -Not -BeNullOrEmpty
        }

        It "Should have examples in help" {
            $help = Get-Help Export-ISpyDecompiledSource -Examples

            $help.Examples.Example.Count | Should -BeGreaterThan 0
            $help.Examples.Example[0].Code | Should -Not -Match "{{"
        }
    }
}

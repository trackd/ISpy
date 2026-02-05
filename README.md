# ISpy

A PowerShell module for decompiling .NET assemblies using the ILSpy decompiler engine.  

## Overview

ISpy provides comprehensive cmdlets to decompile .NET assemblies into readable C# source code. Built on top of the ICSharpCode.Decompiler library (the engine behind ILSpy), it offers a powerful command-line interface for .NET assembly analysis and decompilation tasks.  

### Key Features

- **Complete Assembly Decompilation**: Decompile entire .NET assemblies to organized C# source code
- **Targeted Type Analysis**: Focus on specific types, methods, or namespaces  
- **Cross-Platform Support**: Works with .NET Framework, .NET Core, and .NET 5+ assemblies
- **Flexible Output Options**: Console output, single files, or organized directory structures
- **Advanced Search Capabilities**: Find types and methods across multiple assemblies
- **Pipeline Integration**: Full PowerShell pipeline support for batch operations
- **Dependency Analysis**: Analyze assembly dependencies and relationships

## Installation

### Prerequisites

- **PowerShell**: 7.4

### Building from Source

1. Clone this repository
2. Open a terminal in the project directory
3. Build the project:

   ```cmd
   dotnet build
   ```

4. Import the module:

   ```powershell
   Import-Module .\output\PwshISpy.psd1
   ```

## Quick Start

### Basic Assembly Analysis

```powershell
# Get assembly information
Get-ISpyAssemblyInfo -AssemblyPath "MyLibrary.dll"

# List all public types
Get-ISpyTypes -AssemblyPath "MyLibrary.dll" -PublicOnly

# Find specific types across assemblies
Find-ISpyTypes -Pattern "*Controller*" -Directory "C:\MyApp\bin" -Recurse
```

### Decompiling Source Code

```powershell
# Decompile entire assembly
Get-ISpyDecompiledSource -AssemblyPath "MyLibrary.dll"

# Decompile specific class and save to file
Get-ISpyDecompiledSource -AssemblyPath "MyLibrary.dll" -TypeName "MyNamespace.MyClass" -OutputPath "MyClass.cs"

# Export entire assembly with organized structure
Export-ISpyDecompiledSource -AssemblyPath "MyLibrary.dll" -OutputDirectory ".\Decompiled" -CreateNamespaceDirectories
```

### Practical Examples

```powershell
# Analyze PowerShell cmdlet source code
Get-ISpyDecompiledSource -AssemblyPath "Microsoft.PowerShell.Commands.Management.dll" -TypeName "Microsoft.PowerShell.Commands.GetProcessCommand"

# Find all dependencies of an assembly
Get-ISpyDependencies -AssemblyPath "MyApp.dll" -IncludeVersionDetails

# Search for methods in a type
Get-ISpyMethods -AssemblyPath "MyLibrary.dll" -TypeName "MyClass" -NamePattern "Get*" -PublicOnly
```

## Available Cmdlets

PwshISpy provides nine comprehensive cmdlets for analyzing and decompiling .NET assemblies:

| Cmdlet | Purpose | Documentation |
|--------|---------|---------------|
| **Get-ISpyDecompiledSource** | Core decompilation functionality | [📖 Details](docs/en-us/Get-ISpyDecompiledSource.md) |
| **Get-ISpyAssemblyInfo** | Assembly metadata and information | [📖 Details](docs/en-us/Get-ISpyAssemblyInfo.md) |
| **Get-ISpyTypes** | List and filter types within assemblies | [📖 Details](docs/en-us/Get-ISpyTypes.md) |
| **Get-ISpyMethods** | Analyze methods, properties, and constructors | [📖 Details](docs/en-us/Get-ISpyMethods.md) |
| **Get-ISpyDependencies** | Assembly dependency mapping | [📖 Details](docs/en-us/Get-ISpyDependencies.md) |
| **Export-ISpyDecompiledSource** | Bulk export with organization options | [📖 Details](docs/en-us/Export-ISpyDecompiledSource.md) |
| **Find-ISpyTypes** | Cross-assembly type search | [📖 Details](docs/en-us/Find-ISpyTypes.md) |
| **Get-ISpyDecompiler** | Create configured decompiler instances | [📖 Details](docs/en-us/Get-ISpyDecompiler.md) |
| **Get-ISpyDecompilerVersion** | Get decompiler engine version | [📖 Details](docs/en-us/Get-ISpyDecompilerVersion.md) |

For comprehensive examples and detailed parameter information, see the individual cmdlet documentation files.  

## Common Use Cases

### 🔍 **Reverse Engineering and Learning**

Learn how .NET libraries and frameworks work by examining their implementation.  

### 🛡️ **Security Analysis**

Analyze assemblies for security vulnerabilities or understand malicious code.  

### 📚 **Legacy Code Migration**

Extract source code from legacy assemblies when original source is unavailable.  

### 🔎 **API Discovery**

Explore third-party libraries to understand available functionality.  

### 🐛 **Debugging and Troubleshooting**

Understand why assemblies fail to load or behave unexpectedly.  

## Development

### Project Structure

```text
PwshISpy/
├── src/PwshISpy/          # Source code
│   ├── Cmdlets/            # PowerShell cmdlet implementations
│   ├── Models/             # Data transfer objects
│   └── Utilities/          # Helper classes
├── tests/                  # Pester test suite
├── docs/en-us/            # PowerShell help documentation
├── output/                # Built module package
└── Build-Module.ps1       # Build script
```

## Dependencies

- **ICSharpCode.Decompiler**: Core decompilation engine from ISpy

## Contributing

1. Fork the repository
2. Create a feature branch
3. Follow the coding standards in `.github/copilot-instructions.md`
4. Add tests for new functionality
5. Update documentation as needed
6. Submit a pull request

## License

This project follows the same license terms as the ISpy project it's based on.  

## Related Projects

- [ISpy](https://github.com/icsharpcode/ISpy) - The original .NET decompiler GUI application
- [ICSharpCode.Decompiler](https://www.nuget.org/packages/ICSharpCode.Decompiler/) - The decompiler engine NuGet package

---

**Note**: This module is designed for legitimate reverse engineering, learning, and analysis purposes. Please respect intellectual property rights and licensing terms when decompiling third-party assemblies.

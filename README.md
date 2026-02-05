# ISpy

A PowerShell module for decompiling .NET assemblies using the ILSpy decompiler engine.  

## Overview

ISpy module provides comprehensive cmdlets to decompile .NET assemblies into readable C# source code. Built on top of the ICSharpCode.Decompiler library (the engine behind ILSpy), it offers a powerful command-line interface for .NET assembly analysis and decompilation tasks.  

### Key Features

- **Complete Assembly Decompilation**: Decompile entire .NET assemblies to organized C# source code
- **Targeted Type Analysis**: Focus on specific types, methods, or namespaces  
- **Cross-Platform Support**: Works with .NET Framework, .NET Core, and .NET 5+ assemblies
- **Flexible Output Options**: Console output, single files, or organized directory structures
- **Advanced Search Capabilities**: Find types and methods across multiple assemblies
- **Pipeline Integration**: Full PowerShell pipeline support for batch operations
- **Dependency Analysis**: Analyze assembly dependencies and relationships

## Installation

```powershell
Install-Module ISpy
```

### Prerequisites

- **PowerShell**: 7.4

### Building from Source

1. Clone this repository
2. Open a terminal in the project directory
3. Build the project:

```powershell
& .\build.ps1
```

1. Import the module:

```powershell
Import-Module .\output\ISpy.psd1
```

## Cmdlets

This module exposes the following cmdlets for assembly analysis and decompilation:

| Cmdlet | Purpose | Documentation |
|--------|---------|---------------|
| **Get-DecompiledSource** | Decompile types (returns an object per type) | [📖 Details](docs/en-us/Get-DecompiledSource.md) |
| **Export-DecompiledSource** | Export decompiled types to files with namespace organization | [📖 Details](docs/en-us/Export-DecompiledSource.md) |
| **Get-AssemblyInfo** | Assembly metadata and information | [📖 Details](docs/en-us/Get-AssemblyInfo.md) |
| **Get-Type** | List and filter types within assemblies | [📖 Details](docs/en-us/Get-Type.md) |
| **Expand-Type** | Decompile specific methods or show type source (interactive) | [📖 Details](docs/en-us/Expand-Type.md) |
| **Get-Dependency** | Assembly dependency mapping | [📖 Details](docs/en-us/Get-Dependency.md) |
| **Get-Decompiler** | Create configured `CSharpDecompiler` instances | [📖 Details](docs/en-us/Get-Decompiler.md) |

see  `docs/en-us/` for examples.

## Development

### Project Structure

```text
ISpy/
├── src/                    # Source code
│   ├── Cmdlets/            # PowerShell cmdlet implementations
│   ├── Models/             # Data transfer objects
│   └── Utilities/          # Helper classes
├── tests/                  # Pester test suite
├── docs/en-us/             # PowerShell help documentation
├── output/                 # Built module package
└── build.ps1               # Build script
```

## Dependencies

- **ICSharpCode.Decompiler**: Core decompilation engine from [ILSpy](https://github.com/icsharpcode/ILSpy)

## Contributing

1. Fork the repository
2. Create a feature branch
3. Add tests for new functionality
4. Update documentation as needed
5. Submit a pull request

## License

This project follows the same license terms as the ISpy project it's based on.  

## Libraries

- [ILSpy](https://github.com/icsharpcode/ILSpy) - The original .NET decompiler GUI application
- [ICSharpCode.Decompiler](https://www.nuget.org/packages/ICSharpCode.Decompiler/) - The decompiler engine NuGet package

---

**Note**: This module is designed for legitimate reverse engineering, learning, and analysis purposes. Please respect intellectual property rights and licensing terms when decompiling third-party assemblies.

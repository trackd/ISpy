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
- **Syntax highlighting**: PowerShell Module [TextMate](https://github.com/trackd/TextMate), pipe `Expand-Type` output to `Format-TextMate`

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

| Cmdlet | Purpose |
| -------- | --------- |
| [Expand-Type](docs/en-us/Expand-Type.md) | Decompile specific methods or show type source (interactive) |
| [Export-DecompiledSource](docs/en-us/Export-DecompiledSource.md) | Export decompiled types to files with namespace organization |
| [Get-AssemblyInfo](docs/en-us/Get-AssemblyInfo.md) | Assembly metadata and information |
| [Get-DecompiledSource](docs/en-us/Get-DecompiledSource.md) | Decompile types (returns an object per type) |
| [Get-Dependency](docs/en-us/Get-Dependency.md) | Assembly dependency mapping |
| [Get-Type](docs/en-us/Get-Type.md) | List and filter types within assemblies |
| [New-Decompiler](docs/en-us/New-Decompiler.md) | Create configured `CSharpDecompiler` instances |
| [New-DecompilerSetting](docs/en-us/New-DecompilerSetting.md) | Creates a configurable `DecompilerSettings` |
| [New-DecompilerFormattingOption](docs/en-us/New-DecompilerFormattingOption.md) | Creates a configurable `CSharpFormattingOptions` |

## Examples

### Quick Start: list a few types

```powershell
Get-Type -Path (Join-Path $PSHOME 'System.Console.dll')
```

### Find types by name pattern

```powershell
Get-Type -Path (Join-Path $PSHOME 'System.Console.dll') -NamePattern "*Console*" 
```

### Show assembly metadata

```powershell
Get-AssemblyInfo -Path (Join-Path $PSHOME 'System.Console.dll')
```

### Show external dependencies

```powershell
Get-Dependency -Path (Join-Path $PSHOME 'System.Console.dll') -ExternalOnly
```

### Preview one type's decompiled source object

```powershell
Get-DecompiledSource -Path (Join-Path $PSHOME 'System.Console.dll') | Select-Object -First 1
```

### Expand just one method (small, focused output)

```powershell
Expand-Type -Path (Join-Path $PSHOME 'System.Console.dll') -TypeName 'System.Console' -MethodName 'WriteLine' | Select-Object -First 3
```

### Custom decompilersettings / formatting

```powershell
# custom decompiler + settings + formatting
$formatSplat = @{
    ClassBraceStyle = 'NextLine'
    IndentationString = '  '
    MethodBraceStyle = 'NextLine'
    NewLineAferIndexerOpenBracket = 'NewLine'
    ChainedMethodCallWrapping = 'WrapAlways'
}
$decompilersettingsplat = @{
    AlwaysUseBraces = $true
    CSharpFormattingOptions = New-DecompilerFormattingOption @formatSplat
}
$decompilerSplat = @{ 
    Path = Join-Path $PSHOME 'System.Console.dll' 
    DecompilerSettings = New-DecompilerSetting @decompilersettingsplat
}
$decompiler = New-Decompiler @decompilerSplat
Expand-Type -Decompiler $decompiler -TypeName 'System.Console' -MethodName 'Write'
```

```powershell
# settings + formatting
$formatSplat = @{
    # Stroustrup-ish formatting
    ClassBraceStyle               = 'EndOfLine'
    IndentationString             = '  '
    MethodBraceStyle              = 'EndOfLine'
    NewLineAferIndexerOpenBracket = 'NewLine'
    StatementBraceStyle           = 'EndOfLine'
    ChainedMethodCallWrapping     = 'WrapAlways'
    InterfaceBraceStyle           = 'EndOfLine'
    StructBraceStyle              = 'EndOfLine'
    EnumBraceStyle                = 'EndOfLine'
    ConstructorBraceStyle         = 'EndOfLine'
}
$options = @{
    FileScopedNamespaces    = $true
    AlwaysUseBraces         = $true
    CSharpFormattingOptions = New-DecompilerFormattingOption @formatSplat
}
$Settings = New-DecompilerSetting @options
Expand-Type -Settings $Settings -TypeName 'System.Console' -MethodName 'Write'
```

see  `docs/en-us/` for more examples.

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

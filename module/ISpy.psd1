@{
    # Module manifest for ISpy
    RootModule             = 'lib\ISpy.dll'
    ModuleVersion          = '0.1.0'
    GUID                   = '15ad5935-21a2-4eed-aeec-2232c7507dc2'
    Author                 = 'trackd'
    CompanyName            = 'trackd'
    Copyright              = '(c) trackd. All rights reserved.'
    Description            = 'PowerShell module for decompiling .NET assemblies using ILSpy decompiler. Provides comprehensive cmdlets for assembly analysis, type discovery, method examination, and source code decompilation with advanced filtering and export capabilities.'
    PowerShellVersion      = '7.4'
    # DotNetFrameworkVersion = '4.6.1'
    # CLRVersion             = '4.0'
    # ProcessorArchitecture  = 'None'
    RequiredModules        = @()
    RequiredAssemblies     = @()
    ScriptsToProcess       = @()
    TypesToProcess         = @()
    FormatsToProcess       = @()
    NestedModules          = @()
    FunctionsToExport      = @()
    CmdletsToExport        = @(
        'Get-DecompiledSource'
        'Get-AssemblyInfo'
        'Get-Type'
        'Get-Method'
        'Get-Dependency'
        'Get-Decompiler'
        'Export-DecompiledSource'
        'Find-Type'
        'Show-Type'
    )
    VariablesToExport      = @()
    AliasesToExport        = @()
    DscResourcesToExport   = @()
    ModuleList             = @()
    FileList               = @()
    PrivateData            = @{
        PSData = @{
            Tags                       = @('ILSpy', 'Decompiler', 'c#', 'dotnet', 'Assembly', 'Analysis')
            LicenseUri                 = ''
            ProjectUri                 = 'https://github.com/trackd/PwshIlSpy'
            IconUri                    = ''
            ReleaseNotes               = ''
            ExternalModuleDependencies = @()
            Prerelease                 = ''
        }
    }
    HelpInfoURI            = ''
    DefaultCommandPrefix   = 'Spy'
}

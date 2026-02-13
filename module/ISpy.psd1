@{
    # Module manifest for ISpy
    RootModule             = 'lib/ISpy.dll'
    ModuleVersion          = '0.2.0'
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
    FormatsToProcess       = @('ISpy.Format.ps1xml')
    NestedModules          = @()
    FunctionsToExport      = @()
    CmdletsToExport        = @(
        'Expand-Type'
        'Export-DecompiledSource'
        'Get-AssemblyInfo'
        'Get-DecompiledSource'
        'Get-Dependency'
        'Get-Type'
        'New-Decompiler'
        'New-DecompilerFormattingOption'
        'New-DecompilerSetting'
    )
    VariablesToExport      = @()
    AliasesToExport        = @(
        'ent'
        'gds'
        'dep'
        'epds'
        'gai'
    )
    DscResourcesToExport   = @()
    ModuleList             = @()
    FileList               = @()
    PrivateData            = @{
        PSData = @{
            Tags                       = @('ILSpy', 'Decompiler', 'c#', 'dotnet', 'Assembly', 'Analysis')
            LicenseUri                 = 'https://github.com/trackd/ISpy/blob/main/LICENSE'
            ProjectUri                 = 'https://github.com/trackd/ISpy'
            # IconUri                    = ''
            # ReleaseNotes               = ''
            # ExternalModuleDependencies = @()
            # Prerelease                 = ''
        }
    }
    HelpInfoURI            = 'https://github.com/trackd/ISpy/tree/main/docs/en-us'
}

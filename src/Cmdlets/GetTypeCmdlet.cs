namespace ISpy.Cmdlets;

[Cmdlet(VerbsCommon.Get, "Type")]
[OutputType(typeof(ISpyTypeInfo))]
public class GetTypeCmdlet : PSCmdlet {
    [Parameter(
        Mandatory = true,
        Position = 0,
        ValueFromPipeline = true,
        ValueFromPipelineByPropertyName = true,
        HelpMessage = "Path to the assembly file to analyze"
    )]
    [Alias("AssemblyPath", "PSPath", "FilePath")]
    [ValidateNotNullOrEmpty]
    public string? Path { get; set; }

    [Parameter(HelpMessage = "Filter types by namespace")]
    public string? Namespace { get; set; }

    [Parameter(HelpMessage = "Filter types by name pattern (supports PowerShell wildcards)")]
    public string? NamePattern { get; set; }

    [Parameter(HelpMessage = "Only return public types")]
    public SwitchParameter PublicOnly { get; set; }

    [Parameter(HelpMessage = "Filter by type kinds (c=class, i=interface, s=struct, e=enum, d=delegate)")]
    public TypeKind[]? Typekind { get; set; }

    [Parameter(HelpMessage = "Include compiler-generated types whose names begin with '<'.")]
    public SwitchParameter IncludeCompilerGenerated { get; set; }

    [Parameter(HelpMessage = "Custom decompiler settings to influence type resolution")]
    public DecompilerSettings? Settings { get; set; }

    [Parameter(HelpMessage = "Custom CSharpDecompiler instance to use instead of creating one from path/settings")]
    public CSharpDecompiler? Decompiler { get; set; }

    protected override void ProcessRecord() {
        string? resolvedPath = ResolveAssemblyPath(Path);
        if (resolvedPath is null)
            return;

        try {
            WriteVerbose($"Enumerating types from: {resolvedPath}");

            CSharpDecompiler decompiler = Decompiler ?? DecompilerFactory.Create(resolvedPath, Settings ?? new DecompilerSettings {
                ThrowOnAssemblyResolveErrors = false,
                UseDebugSymbols = false,
                ShowDebugInfo = false,
                UsingDeclarations = true,
            });
            WildcardPattern? nameMatcher = BuildNameMatcher();

            foreach (ITypeDefinition? type in FilterTypes(decompiler.TypeSystem.MainModule.TypeDefinitions, Typekind, nameMatcher)) {
                WriteObject(CreateTypeInfo(type));
            }
        }
        catch (PipelineStoppedException) {
            // Pipeline was stopped by downstream cmdlet (e.g., Select-Object -First)
            // This is normal behavior, just rethrow to let PowerShell handle it
            throw;
        }
        catch (Exception ex) {
            WriteError(new ErrorRecord(
                ex,
                "TypeEnumerationFailed",
                ErrorCategory.InvalidOperation,
                resolvedPath)
            );
        }
    }
    private WildcardPattern? BuildNameMatcher()
        => string.IsNullOrWhiteSpace(NamePattern)
            ? null
            : new WildcardPattern(NamePattern, WildcardOptions.IgnoreCase);

    private IEnumerable<ITypeDefinition> FilterTypes(IEnumerable<ITypeDefinition> candidates, TypeKind[]? kinds, WildcardPattern? matcher) {
        foreach (ITypeDefinition type in candidates) {
            if (!IncludeCompilerGenerated.IsPresent && IsCompilerGenerated(type))
                continue;

            if (PublicOnly.IsPresent && type.Accessibility != Accessibility.Public)
                continue;

            if (!string.IsNullOrEmpty(Namespace) && !string.Equals(Namespace, type.Namespace, StringComparison.OrdinalIgnoreCase))
                continue;

            if (matcher?.IsMatch(type.Name) == false && !matcher.IsMatch(type.FullName))
                continue;

            if (kinds?.Contains(type.Kind) == false)
                continue;

            yield return type;
        }
    }

    private static ISpyTypeInfo CreateTypeInfo(ITypeDefinition type) {
        return new ISpyTypeInfo {
            FullName = type.FullName,
            Name = type.Name,
            Namespace = type.Namespace,
            Kind = type.Kind,
            IsPublic = type.Accessibility == Accessibility.Public,
            IsAbstract = type.IsAbstract,
            IsSealed = type.IsSealed,
            IsInterface = type.Kind == TypeKind.Interface,
            IsEnum = type.Kind == TypeKind.Enum,
            IsClass = type.Kind == TypeKind.Class,
            IsValueType = type.IsReferenceType is false,
            IsCompilerGenerated = IsCompilerGenerated(type),
            BaseType = type.DirectBaseTypes.FirstOrDefault()?.FullName
        };
    }

    private static bool IsCompilerGenerated(ITypeDefinition type)
        => type.Name.Length > 0 && type.Name[0] == '<';

    private string? ResolveAssemblyPath(string? candidate) {
        if (string.IsNullOrEmpty(candidate)) {
            WriteError(new ErrorRecord(
                new ArgumentException("Assembly path must be provided", nameof(candidate)),
                "MissingAssemblyPath",
                ErrorCategory.InvalidArgument,
                candidate));
            return null;
        }

        string resolved = GetUnresolvedProviderPathFromPSPath(candidate);
        if (!File.Exists(resolved)) {
            WriteError(new ErrorRecord(
                new FileNotFoundException($"Assembly file not found: {resolved}"),
                "AssemblyNotFound",
                ErrorCategory.InvalidArgument,
                resolved));
            return null;
        }

        return resolved;
    }
}

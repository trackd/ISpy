namespace ISpy.Cmdlets;

[Cmdlet(VerbsCommon.Get, "Type", DefaultParameterSetName = "ByPath")]
[OutputType(typeof(ISpyTypeInfo))]
public class GetTypeCmdlet : PSCmdlet {
    [Parameter(
        Mandatory = true,
        Position = 0,
        ValueFromPipelineByPropertyName = true,
        HelpMessage = "Path to the assembly file to analyze",
        ParameterSetName = "ByPath"
    )]
    [Alias("AssemblyPath", "PSPath", "FilePath")]
    [ValidateNotNullOrEmpty]
    public string? Path { get; set; }

    [Parameter(
        Mandatory = false,
        Position = 0,
        ValueFromPipeline = true,
        HelpMessage = "Pipeline input that can be an assembly path string or runtime type",
        ParameterSetName = "ByInputObject"
    )]
    public object? InputObject { get; set; }

    [Parameter(
        Mandatory = true,
        Position = 0,
        HelpMessage = "Type name to resolve from currently loaded AppDomain assemblies",
        ParameterSetName = "ByTypeName"
    )]
    [ArgumentCompleter(typeof(LoadedTypeNameCompleter))]
    public string? TypeName { get; set; }

    [Parameter(HelpMessage = "Filter types by namespace")]
    [ArgumentCompleter(typeof(LoadedNamespaceCompleter))]
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
        try {
            WildcardPattern? nameMatcher = BuildNameMatcher();
            var criteria = new TypeSearchCriteria(
                Namespace,
                nameMatcher,
                PublicOnly.IsPresent,
                IncludeCompilerGenerated.IsPresent,
                Typekind);

            switch (ParameterSetName) {
                case "ByInputObject":
                    if (InputObject is null)
                        return;

                    object pipelineValue = InputObject is PSObject psObject && psObject.BaseObject is not null
                        ? psObject.BaseObject
                        : InputObject;

                    if (pipelineValue is Type runtimeType) {
                        if (criteria.Matches(runtimeType))
                            WriteObject(LoadedTypeResolver.CreateTypeInfo(runtimeType));
                        return;
                    }

                    if (pipelineValue is string pipedPath) {
                        string? resolvedInputPath = ResolveAssemblyPath(pipedPath);
                        if (resolvedInputPath is null)
                            return;

                        EnumerateTypesFromAssembly(resolvedInputPath, criteria);
                    }

                    return;

                case "ByTypeName": {
                        bool any = false;
                        foreach (Type type in LoadedTypeResolver.FindLoadedTypesByName(TypeName ?? string.Empty)) {
                            if (!criteria.Matches(type))
                                continue;

                            WriteObject(LoadedTypeResolver.CreateTypeInfo(type));
                            any = true;
                        }

                        if (!any) {
                            WriteError(new ErrorRecord(
                                new ArgumentException($"Loaded type not found: {TypeName}"),
                                "LoadedTypeNotFound",
                                ErrorCategory.ObjectNotFound,
                                TypeName));
                        }

                        return;
                    }

                case "ByPath":
                default:
                    break;
            }

            string? resolvedPath = ResolveAssemblyPath(Path);
            if (resolvedPath is null)
                return;

            EnumerateTypesFromAssembly(resolvedPath, criteria);
        }
        catch (PipelineStoppedException) {
            throw;
        }
        catch (Exception ex) {
            WriteError(new ErrorRecord(
                ex,
                "TypeEnumerationFailed",
                ErrorCategory.InvalidOperation,
                Path ?? (object?)TypeName ?? InputObject)
            );
        }
    }

    private void EnumerateTypesFromAssembly(string resolvedPath, TypeSearchCriteria criteria) {
        WriteVerbose($"Enumerating types from: {resolvedPath}");

        CSharpDecompiler decompiler;
        if (Decompiler is not null) {
            decompiler = Decompiler;
        }
        else if (Settings is not null) {
            decompiler = DecompilerFactory.Create(resolvedPath, Settings);
        }
        else {
            decompiler = ILSpyDecompiler.CreateDecompiler(resolvedPath, useUsingDeclarations: true, showXmlDocumentation: Settings?.ShowXmlDocumentation ?? false);
        }

        foreach (ITypeDefinition type in FilterTypes(decompiler.TypeSystem.MainModule.TypeDefinitions, criteria)) {
            WriteObject(CreateTypeInfo(type, resolvedPath));
        }
    }
    private WildcardPattern? BuildNameMatcher()
        => string.IsNullOrWhiteSpace(NamePattern)
            ? null
            : new WildcardPattern(NamePattern, WildcardOptions.IgnoreCase);

    private static IEnumerable<ITypeDefinition> FilterTypes(IEnumerable<ITypeDefinition> candidates, TypeSearchCriteria criteria) {
        foreach (ITypeDefinition type in candidates) {
            if (!criteria.Matches(type))
                continue;

            yield return type;
        }
    }

    private static ISpyTypeInfo CreateTypeInfo(ITypeDefinition type, string assemblyPath) {
        SearchHelpers.TryFirst(type.DirectBaseTypes, out IType? baseType);

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
            BaseType = baseType?.FullName,
            AssemblyPath = assemblyPath,
            TypeName = type.FullName
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

namespace ISpy.Cmdlets;

/// <summary>
/// Resolves methods or cmdlets and streams ILSpy decompiled source code for the matching members.
/// </summary>
[Cmdlet(VerbsData.Expand, "Type", DefaultParameterSetName = "InputObject")]
[OutputType(
    typeof(string),
    typeof(ISpyDecompilationResult)
)]
[Alias("ent")]
public class ExpandTypeCmdlet : PSCmdlet {
    private readonly Dictionary<string, CSharpDecompiler> _decompilerCache = new(StringComparer.OrdinalIgnoreCase);

    [Parameter(
        ValueFromPipeline = true,
        Position = 0,
        ParameterSetName = "InputObject"
    )]
    public PSObject? InputObject { get; set; }

    [Parameter(
        ValueFromPipeline = true,
        ValueFromPipelineByPropertyName = true,
        ParameterSetName = "TypeInfo"
    )]
    public ISpyTypeInfo? InputType { get; set; }

    [Parameter(
        ValueFromPipeline = true,
        ValueFromPipelineByPropertyName = true,
        ParameterSetName = "DecompilationResult"
    )]
    public ISpyDecompilationResult? DecompilationInput { get; set; }

    [Parameter(
        ParameterSetName = "Explicit",
        ValueFromPipelineByPropertyName = true
    )]
    [Alias("AssemblyPath", "PSPath", "FilePath")]
    public string? Path { get; set; }

    [Parameter(
        Mandatory = true,
        ParameterSetName = "Explicit",
        ValueFromPipelineByPropertyName = true
    )]
    [ArgumentCompleter(typeof(LoadedTypeNameCompleter))]
    public string? TypeName { get; set; }

    [Parameter(
        HelpMessage = "Name of the method to decompile",
        ValueFromPipelineByPropertyName = true
    )]
    [ArgumentCompleter(typeof(LoadedMethodNameCompleter))]
    public string? MethodName { get; set; }

    [Parameter(
        HelpMessage = "When set, emit metadata objects instead of source"
    )]
    public SwitchParameter Metadata { get; set; }

    [Parameter(
        HelpMessage = "When set, include XML documentation comments in decompiled output if present"
    )]
    public SwitchParameter IncludeXml { get; set; }

    [Parameter(
        HelpMessage = "Custom decompiler settings to use when creating decompiler instances for decompilation"
    )]
    public DecompilerSettings? Settings { get; set; }

    [Parameter(
        HelpMessage = "Custom CSharpDecompiler instance to use instead of creating one from assembly path and defaults"
    )]
    public CSharpDecompiler? Decompiler { get; set; }

    private bool ShouldPostProcessOutput => Decompiler is null && Settings is null;

    protected override void ProcessRecord() {
        bool wroteResult = false;

        try {
            switch (ParameterSetName) {
                case "Explicit":
                    wroteResult = HandleExplicitParameters();
                    if (!wroteResult)
                        WriteVerbose("Explicit parameters did not produce output.");
                    return;

                case "TypeInfo":
                    if (InputType is null) {
                        WriteError(new ErrorRecord(new ArgumentNullException(nameof(InputType)), "MissingInputType", ErrorCategory.InvalidArgument, this));
                        return;
                    }

                    if (string.IsNullOrEmpty(MethodName)) {
                        if (TryDecompileTypeByAssembly(InputType.AssemblyPath ?? string.Empty, InputType.TypeName ?? InputType.FullName))
                            return;
                        return;
                    } {
                        string explicitAssemblyPath = GetResolvedAssemblyPath(InputType.AssemblyPath ?? string.Empty);
                        Assembly? explicitAssembly = LoadAssembly(explicitAssemblyPath);
                        if (explicitAssembly is null)
                            return;

                        Type? explicitType = explicitAssembly.GetType(InputType.TypeName ?? InputType.FullName, false, false);
                        if (explicitType is null) {
                            WriteError(new ErrorRecord(new ArgumentException($"Type '{InputType.TypeName ?? InputType.FullName}' not found in assembly '{explicitAssemblyPath}'"), "TypeNotFound", ErrorCategory.ObjectNotFound, InputType.TypeName));
                            return;
                        }

                        IEnumerable<ResolvedMethodTarget> resolved = TypeResolutionHelper.Resolve(explicitType, MethodName);
                        WriteResolvedMethods(resolved);
                        return;
                    }

                case "DecompilationResult":
                    if (DecompilationInput is null) {
                        WriteError(new ErrorRecord(new ArgumentNullException(nameof(DecompilationInput)), "MissingDecompilationInput", ErrorCategory.InvalidArgument, this));
                        return;
                    }

                    if (Metadata.IsPresent) {
                        WriteObject(DecompilationInput);
                        return;
                    }

                    if (!string.IsNullOrEmpty(DecompilationInput.Source)) {
                        WriteFormattedSource(DecompilationInput.Source, DecompilationInput.TypeName, DecompilationInput.AssemblyPath, method: null);
                        return;
                    }

                    return;

                case "InputObject":
                default:
                    // Fall through to pipeline-style resolution using InputObject
                    break;
            }

            // InputObject parameter set: resolve input and attempt to decompile
            PSObject? inputPsObject = InputObject;

            // If callers pipe ISpyTypeInfo or ISpyDecompilationResult by-value into InputObject
            // treat them as their respective parameter sets.
            if (inputPsObject?.BaseObject is ISpyTypeInfo pipedTypeInfo) {
                if (string.IsNullOrEmpty(MethodName)) {
                    if (TryDecompileTypeByAssembly(pipedTypeInfo.AssemblyPath ?? string.Empty, pipedTypeInfo.TypeName ?? pipedTypeInfo.FullName))
                        return;
                    return;
                }

                string pipedAssemblyPath = GetResolvedAssemblyPath(pipedTypeInfo.AssemblyPath ?? string.Empty);
                Assembly? pipedAssembly = LoadAssembly(pipedAssemblyPath);
                if (pipedAssembly is null)
                    return;

                Type? pipedType = pipedAssembly.GetType(pipedTypeInfo.TypeName ?? pipedTypeInfo.FullName, false, false);
                if (pipedType is null) {
                    WriteError(new ErrorRecord(new ArgumentException($"Type '{pipedTypeInfo.TypeName ?? pipedTypeInfo.FullName}' not found in assembly '{pipedAssemblyPath}'"), "TypeNotFound", ErrorCategory.ObjectNotFound, pipedTypeInfo.TypeName));
                    return;
                }

                IEnumerable<ResolvedMethodTarget> resolved = TypeResolutionHelper.Resolve(pipedType, MethodName);
                WriteResolvedMethods(resolved);
                return;
            }

            if (inputPsObject?.BaseObject is ISpyDecompilationResult pipedResult) {
                if (Metadata.IsPresent) {
                    WriteObject(pipedResult);
                    return;
                }

                if (!string.IsNullOrEmpty(pipedResult.Source)) {
                    WriteFormattedSource(pipedResult.Source, pipedResult.TypeName, pipedResult.AssemblyPath, method: null);
                    return;
                }

                return;
            }

            string? inputFullName = inputPsObject?.BaseObject?.GetType()?.FullName;
            string inputShortName = inputFullName is null ? string.Empty : inputFullName.Split('`', 2)[0];
            WriteVerbose($"Show-Type ParameterSetName: {ParameterSetName}, input: {inputShortName}");
            object? resolvedInput = inputPsObject;
            if (PowerShellCommandResolver.TryGetCommandInfo(this, inputPsObject, out CommandInfo? resolvedCommand) && resolvedCommand is not null)
                resolvedInput = resolvedCommand;

            if (string.IsNullOrEmpty(MethodName) && TryResolveTypeInput(resolvedInput!, out Type? resolvedType) && resolvedType is not null) {
                string assemblyPath = ResolveAssemblyPath(resolvedType);
                CSharpDecompiler? decompiler = GetDecompilerForAssembly(assemblyPath);
                if (decompiler is null)
                    return;

                string? source = ILSpyDecompiler.DecompileType(
                    assemblyPath,
                    new FullTypeName(GetFullTypeName(resolvedType)),
                    showXmlDocumentation: IncludeXml.IsPresent,
                    useUsingDeclarations: true,
                    decompiler: decompiler
                );
                WriteDecompiledOutput(null, assemblyPath, source, resolvedType.FullName);
                WriteVerbose($"Show-Type resolved type: {resolvedType.FullName}");
                wroteResult = true;
            }

            if (!wroteResult && PowerShellCommandResolver.TryGetScriptText(this, resolvedInput, out string? scriptText, out string? commandName)) {
                WriteObject(SourceOutputFactory.Create(scriptText!, commandName ?? "script", ".ps1"));
                return;
            }

            if (!wroteResult && string.IsNullOrEmpty(MethodName)) {
                IEnumerable<ResolvedMethodTarget> resolved = TypeResolutionHelper.Resolve(resolvedInput, MethodName);
                WriteResolvedMethods(resolved);
            }
            else if (!wroteResult) {
                foreach (ResolvedMethodTarget r in TypeResolutionHelper.Resolve(resolvedInput, MethodName)) {
                    WriteVerbose($"Show-Type resolved method: {r.Method.DeclaringType?.FullName}.{r.Method.Name} (Token={r.Method.MetadataToken})");
                    WriteResolvedMethod(r);
                    wroteResult = true;
                }
            }

            if (!wroteResult) {
                WriteVerbose("Show-Type did not locate a method to decompile.");
            }
        }
        catch (PipelineStoppedException) {
            throw;
        }
        catch (Exception ex) {
            WriteError(new ErrorRecord(
                ex,
                "ShowTypeDecompilationFailed",
                ErrorCategory.InvalidOperation,
                InputObject ?? (Path as object) ?? this));
        }
    }

    private bool HandleExplicitParameters() {
        if (string.IsNullOrEmpty(TypeName))
            return false;

        if (string.IsNullOrEmpty(Path)) {
            if (!LoadedTypeResolver.TryResolveLoadedType(TypeName, out Type? loadedType) || loadedType is null) {
                WriteError(new ErrorRecord(
                    new ArgumentException($"Loaded type not found: {TypeName}"),
                    "LoadedTypeNotFound",
                    ErrorCategory.ObjectNotFound,
                    TypeName));
                return false;
            }

            if (string.IsNullOrEmpty(MethodName)) {
                string assemblyPath = ResolveAssemblyPath(loadedType);
                string fullTypeName = GetFullTypeName(loadedType);
                CSharpDecompiler? decompiler = GetDecompilerForAssembly(assemblyPath);
                if (decompiler is null)
                    return false;

                string? source = ILSpyDecompiler.DecompileType(
                    assemblyPath,
                    new FullTypeName(fullTypeName),
                    showXmlDocumentation: IncludeXml.IsPresent,
                    useUsingDeclarations: true,
                    decompiler: decompiler);

                WriteDecompiledOutput(null, assemblyPath, source, fullTypeName);
                return true;
            }

            IEnumerable<ResolvedMethodTarget> resolvedLoaded = TypeResolutionHelper.Resolve(loadedType, MethodName);
            WriteResolvedMethods(resolvedLoaded);
            return true;
        }

        string resolvedAssembly = GetResolvedAssemblyPath(Path);
        if (!File.Exists(resolvedAssembly)) {
            WriteError(new ErrorRecord(
                new FileNotFoundException($"Assembly not found: {resolvedAssembly}"),
                "AssemblyNotFound",
                ErrorCategory.InvalidArgument,
                resolvedAssembly));
            return false;
        }

        if (string.IsNullOrEmpty(MethodName)) {
            CSharpDecompiler? decompiler = GetDecompilerForAssembly(resolvedAssembly);
            if (decompiler is null)
                return false;

            string? source = ILSpyDecompiler.DecompileType(
                resolvedAssembly,
                new FullTypeName(TypeName),
                showXmlDocumentation: IncludeXml.IsPresent,
                useUsingDeclarations: true,
                decompiler: decompiler);
            WriteDecompiledOutput(null, resolvedAssembly, source, TypeName);
            return true;
        }

        bool wrote = false;
        foreach (ResolvedMethodTarget resolved in ResolveExplicitTargets(resolvedAssembly)) {
            WriteResolvedMethod(resolved);
            wrote = true;
        }

        if (!wrote) {
            WriteError(new ErrorRecord(
                new ArgumentException($"No method named '{MethodName}' was found on type '{TypeName}'."),
                "MethodNotFound",
                ErrorCategory.ObjectNotFound,
                MethodName));
        }

        return wrote;
    }

    private IEnumerable<ResolvedMethodTarget> ResolveExplicitTargets(string assemblyPath) {
        Assembly? assembly = LoadAssembly(assemblyPath);
        if (assembly is null)
            yield break;

        Type? type = assembly.GetType(TypeName!, false, false);
        if (type is null) {
            WriteError(new ErrorRecord(
                new ArgumentException($"Type '{TypeName}' was not found in assembly '{assemblyPath}'."),
                "TypeNotFound",
                ErrorCategory.ObjectNotFound,
                TypeName));
            yield break;
        }

        foreach (ResolvedMethodTarget resolved in TypeResolutionHelper.Resolve(type, MethodName))
            yield return resolved;
    }

    private void WriteResolvedMethod(ResolvedMethodTarget resolved) {
        CSharpDecompiler? decompiler = GetDecompilerForAssembly(resolved.AssemblyPath);
        if (decompiler is null)
            return;

        string? source = ILSpyDecompiler.DecompileMethod(resolved.Method, showXmlDocumentation: IncludeXml.IsPresent, decompiler: decompiler);

        var result = new ISpyDecompilationResult {
            AssemblyPath = resolved.AssemblyPath,
            TypeName = resolved.Method.DeclaringType?.FullName,
            Source = source,
            Success = !string.IsNullOrEmpty(source),
            MethodNames = [resolved.Method.Name],
            MetadataTokens = [resolved.Method.MetadataToken]
        };

        if (Metadata.IsPresent) {
            WriteObject(result);
            return;
        }

        if (!string.IsNullOrEmpty(source)) {
            WriteFormattedSource(source, resolved.Method.DeclaringType?.FullName, resolved.AssemblyPath, resolved.Method);
        }
    }

    private void WriteDecompiledOutput(MethodBase? method, string assemblyPath, string? source, string? declaringTypeHint) {
        var result = new ISpyDecompilationResult {
            AssemblyPath = assemblyPath,
            TypeName = declaringTypeHint,
            Source = source,
            Success = !string.IsNullOrEmpty(source),
            MethodNames = method is null ? null : [method.Name],
            MetadataTokens = method is null ? null : [method.MetadataToken]
        };

        if (Metadata.IsPresent) {
            WriteObject(result);
            return;
        }

        if (!string.IsNullOrEmpty(result.Source)) {
            WriteFormattedSource(result.Source, declaringTypeHint, assemblyPath, method);
        }
    }

    private string GetResolvedAssemblyPath(string candidate) {
        string resolved = GetUnresolvedProviderPathFromPSPath(candidate);
        return string.IsNullOrEmpty(resolved) ? candidate : resolved;
    }

    private bool TryDecompileTypeByAssembly(string assemblyPathCandidate, string typeFullName) {
        if (string.IsNullOrWhiteSpace(assemblyPathCandidate)) {
            if (!LoadedTypeResolver.TryResolveLoadedType(typeFullName, out Type? loadedType) || loadedType is null) {
                WriteError(new ErrorRecord(new FileNotFoundException($"Assembly path was not provided and loaded type '{typeFullName}' could not be resolved."), "AssemblyNotFound", ErrorCategory.InvalidArgument, typeFullName));
                return false;
            }

            string assemblyPath = ResolveAssemblyPath(loadedType);
            string resolvedTypeName = GetFullTypeName(loadedType);
            CSharpDecompiler? decompiler = GetDecompilerForAssembly(assemblyPath);
            if (decompiler is null)
                return false;

            string? sourceByLoadedType = ILSpyDecompiler.DecompileType(
                assemblyPath,
                new FullTypeName(resolvedTypeName),
                showXmlDocumentation: IncludeXml.IsPresent,
                useUsingDeclarations: true,
                decompiler: decompiler);

            WriteDecompiledOutput(null, assemblyPath, sourceByLoadedType, resolvedTypeName);
            return true;
        }

        string resolvedAssembly = GetResolvedAssemblyPath(assemblyPathCandidate);
        if (!File.Exists(resolvedAssembly)) {
            WriteError(new ErrorRecord(new FileNotFoundException($"Assembly not found: {resolvedAssembly}"), "AssemblyNotFound", ErrorCategory.InvalidArgument, resolvedAssembly));
            return false;
        }

        CSharpDecompiler? typeDecompiler = GetDecompilerForAssembly(resolvedAssembly);
        if (typeDecompiler is null)
            return false;

        string? source = ILSpyDecompiler.DecompileType(
            resolvedAssembly,
            new FullTypeName(typeFullName),
            showXmlDocumentation: IncludeXml.IsPresent,
            useUsingDeclarations: true,
            decompiler: typeDecompiler);

        if (Metadata.IsPresent) {
            Assembly? asm = LoadAssembly(resolvedAssembly);
            if (asm is null) return false;

            Type? refl = asm.GetType(typeFullName, false, false);
            string[] methodNames = [];
            int[] metadataTokens = [];
            if (refl is not null) {
                MethodInfo[] methods = ReflectionCache.GetMethods(refl, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                methodNames = [.. methods.Where(m => !(m.Name?.StartsWith('<') ?? false)).Select(m => m.Name)];
                metadataTokens = [.. methods.Where(m => !(m.Name?.StartsWith('<') ?? false)).Select(m => m.MetadataToken)];
            }

            var result = new ISpyDecompilationResult {
                AssemblyPath = resolvedAssembly,
                TypeName = typeFullName,
                Source = source,
                Success = !string.IsNullOrEmpty(source),
                MethodNames = methodNames,
                MetadataTokens = metadataTokens
            };

            WriteObject(result);
            return true;
        }

        WriteDecompiledOutput(null, resolvedAssembly, source, typeFullName);
        return true;
    }

    private void WriteResolvedMethods(IEnumerable<ResolvedMethodTarget> resolvedTargets) {
        var methodsByAssembly = new Dictionary<string, List<MethodBase>>(StringComparer.OrdinalIgnoreCase);
        var methodNamesByAssembly = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
        var tokensByAssembly = new Dictionary<string, HashSet<int>>(StringComparer.OrdinalIgnoreCase);

        foreach (ResolvedMethodTarget resolved in resolvedTargets) {
            MethodBase method = resolved.Method;
            WriteVerbose($"Show-Type resolved method: {method.DeclaringType?.FullName}.{method.Name} (Token={method.MetadataToken})");

            string assemblyPath = resolved.AssemblyPath;
            if (!methodsByAssembly.TryGetValue(assemblyPath, out List<MethodBase>? methodList)) {
                methodList = [];
                methodsByAssembly[assemblyPath] = methodList;
                methodNamesByAssembly[assemblyPath] = [];
                tokensByAssembly[assemblyPath] = [];
            }

            if (!tokensByAssembly[assemblyPath].Add(method.MetadataToken))
                continue;

            methodList.Add(method);
            methodNamesByAssembly[assemblyPath].Add(method.Name);
        }

        foreach (KeyValuePair<string, List<MethodBase>> entry in methodsByAssembly) {
            if (entry.Value.Count == 0)
                continue;

            CSharpDecompiler? decompiler = GetDecompilerForAssembly(entry.Key);
            if (decompiler is null)
                continue;

            string? source = ILSpyDecompiler.DecompileMethods(
                entry.Value,
                showXmlDocumentation: IncludeXml.IsPresent,
                useUsingDeclarations: true,
                decompiler: decompiler);

            var result = new ISpyDecompilationResult {
                AssemblyPath = entry.Key,
                TypeName = entry.Value[0].DeclaringType?.FullName,
                Source = source,
                Success = !string.IsNullOrEmpty(source),
                MethodNames = [.. methodNamesByAssembly[entry.Key]],
                MetadataTokens = [.. tokensByAssembly[entry.Key]]
            };

            if (Metadata.IsPresent) {
                WriteObject(result);
            }
            else if (!string.IsNullOrEmpty(source)) {
                WriteFormattedSource(source, entry.Value[0].DeclaringType?.FullName, entry.Key, entry.Value[0]);
            }
        }
    }

    private void WriteFormattedSource(string source, string? declaringTypeFullName, string? assemblyPath, MethodBase? method) {
        if (ShouldPostProcessOutput) {
            WriteObject(SourceOutputFactory.CreateFromTypeName(
                source,
                declaringTypeFullName,
                assemblyPath: assemblyPath,
                method: method,
                preserveUsingDeclarations: false));
            return;
        }

        WriteObject(SourceOutputFactory.CreateFromTypeName(source, declaringTypeFullName));
    }

    private static bool TryResolveTypeInput(object input, out Type? resolvedType) {
        resolvedType = null;

        if (input is PSObject psObject && psObject.BaseObject is not null)
            return TryResolveTypeInput(psObject.BaseObject, out resolvedType);

        if (input is IEnumerable enumerable and not string) {
            foreach (object? item in enumerable) {
                if (item is null) continue;
                if (TryResolveTypeInput(item, out resolvedType)) return true;
            }
            return false;
        }

        if (input is Type inputType) {
            resolvedType = inputType;
            return true;
        }

        if (input is CommandInfo command) {
            command = PowerShellCommandResolver.ResolveAlias(command);

            if (command is CmdletInfo cmdlet) {
                resolvedType = cmdlet.ImplementingType;
                return true;
            }
        }

        return false;
    }

    private static string ResolveAssemblyPath(Type type)
        => type.Assembly.Location;

    private static string GetFullTypeName(Type type)
        => string.IsNullOrEmpty(type.FullName) ? type.Name : type.FullName;

    private CSharpDecompiler? GetDecompilerForAssembly(string assemblyPath) {
        if (Decompiler is not null)
            return Decompiler;

        string normalizedPath = GetResolvedAssemblyPath(assemblyPath);
        if (_decompilerCache.TryGetValue(normalizedPath, out CSharpDecompiler? cachedDecompiler))
            return cachedDecompiler;

        try {
            DecompilerSettings settings = Settings ?? new DecompilerSettings {
                ThrowOnAssemblyResolveErrors = false,
                UseDebugSymbols = false,
                ShowDebugInfo = false,
                UsingDeclarations = true,
                ShowXmlDocumentation = IncludeXml.IsPresent,
                FileScopedNamespaces = true
            };

            if (Settings is not null && IncludeXml.IsPresent)
                settings.ShowXmlDocumentation = true;

            CSharpDecompiler created = DecompilerFactory.Create(normalizedPath, settings);
            _decompilerCache[normalizedPath] = created;
            return created;
        }
        catch (Exception ex) {
            WriteError(new ErrorRecord(
                ex,
                "DecompilerCreationFailed",
                ErrorCategory.InvalidOperation,
                normalizedPath));
            return null;
        }
    }


    private Assembly? LoadAssembly(string assemblyPath) {
        string normalizedPath = GetResolvedAssemblyPath(assemblyPath);
        foreach (Assembly loaded in AssemblyLoadContext.Default.Assemblies) {
            if (string.Equals(loaded.Location, normalizedPath, StringComparison.OrdinalIgnoreCase))
                return loaded;
        }

        try {
            return AssemblyLoadContext.Default.LoadFromAssemblyPath(normalizedPath);
        }
        catch (Exception ex) {
            WriteError(new ErrorRecord(
                ex,
                "AssemblyLoadFailed",
                ErrorCategory.ResourceUnavailable,
                assemblyPath));
            return null;
        }
    }
}

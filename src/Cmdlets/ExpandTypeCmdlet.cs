namespace ISpy.Cmdlets;

/// <summary>
/// Resolves methods or cmdlets and streams ILSpy decompiled source code for the matching members.
/// </summary>
[Cmdlet(VerbsData.Expand, "Type")]
[OutputType(typeof(string))]
[OutputType(typeof(ISpyDecompilationResult))]
[Alias("ent")]
public class ExpandTypeCmdlet : PSCmdlet {
    [Parameter(
        Position = 0,
        ValueFromPipeline = true,
        ValueFromPipelineByPropertyName = true,
        HelpMessage = "MethodInfo, CommandInfo, Type, or PSMethod objects from which to resolve the method to decompile."
    )]
    [ValidateNotNull]
    public PSObject? InputObject { get; set; }

    [Parameter(
        ValueFromPipelineByPropertyName = true,
        HelpMessage = "Path to the assembly that contains the target type."
    )]
    [Alias("AssemblyPath", "PSPath")]
    public string? Path { get; set; }

    [Parameter(
        ValueFromPipelineByPropertyName = true,
        HelpMessage = "Full name of the type that contains the target method (e.g., Namespace.TypeName)."
    )]
    public string? TypeName { get; set; }

    [Parameter(
        HelpMessage = "Name of the method to decompile. Overloads with this name are returned."
    )]
    public string? MethodName { get; set; }

    [Parameter(
        HelpMessage = "Emit metadata objects alongside the source text."
    )]
    public SwitchParameter Metadata { get; set; }

    protected override void ProcessRecord() {
        bool wroteResult = false;

        try {
            string? inputFullName = InputObject?.BaseObject?.GetType()?.FullName;
            string inputShortName = inputFullName is null ? string.Empty : inputFullName.Split('`', 2)[0];
            WriteVerbose($"Show-Type input: {inputShortName}");
            object? resolvedInput = InputObject;
            if (PowerShellCommandResolver.TryGetCommandInfo(this, InputObject, out CommandInfo? resolvedCommand) && resolvedCommand is not null)
                resolvedInput = resolvedCommand;

            if (string.IsNullOrEmpty(MethodName) && TryResolveTypeInput(resolvedInput!, out Type? resolvedType) && resolvedType is not null) {
                string assemblyPath = ResolveAssemblyPath(resolvedType);
                string? source = ILSpyDecompiler.DecompileType(assemblyPath, new FullTypeName(GetFullTypeName(resolvedType)));
                WriteDecompiledOutput(null, assemblyPath, source, resolvedType.FullName);
                WriteVerbose($"Show-Type resolved type: {resolvedType.FullName}");
                wroteResult = true;
            }

            if (!wroteResult && PowerShellCommandResolver.TryGetScriptText(this, resolvedInput, out string? scriptText, out string? commandName)) {
                WriteObject(SourceOutputFactory.Create(scriptText!, commandName ?? "script", ".ps1"));
                return;
            }

            if (!wroteResult && string.IsNullOrEmpty(MethodName)) {
                var methodsByAssembly = new Dictionary<string, List<MethodBase>>(StringComparer.OrdinalIgnoreCase);
                var methodNamesByAssembly = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
                var tokensByAssembly = new Dictionary<string, HashSet<int>>(StringComparer.OrdinalIgnoreCase);
                int resolvedCount = 0;

                foreach (ResolvedMethodTarget resolved in TypeResolutionHelper.Resolve(resolvedInput, MethodName)) {
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
                    resolvedCount++;
                }

                foreach (KeyValuePair<string, List<MethodBase>> entry in methodsByAssembly) {
                    if (entry.Value.Count == 0)
                        continue;

                    string? source = ILSpyDecompiler.DecompileMethods(entry.Value);

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
                        WriteObject(SourceOutputFactory.CreateFromTypeName(source, entry.Value[0].DeclaringType?.FullName, assemblyPath: entry.Key, method: entry.Value[0]));
                    }

                    wroteResult = true;
                }

                WriteVerbose($"Show-Type resolved method count: {resolvedCount}");
            }
            else if (!wroteResult) {
                foreach (ResolvedMethodTarget resolved in TypeResolutionHelper.Resolve(resolvedInput, MethodName)) {
                    WriteVerbose($"Show-Type resolved method: {resolved.Method.DeclaringType?.FullName}.{resolved.Method.Name} (Token={resolved.Method.MetadataToken})");
                    WriteResolvedMethod(resolved);
                    wroteResult = true;
                }
            }

            if (!wroteResult) {
                wroteResult = HandleExplicitParameters();
            }

            if (!wroteResult) {
                WriteVerbose("Show-Type did not locate a method to decompile.");
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
                "ShowTypeDecompilationFailed",
                ErrorCategory.InvalidOperation,
                InputObject ?? (Path as object) ?? this));
        }
    }

    private bool HandleExplicitParameters() {
        if (string.IsNullOrEmpty(Path) || string.IsNullOrEmpty(TypeName))
            return false;

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
            string? source = ILSpyDecompiler.DecompileType(resolvedAssembly, new FullTypeName(TypeName));
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
        string? source = ILSpyDecompiler.DecompileMethod(resolved.Method);

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

        if (!string.IsNullOrEmpty(source))
            WriteObject(SourceOutputFactory.CreateFromTypeName(source, resolved.Method.DeclaringType?.FullName, assemblyPath: resolved.AssemblyPath, method: resolved.Method));
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

        if (!string.IsNullOrEmpty(result.Source))
            WriteObject(SourceOutputFactory.CreateFromTypeName(result.Source, declaringTypeHint, assemblyPath: assemblyPath));
    }

    private string GetResolvedAssemblyPath(string candidate) {
        string resolved = GetUnresolvedProviderPathFromPSPath(candidate);
        return string.IsNullOrEmpty(resolved) ? candidate : resolved;
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

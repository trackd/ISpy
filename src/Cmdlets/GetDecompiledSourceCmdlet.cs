namespace ISpy.Cmdlets;

[Cmdlet(VerbsCommon.Get, "DecompiledSource")]
[OutputType(typeof(ISpyDecompilationResult))]
[Alias("gds")]
public class GetDecompiledSourceCmdlet : PSCmdlet {
    [Parameter(
        Mandatory = false,
        Position = 0,
        ValueFromPipeline = true,
        ValueFromPipelineByPropertyName = true,
        HelpMessage = "Path to the assembly file to decompile. Can be a DLL, EXE, or any valid .NET assembly format."
    )]
    [ValidateNotNullOrEmpty]
    [Alias("AssemblyPath", "PSPath", "FilePath")]
    public string? Path { get; set; }

    [Parameter(
        Mandatory = false,
        Position = 1,
        HelpMessage = "Specific type name to decompile (if not specified, decompiles entire assembly). Use the format 'Namespace.ClassName' or 'Namespace.OuterClass+InnerClass' for nested types."
    )]
    [ArgumentCompleter(typeof(LoadedTypeNameCompleter))]
    public string? TypeName { get; set; }

    [Parameter(
        Mandatory = false,
        HelpMessage = "Custom decompiler settings to control the decompilation process."
    )]
    public DecompilerSettings? Settings { get; set; }

    [Parameter(
        Mandatory = false,
        HelpMessage = "Custom CSharpDecompiler instance to use instead of creating one from path/settings."
    )]
    public CSharpDecompiler? Decompiler { get; set; }

    protected override void ProcessRecord() {
        try {
            string? candidatePath = Path;
            string? resolvedTypeName = TypeName;

            if (string.IsNullOrWhiteSpace(candidatePath)) {
                if (TypeName is not { Length: > 0 } requestedTypeName || string.IsNullOrWhiteSpace(requestedTypeName)) {
                    WriteError(new ErrorRecord(
                        new ArgumentException("Path or TypeName must be provided."),
                        "MissingPathOrTypeName",
                        ErrorCategory.InvalidArgument,
                        this));
                    return;
                }

                if (!LoadedTypeResolver.TryResolveLoadedType(requestedTypeName, out Type? loadedType) || loadedType is null) {
                    WriteError(new ErrorRecord(
                        new InvalidOperationException($"Loaded type not found: {requestedTypeName}"),
                        "TypeNotFound",
                        ErrorCategory.ObjectNotFound,
                        requestedTypeName));
                    return;
                }

                try {
                    candidatePath = loadedType.Assembly.Location;
                }
                catch {
                    candidatePath = null;
                }

                if (string.IsNullOrWhiteSpace(candidatePath)) {
                    WriteError(new ErrorRecord(
                        new FileNotFoundException($"Assembly location is unavailable for loaded type: {requestedTypeName}"),
                        "AssemblyNotFound",
                        ErrorCategory.ObjectNotFound,
                        requestedTypeName
                    ));
                    return;
                }

                resolvedTypeName = loadedType.FullName ?? loadedType.Name;
            }

            if (candidatePath is not { Length: > 0 } assemblyPathInput)
                return;

            string resolvedPath = GetUnresolvedProviderPathFromPSPath(assemblyPathInput);
            if (!File.Exists(resolvedPath)) {
                WriteError(new ErrorRecord(
                    new FileNotFoundException($"Assembly file not found: {resolvedPath}"),
                    "AssemblyNotFound",
                    ErrorCategory.InvalidArgument,
                    resolvedPath
                ));
                return;
            }

            WriteVerbose($"Loading assembly: {resolvedPath}");

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

            if (!string.IsNullOrEmpty(resolvedTypeName)) {
                // Decompile a specific type
                ITypeDefinition? typeDef = decompiler.TypeSystem.FindType(new FullTypeName(resolvedTypeName)).GetDefinition();
                if (typeDef == null) {
                    WriteError(new ErrorRecord(
                        new InvalidOperationException($"Type not found: {resolvedTypeName}"),
                        "TypeNotFound",
                        ErrorCategory.ObjectNotFound,
                        resolvedTypeName));
                    return;
                }
                ISpyDecompilationResult result = CreateDecompilationResultForType(decompiler, resolvedPath, typeDef);
                WriteObject(result);
            }
            else {
                // Decompile all types in the assembly
                var results = new List<ISpyDecompilationResult>();
                foreach (ITypeDefinition type in decompiler.TypeSystem.MainModule.TypeDefinitions.Where(t => !(t.Name.Length > 0 && t.Name[0] == '<'))) {
                    ISpyDecompilationResult result = CreateDecompilationResultForType(decompiler, resolvedPath, type);
                    results.Add(result);
                }
                WriteObject(results, true);
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
                "DecompilationError",
                ErrorCategory.InvalidOperation,
                Path));
        }
    }


    private static ISpyDecompilationResult CreateDecompilationResultForType(CSharpDecompiler decompiler, string assemblyPath, ITypeDefinition type) {
        try {
            string source = decompiler.DecompileTypeAsString(type.FullTypeName);
            string[] methodNames = [.. type.Methods.Where(m => !m.IsCompilerGenerated()).Select(m => m.Name)];
            int[] metadataTokens = [.. type.Methods.Where(m => !m.IsCompilerGenerated()).Select(m => m.MetadataToken.GetHashCode())];
            return new ISpyDecompilationResult {
                AssemblyPath = assemblyPath,
                TypeName = type.FullName,
                Source = source,
                Success = !string.IsNullOrEmpty(source),
                MethodNames = methodNames,
                MetadataTokens = metadataTokens,
            };
        }
        catch {
            return new ISpyDecompilationResult {
                AssemblyPath = assemblyPath,
                TypeName = type.FullName,
                Source = null,
                Success = false,
                MethodNames = [],
                MetadataTokens = [],
            };
        }
    }

}

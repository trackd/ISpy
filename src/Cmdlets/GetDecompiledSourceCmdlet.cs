namespace ISpy.Cmdlets;

[Cmdlet(VerbsCommon.Get, "DecompiledSource", SupportsShouldProcess = true)]
[OutputType(typeof(ISpyDecompilationResult))]
[Alias("gds")]
public class GetDecompiledSourceCmdlet : PSCmdlet {
    [Parameter(
        Mandatory = true,
        Position = 0,
        ValueFromPipeline = true,
        ValueFromPipelineByPropertyName = true,
        HelpMessage = "Path to the assembly file to decompile. Can be a DLL, EXE, or any valid .NET assembly format."
    )]
    [ValidateNotNullOrEmpty]
    [Alias("AssemblyPath", "PSPath")]
    public string? Path { get; set; }

    [Parameter(
        Mandatory = false,
        Position = 1,
        HelpMessage = "Specific type name to decompile (if not specified, decompiles entire assembly). Use the format 'Namespace.ClassName' or 'Namespace.OuterClass+InnerClass' for nested types."
    )]
    public string? TypeName { get; set; }

    [Parameter(
        Mandatory = false,
        HelpMessage = "Custom decompiler settings to control the decompilation process."
    )]
    public DecompilerSettings? Settings { get; set; }

    protected override void ProcessRecord() {
        try {
            string resolvedPath = GetUnresolvedProviderPathFromPSPath(Path);
            if (!File.Exists(resolvedPath)) {
                WriteError(new ErrorRecord(
                    new FileNotFoundException($"Assembly file not found: {resolvedPath}"),
                    "AssemblyNotFound",
                    ErrorCategory.InvalidArgument,
                    resolvedPath));
                return;
            }

            WriteVerbose($"Loading assembly: {resolvedPath}");

            DecompilerSettings decompilerSettings = Settings ?? new DecompilerSettings();
            var decompiler = new CSharpDecompiler(resolvedPath, decompilerSettings);

            if (!string.IsNullOrEmpty(TypeName)) {
                // Decompile a specific type
                ITypeDefinition? typeDef = decompiler.TypeSystem.FindType(new FullTypeName(TypeName)).GetDefinition();
                if (typeDef == null) {
                    WriteError(new ErrorRecord(
                        new InvalidOperationException($"Type not found: {TypeName}"),
                        "TypeNotFound",
                        ErrorCategory.ObjectNotFound,
                        TypeName));
                    return;
                }
                ISpyDecompilationResult result = CreateDecompilationResultForType(decompiler, resolvedPath, typeDef);
                WriteObject(result);
            }
            else {
                // Decompile all types in the assembly
                var results = new List<ISpyDecompilationResult>();
                foreach (ITypeDefinition type in decompiler.TypeSystem.MainModule.TypeDefinitions.Where(t => !t.Name.StartsWith('<'))) {
                    ISpyDecompilationResult result = CreateDecompilationResultForType(decompiler, resolvedPath, type);
                    results.Add(result);
                }
                WriteObject(results, true);
            }
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

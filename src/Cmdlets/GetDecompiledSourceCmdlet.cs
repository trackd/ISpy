namespace ISpy.Cmdlets;

[Cmdlet(VerbsCommon.Get, "DecompiledSource", SupportsShouldProcess = true)]
[OutputType(typeof(ISpyDecompilationResult))]
public class GetDecompiledSourceCmdlet : PSCmdlet {
    [Parameter(
        Mandatory = true,
        Position = 0,
        ValueFromPipeline = true,
        ValueFromPipelineByPropertyName = true,
        HelpMessage = "Path to the assembly file to decompile. Can be a DLL, EXE, or any valid .NET assembly format.")]
    [ValidateNotNullOrEmpty]
    [Alias("AssemblyPath", "PSPath")]
    public string? Path { get; set; }

    [Parameter(
        Mandatory = false,
        Position = 1,
        HelpMessage = "Specific type name to decompile (if not specified, decompiles entire assembly). Use the format 'Namespace.ClassName' or 'Namespace.OuterClass+InnerClass' for nested types.")]
    public string? TypeName { get; set; }

    [Parameter(
        Mandatory = false,
        HelpMessage = "Output file path to save the decompiled source code. If not provided, output will be displayed in the console.")]
    public string? OutputPath { get; set; }

    [Parameter(
        Mandatory = false,
        HelpMessage = "Custom decompiler settings to control the decompilation process.")]
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

            string decompiledCode = TypeName switch {
                not null => DecompileSpecificType(decompiler, TypeName),
                null => DecompileWholeAssembly(decompiler)
            };

            string resolvedAssemblyPath = resolvedPath;

            var result = new ISpyDecompilationResult {
                AssemblyPath = resolvedAssemblyPath,
                TypeName = TypeName,
                Source = decompiledCode,
                Success = !string.IsNullOrEmpty(decompiledCode),
            };

            if (OutputPath is not null) {
                SaveToFile(decompiledCode, OutputPath);
                result.FilePath = GetUnresolvedProviderPathFromPSPath(OutputPath);
            }

            WriteObject(result);
        }
        catch (Exception ex) {
            WriteError(new ErrorRecord(
                ex,
                "DecompilationError",
                ErrorCategory.InvalidOperation,
                Path));
        }
    }

    private string DecompileSpecificType(CSharpDecompiler decompiler, string typeName) {
        WriteVerbose($"Decompiling type: {typeName}");

        try {
            var fullTypeName = new FullTypeName(typeName);
            return decompiler.DecompileTypeAsString(fullTypeName);
        }
        catch (StackOverflowException) {
            WriteError(new ErrorRecord(
                new InvalidOperationException($"Stack overflow occurred while decompiling type '{typeName}'. This is a known issue with the decompiler library for certain complex types with circular references."),
                "DecompilerStackOverflow",
                ErrorCategory.LimitsExceeded,
                typeName));
            return string.Empty;
        }
        catch (Exception ex) {
            WriteError(new ErrorRecord(
                new InvalidOperationException($"Failed to decompile type '{typeName}': {ex.Message}", ex),
                "TypeDecompilationError",
                ErrorCategory.InvalidOperation,
                typeName));
            return string.Empty;
        }
    }

    private string DecompileWholeAssembly(CSharpDecompiler decompiler) {
        WriteVerbose("Decompiling entire assembly");

        try {
            return decompiler.DecompileWholeModuleAsString();
        }
        catch (StackOverflowException) {
            WriteError(new ErrorRecord(
                new InvalidOperationException("Stack overflow occurred while decompiling assembly. This is a known issue with the decompiler library for assemblies containing types with circular references."),
                "DecompilerStackOverflow",
                ErrorCategory.LimitsExceeded,
                Path));
            return string.Empty;
        }
    }

    private void SaveToFile(string content, string outputPath) {
        string resolvedOutput = GetUnresolvedProviderPathFromPSPath(outputPath);
        WriteVerbose($"Writing output to: {resolvedOutput}");

        string? outputDir = System.IO.Path.GetDirectoryName(resolvedOutput);
        if (!string.IsNullOrEmpty(outputDir) && !Directory.Exists(outputDir)) {
            Directory.CreateDirectory(outputDir);
        }

        if (ShouldProcess(resolvedOutput, "Write decompiled output")) {
            File.WriteAllText(resolvedOutput, content);
            WriteVerbose($"Decompiled source saved to: {resolvedOutput}");
        }
    }
}

namespace ISpy.Cmdlets;

[Cmdlet(VerbsData.Export, "DecompiledSource", SupportsShouldProcess = true)]
[OutputType(typeof(ISpyExportResult))]
public class ExportDecompiledSourceCmdlet : PSCmdlet {
    [Parameter(
        Mandatory = true,
        Position = 0,
        ValueFromPipeline = true,
        ValueFromPipelineByPropertyName = true,
        HelpMessage = "Path to the assembly file to decompile")]
    [ValidateNotNullOrEmpty]
    [Alias("AssemblyPath", "PSPath")]
    public string? Path { get; set; }

    [Parameter(
        Mandatory = true,
        Position = 1,
        HelpMessage = "Output directory path")]
    [ValidateNotNullOrEmpty]
    public required string OutputDirectory { get; set; }

    [Parameter(
        Mandatory = false,
        HelpMessage = "Specific types to export (if not specified, exports all types)")]
    public string[]? TypeNames { get; set; }

    [Parameter(
        Mandatory = false,
        HelpMessage = "Namespace filter")]
    public string? Namespace { get; set; }

    [Parameter(
        Mandatory = false,
        HelpMessage = "Create subdirectories for namespaces")]
    public SwitchParameter CreateNamespaceDirectories { get; set; }

    [Parameter(
        Mandatory = false,
        HelpMessage = "Decompiler settings")]
    public DecompilerSettings? Settings { get; set; }

    [Parameter(
        Mandatory = false,
        HelpMessage = "Overwrite existing files")]
    public SwitchParameter Force { get; set; }

    protected override void ProcessRecord() {
        try {
            string resolvedAssembly = GetUnresolvedProviderPathFromPSPath(Path);
            if (!File.Exists(resolvedAssembly)) {
                WriteError(new ErrorRecord(
                    new FileNotFoundException($"Assembly file not found: {resolvedAssembly}"),
                    "AssemblyNotFound",
                    ErrorCategory.InvalidArgument,
                    resolvedAssembly));
                return;
            }

            string resolvedOutputDir = GetUnresolvedProviderPathFromPSPath(OutputDirectory);
            if (!Directory.Exists(resolvedOutputDir)) {
                WriteVerbose($"Creating output directory: {resolvedOutputDir}");
                Directory.CreateDirectory(resolvedOutputDir);
            }

            WriteVerbose($"Loading assembly: {resolvedAssembly}");

            DecompilerSettings decompilerSettings = Settings ?? new DecompilerSettings();
            var decompiler = new CSharpDecompiler(resolvedAssembly, decompilerSettings);

            int exportedFiles = 0;
            int skippedFiles = 0;

            if (TypeNames is { Length: > 0 }) {
                // Export specific types
                foreach (string typeName in TypeNames) {
                    try {
                        WriteVerbose($"Decompiling type: {typeName}");
                        ITypeDefinition? type = decompiler.TypeSystem.FindType(new FullTypeName(typeName)).GetDefinition();

                        if (type == null) {
                            WriteWarning($"Type not found: {typeName}");
                            continue;
                        }

                        ISpyExportResult result = ExportType(decompiler, type, resolvedOutputDir);
                        if (result.Success) {
                            exportedFiles++;
                            WriteObject(result);
                        }
                        else {
                            skippedFiles++;
                        }
                    }
                    catch (Exception ex) {
                        WriteError(new ErrorRecord(
                            ex,
                            "TypeDecompilationError",
                            ErrorCategory.InvalidOperation,
                            typeName));
                    }
                }
            }
            else {
                // Export all types
                WriteVerbose("Decompiling all types in assembly");

                IEnumerable<ITypeDefinition> types = decompiler.TypeSystem.MainModule.TypeDefinitions;

                if (!string.IsNullOrEmpty(Namespace)) {
                    types = types.Where(t => t.Namespace.Equals(Namespace, StringComparison.OrdinalIgnoreCase));
                }

                foreach (ITypeDefinition? type in types.Where(t => !t.Name.StartsWith('<'))) // Filter out compiler-generated types
                {
                    try {
                        WriteVerbose($"Decompiling type: {type.FullName}");
                        ISpyExportResult result = ExportType(decompiler, type, resolvedOutputDir);
                        if (result.Success) {
                            exportedFiles++;
                            WriteObject(result);
                        }
                        else {
                            skippedFiles++;
                        }
                    }
                    catch (Exception ex) {
                        WriteWarning($"Failed to decompile type {type.FullName}: {ex.Message}");
                        skippedFiles++;
                    }
                }
            }

            WriteVerbose($"Export completed. Files exported: {exportedFiles}, Files skipped: {skippedFiles}");
        }
        catch (Exception ex) {
            WriteError(new ErrorRecord(
                ex,
                "ExportError",
                ErrorCategory.InvalidOperation,
                Path));
        }
    }

    private ISpyExportResult ExportType(CSharpDecompiler decompiler, ITypeDefinition type, string outputDirectory) {
        try {
            string decompiledCode;
            try {
                decompiledCode = decompiler.DecompileTypeAsString(type.FullTypeName);
            }
            catch (StackOverflowException) {
                WriteWarning($"Stack overflow occurred while decompiling type '{type.FullTypeName}'. Skipping this type due to known decompiler library issue with circular references.");
                return new ISpyExportResult {
                    Success = false,
                    TypeName = type.FullTypeName.ToString(),
                    FilePath = string.Empty,
                    Message = "Stack overflow in decompiler library"
                };
            }

            string fileName = GetSafeFileName(type.Name) + ".cs";

            string filePath = CreateNamespaceDirectories.IsPresent && !string.IsNullOrEmpty(type.Namespace)
                ? System.IO.Path.Combine(outputDirectory, type.Namespace.Replace('.', System.IO.Path.DirectorySeparatorChar), fileName)
                : System.IO.Path.Combine(outputDirectory, fileName);

            if (CreateNamespaceDirectories.IsPresent && !string.IsNullOrEmpty(type.Namespace)) {
                string namespaceDir = System.IO.Path.GetDirectoryName(filePath)!;
                if (!Directory.Exists(namespaceDir)) {
                    Directory.CreateDirectory(namespaceDir);
                }
            }

            if (File.Exists(filePath) && !Force.IsPresent) {
                return new ISpyExportResult {
                    TypeName = type.FullName,
                    FilePath = filePath,
                    Success = false,
                    Message = "File already exists (use -Force to overwrite)"
                };
            }

            if (ShouldProcess(filePath, "Write decompiled file")) {
                File.WriteAllText(filePath, decompiledCode);
            }

            return new ISpyExportResult {
                TypeName = type.FullName,
                FilePath = filePath,
                Success = true,
                Message = "Exported successfully"
            };
        }
        catch (Exception ex) {
            return new ISpyExportResult {
                TypeName = type.FullName,
                FilePath = string.Empty,
                Success = false,
                Message = $"Export failed: {ex.Message}"
            };
        }
    }

    private static string GetSafeFileName(string fileName) =>
        string.Join("_", fileName.Split(System.IO.Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries));
}

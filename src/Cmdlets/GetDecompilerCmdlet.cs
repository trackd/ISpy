namespace ISpy.Cmdlets;

[Cmdlet(VerbsCommon.Get, "Decompiler")]
[OutputType(typeof(CSharpDecompiler))]
public class GetDecompilerCmdlet : PSCmdlet {
    [Parameter(
        Position = 0,
        Mandatory = true,
        ValueFromPipeline = true,
        ValueFromPipelineByPropertyName = true,
        HelpMessage = "Path to the assembly you want to decompile")]
    [Alias("PSPath", "AssemblyPath")]
    [ValidateNotNullOrEmpty]
    public string? Path { get; set; }

    [Parameter(HelpMessage = "C# Language version to be used by the decompiler")]
    public LanguageVersion LanguageVersion { get; set; } = LanguageVersion.Latest;

    [Parameter(HelpMessage = "Remove dead stores")]
    public SwitchParameter RemoveDeadStores { get; set; }

    [Parameter(HelpMessage = "Remove dead code")]
    public SwitchParameter RemoveDeadCode { get; set; }

    [Parameter(HelpMessage = "Path to PDB file for debug information")]
    [ValidateNotNullOrEmpty]
    public string? PDBFilePath { get; set; }

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

            string? pdbPath = null;
            if (!string.IsNullOrEmpty(PDBFilePath)) {
                pdbPath = GetUnresolvedProviderPathFromPSPath(PDBFilePath);
                if (!File.Exists(pdbPath)) {
                    WriteError(new ErrorRecord(
                        new FileNotFoundException($"PDB file not found: {pdbPath}"),
                        "PDBNotFound",
                        ErrorCategory.InvalidArgument,
                        pdbPath));
                    return;
                }
            }

            WriteVerbose($"Creating decompiler for assembly: {resolvedPath}");

            using var fileStream = new FileStream(resolvedPath, FileMode.Open, FileAccess.Read);
            var module = new PEFile(resolvedPath, fileStream, PEStreamOptions.Default);

            var decompiler = new CSharpDecompiler(resolvedPath, new DecompilerSettings(LanguageVersion) {
                ThrowOnAssemblyResolveErrors = false,
                RemoveDeadCode = RemoveDeadCode.IsPresent,
                RemoveDeadStores = RemoveDeadStores.IsPresent,
                UseDebugSymbols = false,
                ShowDebugInfo = false,
            });

            WriteObject(decompiler);
        }
        catch (Exception ex) {
            WriteVerbose(ex.ToString());
            WriteError(new ErrorRecord(
                ex,
                "DecompilerCreationError",
                ErrorCategory.OperationStopped,
                Path));
        }
    }
}

namespace ISpy.Cmdlets;

[Cmdlet(VerbsCommon.New, "Decompiler")]
[OutputType(typeof(CSharpDecompiler))]
public class NewDecompilerCmdlet : PSCmdlet {
    [Parameter(
        Position = 0,
        Mandatory = true,
        ValueFromPipeline = true,
        ValueFromPipelineByPropertyName = true,
        HelpMessage = "Path to the assembly you want to decompile")]
    [Alias("PSPath", "AssemblyPath", "FilePath")]
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

    [Parameter]
    public DecompilerSettings? DecompilerSettings { get; set; }

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

            DecompilerSettings settings = DecompilerSettings ?? new DecompilerSettings(LanguageVersion) {
                ThrowOnAssemblyResolveErrors = false,
                RemoveDeadCode = RemoveDeadCode.IsPresent,
                RemoveDeadStores = RemoveDeadStores.IsPresent,
                UseDebugSymbols = false,
                ShowDebugInfo = false,
            };

            CSharpDecompiler decompiler = DecompilerFactory.Create(resolvedPath, settings);
            WriteObject(decompiler);
        }
        catch (PipelineStoppedException) {
            // Pipeline was stopped by downstream cmdlet (e.g., Select-Object -First)
            // This is normal behavior, just rethrow to let PowerShell handle it
            throw;
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

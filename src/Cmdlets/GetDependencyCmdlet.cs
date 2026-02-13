namespace ISpy.Cmdlets;

[Cmdlet(VerbsCommon.Get, "Dependency")]
[OutputType(typeof(ISpyDependencyInfo))]
[Alias("dep")]
public class GetDependencyCmdlet : PSCmdlet {
    [Parameter(
        Mandatory = true,
        Position = 0,
        ValueFromPipeline = true,
        ValueFromPipelineByPropertyName = true,
        HelpMessage = "Path to the assembly file to analyze")]
    [ValidateNotNullOrEmpty]
    [Alias("AssemblyPath", "PSPath", "FilePath")]
    public string? Path { get; set; }

    [Parameter(
        HelpMessage = "Only show external dependencies (exclude self-references)"
    )]
    public SwitchParameter ExternalOnly { get; set; }

    protected override void ProcessRecord() {
        try {
            string resolvedPath = GetUnresolvedProviderPathFromPSPath(Path);
            WriteVerbose($"Loading assembly: {resolvedPath}");

            if (!File.Exists(resolvedPath)) {
                WriteError(new ErrorRecord(
                    new FileNotFoundException($"Assembly file not found: {resolvedPath}"),
                    "AssemblyNotFound",
                    ErrorCategory.InvalidArgument,
                    resolvedPath));
                return;
            }

            using var stream = new FileStream(resolvedPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var peReader = new PEReader(stream);
            MetadataReader metadataReader = peReader.GetMetadataReader();

            AssemblyReferenceHandleCollection assemblyReferences = metadataReader.AssemblyReferences;
            WriteVerbose($"Found {assemblyReferences.Count} dependencies");

            foreach (AssemblyReferenceHandle assemblyRef in assemblyReferences) {
                System.Reflection.Metadata.AssemblyReference reference = metadataReader.GetAssemblyReference(assemblyRef);
                string name = metadataReader.GetString(reference.Name);

                if (ExternalOnly.IsPresent) {
                    // Get current assembly name for comparison
                    AssemblyDefinition assemblyDef = metadataReader.GetAssemblyDefinition();
                    string currentName = metadataReader.GetString(assemblyDef.Name);
                    if (name == currentName) {
                        continue; // Skip self-references
                    }
                }

                Version version = reference.Version;
                string culture = reference.Culture.IsNil ? "neutral" : metadataReader.GetString(reference.Culture);
                string publicKeyOrToken = reference.PublicKeyOrToken.IsNil ? "null" :
                    Convert.ToHexString(metadataReader.GetBlobBytes(reference.PublicKeyOrToken)).ToLowerInvariant();

                var dependencyInfo = new ISpyDependencyInfo {
                    Name = name,
                    FullName = $"{name}, Version={version}, Culture={culture}, PublicKeyToken={publicKeyOrToken}",
                    Version = version,
                    Culture = culture,
                    PublicKeyToken = publicKeyOrToken,
                };

                WriteObject(dependencyInfo);
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
                "DependencyAnalysisError",
                ErrorCategory.InvalidOperation,
                Path));
        }
    }
}

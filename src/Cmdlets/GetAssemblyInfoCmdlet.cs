using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

namespace ISpy.Cmdlets;

[Cmdlet(VerbsCommon.Get, "AssemblyInfo")]
[OutputType(typeof(ISpyAssemblyInfo))]
public class GetAssemblyInfoCmdlet : PSCmdlet {
    [Parameter(
        Mandatory = true,
        Position = 0,
        ValueFromPipeline = true,
        ValueFromPipelineByPropertyName = true,
        HelpMessage = "Path to the assembly file to analyze")]
    [ValidateNotNullOrEmpty]
    [Alias("AssemblyPath", "PSPath")]
    public string? Path { get; set; }

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
            var decompiler = new CSharpDecompiler(resolvedPath, new DecompilerSettings());
            MetadataModule module = decompiler.TypeSystem.MainModule;

            // Prefer reading manifest data via AssemblyName where possible (does not load the assembly).
            Version version = new();
            string culture = "neutral";
            string publicKeyToken = "null";
            string processorArchitecture = "Unknown";
            int moduleCount = 1;
            bool hasEntryPoint = false;
            string? entryPoint = null;

            try {
                var asmName = AssemblyName.GetAssemblyName(resolvedPath);
                if (asmName.Version is not null)
                    version = asmName.Version;

                culture = string.IsNullOrEmpty(asmName.CultureName) ? "neutral" : asmName.CultureName;

                byte[]? pkt = asmName.GetPublicKeyToken();
                publicKeyToken = pkt is null || pkt.Length == 0
                    ? "null"
                    : string.Concat(pkt.Select(b => b.ToString("x2", CultureInfo.InvariantCulture)));
            }
            catch {
                // Fall back to manifest parsing when AssemblyName cannot be read
            }

            // Inspect PE headers/metadata for entry point and module count
            try {
                using FileStream stream = File.OpenRead(resolvedPath);
                using var peReader = new PEReader(stream);
                PEHeaders peHeaders = peReader.PEHeaders;
                if (peHeaders.PEHeader is not null and PEHeader peHeader)
                    hasEntryPoint = peHeader.AddressOfEntryPoint != 0;

                if (peHeaders.CoffHeader is CoffHeader coffHeader)
                    processorArchitecture = coffHeader.Machine.ToString();

            }
            catch {
                // non-fatal - best effort only
            }

            // Defensive: some metadata properties can be null or malformed; provide safe fallbacks.
            string assemblyFullName = module.AssemblyName ?? System.IO.Path.GetFileName(resolvedPath) ?? "Unknown";
            int commaIndex = assemblyFullName.IndexOf(',');
            string assemblySimpleName = commaIndex > 0
                ? assemblyFullName[..commaIndex]
                : assemblyFullName;

            int typeCount = 0;
            try { typeCount = module.TypeDefinitions?.Count() ?? 0; } catch { /* best-effort */ }

            var assemblyInfo = new ISpyAssemblyInfo {
                FullName = assemblyFullName,
                Name = string.IsNullOrEmpty(assemblySimpleName) ? System.IO.Path.GetFileNameWithoutExtension(resolvedPath) ?? assemblyFullName : assemblySimpleName,
                Version = version,
                Culture = culture,
                PublicKeyToken = publicKeyToken,
                ProcessorArchitecture = processorArchitecture,
                ModuleCount = moduleCount,
                TypeCount = typeCount,
                HasEntryPoint = hasEntryPoint,
                EntryPoint = entryPoint,
                TargetFramework = GetTargetFramework(module),
                FilePath = resolvedPath
            };

            WriteObject(assemblyInfo);
        }
        catch (Exception ex) {
            WriteError(new ErrorRecord(
                ex,
                "AssemblyAnalysisError",
                ErrorCategory.InvalidOperation,
                Path));
        }
    }

    private static string GetTargetFramework(MetadataModule module) {
        try {
            IAttribute? targetFrameworkAttr = module.GetAssemblyAttributes()
                .FirstOrDefault(attr => attr.AttributeType.FullName == "System.Runtime.Versioning.TargetFrameworkAttribute");

            return targetFrameworkAttr?.FixedArguments.Length > 0
                ? targetFrameworkAttr.FixedArguments[0].Value?.ToString() ?? "Unknown"
                : "Unknown";
        }
        catch {
            return "Unknown";
        }
    }
}

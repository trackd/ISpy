
namespace ISpy.Cmdlets;

[Cmdlet(VerbsCommon.Get, "AssemblyInfo")]
[OutputType(typeof(ISpyAssemblyInfo))]
[Alias("gai")]
public class GetAssemblyInfoCmdlet : PSCmdlet {
    [Parameter(
        Mandatory = true,
        Position = 0,
        ValueFromPipeline = true,
        ValueFromPipelineByPropertyName = true,
        HelpMessage = "Path to the assembly file to analyze")]
    [ValidateNotNullOrEmpty]
    [Alias("AssemblyPath", "PSPath", "FilePath")]
    public string? Path { get; set; }

    protected override void ProcessRecord() {
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

        Version version = new();
        string culture = "neutral";
        string publicKeyToken = "null";
        string processorArchitecture = "Unknown";
        bool hasEntryPoint = false;
        string targetFramework = "Unknown";
        string assemblyFullName = System.IO.Path.GetFileName(resolvedPath) ?? "Unknown";
        string assemblySimpleName = assemblyFullName;
        int typeCount = 0;

        // Use AssemblyName for basic info
        try {
            var asmName = AssemblyName.GetAssemblyName(resolvedPath);
            if (asmName.Version is not null)
                version = asmName.Version;
            culture = string.IsNullOrEmpty(asmName.CultureName) ? "neutral" : asmName.CultureName;
            byte[]? pkt = asmName.GetPublicKeyToken();
            publicKeyToken = pkt is null || pkt.Length == 0
                ? "null"
                : string.Concat(pkt.Select(b => b.ToString("x2", CultureInfo.InvariantCulture)));
            assemblyFullName = asmName.FullName ?? assemblyFullName;
            int commaIndex = assemblyFullName.IndexOf(',');
            assemblySimpleName = commaIndex > 0 ? assemblyFullName[..commaIndex] : assemblyFullName;
        }
        catch (Exception ex) {
            WriteVerbose($"Could not read AssemblyName for '{resolvedPath}': {ex.Message}");
        }

        // Use PEReader and MetadataReader for richer info
        try {
            using FileStream stream = File.OpenRead(resolvedPath);
            using var peReader = new PEReader(stream);
            PEHeaders peHeaders = peReader.PEHeaders;
            if (peHeaders.PEHeader is not null and PEHeader peHeader)
                hasEntryPoint = peHeader.AddressOfEntryPoint != 0;
            if (peHeaders.CoffHeader is CoffHeader coffHeader) {
                // Use CorFlags to determine architecture
                MetadataReader mdReader = peReader.GetMetadataReader();
                CorFlags? corFlags = peHeaders.CorHeader?.Flags;
                bool is32BitRequired = corFlags.HasValue && ((int)corFlags.Value & 0x2) != 0;
                bool is32BitPreferred = corFlags.HasValue && ((int)corFlags.Value & 0x20000) != 0;
                bool isILOnly = corFlags.HasValue && ((int)corFlags.Value & 0x1) != 0;
                bool is64Bit = coffHeader.Machine is Machine.Amd64 or Machine.IA64;
                bool isArm = coffHeader.Machine is Machine.Arm or Machine.Arm64;
                processorArchitecture = is64Bit
                    ? "Amd64"
                    : isArm
                    ? coffHeader.Machine.ToString()
                    : is32BitRequired ? "x86" : isILOnly && !is32BitRequired && !is64Bit && !isArm ? "AnyCPU" : coffHeader.Machine.ToString();
            }

            // Use MetadataReader for TargetFramework and TypeCount
            if (peReader.HasMetadata) {
                MetadataReader mdReader = peReader.GetMetadataReader();
                // TargetFramework
                foreach (CustomAttributeHandle handle in mdReader.GetCustomAttributes(EntityHandle.ModuleDefinition)) {
                    CustomAttribute attr = mdReader.GetCustomAttribute(handle);
                    EntityHandle ctorHandle = attr.Constructor;
                    string attrTypeName = "";
                    if (ctorHandle.Kind == HandleKind.MemberReference) {
                        MemberReference memberRef = mdReader.GetMemberReference((MemberReferenceHandle)ctorHandle);
                        EntityHandle container = memberRef.Parent;
                        if (container.Kind == HandleKind.TypeReference) {
                            TypeReference typeRef = mdReader.GetTypeReference((TypeReferenceHandle)container);
                            attrTypeName = mdReader.GetString(typeRef.Namespace) + "." + mdReader.GetString(typeRef.Name);
                        }
                    }
                    if (attrTypeName == "System.Runtime.Versioning.TargetFrameworkAttribute") {
                        // Parse the value blob manually
                        BlobHandle valueBlob = attr.Value;
                        BlobReader blobReader = mdReader.GetBlobReader(valueBlob);
                        // Skip prolog (2 bytes)
                        _ = blobReader.ReadUInt16();
                        string? tfm = blobReader.ReadSerializedString();
                        if (!string.IsNullOrEmpty(tfm)) {
                            targetFramework = tfm;
                            break;
                        }
                    }
                }
                // TypeCount
                try {
                    typeCount = mdReader.TypeDefinitions.Count;
                }
                catch { typeCount = 0; }
            }
        }
        catch (PipelineStoppedException) {
            // Pipeline was stopped by downstream cmdlet (e.g., Select-Object -First)
            // This is normal behavior, just rethrow to let PowerShell handle it
            throw;
        }
        catch (Exception ex) {
            WriteVerbose($"Could not read PE headers or metadata for '{resolvedPath}': {ex.Message}");
        }

        // Fallback: try to resolve dependencies from loaded assemblies if needed (for future extensibility)
        // Example: if you ever need to resolve a type, use this pattern
        // var loaded = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == "SomeDependency");

        var assemblyInfo = new ISpyAssemblyInfo {
            FullName = assemblyFullName,
            Name = string.IsNullOrEmpty(assemblySimpleName) ? System.IO.Path.GetFileNameWithoutExtension(resolvedPath) ?? assemblyFullName : assemblySimpleName,
            Version = version,
            Culture = culture,
            PublicKeyToken = publicKeyToken,
            ProcessorArchitecture = processorArchitecture,
            Types = typeCount,
            HasEntryPoint = hasEntryPoint,
            TargetFramework = targetFramework,
            FilePath = resolvedPath
        };

        WriteObject(assemblyInfo);
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

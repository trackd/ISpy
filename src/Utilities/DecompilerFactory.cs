namespace ISpy.Utilities;

internal static class DecompilerFactory {
    public static CSharpDecompiler Create(string assemblyPath, DecompilerSettings settings) {
        ArgumentException.ThrowIfNullOrWhiteSpace(assemblyPath);
        ArgumentNullException.ThrowIfNull(settings);

        string targetFrameworkId = DetectTargetFrameworkId(assemblyPath);
        var resolver = new UniversalAssemblyResolver(
            assemblyPath,
            false,
            targetFrameworkId,
            runtimePack: null,
            PEStreamOptions.Default,
            MetadataReaderOptions.Default
        );

        return new CSharpDecompiler(assemblyPath, resolver, settings);
    }

    private static string DetectTargetFrameworkId(string assemblyPath) {
        try {
            using var stream = new FileStream(assemblyPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var module = new PEFile(assemblyPath, stream, PEStreamOptions.Default);
            return module.Metadata.DetectTargetFrameworkId();
        }
        catch {
            return string.Empty;
        }
    }
}

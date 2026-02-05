namespace ISpy.Models;

public class ISpyDecompilationResult {
    public string AssemblyPath { get; set; } = string.Empty;
    public string? TypeName { get; set; }
    public bool Success { get; set; }
    public string? FilePath { get; set; }
    public string? Source { get; set; }
    public string? Message { get; set; }
    public bool IncludeDebugInfo { get; set; }
    public string[]? MethodNames { get; set; }
    public int[]? MetadataTokens { get; set; }
}

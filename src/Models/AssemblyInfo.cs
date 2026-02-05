namespace ISpy.Models;

public class ISpyAssemblyInfo {
    public string FullName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Culture { get; set; } = string.Empty;
    public string PublicKeyToken { get; set; } = string.Empty;
    public string ProcessorArchitecture { get; set; } = string.Empty;
    public int ModuleCount { get; set; }
    public int TypeCount { get; set; }
    public bool HasEntryPoint { get; set; }
    public string? EntryPoint { get; set; }
    public string TargetFramework { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
}

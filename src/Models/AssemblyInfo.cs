namespace ISpy.Models;

public class ISpyAssemblyInfo {
    public string FullName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Version? Version { get; set; }
    public string? Culture { get; set; }
    public string? PublicKeyToken { get; set; }
    public string? ProcessorArchitecture { get; set; }
    public int? ModuleCount { get; set; }
    public int? TypeCount { get; set; }
    public bool? HasEntryPoint { get; set; }
    public string? EntryPoint { get; set; }
    public string? TargetFramework { get; set; }
    public string? FilePath { get; set; }
}

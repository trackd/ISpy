namespace ISpy.Models;

public class ISpyDependencyInfo {
    public string Name { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public Version? Version { get; set; }
    public string? Culture { get; set; }
    public string? PublicKeyToken { get; set; }
}

namespace ISpy.Models;

public class ISpyDependencyInfo {
    public string Name { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Culture { get; set; } = string.Empty;
    public string PublicKeyToken { get; set; } = string.Empty;
    public int MajorVersion { get; set; }
    public int MinorVersion { get; set; }
    public int BuildNumber { get; set; }
    public int RevisionNumber { get; set; }
}

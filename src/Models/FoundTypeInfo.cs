namespace ISpy.Models;

public class ISpyFoundTypeInfo {
    public string TypeName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Namespace { get; set; } = string.Empty;
    public string AssemblyName { get; set; } = string.Empty;
    public string AssemblyPath { get; set; } = string.Empty;
    public string AssemblyVersion { get; set; } = string.Empty;
    public bool IsPublic { get; set; }
    public bool IsInterface { get; set; }
    public bool IsAbstract { get; set; }
    public bool IsSealed { get; set; }
    public bool IsEnum { get; set; }
    public bool IsClass { get; set; }
}

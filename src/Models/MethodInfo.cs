namespace ISpy.Models;

public class ISpyMethodInfo {
    public string Name { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string DeclaringType { get; set; } = string.Empty;
    public string ReturnType { get; set; } = string.Empty;
    public bool IsPublic { get; set; }
    public bool IsPrivate { get; set; }
    public bool IsProtected { get; set; }
    public bool IsInternal { get; set; }
    public bool IsStatic { get; set; }
    public bool IsVirtual { get; set; }
    public bool IsAbstract { get; set; }
    public bool IsSealed { get; set; }
    public bool IsConstructor { get; set; }
    public bool IsGetter { get; set; }
    public bool IsSetter { get; set; }
    public int ParameterCount { get; set; }
    public string Parameters { get; set; } = string.Empty;
}

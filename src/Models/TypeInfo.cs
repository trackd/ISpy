namespace ISpy.Models;

public class ISpyTypeInfo {
    public string FullName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Namespace { get; set; } = string.Empty;
    public bool IsPublic { get; set; }
    public bool IsAbstract { get; set; }
    public bool IsSealed { get; set; }
    public bool IsInterface { get; set; }
    public bool IsEnum { get; set; }
    public bool IsClass { get; set; }
    public bool IsValueType { get; set; }
    public string? BaseType { get; set; }
    public int InterfaceCount { get; set; }
    public int MethodCount { get; set; }
    public int PropertyCount { get; set; }
    public int FieldCount { get; set; }
    public int EventCount { get; set; }
}

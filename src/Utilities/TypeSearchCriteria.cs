namespace ISpy.Utilities;

internal sealed class TypeSearchCriteria {
    public string? Namespace { get; }
    public WildcardPattern? NameMatcher { get; }
    public bool PublicOnly { get; }
    public bool IncludeCompilerGenerated { get; }
    public IReadOnlyCollection<TypeKind>? TypeKinds { get; }

    public TypeSearchCriteria(
        string? @namespace,
        WildcardPattern? nameMatcher,
        bool publicOnly,
        bool includeCompilerGenerated,
        IReadOnlyCollection<TypeKind>? typeKinds) {
        Namespace = @namespace;
        NameMatcher = nameMatcher;
        PublicOnly = publicOnly;
        IncludeCompilerGenerated = includeCompilerGenerated;
        TypeKinds = typeKinds;
    }

    public bool Matches(ITypeDefinition type) {
        if (!IncludeCompilerGenerated && IsCompilerGenerated(type.Name))
            return false;

        if (PublicOnly && type.Accessibility != Accessibility.Public)
            return false;

        if (!string.IsNullOrEmpty(Namespace) && !string.Equals(Namespace, type.Namespace, StringComparison.OrdinalIgnoreCase))
            return false;

        if (NameMatcher is not null && !NameMatcher.IsMatch(type.Name) && !NameMatcher.IsMatch(type.FullName))
            return false;

        return TypeKinds?.Contains(type.Kind) != false;
    }

    public bool Matches(Type type) {
        if (!IncludeCompilerGenerated && IsCompilerGenerated(type.Name))
            return false;

        if (PublicOnly && !(type.IsPublic || type.IsNestedPublic))
            return false;

        if (!string.IsNullOrEmpty(Namespace) && !string.Equals(Namespace, type.Namespace, StringComparison.OrdinalIgnoreCase))
            return false;

        if (NameMatcher is not null) {
            string fullName = type.FullName ?? type.Name;
            if (!NameMatcher.IsMatch(type.Name) && !NameMatcher.IsMatch(fullName))
                return false;
        }

        var runtimeKind = LoadedTypeResolver.ToTypeKind(type);
        return TypeKinds?.Contains(runtimeKind) != false;
    }

    private static bool IsCompilerGenerated(string typeName)
        => typeName.Length > 0 && typeName[0] == '<';
}

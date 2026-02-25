namespace ISpy.Utilities;

public static class SourceOutputFactory {
    /// <summary>
    /// Build output with PSChildName for downstream file-oriented processing.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="fileBaseName"></param>
    /// <param name="extension"></param>
    /// <returns></returns>
    public static PSObject Create(string source, string fileBaseName, string extension = ".cs") {
        ArgumentNullException.ThrowIfNull(source);

        if (string.IsNullOrEmpty(fileBaseName))
            fileBaseName = "decompiled";

        string safeName = GetSafeFileName(fileBaseName) + extension;
        var wrappedSource = PSObject.AsPSObject(source);
        wrappedSource.Properties.Add(new PSNoteProperty("PSChildName", safeName));
        return wrappedSource;
    }

    /// <summary>
    /// Derive a reasonable file name from a type full name.
    /// </summary>
    /// <param name="declaringTypeFullName"></param>
    /// <returns></returns>
    public static string GetFileBaseNameFromTypeName(string? declaringTypeFullName) {
        return string.IsNullOrEmpty(declaringTypeFullName)
            ? "decompiled"
            : declaringTypeFullName.Contains('.')
            ? declaringTypeFullName[(declaringTypeFullName.LastIndexOf('.') + 1)..]
            : declaringTypeFullName;
    }

    /// <summary>
    /// Build output using a type name as the file base name.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="declaringTypeFullName"></param>
    /// <param name="extension"></param>
    /// <returns></returns>
    public static PSObject CreateFromTypeName(string source, string? declaringTypeFullName, string extension = ".cs", bool preserveUsingDeclarations = true) {
        string fileBaseName = GetFileBaseNameFromTypeName(declaringTypeFullName);
        return Create(source, fileBaseName, extension);
    }

    /// <summary>
    /// Overload that accepts an optional assembly path to enable XML doc injection
    /// </summary>
    /// <param name="source"></param>
    /// <param name="declaringTypeFullName"></param>
    /// <param name="assemblyPath"></param>
    /// <param name="extension"></param>
    /// <returns></returns>
    public static PSObject CreateFromTypeName(string source, string? declaringTypeFullName, string? assemblyPath, string extension = ".cs", bool preserveUsingDeclarations = false) {
        string processed = PostProcessSource(source, declaringTypeFullName, assemblyPath, method: null, preserveUsingDeclarations: preserveUsingDeclarations);
        string fileBaseName = GetFileBaseNameFromTypeName(declaringTypeFullName);
        return Create(processed, fileBaseName, extension);
    }

    /// <summary>
    /// Overload that accepts method context to inject method-level XML docs when available.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="declaringTypeFullName"></param>
    /// <param name="assemblyPath"></param>
    /// <param name="method"></param>
    /// <param name="extension"></param>
    /// <returns></returns>
    public static PSObject CreateFromTypeName(string source, string? declaringTypeFullName, string? assemblyPath, MethodBase? method, string extension = ".cs", bool preserveUsingDeclarations = true) {
        string processed = PostProcessSource(source, declaringTypeFullName, assemblyPath, method, preserveUsingDeclarations: preserveUsingDeclarations);
        string fileBaseName = GetFileBaseNameFromTypeName(declaringTypeFullName);
        return Create(processed, fileBaseName, extension);
    }

    private static string PostProcessSource(string source, string? declaringTypeFullName, string? assemblyPath, MethodBase? method, bool preserveUsingDeclarations = true) {
        if (string.IsNullOrEmpty(source))
            return source;

        string[] lines = source.Replace("\r\n", "\n").Split('\n');
        List<string> remaining = [.. lines];

        if (!preserveUsingDeclarations) {
            remaining = [..
                remaining.Where(l => {
                    string trimmed = l.Trim();
                    if (trimmed.StartsWith("global using ", StringComparison.Ordinal) && trimmed.EndsWith(';'))
                        return false;

                    if (trimmed.StartsWith("using ", StringComparison.Ordinal)
                        && trimmed.EndsWith(';')
                        && !trimmed.Contains('(')
                        && !trimmed.StartsWith("using var ", StringComparison.Ordinal)) {
                        return false;
                    }

                    return true;
                })
            ];

            while (remaining.Count > 0 && string.IsNullOrWhiteSpace(remaining[0]))
                remaining.RemoveAt(0);
        }

        if (method is not null)
            AddNamespaceHeaderComment(remaining, declaringTypeFullName);

        // XML comment shaping is controlled by decompiler settings (ShowXmlDocumentation)
        // and not altered here.

        return string.Join(Environment.NewLine, remaining);
    }

    private static void AddNamespaceHeaderComment(List<string> lines, string? declaringTypeFullName) {
        if (lines.Count == 0)
            return;

        string? ns = null;
        SearchHelpers.TryFirst(
            lines,
            static l => l.TrimStart().StartsWith("namespace ", StringComparison.Ordinal),
            out string? namespaceLine);
        if (namespaceLine is not null) {
            ns = namespaceLine.Trim();
            if (ns.StartsWith("namespace ", StringComparison.Ordinal))
                ns = ns["namespace ".Length..];

            ns = ns.TrimEnd(';').Trim();
        }

        // Prefer a fully-qualified type name when available (fqdn), otherwise fall back to namespace
        string? header = !string.IsNullOrEmpty(declaringTypeFullName)
            ? declaringTypeFullName
            : ns;

        if (string.IsNullOrEmpty(header))
            return;

        // Avoid inserting a duplicate header comment
        if (lines[0].StartsWith("// " + header, StringComparison.Ordinal))
            return;

        lines.Insert(0, $"// {header}");
        lines.Insert(1, string.Empty);
    }
    private static string GetSafeFileName(string fileName) =>
        string.Join("_", fileName.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries));
}

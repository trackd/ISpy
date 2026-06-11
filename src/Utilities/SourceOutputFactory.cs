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
        ArgumentGuards.ThrowIfNull(source, nameof(source));

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
        if (declaringTypeFullName is not { Length: > 0 } nonNullTypeName)
            return "decompiled";

        int dotIndex = nonNullTypeName.LastIndexOf('.');
#if NETSTANDARD2_0
        // System.Range is not available in netstandard2.0.
        return dotIndex >= 0
            ? nonNullTypeName.Substring(dotIndex + 1)
            : nonNullTypeName;

#else
        return dotIndex >= 0
            ? nonNullTypeName[(dotIndex + 1)..]
            : nonNullTypeName;
#endif
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
                    if (trimmed.StartsWith("global using ", StringComparison.Ordinal)
#if NETSTANDARD2_0
                        && trimmed.EndsWith(";", StringComparison.Ordinal)
#else
                        && trimmed.EndsWith(';')
#endif
                    ) {
                        return false;
                    }

                    if (trimmed.StartsWith("using ", StringComparison.Ordinal)
#if NETSTANDARD2_0
                        && trimmed.EndsWith(";", StringComparison.Ordinal)
#else
                        && trimmed.EndsWith(';')
#endif
                        && trimmed.IndexOf('(') < 0
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

        // Keep intentional spacing inside the body, but avoid trailing blank lines at EOF.
        while (remaining.Count > 0 && string.IsNullOrWhiteSpace(remaining[remaining.Count - 1]))
            remaining.RemoveAt(remaining.Count - 1);

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
#if NETSTANDARD2_0
                ns = ns.Substring("namespace ".Length);
#else
                ns = ns["namespace ".Length..];
#endif

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

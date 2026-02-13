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
    public static PSObject CreateFromTypeName(string source, string? declaringTypeFullName, string extension = ".cs") {
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
    public static PSObject CreateFromTypeName(string source, string? declaringTypeFullName, string? assemblyPath, string extension = ".cs") {
        string processed = PostProcessSource(source, declaringTypeFullName, assemblyPath, method: null);
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
    public static PSObject CreateFromTypeName(string source, string? declaringTypeFullName, string? assemblyPath, MethodBase? method, string extension = ".cs") {
        string processed = PostProcessSource(source, declaringTypeFullName, assemblyPath, method);
        string fileBaseName = GetFileBaseNameFromTypeName(declaringTypeFullName);
        return Create(processed, fileBaseName, extension);
    }

    internal static string RemoveIndentation(string source, int indent) {
        ArgumentNullException.ThrowIfNull(source);
        if (indent <= 0)
            return source;

        string[] lines = source.Replace("\r\n", "\n").Split('\n');
        string[] result = new string[lines.Length];

        for (int i = 0; i < lines.Length; i++) {
            string line = lines[i];
            int removable = 0;

            while (removable < line.Length && removable < indent && char.IsWhiteSpace(line[removable])) {
                removable++;
            }

            result[i] = line[removable..];
        }

        return string.Join(Environment.NewLine, result);
    }
    private static string PostProcessSource(string source, string? declaringTypeFullName, string? assemblyPath, MethodBase? method) {
        if (string.IsNullOrEmpty(source))
            return source;

        string[] lines = source.Replace("\r\n", "\n").Split('\n');

        // Remove top-level using declarations (consecutive lines starting with 'using')
        int idx = 0;
        bool sawUsing = false;
        while (idx < lines.Length) {
            string trimmed = lines[idx].TrimStart();
            if (trimmed.StartsWith("using ", StringComparison.Ordinal) || trimmed.StartsWith("global using ", StringComparison.Ordinal)) {
                sawUsing = true;
                idx++;
                continue;
            }

            // skip a single blank line after usings
            if (sawUsing && string.IsNullOrWhiteSpace(lines[idx])) {
                idx++;
            }

            break;
        }

        var remaining = lines.Skip(idx).ToList();

        if (method is not null)
            AddNamespaceHeaderComment(remaining, declaringTypeFullName);

        // If assembly xml exists, try to inject type XML documentation above the type declaration
        if (!string.IsNullOrEmpty(assemblyPath) && !string.IsNullOrEmpty(declaringTypeFullName)) {
            try {
                string xmlPath = Path.ChangeExtension(assemblyPath, ".xml");
                if (File.Exists(xmlPath)) {
                    var doc = XDocument.Load(xmlPath);
                    string memberName = "T:" + declaringTypeFullName;
                    XElement? typeMember = doc.Root?.Elements("member").FirstOrDefault(e => (string?)e.Attribute("name") == memberName);
                    if (typeMember is not null) {
                        string typeShortName = declaringTypeFullName.Contains('.') ? declaringTypeFullName[(declaringTypeFullName.LastIndexOf('.') + 1)..] : declaringTypeFullName;
                        int insertAt = FindTypeDeclarationInsertIndex(remaining, typeShortName);
                        InsertXmlCommentBlock(remaining, insertAt, typeMember);
                    }

                    if (method is not null) {
                        XElement? methodMember = FindMethodXmlMember(doc, method, declaringTypeFullName);
                        if (methodMember is not null) {
                            int insertAt = FindMethodDeclarationInsertIndex(remaining, method.Name);
                            InsertXmlCommentBlock(remaining, insertAt, methodMember);
                        }
                    }
                }
            }
            catch {
                // on any failure, fall back to original source (don't block decompilation)
            }
        }

        return string.Join(Environment.NewLine, remaining);
    }

    private static void AddNamespaceHeaderComment(List<string> lines, string? declaringTypeFullName) {
        if (lines.Count == 0)
            return;

        string? ns = null;
        string? namespaceLine = lines.FirstOrDefault(l => l.TrimStart().StartsWith("namespace ", StringComparison.Ordinal));
        if (namespaceLine is not null) {
            ns = namespaceLine.Trim();
            if (ns.StartsWith("namespace ", StringComparison.Ordinal))
                ns = ns["namespace ".Length..];

            ns = ns.TrimEnd(';').Trim();
        }

        if (string.IsNullOrEmpty(ns) && !string.IsNullOrEmpty(declaringTypeFullName)) {
            int lastDot = declaringTypeFullName.LastIndexOf('.');
            if (lastDot > 0)
                ns = declaringTypeFullName[..lastDot];
        }

        if (string.IsNullOrEmpty(ns))
            return;

        if (lines[0].StartsWith("// namespace ", StringComparison.Ordinal))
            return;

        lines.Insert(0, $"// namespace {ns}");
        lines.Insert(1, string.Empty);
    }

    private static int FindTypeDeclarationInsertIndex(List<string> lines, string typeShortName) {
        for (int i = 0; i < lines.Count; i++) {
            string l = lines[i];
            if (l.Contains("class " + typeShortName, StringComparison.Ordinal)
                || l.Contains("struct " + typeShortName, StringComparison.Ordinal)
                || l.Contains("interface " + typeShortName, StringComparison.Ordinal)
                || l.Contains("enum " + typeShortName, StringComparison.Ordinal)) {
                return i;
            }
        }

        return 0;
    }

    private static int FindMethodDeclarationInsertIndex(List<string> lines, string methodName) {
        for (int i = 0; i < lines.Count; i++) {
            if (lines[i].Contains(methodName + "(", StringComparison.Ordinal))
                return i;
        }

        return 0;
    }

    private static void InsertXmlCommentBlock(List<string> lines, int insertAt, XElement xmlMember) {
        string[] summary = xmlMember.Element("summary")?.Nodes().Select(n => n.ToString()).ToArray() ?? [];
        string[] remarks = xmlMember.Element("remarks")?.Nodes().Select(n => n.ToString()).ToArray() ?? [];
        if (summary.Length == 0 && remarks.Length == 0)
            return;

        var commentLines = new List<string>();
        if (summary.Length > 0) {
            commentLines.Add("/// <summary>");
            foreach (string s in summary) {
                string cleaned = s.Replace("\n", " ").Trim();
                if (!string.IsNullOrEmpty(cleaned))
                    commentLines.Add("/// " + cleaned);
            }
            commentLines.Add("/// </summary>");
        }

        if (remarks.Length > 0) {
            commentLines.Add("/// <remarks>");
            foreach (string r in remarks) {
                string cleaned = r.Replace("\n", " ").Trim();
                if (!string.IsNullOrEmpty(cleaned))
                    commentLines.Add("/// " + cleaned);
            }
            commentLines.Add("/// </remarks>");
        }

        if (commentLines.Count == 0)
            return;

        insertAt = Math.Clamp(insertAt, 0, lines.Count);
        lines.InsertRange(insertAt, commentLines);
    }

    private static XElement? FindMethodXmlMember(XDocument doc, MethodBase method, string declaringTypeFullName) {
        string exact = BuildMethodXmlMemberName(method, declaringTypeFullName);
        XElement? found = doc.Root?.Elements("member").FirstOrDefault(e => string.Equals((string?)e.Attribute("name"), exact, StringComparison.Ordinal));
        if (found is not null)
            return found;

        string methodName = method.IsConstructor ? "#ctor" : method.Name;
        string prefix = $"M:{declaringTypeFullName}.{methodName}";
        return doc.Root?.Elements("member").FirstOrDefault(e => ((string?)e.Attribute("name"))?.StartsWith(prefix, StringComparison.Ordinal) == true);
    }

    private static string BuildMethodXmlMemberName(MethodBase method, string declaringTypeFullName) {
        string methodName = method.IsConstructor ? "#ctor" : method.Name;
        ParameterInfo[] parameters = method.GetParameters();
        if (parameters.Length == 0)
            return $"M:{declaringTypeFullName}.{methodName}";

        string parameterList = string.Join(",", parameters.Select(p => GetXmlDocTypeName(p.ParameterType)));
        return $"M:{declaringTypeFullName}.{methodName}({parameterList})";
    }

    private static string GetXmlDocTypeName(Type type) {
        if (type.IsByRef)
            return GetXmlDocTypeName(type.GetElementType()!) + "@";

        if (type.IsArray) {
            string element = GetXmlDocTypeName(type.GetElementType()!);
            int rank = type.GetArrayRank();
            return rank == 1 ? element + "[]" : $"{element}[{new string(',', rank - 1)}]";
        }

        if (type.IsGenericParameter)
            return type.DeclaringMethod is null ? $"`{type.GenericParameterPosition}" : $"``{type.GenericParameterPosition}";

        if (type.IsGenericType) {
            Type genericDef = type.GetGenericTypeDefinition();
            string name = (genericDef.FullName ?? genericDef.Name).Replace('+', '.');
            int tick = name.IndexOf('`');
            if (tick >= 0)
                name = name[..tick];

            string args = string.Join(",", type.GetGenericArguments().Select(GetXmlDocTypeName));
            return $"{name}{{{args}}}";
        }

        return (type.FullName ?? type.Name).Replace('+', '.');
    }

    private static string GetSafeFileName(string fileName) =>
        string.Join("_", fileName.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries));
}

namespace ISpy.Utilities;

internal static class ILSpyDecompiler {
    public static string DecompileMethod(MethodBase method, bool showXmlDocumentation = false, CSharpDecompiler? decompiler = null) {
        string? assemblyPath = method.Module?.FullyQualifiedName ?? method.DeclaringType?.Assembly.Location;
        return string.IsNullOrEmpty(assemblyPath)
            ? throw new InvalidOperationException("Unable to determine the assembly path for the selected method.")
            : DecompileToken(assemblyPath, method.MetadataToken, showXmlDocumentation: showXmlDocumentation, decompiler: decompiler);
    }

    public static string DecompileType(string assemblyPath, FullTypeName fullTypeName, bool showXmlDocumentation = false, bool useUsingDeclarations = true, CSharpDecompiler? decompiler = null) {
        CSharpDecompiler activeDecompiler = decompiler ?? CreateDecompiler(assemblyPath, useUsingDeclarations: useUsingDeclarations, showXmlDocumentation: showXmlDocumentation);
        return activeDecompiler.DecompileTypeAsString(fullTypeName);
    }

    public static string DecompileMethods(IEnumerable<MethodBase> methods, bool showXmlDocumentation = false, bool useUsingDeclarations = true, CSharpDecompiler? decompiler = null) {
        ArgumentNullException.ThrowIfNull(methods);

        string? assemblyPath = null;
        var handles = new List<EntityHandle>();
        var seenTokens = new HashSet<int>();

        foreach (MethodBase method in methods) {
            if (method is null)
                continue;

            string? methodAssemblyPath = method.Module?.FullyQualifiedName ?? method.DeclaringType?.Assembly.Location;
            if (string.IsNullOrEmpty(methodAssemblyPath))
                throw new InvalidOperationException("Unable to determine the assembly path for the selected method.");

            if (assemblyPath is null)
                assemblyPath = methodAssemblyPath;
            else if (!string.Equals(assemblyPath, methodAssemblyPath, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("All methods must come from the same assembly to decompile together.");

            int metadataToken = method.MetadataToken;
            if (metadataToken == 0)
                throw new InvalidOperationException("The selected method does not have a valid metadata token.");

            if (!seenTokens.Add(metadataToken))
                continue;

            var handle = (EntityHandle)MetadataTokens.Handle(metadataToken);
            if (handle.IsNil || handle.Kind != HandleKind.MethodDefinition)
                throw new InvalidOperationException("The selected method does not map to a method definition token.");

            handles.Add(handle);
        }

        if (assemblyPath is null || handles.Count == 0)
            throw new InvalidOperationException("No methods were provided to decompile.");

        CSharpDecompiler activeDecompiler = decompiler ?? CreateDecompiler(assemblyPath, useUsingDeclarations: useUsingDeclarations, showXmlDocumentation: showXmlDocumentation);
        return activeDecompiler.DecompileAsString(handles);
    }

    private static string DecompileToken(string assemblyPath, int metadataToken, bool showXmlDocumentation = false, CSharpDecompiler? decompiler = null) {
        if (metadataToken == 0)
            throw new InvalidOperationException("The selected method does not have a valid metadata token.");
        CSharpDecompiler activeDecompiler = decompiler ?? CreateDecompiler(assemblyPath, useUsingDeclarations: true, showXmlDocumentation: showXmlDocumentation);
        var handle = (EntityHandle)MetadataTokens.Handle(metadataToken);
        return handle.IsNil || handle.Kind != HandleKind.MethodDefinition
            ? throw new InvalidOperationException("The selected method does not map to a method definition token.")
            : activeDecompiler.DecompileAsString(handle);
    }

    private static CSharpDecompiler CreateDecompiler(string assemblyPath, bool useUsingDeclarations, bool showXmlDocumentation = false) {
        var settings = new DecompilerSettings {
            ThrowOnAssemblyResolveErrors = false,
            UseDebugSymbols = false,
            ShowDebugInfo = false,
            UsingDeclarations = useUsingDeclarations,
            ShowXmlDocumentation = showXmlDocumentation,
            FileScopedNamespaces = true
        };

        return DecompilerFactory.Create(assemblyPath, settings);
    }

}

namespace ISpy.Utilities;

internal static class ILSpyDecompiler {
    public static string DecompileMethod(MethodBase method) {
        string? assemblyPath = method.Module?.FullyQualifiedName ?? method.DeclaringType?.Assembly.Location;
        return string.IsNullOrEmpty(assemblyPath)
            ? throw new InvalidOperationException("Unable to determine the assembly path for the selected method.")
            : DecompileToken(assemblyPath, method.MetadataToken);
    }

    public static string DecompileType(string assemblyPath, FullTypeName fullTypeName) {
        CSharpDecompiler decompiler = CreateDecompiler(assemblyPath, useUsingDeclarations: true);
        return decompiler.DecompileTypeAsString(fullTypeName);
    }

    public static string DecompileMethods(IEnumerable<MethodBase> methods) {
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

        // When decompiling multiple methods from the same assembly, enable using declarations
        // so the output contains friendly type names and a single consolidated set of usings.
        CSharpDecompiler decompiler = CreateDecompiler(assemblyPath, useUsingDeclarations: true);
        return decompiler.DecompileAsString(handles);
    }

    private static string DecompileToken(string assemblyPath, int metadataToken) {
        if (metadataToken == 0)
            throw new InvalidOperationException("The selected method does not have a valid metadata token.");

        CSharpDecompiler decompiler = CreateDecompiler(assemblyPath, useUsingDeclarations: true);
        var handle = (EntityHandle)MetadataTokens.Handle(metadataToken);
        return handle.IsNil || handle.Kind != HandleKind.MethodDefinition
            ? throw new InvalidOperationException("The selected method does not map to a method definition token.")
            : decompiler.DecompileAsString(handle);
    }

    private static CSharpDecompiler CreateDecompiler(string assemblyPath, bool useUsingDeclarations) {
        var settings = new DecompilerSettings {
            ThrowOnAssemblyResolveErrors = false,
            UseDebugSymbols = false,
            ShowDebugInfo = false,
            UsingDeclarations = useUsingDeclarations,
        };

        return DecompilerFactory.Create(assemblyPath, settings);
    }

}

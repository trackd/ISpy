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

    internal static CSharpDecompiler CreateDecompiler(string assemblyPath, bool useUsingDeclarations, bool showXmlDocumentation = false) {
        var settings = new DecompilerSettings {
            ThrowOnAssemblyResolveErrors = false,
            UseDebugSymbols = false,
            ShowDebugInfo = false,
            UsingDeclarations = useUsingDeclarations,
            ShowXmlDocumentation = showXmlDocumentation,
            FileScopedNamespaces = true
        };

        // formatting options for Stroustrup-ish style:

        CSharpFormattingOptions fmt = settings.CSharpFormattingOptions;
        fmt.ClassBraceStyle = BraceStyle.EndOfLine;
        fmt.MethodBraceStyle = BraceStyle.EndOfLine;
        fmt.ConstructorBraceStyle = BraceStyle.EndOfLine;
        fmt.InterfaceBraceStyle = BraceStyle.EndOfLine;
        fmt.StructBraceStyle = BraceStyle.EndOfLine;
        fmt.EnumBraceStyle = BraceStyle.EndOfLine;
        fmt.StatementBraceStyle = BraceStyle.EndOfLine;
        fmt.ConstructorBraceStyle = BraceStyle.EndOfLine;
        fmt.PropertyBraceStyle = BraceStyle.EndOfLine;
        fmt.PropertyGetBraceStyle = BraceStyle.EndOfLine;
        fmt.PropertySetBraceStyle = BraceStyle.EndOfLine;
        fmt.EventBraceStyle = BraceStyle.EndOfLine;
        fmt.EventAddBraceStyle = BraceStyle.EndOfLine;
        fmt.EventRemoveBraceStyle = BraceStyle.EndOfLine;
        fmt.AnonymousMethodBraceStyle = BraceStyle.EndOfLine;
        fmt.ArrayInitializerBraceStyle = BraceStyle.EndOfLine;
        fmt.NamespaceBraceStyle = BraceStyle.EndOfLine;
        fmt.IndentationString = "    ";
        fmt.ChainedMethodCallWrapping = Wrapping.WrapIfTooLong;

        // Ensure method declaration parentheses and braces stay on same line
        fmt.MethodDeclarationClosingParenthesesOnNewLine = NewLinePlacement.SameLine;
        fmt.MethodCallClosingParenthesesOnNewLine = NewLinePlacement.SameLine;
        fmt.NewLineAferMethodDeclarationOpenParentheses = NewLinePlacement.SameLine;
        fmt.NewLineAferMethodCallOpenParentheses = NewLinePlacement.SameLine;
        fmt.MethodDeclarationParameterWrapping = Wrapping.WrapIfTooLong;
        fmt.MethodCallArgumentWrapping = Wrapping.WrapIfTooLong;
        fmt.CatchNewLinePlacement = NewLinePlacement.SameLine;

        // Indentation and block layout
        fmt.IndentBlocks = true;
        fmt.IndentBlocksInsideExpressions = true;
        fmt.IndentMethodBody = true;
        fmt.IndentClassBody = true;
        fmt.IndentStructBody = true;
        fmt.IndentEnumBody = true;
        fmt.IndentInterfaceBody = true;
        fmt.IndentNamespaceBody = true;
        fmt.IndentPropertyBody = true;
        fmt.IndentSwitchBody = true;
        fmt.IndentCaseBody = true;

        // Spacing preferences (make operators and commas readable)
        fmt.SpaceAfterMethodCallParameterComma = true;
        fmt.SpaceAfterMethodDeclarationParameterComma = true;
        fmt.SpaceAroundAssignment = true;
        fmt.SpaceAroundLogicalOperator = true;
        fmt.SpaceAroundEqualityOperator = true;
        fmt.SpaceAroundMultiplicativeOperator = true;
        fmt.SpaceAroundAdditiveOperator = true;

        // Blank lines to separate members/types for readability
        fmt.MinimumBlankLinesBetweenMembers = 1;
        fmt.MinimumBlankLinesBetweenTypes = 1;

        // Misc
        fmt.KeepCommentsAtFirstColumn = false;
        fmt.RemoveEndOfLineWhiteSpace = true;
        fmt.AlignElseInIfStatements = true;


        return DecompilerFactory.Create(assemblyPath, settings);
    }

}

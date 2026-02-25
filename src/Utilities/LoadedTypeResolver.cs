namespace ISpy.Utilities;

internal static class LoadedTypeResolver {
    private static readonly ConcurrentDictionary<Assembly, Type[]> AssemblyTypesCache = new();
    private static readonly object IndexSync = new();
    private static int _refreshQueued;

    private static TypeIndexSnapshot _snapshot = TypeIndexSnapshot.Empty;

    static LoadedTypeResolver() {
        AppDomain.CurrentDomain.AssemblyLoad += (_, _) => QueueRefresh();
        QueueRefresh();
    }

    public static IEnumerable<Type> EnumerateLoadedTypes() {
        TypeIndexSnapshot snapshot = GetSnapshot();
        foreach (Type type in snapshot.Types)
            yield return type;
    }

    public static IEnumerable<string> EnumerateLoadedTypeNames() {
        TypeIndexSnapshot snapshot = GetSnapshot();
        foreach (string typeName in snapshot.TypeNames)
            yield return typeName;
    }

    public static IEnumerable<string> EnumerateLoadedNamespaces() {
        TypeIndexSnapshot snapshot = GetSnapshot();
        foreach (string ns in snapshot.Namespaces)
            yield return ns;
    }

    public static IEnumerable<Type> FindLoadedTypesByName(string typeName) {
        if (string.IsNullOrWhiteSpace(typeName))
            yield break;

        TypeIndexSnapshot snapshot = GetSnapshot();

        string query = typeName.Trim();
        bool hasWildcard = query.IndexOfAny(['*', '?']) >= 0;
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        if (hasWildcard) {
            var matcher = new WildcardPattern(query, WildcardOptions.IgnoreCase);
            foreach (Type type in snapshot.Types) {
                string fullName = type.FullName ?? type.Name;
                if (!matcher.IsMatch(type.Name) && !matcher.IsMatch(fullName))
                    continue;

                if (!seen.Add(fullName))
                    continue;

                yield return type;
            }

            yield break;
        }

        if (snapshot.ByFullName.TryGetValue(query, out IReadOnlyList<Type>? byFullName)) {
            foreach (Type type in byFullName) {
                string fullName = type.FullName ?? type.Name;
                if (seen.Add(fullName))
                    yield return type;
            }
        }

        if (snapshot.ByName.TryGetValue(query, out IReadOnlyList<Type>? byName)) {
            foreach (Type type in byName) {
                string fullName = type.FullName ?? type.Name;
                if (seen.Add(fullName))
                    yield return type;
            }
        }
    }

    public static bool TryResolveLoadedType(string typeName, out Type? resolvedType)
        => SearchHelpers.TryFirst(FindLoadedTypesByName(typeName), out resolvedType);

    public static bool IsCompilerGenerated(Type type) {
        if (type is null)
            return false;

        // Prefer explicit attribute check where available
        try {
            if (type.IsDefined(typeof(CompilerGeneratedAttribute), false))
                return true;
        }
        catch {
            // ignore reflection issues and fall back to name checks
        }

        string name = type.Name ?? string.Empty;
        if (name.Length > 0 && name.Contains('<'))
            return true;

        string full = type.FullName ?? string.Empty;
        return full.Length > 0 && full.Contains('<');
    }

    public static bool IsCompilerGenerated(MemberInfo? member) {
        if (member is null)
            return false;

        try {
            if (member.IsDefined(typeof(CompilerGeneratedAttribute), false))
                return true;
        }
        catch {
            // ignore reflection issues and fall back to name checks
        }

        string name = member.Name ?? string.Empty;
        return name.Length > 0 && name.Contains('<');
    }

    public static TypeKind ToTypeKind(Type type) {
        if (type == typeof(void))
            return TypeKind.Void;
        if (type.IsInterface)
            return TypeKind.Interface;
        if (type.IsEnum)
            return TypeKind.Enum;
        if (type.IsPointer)
            return TypeKind.Pointer;
        if (type.IsByRef)
            return TypeKind.ByReference;
        if (type.IsArray)
            return TypeKind.Array;
        if (typeof(MulticastDelegate).IsAssignableFrom(type.BaseType))
            return TypeKind.Delegate;
        if (type.IsValueType)
            return TypeKind.Struct;
        if (type.IsClass)
            return TypeKind.Class;

        return TypeKind.Unknown;
    }

    public static ISpyTypeInfo CreateTypeInfo(Type type) {
        string fullName = type.FullName ?? type.Name;
        string? assemblyPath = null;
        if (!type.Assembly.IsDynamic) {
            try {
                assemblyPath = type.Assembly.Location;
            }
            catch {
                assemblyPath = null;
            }
        }

        return new ISpyTypeInfo {
            FullName = fullName,
            Name = type.Name,
            Namespace = type.Namespace ?? string.Empty,
            Kind = ToTypeKind(type),
            IsPublic = type.IsPublic || type.IsNestedPublic,
            IsAbstract = type.IsAbstract,
            IsSealed = type.IsSealed,
            IsInterface = type.IsInterface,
            IsEnum = type.IsEnum,
            IsClass = type.IsClass,
            IsValueType = type.IsValueType,
            IsCompilerGenerated = IsCompilerGenerated(type),
            BaseType = type.BaseType?.FullName,
            AssemblyPath = assemblyPath,
            TypeName = fullName
        };
    }

    private static TypeIndexSnapshot GetSnapshot() {
        EnsureIndexQueued();
        return Volatile.Read(ref _snapshot);
    }

    private static void EnsureIndexQueued() {
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        int stamp = ComputeAssemblyStamp(assemblies);
        TypeIndexSnapshot snapshot = Volatile.Read(ref _snapshot);
        if (snapshot.Stamp == stamp)
            return;

        if (snapshot.Types.Length == 0) {
            RefreshIndexSynchronously(assemblies, stamp);
            return;
        }

        QueueRefresh();
    }

    private static void RefreshIndexSynchronously(Assembly[] assemblies, int stamp) {
        lock (IndexSync) {
            TypeIndexSnapshot current = Volatile.Read(ref _snapshot);
            if (current.Stamp == stamp)
                return;

            TypeIndexSnapshot refreshed = BuildSnapshot(assemblies, stamp);
            Volatile.Write(ref _snapshot, refreshed);
        }
    }

    private static void QueueRefresh() {
        if (Interlocked.Exchange(ref _refreshQueued, 1) == 1)
            return;

        _ = Task.Run(RefreshIndex);
    }

    private static void RefreshIndex() {
        try {
            lock (IndexSync) {
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                int stamp = ComputeAssemblyStamp(assemblies);
                TypeIndexSnapshot current = Volatile.Read(ref _snapshot);
                if (current.Stamp == stamp)
                    return;

                TypeIndexSnapshot refreshed = BuildSnapshot(assemblies, stamp);
                Volatile.Write(ref _snapshot, refreshed);
            }
        }
        finally {
            Interlocked.Exchange(ref _refreshQueued, 0);

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            int stamp = ComputeAssemblyStamp(assemblies);
            if (Volatile.Read(ref _snapshot).Stamp != stamp)
                QueueRefresh();
        }
    }

    private static TypeIndexSnapshot BuildSnapshot(Assembly[] assemblies, int stamp) {
        List<Type> allTypes = [];
        var byFullNameBuilder = new Dictionary<string, List<Type>>(StringComparer.OrdinalIgnoreCase);
        var byNameBuilder = new Dictionary<string, List<Type>>(StringComparer.OrdinalIgnoreCase);
        var typeNameSet = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);
        var namespaceSet = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (Assembly assembly in assemblies.OrderBy(a => a.FullName, StringComparer.OrdinalIgnoreCase)) {
            foreach (Type type in GetAssemblyTypesCached(assembly)) {
                string tName = type.Name ?? string.Empty;
                string? tFull = type.FullName;
                if (IsCompilerGenerated(type)) {
                    continue;
                }

                allTypes.Add(type);

                string? fullName = type.FullName ?? type.Name;

                if (!byFullNameBuilder.TryGetValue(fullName!, out List<Type>? fullList)) {
                    fullList = [];
                    byFullNameBuilder[fullName!] = fullList;
                }
                fullList.Add(type);

                string nameKey = type.Name ?? string.Empty;
                if (!byNameBuilder.TryGetValue(nameKey, out List<Type>? nameList)) {
                    nameList = [];
                    byNameBuilder[nameKey] = nameList;
                }
                nameList.Add(type);

                typeNameSet.Add(fullName!);
                if (!string.IsNullOrWhiteSpace(type.Namespace))
                    namespaceSet.Add(type.Namespace);
            }
        }

        return new TypeIndexSnapshot(
            stamp,
            [.. allTypes],
            byFullNameBuilder.ToDictionary(kvp => kvp.Key, kvp => (IReadOnlyList<Type>)kvp.Value, StringComparer.OrdinalIgnoreCase),
            byNameBuilder.ToDictionary(kvp => kvp.Key, kvp => (IReadOnlyList<Type>)kvp.Value, StringComparer.OrdinalIgnoreCase),
            [.. typeNameSet],
            [.. namespaceSet]);
    }

    private static int ComputeAssemblyStamp(IEnumerable<Assembly> assemblies) {
        HashCode hash = new();

        foreach (Assembly assembly in assemblies.OrderBy(a => a.FullName, StringComparer.OrdinalIgnoreCase)) {
            hash.Add(assembly.FullName, StringComparer.OrdinalIgnoreCase);
            if (!assembly.IsDynamic) {
                try {
                    hash.Add(assembly.Location, StringComparer.OrdinalIgnoreCase);
                }
                catch {
                    // ignored for dynamic/special-case assemblies
                }
            }
        }

        return hash.ToHashCode();
    }

    private static Type[] GetAssemblyTypesCached(Assembly assembly) {
        return AssemblyTypesCache.GetOrAdd(assembly, a => {
            Type[] types;
            try {
                types = a.GetTypes();
            }
            catch (ReflectionTypeLoadException ex) {
                types = [.. ex.Types.Where(t => t is not null).Cast<Type>()];
            }
            catch {
                return [];
            }

            return types;
        });
    }
}

internal sealed record TypeIndexSnapshot(
    int Stamp,
    Type[] Types,
    Dictionary<string, IReadOnlyList<Type>> ByFullName,
    Dictionary<string, IReadOnlyList<Type>> ByName,
    string[] TypeNames,
    string[] Namespaces) {
    public static readonly TypeIndexSnapshot Empty = new(
        int.MinValue,
        [],
        new Dictionary<string, IReadOnlyList<Type>>(StringComparer.OrdinalIgnoreCase),
        new Dictionary<string, IReadOnlyList<Type>>(StringComparer.OrdinalIgnoreCase),
        [],
        []
    );
}

public sealed class LoadedTypeNameCompleter : IArgumentCompleter {
    public IEnumerable<CompletionResult> CompleteArgument(string commandName, string parameterName, string wordToComplete, System.Management.Automation.Language.CommandAst commandAst, IDictionary fakeBoundParameters) {
        string wildcard = string.IsNullOrWhiteSpace(wordToComplete) ? "*" : "*" + wordToComplete + "*";
        var matcher = new WildcardPattern(wildcard, WildcardOptions.IgnoreCase);

        return LoadedTypeResolver
            .EnumerateLoadedTypeNames()
            .Where(n => matcher.IsMatch(n))
            // Exclude any compiler-generated type identifiers (angle-bracket tokens)
            // .Where(n => !n.Contains('<') && !n.Contains('>'))
            // .Take(200)
            .Select(n => new CompletionResult(n, n, CompletionResultType.Type, n));
    }
}

public sealed class LoadedMethodNameCompleter : IArgumentCompleter {
    public IEnumerable<CompletionResult> CompleteArgument(string commandName, string parameterName, string wordToComplete, System.Management.Automation.Language.CommandAst commandAst, IDictionary fakeBoundParameters) {
        if (!fakeBoundParameters.Contains("TypeName"))
            return [];

        string? typeName = fakeBoundParameters["TypeName"]?.ToString();
        if (string.IsNullOrWhiteSpace(typeName) || !LoadedTypeResolver.TryResolveLoadedType(typeName, out Type? type) || type is null)
            return [];

        string wildcard = string.IsNullOrWhiteSpace(wordToComplete) ? "*" : "*" + wordToComplete + "*";
        var matcher = new WildcardPattern(wildcard, WildcardOptions.IgnoreCase);

        return type
            .GetMethodsCached()
            .Where(m => !LoadedTypeResolver.IsCompilerGenerated(m))
            .Select(m => m.Name)
            .Where(n => matcher.IsMatch(n))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(n => n, StringComparer.OrdinalIgnoreCase)
            .Select(n => new CompletionResult(n, n, CompletionResultType.Method, n));
    }
}

public sealed class LoadedNamespaceCompleter : IArgumentCompleter {
    public IEnumerable<CompletionResult> CompleteArgument(string commandName, string parameterName, string wordToComplete, System.Management.Automation.Language.CommandAst commandAst, IDictionary fakeBoundParameters) {
        string wildcard = string.IsNullOrWhiteSpace(wordToComplete) ? "*" : "*" + wordToComplete + "*";
        var matcher = new WildcardPattern(wildcard, WildcardOptions.IgnoreCase);

        return LoadedTypeResolver
            .EnumerateLoadedNamespaces()
            .Where(n => matcher.IsMatch(n))
            // .Take(200)
            .Select(n => new CompletionResult(n, n, CompletionResultType.Namespace, n));
    }
}

file static class LoadedMethodNameCompleterExtensions {
    public static IEnumerable<MethodInfo> GetMethodsCached(this Type type)
        => ReflectionCache.GetMethods(type, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
}

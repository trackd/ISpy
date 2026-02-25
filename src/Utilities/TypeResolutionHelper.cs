namespace ISpy.Utilities;

internal sealed record ResolvedMethodTarget(MethodBase Method, string AssemblyPath);

internal static class TypeResolutionHelper {
    private const BindingFlags MethodFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
    private static readonly string[] DefaultCmdletMethods = ["ProcessRecord", "BeginProcessing", "EndProcessing"];

    public static IEnumerable<ResolvedMethodTarget> Resolve(object? input, string? methodName) {
        if (input is null)
            yield break;

        if (input is PSObject psObject && psObject.BaseObject is not null) {
            foreach (ResolvedMethodTarget candidate in Resolve(psObject.BaseObject, methodName))
                yield return candidate;
            yield break;
        }

        if (input is PSMethod psMethod) {
            foreach (MethodBase overloadMethod in GetPSMethodOverloads(psMethod)) {
                if (!IsNameMatch(overloadMethod, methodName))
                    continue;

                ResolvedMethodTarget? resolved = CreateTarget(overloadMethod);
                if (resolved is not null)
                    yield return resolved;
            }
            yield break;
        }

        if (input is IEnumerable enumerable and not string) {
            foreach (object? item in enumerable) {
                foreach (ResolvedMethodTarget candidate in Resolve(item, methodName))
                    yield return candidate;
            }
            yield break;
        }
        if (TryExtractMethod(input, out MethodBase? method) && method is not null && IsNameMatch(method, methodName)) {
            ResolvedMethodTarget? resolved = CreateTarget(method);
            if (resolved is not null)
                yield return resolved;
            yield break;
        }

        if (input is CommandInfo command) {
            foreach (ResolvedMethodTarget candidate in ResolveCommand(command, methodName))
                yield return candidate;
            yield break;
        }

        if (input is Type type) {
            foreach (ResolvedMethodTarget candidate in ResolveType(type, methodName))
                yield return candidate;
        }
    }

    private static IEnumerable<ResolvedMethodTarget> ResolveCommand(CommandInfo command, string? methodName) {
        while (command is AliasInfo alias && alias.ResolvedCommand is not null)
            command = alias.ResolvedCommand;

        if (command is CmdletInfo cmdlet) {
            foreach (ResolvedMethodTarget candidate in ResolveType(cmdlet.ImplementingType, methodName))
                yield return candidate;
        }
    }

    private static IEnumerable<ResolvedMethodTarget> ResolveType(Type? type, string? methodName) {
        if (type is null)
            yield break;

        if (string.IsNullOrEmpty(methodName)) {
            foreach (string methodCandidate in DefaultCmdletMethods) {
                MethodBase? candidate = type.GetMethod(methodCandidate, MethodFlags);
                ResolvedMethodTarget? resolved = CreateTarget(candidate);
                if (resolved is not null)
                    yield return resolved;
            }

            yield break;
        }

        foreach (MethodInfo methodCandidate in ReflectionCache.GetMethods(type, MethodFlags)) {
            if (!IsNameMatch(methodCandidate, methodName))
                continue;

            ResolvedMethodTarget? resolved = CreateTarget(methodCandidate);
            if (resolved is not null)
                yield return resolved;
        }
    }

    private static bool IsNameMatch(MethodBase method, string? methodName)
        => string.IsNullOrEmpty(methodName) || string.Equals(method.Name, methodName, StringComparison.OrdinalIgnoreCase);

    private static ResolvedMethodTarget? CreateTarget(MethodBase? method) {
        if (method is null)
            return null;

        string? assemblyPath = method.Module?.FullyQualifiedName ?? method.DeclaringType?.Assembly.Location;
        return string.IsNullOrEmpty(assemblyPath) ? null : new ResolvedMethodTarget(method, assemblyPath);
    }

    private static bool TryExtractMethod(object input, out MethodBase? method) {
        method = input as MethodBase;
        if (method is not null)
            return true;

        if (TryGetPropertyValue(input, "Method", out object? extracted) && extracted is MethodBase methodProperty) {
            method = methodProperty;
            return true;
        }

        if (TryGetPropertyValue(input, "Value", out extracted)) {
            if (extracted is MethodBase methodValue) {
                method = methodValue;
                return true;
            }

            if (extracted is Delegate delegateValue) {
                method = delegateValue.Method;
                return true;
            }
        }

        if (TryGetPropertyValue(input, "BaseObject", out extracted) && extracted is not null && !ReferenceEquals(extracted, input))
            return TryExtractMethod(extracted, out method);

        method = null;
        return false;
    }

    private static IEnumerable<MethodBase> GetPSMethodOverloads(PSMethod psMethod) {
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
        FieldInfo? adapterDataField = psMethod.GetType().GetField("adapterData", flags);
        if (adapterDataField is null)
            yield break;

        object? adapterData = adapterDataField.GetValue(psMethod);
        if (adapterData is null)
            yield break;

        FieldInfo? structuresField = adapterData.GetType().GetField("methodInformationStructures", flags);
        if (structuresField is null)
            yield break;

        if (structuresField.GetValue(adapterData) is not IEnumerable structures)
            yield break;

        foreach (object? structure in structures) {
            if (structure is null)
                continue;

            FieldInfo? methodField = structure.GetType().GetField("method", flags);
            if (methodField?.GetValue(structure) is MethodBase method)
                yield return method;
        }
    }

    private static bool TryGetPropertyValue(object target, string propertyName, out object? value) {
        value = null;
        Type type = target.GetType();
        PropertyInfo? property = ReflectionCache.GetProperty(type, propertyName);
        if (property is null)
            return false;

        value = property.GetValue(target);
        return value is not null;
    }
}

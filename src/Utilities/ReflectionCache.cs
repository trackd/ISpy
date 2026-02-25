
namespace ISpy.Utilities;

internal static class ReflectionCache {
    public static bool IsRecord(Type type) {
        // Records have compiler-generated EqualityContract property and specific clone method
        return type.GetMethod("<Clone>$", BindingFlags.Public | BindingFlags.Instance) != null ||
                type.GetProperty("EqualityContract", BindingFlags.NonPublic | BindingFlags.Instance) != null;
    }
    private static readonly ConcurrentDictionary<Type, PropertyInfo[]> _propertiesCache = new();
    private static readonly ConcurrentDictionary<(Type Type, BindingFlags Flags), MethodInfo[]> _methodsCache = new();
    private static readonly ConcurrentDictionary<(Type Type, string PropertyName), PropertyInfo?> _propertyLookupCache = new();

    public static PropertyInfo[] GetPublicInstanceProperties(Type type) {
        return _propertiesCache.GetOrAdd(type, t => {
            try {
                PropertyInfo[] props = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                // For records, maintain declaration order (positional parameters come first)
                return IsRecord(t) ? [.. props.OrderBy(p => p.MetadataToken)] : props;
            }
            catch {
                return [];
            }
        });
    }

    public static MethodInfo[] GetMethods(Type type, BindingFlags flags) {
        return _methodsCache.GetOrAdd((type, flags), key => {
            try {
                return key.Type.GetMethods(key.Flags);
            }
            catch {
                return [];
            }
        });
    }

    public static PropertyInfo? GetProperty(Type type, string propertyName) {
        return _propertyLookupCache.GetOrAdd((type, propertyName), key => {
            try {
                return key.Type.GetProperty(key.PropertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            }
            catch {
                return null;
            }
        });
    }

    public static void Clear() {
        _propertiesCache.Clear();
        _methodsCache.Clear();
        _propertyLookupCache.Clear();
    }
}

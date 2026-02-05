
namespace ISpy.Utilities;

internal static class ReflectionCache {

    public static bool IsRecord(Type type) {
        // Records have compiler-generated EqualityContract property and specific clone method
        return type.GetMethod("<Clone>$", BindingFlags.Public | BindingFlags.Instance) != null ||
                type.GetProperty("EqualityContract", BindingFlags.NonPublic | BindingFlags.Instance) != null;
    }
    private static readonly ConcurrentDictionary<Type, PropertyInfo[]> _propertiesCache = new();

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

    public static void Clear() => _propertiesCache.Clear();
}

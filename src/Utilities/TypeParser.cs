

namespace ISpy.Utilities;

public static class TypesParser {
    public static HashSet<TypeKind> ParseSelection(string[] values) {
        var possibleValues = new Dictionary<string, TypeKind>(StringComparer.OrdinalIgnoreCase) {
            ["class"] = TypeKind.Class,
            ["struct"] = TypeKind.Struct,
            ["interface"] = TypeKind.Interface,
            ["enum"] = TypeKind.Enum,
            ["delegate"] = TypeKind.Delegate
        };
        var kinds = new HashSet<TypeKind>();
        if (values.Length == 1 && !possibleValues.Keys.Any(v => values[0].StartsWith(v, StringComparison.OrdinalIgnoreCase))) {
            foreach (char ch in values[0]) {
                switch (ch) {
                    case 'c':
                        kinds.Add(TypeKind.Class);
                        break;
                    case 'i':
                        kinds.Add(TypeKind.Interface);
                        break;
                    case 's':
                        kinds.Add(TypeKind.Struct);
                        break;
                    case 'd':
                        kinds.Add(TypeKind.Delegate);
                        break;
                    case 'e':
                        kinds.Add(TypeKind.Enum);
                        break;
                    default:
                        break;
                }
            }
        }
        else {
            foreach (string value in values) {
                string v = value;
                while (v.Length > 0 && !possibleValues.ContainsKey(v))
                    v = v[..^1];
                if (possibleValues.TryGetValue(v, out TypeKind kind))
                    kinds.Add(kind);
            }
        }
        return kinds;
    }
}

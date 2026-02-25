namespace ISpy.Utilities;

internal static class SearchHelpers {
    public static bool TryFirst<T>(IEnumerable<T> source, out T? result) {
        foreach (T item in source) {
            result = item;
            return true;
        }

        result = default;
        return false;
    }

    public static bool TryFirst<T>(IEnumerable<T> source, Func<T, bool> predicate, out T? result) {
        foreach (T item in source) {
            if (!predicate(item))
                continue;

            result = item;
            return true;
        }

        result = default;
        return false;
    }
}

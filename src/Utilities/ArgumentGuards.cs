using System.Diagnostics.CodeAnalysis;

namespace ISpy.Utilities;

internal static class ArgumentGuards {
#if NET8_0_OR_GREATER
    public static void ThrowIfNull([NotNull] object? value, string paramName)
        => ArgumentNullException.ThrowIfNull(value, paramName);

    public static void ThrowIfNullOrWhiteSpace([NotNull] string? value, string paramName)
        => ArgumentException.ThrowIfNullOrWhiteSpace(value, paramName);
#else
#pragma warning disable CS8777
    public static void ThrowIfNull([NotNull] object? value, string paramName) {
        if (value is not null) {
            return;
        }

        throw new ArgumentNullException(paramName);
    }

    public static void ThrowIfNullOrWhiteSpace([NotNull] string? value, string paramName) {
        if (!string.IsNullOrWhiteSpace(value)) {
            return;
        }

        throw new ArgumentException("Value cannot be null or whitespace.", paramName);
    }
#pragma warning restore CS8777
#endif
}

namespace ISpy.Utilities;

public static class SourceOutputFactory {
    // Build output with PSChildName for downstream file-oriented processing.
    public static PSObject Create(string source, string fileBaseName, string extension = ".cs") {
        if (string.IsNullOrEmpty(source))
            throw new ArgumentException("Source cannot be null or empty.", nameof(source));

        if (string.IsNullOrEmpty(fileBaseName))
            fileBaseName = "decompiled";

        string safeName = GetSafeFileName(fileBaseName) + extension;
        var wrappedSource = PSObject.AsPSObject(source);
        wrappedSource.Properties.Add(new PSNoteProperty("PSChildName", safeName));
        return wrappedSource;
    }

    // Derive a reasonable file name from a type full name.
    public static string GetFileBaseNameFromTypeName(string? declaringTypeFullName) {
        return string.IsNullOrEmpty(declaringTypeFullName)
            ? "decompiled"
            : declaringTypeFullName.Contains('.')
            ? declaringTypeFullName[(declaringTypeFullName.LastIndexOf('.') + 1)..]
            : declaringTypeFullName;
    }

    // Build output using a type name as the file base name.
    public static PSObject CreateFromTypeName(string source, string? declaringTypeFullName, string extension = ".cs") {
        string fileBaseName = GetFileBaseNameFromTypeName(declaringTypeFullName);
        return Create(source, fileBaseName, extension);
    }

    private static string GetSafeFileName(string fileName) =>
        string.Join("_", fileName.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries));
}

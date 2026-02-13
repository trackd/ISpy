namespace ISpy.Models;

public class ISpyExportResult {
    public string TypeName { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;

}

namespace ISpy.Cmdlets;

[Cmdlet(VerbsCommon.Get, "Framework")]
[OutputType(typeof(string))]
public class GetTargetFramework : PSCmdlet {
    [Parameter(Position = 0, Mandatory = true)]
    public CSharpDecompiler? Decompiler { get; set; }

    protected override void ProcessRecord() {
        if (Decompiler is null) {
            WriteError(new ErrorRecord(new ArgumentNullException(nameof(Decompiler)), "DecompilerNotFound", ErrorCategory.InvalidArgument, null));
            return;
        }

        MetadataFile module = Decompiler.TypeSystem.MainModule.MetadataFile;
        WriteObject(module.Metadata.DetectTargetFrameworkId());
    }
}

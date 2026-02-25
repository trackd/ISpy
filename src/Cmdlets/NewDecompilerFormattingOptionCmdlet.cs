namespace ISpy.Cmdlets;

[Cmdlet(VerbsCommon.New, "DecompilerFormattingOption")]
[OutputType(typeof(CSharpFormattingOptions))]
public class NewDecompilerFormattingOptionCmdlet : PSCmdlet, IDynamicParameters {

    private RuntimeDefinedParameterDictionary? dynamicParameters;
    public object GetDynamicParameters() {
        dynamicParameters ??= CSharpFormattingOptionsDynamicParameters.CreateRuntimeParameters();
        return dynamicParameters;
    }

    protected override void EndProcessing() {
        CSharpFormattingOptions options = CSharpFormattingOptionsDynamicParameters.CreateFromBoundParameters(MyInvocation.BoundParameters);
        WriteObject(options);
    }
}

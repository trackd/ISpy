namespace ISpy.Cmdlets;

[Cmdlet(VerbsCommon.New, "DecompilerSetting")]
[OutputType(typeof(DecompilerSettings))]
public class NewDecompilerSettingCmdlet : PSCmdlet, IDynamicParameters {

    private RuntimeDefinedParameterDictionary? dynamicParameters;

    [Parameter(HelpMessage = "C# Language version to be used by the decompiler")]
    public LanguageVersion LanguageVersion { get; set; } = LanguageVersion.Latest;

    [Parameter]
    public CSharpFormattingOptions? CSharpFormattingOptions { get; set; }
    public object GetDynamicParameters() {
        dynamicParameters ??= DecompilerSettingsDynamicParameters.CreateSwitchParameters();
        return dynamicParameters;
    }

    protected override void EndProcessing() {

        var settings = new DecompilerSettings(LanguageVersion) {
            ThrowOnAssemblyResolveErrors = false,
            UseDebugSymbols = false,
            ShowDebugInfo = false,
            UsingDeclarations = true,
        };

        if (CSharpFormattingOptions is not null)
            settings.CSharpFormattingOptions = CSharpFormattingOptions;

        DecompilerSettingsDynamicParameters.ApplyBoundSwitches(MyInvocation.BoundParameters, settings);
        WriteObject(settings);
    }
}

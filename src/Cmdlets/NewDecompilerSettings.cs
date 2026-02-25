namespace ISpy.Cmdlets;

[Cmdlet(VerbsCommon.New, "DecompilerSetting")]
[OutputType(typeof(DecompilerSettings))]
public class NewDecompilerSettingCmdlet : PSCmdlet, IDynamicParameters {

    private RuntimeDefinedParameterDictionary? dynamicParameters;

    [Parameter(HelpMessage = "C# Language version to be used by the decompiler")]
    public LanguageVersion LanguageVersion { get; set; } = LanguageVersion.Latest;

    [Parameter(
        HelpMessage = "Use New-DecompilerFormattingOption to create a formatting options object with specific settings."
    )]
    [Alias("CSharpFormattingOptions")]
    public CSharpFormattingOptions? DecompilerFormattingOption { get; set; }
    public object GetDynamicParameters() {
        dynamicParameters ??= DecompilerSettingsDynamicParameters.CreateSwitchParameters();
        return dynamicParameters;
    }

    protected override void EndProcessing() {

        var settings = new DecompilerSettings(LanguageVersion);

        if (DecompilerFormattingOption is not null)
            settings.CSharpFormattingOptions = DecompilerFormattingOption;

        DecompilerSettingsDynamicParameters.ApplyBoundSwitches(MyInvocation.BoundParameters, settings);
        WriteObject(settings);
    }
}

namespace ISpy.Utilities;

internal static class CSharpFormattingOptionsDynamicParameters {
    private static readonly Lazy<Dictionary<string, PropertyInfo>> BoolFormattingProperties =
        new(CreateBoolFormattingProperties);

    public static RuntimeDefinedParameterDictionary CreateSwitchParameters() {
        var parameters = new RuntimeDefinedParameterDictionary();

        foreach (KeyValuePair<string, PropertyInfo> property in BoolFormattingProperties.Value) {
            var attributes = new System.Collections.ObjectModel.Collection<Attribute> {
                new ParameterAttribute {
                    Mandatory = false,
                    HelpMessage = $"Set CSharpFormattingOptions.{property.Key} to $true."
                }
            };

            var dynamicParameter = new RuntimeDefinedParameter(property.Key, typeof(SwitchParameter), attributes);
            parameters[property.Key] = dynamicParameter;
        }

        return parameters;
    }

    public static CSharpFormattingOptions CreateFromBoundParameters(IDictionary boundParameters) {
        ArgumentNullException.ThrowIfNull(boundParameters);

        // CSharpFormattingOptions has no public parameterless constructor.
        // Clone a baseline instance from DecompilerSettings and then apply bound values.
        CSharpFormattingOptions options = new DecompilerSettings().CSharpFormattingOptions.Clone();

        ApplyBoundValues(boundParameters, options);
        return options;
    }

    private static void ApplyBoundValues(IDictionary boundParameters, CSharpFormattingOptions options) {
        ArgumentNullException.ThrowIfNull(boundParameters);
        ArgumentNullException.ThrowIfNull(options);

        foreach (KeyValuePair<string, PropertyInfo> property in BoolFormattingProperties.Value) {
            if (!boundParameters.Contains(property.Key))
                continue;

            object? rawValue = boundParameters[property.Key];
            bool value = rawValue switch {
                SwitchParameter switchValue => switchValue.IsPresent,
                bool boolValue => boolValue,
                _ => false,
            };

            property.Value.SetValue(options, value);
        }
    }

    private static Dictionary<string, PropertyInfo> CreateBoolFormattingProperties() {
        var properties = new Dictionary<string, PropertyInfo>(StringComparer.OrdinalIgnoreCase);

        foreach (PropertyInfo property in typeof(CSharpFormattingOptions).GetProperties(BindingFlags.Instance | BindingFlags.Public)) {
            if (property.PropertyType != typeof(bool) || !property.CanWrite || property.SetMethod is null)
                continue;

            properties[property.Name] = property;
        }

        return properties;
    }
}

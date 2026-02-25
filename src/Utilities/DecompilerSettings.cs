namespace ISpy.Utilities;

internal static class DecompilerSettingsDynamicParameters {
    private static readonly Lazy<Dictionary<string, PropertyInfo>> BoolSettingsProperties =
        new(CreateBoolSettingsProperties);

    public static RuntimeDefinedParameterDictionary CreateSwitchParameters() {
        var parameters = new RuntimeDefinedParameterDictionary();

        foreach (KeyValuePair<string, PropertyInfo> property in BoolSettingsProperties.Value) {
            var attributes = new Collection<Attribute> {
                new ParameterAttribute {
                    Mandatory = false,
                    HelpMessage = $"Set DecompilerSettings.{property.Key} to $true."
                }
            };

            parameters[property.Key] = new RuntimeDefinedParameter(property.Key, typeof(SwitchParameter), attributes);
        }

        return parameters;
    }

    public static void ApplyBoundSwitches(IDictionary boundParameters, DecompilerSettings settings) {
        ArgumentNullException.ThrowIfNull(boundParameters);
        ArgumentNullException.ThrowIfNull(settings);

        foreach (KeyValuePair<string, PropertyInfo> property in BoolSettingsProperties.Value) {
            if (!boundParameters.Contains(property.Key))
                continue;

            object? rawValue = boundParameters[property.Key];
            bool value = rawValue switch {
                SwitchParameter switchValue => switchValue.IsPresent,
                bool boolValue => boolValue,
                _ => false,
            };

            property.Value.SetValue(settings, value);
        }
    }

    private static Dictionary<string, PropertyInfo> CreateBoolSettingsProperties() {
        var properties = new Dictionary<string, PropertyInfo>(StringComparer.OrdinalIgnoreCase);

        foreach (PropertyInfo property in typeof(DecompilerSettings).GetProperties(BindingFlags.Instance | BindingFlags.Public)) {
            if (property.PropertyType != typeof(bool) || !property.CanWrite || property.SetMethod is null)
                continue;

            properties[property.Name] = property;
        }

        return properties;
    }
}

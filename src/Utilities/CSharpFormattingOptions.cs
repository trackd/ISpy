namespace ISpy.Utilities;

internal static class CSharpFormattingOptionsDynamicParameters {
    private static readonly Lazy<Dictionary<string, PropertyInfo>> FormattingProperties =
        new(CreateFormattingProperties);

    public static RuntimeDefinedParameterDictionary CreateRuntimeParameters() {
        var parameters = new RuntimeDefinedParameterDictionary();

        foreach (KeyValuePair<string, PropertyInfo> property in FormattingProperties.Value) {
            var attributes = new Collection<Attribute> {
                new ParameterAttribute {
                    Mandatory = false,
                    HelpMessage = BuildHelpMessage(property.Value)
                }
            };

            Type parameterType = property.Value.PropertyType == typeof(bool)
                ? typeof(SwitchParameter)
                : property.Value.PropertyType;

            if (property.Value.PropertyType.IsEnum)
                attributes.Add(new ValidateSetAttribute(Enum.GetNames(property.Value.PropertyType)));

            parameters[property.Key] = new RuntimeDefinedParameter(property.Key, parameterType, attributes);
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

        foreach (KeyValuePair<string, PropertyInfo> property in FormattingProperties.Value) {
            if (!boundParameters.Contains(property.Key))
                continue;

            object? rawValue = boundParameters[property.Key];
            object? value = ConvertBoundValue(rawValue, property.Value.PropertyType);
            property.Value.SetValue(options, value);
        }
    }

    private static Dictionary<string, PropertyInfo> CreateFormattingProperties() {
        var properties = new Dictionary<string, PropertyInfo>(StringComparer.OrdinalIgnoreCase);

        foreach (PropertyInfo property in typeof(CSharpFormattingOptions).GetProperties(BindingFlags.Instance | BindingFlags.Public)) {
            if (!property.CanWrite || property.SetMethod is null)
                continue;

            properties[property.Name] = property;
        }

        return properties;
    }

    private static object? ConvertBoundValue(object? rawValue, Type targetType) {
        if (targetType == typeof(bool)) {
            return rawValue switch {
                SwitchParameter switchValue => switchValue.IsPresent,
                bool boolValue => boolValue,
                _ => false,
            };
        }

        if (rawValue is null)
            return null;

        if (targetType.IsInstanceOfType(rawValue))
            return rawValue;

        if (targetType.IsEnum)
            return rawValue is string enumName
                ? Enum.Parse(targetType, enumName, ignoreCase: true)
                : Enum.ToObject(targetType, rawValue);

        return Convert.ChangeType(rawValue, targetType, CultureInfo.InvariantCulture);
    }

    private static string BuildHelpMessage(PropertyInfo property)
        => property.PropertyType == typeof(bool)
            ? $"Set CSharpFormattingOptions.{property.Name} to $true."
            : $"Set CSharpFormattingOptions.{property.Name} ({property.PropertyType.Name}).";
}

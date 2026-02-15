using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Nuclei.Core.Config.Datasources;

/// <summary>
///     Parses datasource configuration JSON into strongly typed definitions.
/// </summary>
public static class DatasourceConfigParser
{
    /// <summary>
    ///     The name of the default datasource configuration.
    /// </summary>
    private const string DefaultName = "*";

    /// <summary>
    ///     Parses the given JSON string into a <see cref="DatasourceConfigSet" /> containing all datasource configurations.
    /// </summary>
    /// <param name="json">
    ///     The JSON string containing datasource configurations. Must be a JSON object where each property is
    ///     a datasource name and its value is the configuration object.
    /// </param>
    /// <returns> A <see cref="DatasourceConfigSet" /> containing all parsed datasource configurations.</returns>
    public static DatasourceConfigSet Parse(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            throw new ArgumentException("Datasource configuration JSON cannot be empty.");

        using var document = JsonDocument.Parse(json);
        if (document.RootElement.ValueKind != JsonValueKind.Object)
            throw new FormatException("Datasource configuration must be a JSON object.");

        var configs = new Dictionary<string, DatasourceConfig>(StringComparer.OrdinalIgnoreCase);

        foreach (var property in document.RootElement.EnumerateObject())
        {
            var name = property.Name;
            if (property.Value.ValueKind != JsonValueKind.Object)
                throw new FormatException($"Datasource '{name}' must be a JSON object.");

            configs[name] = ParseDatasource(name, property.Value);
        }

        return new DatasourceConfigSet(configs, DefaultName);
    }

    private static DatasourceConfig ParseDatasource(string name, JsonElement element)
    {
        var host = GetRequiredString(element, "host");
        var pooling = GetOptionalBool(element, "pooling", true);
        var timeoutSeconds = GetOptionalInt(element, "timeoutMillis", 30000);
        var connectTimeoutSeconds = GetOptionalInt(element, "connectTimeoutMillis", 5000);
        var readOnly = GetOptionalBool(element, "readOnly", false);
        var options = GetOptions(element);

        if (timeoutSeconds <= 0)
            throw new FormatException($"Datasource '{name}' timeoutSeconds must be greater than 0.");
        if (connectTimeoutSeconds <= 0)
            throw new FormatException($"Datasource '{name}' connectTimeoutSeconds must be greater than 0.");

        return new DatasourceConfig(
            name,
            host,
            pooling,
            timeoutSeconds,
            connectTimeoutSeconds,
            readOnly,
            options);
    }

    private static string GetRequiredString(JsonElement element, string property)
    {
        if (!element.TryGetProperty(property, out var value) || value.ValueKind != JsonValueKind.String)
            throw new FormatException($"Datasource entry is missing required string property '{property}'.");

        var result = value.GetString() ?? string.Empty;
        return string.IsNullOrWhiteSpace(result)
            ? throw new FormatException($"Datasource property '{property}' cannot be empty.")
            : result;
    }

    private static bool GetOptionalBool(JsonElement element, string property, bool defaultValue)
    {
        if (!element.TryGetProperty(property, out var value))
            return defaultValue;

        return value.ValueKind switch
        {
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            _ => throw new FormatException($"Datasource property '{property}' must be a boolean.")
        };
    }

    private static int GetOptionalInt(JsonElement element, string property, int defaultValue)
    {
        if (!element.TryGetProperty(property, out var value))
            return defaultValue;

        if (value.ValueKind != JsonValueKind.Number || !value.TryGetInt32(out var result))
            throw new FormatException($"Datasource property '{property}' must be an integer.");

        return result;
    }

    private static IReadOnlyDictionary<string, string> GetOptions(JsonElement element)
    {
        if (!element.TryGetProperty("options", out var optionsElement) ||
            optionsElement.ValueKind == JsonValueKind.Null)
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        if (optionsElement.ValueKind != JsonValueKind.Object)
            throw new FormatException("Datasource options must be a JSON object.");

        var options = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var option in optionsElement.EnumerateObject())
            options[option.Name] = ConvertOptionValue(option.Value);

        return options;
    }

    private static string ConvertOptionValue(JsonElement value)
    {
        return value.ValueKind switch
        {
            JsonValueKind.String => value.GetString() ?? string.Empty,
            JsonValueKind.Number => value.GetRawText(),
            JsonValueKind.True => "true",
            JsonValueKind.False => "false",
            JsonValueKind.Null => string.Empty,
            _ => throw new FormatException("Datasource option values must be string, number, boolean, or null.")
        };
    }
}
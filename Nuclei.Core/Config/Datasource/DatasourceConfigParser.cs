using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Nuclei.Core.Config.Datasource;

/// <summary>
///     Parses datasource configuration JSON into strongly typed definitions.
/// </summary>
public static class DatasourceConfigParser
{
    /// <summary>
    ///     The name of the default datasource configuration.
    /// </summary>
    private const string DefaultName = "*";
    private const string BindingsProperty = "bindings";
    private const string SourcesProperty = "sources";

    /// <summary>
    ///     Parses the given JSON string into a <see cref="DatasourceCatalogue" /> containing all datasource configurations.
    /// </summary>
    /// <param name="json">
    ///     The JSON string containing datasource configurations. Must be a JSON object where each property is
    ///     a datasource name and its value is the configuration object.
    /// </param>
    /// <returns> A <see cref="DatasourceCatalogue" /> containing all parsed datasource configurations.</returns>
    public static DatasourceCatalogue Parse(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            throw new ArgumentException("Datasource configuration JSON cannot be empty.");

        using var document = JsonDocument.Parse(json);
        if (document.RootElement.ValueKind != JsonValueKind.Object)
            throw new FormatException("Datasource configuration must be a JSON object.");

        var root = document.RootElement;

        var sourcesElement = GetRequiredObject(root, SourcesProperty);
        var sources = ParseSources(sourcesElement);

        var bindingsElement = GetRequiredObject(root, BindingsProperty);
        var bindings = ParseBindings(bindingsElement, sources);

        return new DatasourceCatalogue(bindings, sources, DefaultName);
    }

    private static IReadOnlyDictionary<string, DatasourceConfig> ParseSources(JsonElement sourcesElement)
    {
        var sources = new Dictionary<string, DatasourceConfig>(StringComparer.OrdinalIgnoreCase);
        foreach (var property in sourcesElement.EnumerateObject())
        {
            var name = property.Name;
            if (property.Value.ValueKind != JsonValueKind.Object)
                throw new FormatException($"Datasource source '{name}' must be a JSON object.");

            sources[name] = ParseDatasource(name, property.Value);
        }

        return sources;
    }

    private static IReadOnlyDictionary<string, DatasourceBindingConfig> ParseBindings(
        JsonElement bindingsElement,
        IReadOnlyDictionary<string, DatasourceConfig> sources)
    {
        var bindings = new Dictionary<string, DatasourceBindingConfig>(StringComparer.OrdinalIgnoreCase);
        foreach (var property in bindingsElement.EnumerateObject())
        {
            var name = property.Name;
            if (property.Value.ValueKind != JsonValueKind.Object)
                throw new FormatException($"Datasource binding '{name}' must be a JSON object.");

            var binding = ParseBinding(name, property.Value, sources);
            bindings[name] = binding;
        }

        return bindings;
    }

    private static DatasourceBindingConfig ParseBinding(
        string name,
        JsonElement element,
        IReadOnlyDictionary<string, DatasourceConfig> sources)
    {
        var writeSources = ParseBindingSources(element, "write", sources, name, true, null);
        var readSources = ParseBindingSources(element, "read", sources, name, false, writeSources);
        return new DatasourceBindingConfig(name, writeSources, readSources);
    }

    private static IReadOnlyList<string> ParseBindingSources(
        JsonElement element,
        string propertyName,
        IReadOnlyDictionary<string, DatasourceConfig> sources,
        string bindingName,
        bool required,
        IReadOnlyList<string>? fallback)
    {
        if (!element.TryGetProperty(propertyName, out var propertyElement))
        {
            if (required)
                throw new FormatException($"Datasource binding '{bindingName}' must define a {propertyName} source.");

            return fallback is null ? Array.Empty<string>() : new List<string>(fallback);
        }

        var result = new List<string>();
        switch (propertyElement.ValueKind)
        {
            case JsonValueKind.String:
                AddBindingSource(result, propertyElement.GetString(), sources, bindingName, propertyName);
                break;
            case JsonValueKind.Array:
                foreach (var item in propertyElement.EnumerateArray())
                {
                    if (item.ValueKind != JsonValueKind.String)
                        throw new FormatException($"Datasource binding '{bindingName}' {propertyName} sources must be strings.");

                    AddBindingSource(result, item.GetString(), sources, bindingName, propertyName);
                }
                break;
            default:
                throw new FormatException($"Datasource binding '{bindingName}' {propertyName} sources must be a string or array.");
        }

        if (result.Count != 0)
            return result;

        if (required)
            throw new FormatException($"Datasource binding '{bindingName}' must define at least one {propertyName} source.");

        return fallback ?? [];
    }

    private static void AddBindingSource(
        ICollection<string> result,
        string? sourceName,
        IReadOnlyDictionary<string, DatasourceConfig> sources,
        string bindingName,
        string role)
    {
        if (string.IsNullOrWhiteSpace(sourceName))
            throw new FormatException($"Datasource binding '{bindingName}' {role} source cannot be empty.");

        if (!sources.ContainsKey(sourceName!))
            throw new FormatException($"Datasource binding '{bindingName}' references unknown {role} source '{sourceName}'.");

        result.Add(sourceName!);
    }

    private static JsonElement GetRequiredObject(JsonElement root, string property)
    {
        if (!root.TryGetProperty(property, out var element) || element.ValueKind != JsonValueKind.Object)
            throw new FormatException($"Datasource configuration must contain a '{property}' object.");

        return element;
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
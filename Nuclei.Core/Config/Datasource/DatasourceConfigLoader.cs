using System;
using System.IO;
using Nuclei.Abstractions.BepInEx.Config;
using Nuclei.Abstractions.BepInEx.Logging;
using Nuclei.Abstractions.Nuclei.Decorators;

namespace Nuclei.Core.Config.Datasource;

/// <summary>
///     Loads datasource configuration from the config system.
/// </summary>
public static class DatasourceConfigLoader
{
    private const string SectionName = "General";
    private const string FileKeyName = "DatasourcesFile";

    private const string DefaultJson =
        "{\n" +
        "  \"bindings\": {\n" +
        "    \"*\": {\n" +
        "      \"write\": \"default\",\n" +
        "      \"read\": \"default\"\n" +
        "    }\n" +
        "  },\n" +
        "  \"sources\": {\n" +
        "    \"default\": {\n" +
        "      \"host\": \"sqlite://nuclei.db\",\n" +
        "      \"pooling\": true,\n" +
        "      \"timeoutMillis\": 30000,\n" +
        "      \"connectTimeoutMillis\": 5000,\n" +
        "      \"readOnly\": false,\n" +
        "      \"options\": {}\n" +
        "    }\n" +
        "  }\n" +
        "}";

    /// <summary>
    ///     Loads datasource configuration from the provided config provider and logger.
    /// </summary>
    public static DatasourceCatalogue Load(IConfigProvider configProvider, ILogger logger)
    {
        var datasourceLogger = logger.WithScope(nameof(DatasourceConfigLoader));

        var fileEntry = configProvider.Bind(
            SectionName,
            FileKeyName,
            "datasources.json",
            "Path to a datasource JSON file. Relative paths are resolved against the config directory.");

        datasourceLogger.Debug("Loading datasource configuration...");

        try
        {
            var json = ResolveDatasourceJson(fileEntry.Value, configProvider.ConfigDirectory, datasourceLogger);
            var result = DatasourceConfigParser.Parse(json);
            datasourceLogger.Debug($"Loaded {result.Bindings.Count} binding(s) and {result.Sources.Count} source(s).");
            return result;
        }
        catch (Exception exception)
        {
            datasourceLogger.Error("Failed to parse datasource configuration.", exception);
            throw;
        }
    }

    private static string ResolveDatasourceJson(string filePath, string configDirectory, ILogger logger)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new FormatException("Datasource file path cannot be empty.");

        var resolvedPath = Path.IsPathRooted(filePath)
            ? filePath
            : Path.Combine(configDirectory, filePath);

        if (File.Exists(resolvedPath))
            return File.ReadAllText(resolvedPath);

        Directory.CreateDirectory(Path.GetDirectoryName(resolvedPath) ?? configDirectory);
        File.WriteAllText(resolvedPath, DefaultJson);
        logger.Warn($"Datasource config file not found. Created default file at '{resolvedPath}'.");
        return DefaultJson;

    }
}

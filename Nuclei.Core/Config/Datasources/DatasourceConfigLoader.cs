using System;
using Nuclei.Abstractions.BepInEx.Config;
using Nuclei.Abstractions.BepInEx.Logging;
using Nuclei.Abstractions.Nuclei.Decorators;

namespace Nuclei.Core.Config.Datasources;

/// <summary>
///     Loads datasource configuration from the config system.
/// </summary>
public static class DatasourceConfigLoader
{
    private const string SectionName = "General";
    private const string KeyName = "Datasources";

    private const string DefaultJson =
        "{\n" +
        "  \"*\": {\n" +
        "    \"host\": \"sqlite://nuclei.db\",\n" +
        "    \"pooling\": true,\n" +
        "    \"timeoutMillis\": 30000,\n" +
        "    \"connectTimeoutMillis\": 5000,\n" +
        "    \"readOnly\": false,\n" +
        "    \"options\": {}\n" +
        "  }\n" +
        "}";

    /// <summary>
    ///     Loads datasource configuration from the provided config provider and logger.
    /// </summary>
    public static DatasourceConfigSet Load(IConfigProvider configProvider, ILogger logger)
    {
        var datasourceLogger = logger.WithTimestamp().WithScope(nameof(DatasourceConfigLoader));

        var entry = configProvider.Bind(
            SectionName,
            KeyName,
            DefaultJson,
            "JSON map of datasource definitions. Keys are datasource names; '*' is the default datasource.");

        datasourceLogger.Debug("Loading datasource configuration...");

        try
        {
            var result = DatasourceConfigParser.Parse(entry.Value);
            datasourceLogger.Debug($"Loaded {result.Datasources.Count} datasource configuration(s).");
            return result;
        }
        catch (Exception exception)
        {
            datasourceLogger.Error("Failed to parse datasource configuration.", exception);
            throw;
        }
    }
}


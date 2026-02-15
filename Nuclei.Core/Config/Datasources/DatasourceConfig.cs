using System.Collections.Generic;

namespace Nuclei.Core.Config.Datasources;

/// <summary>
///     Represents a configured datasource definition.
/// </summary>
public sealed record DatasourceConfig(
    string Name,
    string Host,
    bool Pooling = true,
    int TimeoutMillis = 30000,
    int ConnectTimeoutMillis = 5000,
    bool ReadOnly = false,
    IReadOnlyDictionary<string, string>? Options = null
);
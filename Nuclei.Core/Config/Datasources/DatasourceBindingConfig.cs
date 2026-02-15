using System.Collections.Generic;

namespace Nuclei.Core.Config.Datasources;

/// <summary>
///     Represents a logical datasource binding to concrete datasource sources.
/// </summary>
public sealed record DatasourceBindingConfig(
    string Name,
    IReadOnlyList<string> WriteSources,
    IReadOnlyList<string> ReadSources
);
using System;
using System.Collections.Generic;

namespace Nuclei.Core.Config.Datasources;

/// <summary>
///     Holds a set of configured datasources.
/// </summary>
public sealed class DatasourceConfigSet(IReadOnlyDictionary<string, DatasourceConfig> configs, string defaultName)
{
    private readonly string _defaultName = defaultName ?? throw new ArgumentNullException(nameof(defaultName));

    /// <summary>
    ///     Gets the configured datasources, keyed by name.
    /// </summary>
    public IReadOnlyDictionary<string, DatasourceConfig> Datasources { get; } = configs ?? throw new ArgumentNullException(nameof(configs));

    /// <summary>
    ///     Gets the default datasource configuration.
    /// </summary>
    /// <returns> The default datasource configuration. </returns>
    public DatasourceConfig GetDefault()
    {
        return Get(_defaultName);
    }

    /// <summary>
    ///     Gets the datasource configuration for the specified name, or the default if the name is null, empty, or '*'.
    /// </summary>
    /// <param name="name"> The name of the datasource to get, or null/empty/'*' for the default. </param>
    /// <returns> The datasource configuration for the specified name. </returns>
    /// <exception cref="KeyNotFoundException"> Thrown if the specified datasource name is not configured. </exception>
    public DatasourceConfig Get(string name)
    {
        if (string.IsNullOrWhiteSpace(name) || name == "*")
            name = _defaultName;

        return Datasources.TryGetValue(name.ToLower(), out var config)
            ? config
            : throw new KeyNotFoundException($"Datasource '{name}' is not configured.");
    }
}
using System;
using System.Collections.Generic;

namespace Nuclei.Core.Config.Datasources;

/// <summary>
///     Holds a set of configured datasources.
/// </summary>
public sealed class DatasourceConfigSet(
    IReadOnlyDictionary<string, DatasourceBindingConfig> bindings,
    IReadOnlyDictionary<string, DatasourceConfig> sources,
    string defaultBindingName)
{
    private readonly string _defaultBindingName =
        defaultBindingName ?? throw new ArgumentNullException(nameof(defaultBindingName));

    /// <summary>
    ///     Gets the configured datasource bindings, keyed by logical name.
    /// </summary>
    public IReadOnlyDictionary<string, DatasourceBindingConfig> Bindings { get; } =
        bindings ?? throw new ArgumentNullException(nameof(bindings));

    /// <summary>
    ///     Gets the configured datasource sources, keyed by source name.
    /// </summary>
    public IReadOnlyDictionary<string, DatasourceConfig> Sources { get; } =
        sources ?? throw new ArgumentNullException(nameof(sources));

    /// <summary>
    ///     Gets the default datasource binding.
    /// </summary>
    public DatasourceBindingConfig GetDefaultBinding()
    {
        return GetBinding(_defaultBindingName);
    }

    /// <summary>
    ///     Gets the datasource binding for the specified name, or the default if the name is null, empty, or '*'.
    /// </summary>
    public DatasourceBindingConfig GetBinding(string name)
    {
        if (string.IsNullOrWhiteSpace(name) || name == "*")
            name = _defaultBindingName;

        return Bindings.TryGetValue(name, out var binding)
            ? binding
            : throw new KeyNotFoundException($"Datasource binding '{name}' is not configured.");
    }

    /// <summary>
    ///     Gets the datasource source configuration for the specified name.
    /// </summary>
    public DatasourceConfig GetSource(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Datasource source name cannot be empty.", nameof(name));

        return Sources.TryGetValue(name, out var source)
            ? source
            : throw new KeyNotFoundException($"Datasource source '{name}' is not configured.");
    }

    /// <summary>
    ///     Gets all read datasource source configurations for the specified binding name.
    /// </summary>
    /// <param name="bindingName"> The name of the datasource binding to get the read sources for. </param>
    /// <returns> A collection of datasource source configurations for the specified binding. </returns>
    /// <exception cref="KeyNotFoundException">
    ///     Thrown if the specified binding is not configured, or if any of the read
    ///     sources referenced by the binding are not configured.
    /// </exception>
    public IReadOnlyCollection<DatasourceConfig> GetAllReadSourcesForBinding(string bindingName)
    {
        return GetSourcesForBinding(bindingName, GetBinding(bindingName).ReadSources);
    }

    /// <summary>
    ///     Gets all write datasource source configurations for the specified binding name.
    /// </summary>
    /// <param name="bindingName"> The name of the datasource binding to get the write sources for. </param>
    /// <returns> A collection of datasource source configurations for the specified binding. </returns>
    /// <exception cref="KeyNotFoundException">
    ///     Thrown if the specified binding is not configured, or if any of the write sources referenced by the binding
    ///     are not configured.
    /// </exception>
    public IReadOnlyCollection<DatasourceConfig> GetAllWriteSourcesForBinding(string bindingName)
    {
        return GetSourcesForBinding(bindingName, GetBinding(bindingName).WriteSources);
    }

    private IReadOnlyCollection<DatasourceConfig> GetSourcesForBinding(string bindingName, IEnumerable<string> sourceNames)
    {
        var sources = new List<DatasourceConfig>();
        foreach (var sourceName in sourceNames)
            if (Sources.TryGetValue(sourceName, out var source))
                sources.Add(source);
            else
                throw new KeyNotFoundException($"Datasource source '{sourceName}' referenced by binding '{bindingName}' is not configured.");
        return sources;
    }
}
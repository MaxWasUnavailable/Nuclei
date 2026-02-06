namespace Nuclei.Abstractions.Config;

/// <summary>
///     Defines a provider for configuration entries.
/// </summary>
public interface IConfigProvider
{
    /// <summary>
    ///     Binds a configuration entry to the specified section and key.
    /// </summary>
    /// <param name="section"> Configuration section. </param>
    /// <param name="key"> Configuration key. </param>
    /// <param name="defaultValue"> Default value. </param>
    /// <param name="description"> Description of the configuration entry. </param>
    /// <typeparam name="T"> Type of the configuration entry. </typeparam>
    /// <returns> The configuration entry. </returns>
    IConfigEntry<T> Bind<T>(string section, string key, T defaultValue, string description);

    /// <summary>
    ///     Reloads the configuration from the underlying source.
    /// </summary>
    void Reload();
}

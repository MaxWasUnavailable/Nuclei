using BepInEx.Configuration;
using Nuclei.Abstractions.Config;

namespace Nuclei.Plugin.Config;

/// <summary>
///     Implementation of <see cref="IConfigProvider" /> for BepInEx.
/// </summary>
internal sealed class BepInExConfigProvider(ConfigFile configFile) : IConfigProvider
{
    /// <inheritdoc />
    public IConfigEntry<T> Bind<T>(string section, string key, T defaultValue, string description)
    {
        var entry = configFile.Bind(section, key, defaultValue, description);
        return new BepInExConfigEntry<T>(entry);
    }

    /// <inheritdoc />
    public void Reload()
    {
        configFile.Reload();
    }
}

using BepInEx.Configuration;
using Nuclei.Abstractions.BepInEx.Config;

namespace Nuclei.Plugin.Config;

/// <summary>
///     A BepInEx implementation of <see cref="IConfigEntry{T}" />.
/// </summary>
/// <typeparam name="T"> The type of the configuration entry. </typeparam>
internal sealed class BepInExConfigEntry<T>(ConfigEntry<T> entry) : IConfigEntry<T>
{
    /// <inheritdoc />
    public T Value
    {
        get => entry.Value;
        set => entry.Value = value;
    }
}

namespace Nuclei.Abstractions.Config;

/// <summary>
///     Defines a configuration entry.
/// </summary>
/// <typeparam name="T"> The type of the configuration entry. </typeparam>
public interface IConfigEntry<T>
{
    /// <summary>
    ///     Gets or sets the value of the configuration entry.
    /// </summary>
    T Value { get; set; }
}

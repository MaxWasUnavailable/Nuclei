namespace Nuclei.Abstractions.Units;

/// <summary>
///     Defines a unit identity.
/// </summary>
public interface IUnitInfo
{
    /// <summary>
    ///     The unit identifier, if known.
    /// </summary>
    string Id { get; }

    /// <summary>
    ///     The unit display name.
    /// </summary>
    string Name { get; }
}


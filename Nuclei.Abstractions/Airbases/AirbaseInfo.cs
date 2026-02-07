namespace Nuclei.Abstractions.Airbases;

/// <summary>
///     Defines an airbase.
/// </summary>
/// <param name="Name">The airbase display name.</param>
public sealed record AirbaseInfo(string Name) : IAirbaseInfo;

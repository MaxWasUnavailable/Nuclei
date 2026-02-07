namespace Nuclei.Abstractions.Units;

/// <summary>
///     Defines a unit.
/// </summary>
/// <param name="Id">The unit identifier, if known.</param>
/// <param name="Name">The unit display name.</param>
public sealed record UnitInfo(string Id, string Name) : IUnitInfo;

namespace Nuclei.Abstractions.NO.Factions;

/// <summary>
///     Defines a faction.
/// </summary>
/// <param name="Name">The faction display name.</param>
public sealed record FactionInfo(string Name) : IFactionInfo;

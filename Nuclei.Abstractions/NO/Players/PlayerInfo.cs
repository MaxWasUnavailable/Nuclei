namespace Nuclei.Abstractions.NO.Players;

/// <summary>
///     Default implementation of <see cref="IPlayerInfo"/>.
/// </summary>
/// <param name="Name">The player's name.</param>
/// <param name="SteamId">The player's Steam ID.</param>
public sealed record PlayerInfo(string? Name, ulong SteamId) : IPlayerInfo;

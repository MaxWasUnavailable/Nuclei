namespace Nuclei.Abstractions.NO.Players;

/// <summary>
///     Stable, game-agnostic player identity used across Nuclei APIs.
/// </summary>
public interface IPlayerInfo
{
    /// <summary>
    ///     The player's display name, if available.
    /// </summary>
    string? Name { get; }

    /// <summary>
    ///     The player's Steam ID, if available.
    /// </summary>
    ulong SteamId { get; }
}


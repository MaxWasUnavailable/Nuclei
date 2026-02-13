using Mirage;
using Nuclei.Abstractions.NO.Players;

namespace Nuclei.Adapters.Players;

/// <summary>
///     Adapter for resolving player information from network player objects.
/// </summary>
public interface INetworkPlayerLookup
{
    /// <summary>
    ///     Builds a stable player info object for the given network player.
    /// </summary>
    /// <param name="networkPlayer"> The network player to build the info from. </param>
    /// <returns> The built player info object. </returns>
    IPlayerInfo FromNetworkPlayer(INetworkPlayer networkPlayer);

    /// <summary>
    ///     Resolves a network player from the provided player info.
    /// </summary>
    /// <param name="playerInfo"> The player info to resolve from. </param>
    /// <returns> The resolved network player, or null if no matching player was found. </returns>
    INetworkPlayer? FromPlayerInfo(IPlayerInfo playerInfo);
}


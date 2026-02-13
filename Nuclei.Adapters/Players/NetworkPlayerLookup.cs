using System;
using System.Linq;
using Mirage;
using NuclearOption.Networking;
using Nuclei.Abstractions.NO.Players;

namespace Nuclei.Adapters.Players;

/// <summary>
///     Default adapter for resolving player info using the game networking stack.
/// </summary>
public sealed class NetworkPlayerLookup : INetworkPlayerLookup
{
    /// <inheritdoc />
    public IPlayerInfo FromNetworkPlayer(INetworkPlayer networkPlayer)
    {
        if (networkPlayer == null)
            throw new ArgumentNullException(nameof(networkPlayer));

        var identity = networkPlayer.Identity;
        if (!identity)
            return new PlayerInfo(null, 0UL);

        var playerName = identity.GetComponent<Player>()?.PlayerName;
        var steamId = 0UL;

        var networkManager = NetworkManagerNuclearOption.i;
        if (networkManager?.authenticator)
            steamId = networkManager!.authenticator.GetSteamId(networkPlayer).m_SteamID;

        return new PlayerInfo(playerName, steamId);
    }

    /// <summary>
    ///     Resolves a network player from the provided player info.
    /// </summary>
    /// <param name="playerInfo"> The player info to resolve from. </param>
    /// <returns> The resolved network player, or null if no matching player was found. </returns>
    public INetworkPlayer? FromPlayerInfo(IPlayerInfo playerInfo)
    {
        if (playerInfo == null)
            throw new ArgumentNullException(nameof(playerInfo));

        var networkManager = NetworkManagerNuclearOption.i;
        if (!networkManager?.authenticator)
            return null;

        return (from networkPlayer in networkManager!.Server.AllPlayers
            let steamId = networkManager.authenticator.GetSteamId(networkPlayer).m_SteamID
            where steamId == playerInfo.SteamId
            select networkPlayer).FirstOrDefault();
    }
}
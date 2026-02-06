using System;
using Mirage;
using NuclearOption.Networking;
using Nuclei.Abstractions.Players;

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
        {
            throw new ArgumentNullException(nameof(networkPlayer));
        }

        var identity = networkPlayer.Identity;
        if (!identity)
        {
            return new PlayerInfo(null, 0UL);
        }

        var playerName = identity.GetComponent<Player>()?.PlayerName;
        var steamId = 0UL;

        var networkManager = NetworkManagerNuclearOption.i;
        if (networkManager?.authenticator != null)
        {
            steamId = networkManager.authenticator.GetSteamId(networkPlayer).m_SteamID;
        }

        return new PlayerInfo(playerName, steamId);
    }
}

using System;
using System.Linq;
using Mirage;
using NuclearOption.Networking;

namespace Nuclei.Helpers;

/// <summary>
///     Helper class for player-related operations.
/// </summary>
public static class PlayerUtils
{
    /// <summary>
    ///     Get the Player object from an INetworkPlayer object.
    /// </summary>
    /// <param name="networkPlayer"> The INetworkPlayer object. </param>
    /// <returns> The Player object, if available. </returns>
    public static Player? GetPlayer(this INetworkPlayer networkPlayer)
    {
        return networkPlayer.Identity?.GetComponent<Player>();
    }

    /// <summary>
    ///     Get the Player object from an INetworkPlayer object, if available.
    /// </summary>
    /// <param name="networkPlayer"> The INetworkPlayer object. </param>
    /// <param name="playerComponent"> The Player component, if available. </param>
    /// <returns> The Player object, if available. </returns>
    public static bool TryGetPlayer(this INetworkPlayer networkPlayer, out Player? playerComponent)
    {
        playerComponent = networkPlayer.GetPlayer();
        return playerComponent != null;
    }

    /// <summary>
    ///     Try to find a player by name.
    /// </summary>
    /// <param name="playerName"> The name of the player to find. </param>
    /// <param name="playerObject"> The Player object, if available. </param>
    /// <returns></returns>
    public static bool TryFindPlayer(string playerName, out Player? playerObject)
    {
        playerObject = Globals.AuthenticatedPlayers.FirstOrDefault(p => string.Equals(p.GetPlayer()?.PlayerName, playerName, StringComparison.CurrentCultureIgnoreCase))?.GetPlayer();
        return playerObject != null;
    }
}
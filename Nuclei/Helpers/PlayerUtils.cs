using System.Linq;
using Mirage;

namespace Nuclei.Helpers;

/// <summary>
///     Helper class for player-related operations.
/// </summary>
public static class PlayerUtils
{
    /// <summary>
    ///     Get the Player object from an INetworkPlayer object, if available.
    /// </summary>
    /// <param name="networkPlayer"> The INetworkPlayer object. </param>
    /// <param name="playerObject"> The Player object, if available. </param>
    /// <returns> The Player object, if available. </returns>
    public static bool TryGetPlayerFromINetworkPlayer(INetworkPlayer networkPlayer, out Player? playerObject)
    {
        playerObject = networkPlayer.Identity?.GetComponent<Player>();
        return playerObject != null;
    }

    /// <summary>
    ///     Try to find a player by name.
    /// </summary>
    /// <param name="playerName"> The name of the player to find. </param>
    /// <param name="playerObject"> The Player object, if available. </param>
    /// <returns></returns>
    public static bool TryFindPlayer(string playerName, out Player? playerObject)
    {
        playerObject = FactionHQ.playersCache.FirstOrDefault(p => p.PlayerName == playerName);
        return playerObject != null;
    }
    
}
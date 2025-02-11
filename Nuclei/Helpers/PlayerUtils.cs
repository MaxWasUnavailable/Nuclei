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
    public static bool TryGetPlayer(INetworkPlayer networkPlayer, out Player? playerObject)
    {
        playerObject = networkPlayer.Identity?.GetComponent<Player>();
        return playerObject != null;
    }
    
}
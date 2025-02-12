using System;

namespace Nuclei.Events;

/// <summary>
///     Declares player-related events.
/// </summary>
public static class PlayerEvents
{
    /// <summary>
    ///     Event handler for when a player joins the game.
    /// </summary>
    public static event EventHandler<Player>? PlayerJoined;

    internal static void OnPlayerJoined(Player e)
    {
        PlayerJoined?.Invoke(null, e);
    }
    
    /// <summary>
    ///     Event handler for when a player leaves the game.
    /// </summary>
    public static event EventHandler<Player>? PlayerLeft;

    internal static void OnPlayerLeft(Player e)
    {
        PlayerLeft?.Invoke(null, e);
    }
}
using NuclearOption.Networking;
using Nuclei.Features;
using Nuclei.Features.Commands.DefaultCommands;
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
    public static event Action<Player>? PlayerJoined;

    internal static void OnPlayerJoined(Player e)
    {
        // Check ban status
        var steamId = e.SteamID;
        if (NucleiConfig.IsBanned(steamId))
        {
            Nuclei.Logger.LogInfo($"Player {e.PlayerName} is banned. Kicking...");
            KickCommand.Kick(e);
            return;
        }
            
        PlayerJoined?.Invoke(e);
    }
    
    /// <summary>
    ///     Event handler for when a player leaves the game.
    /// </summary>
    public static event Action<Player>? PlayerLeft;

    internal static void OnPlayerLeft(Player e)
    {
        PlayerLeft?.Invoke(e);
    }
}
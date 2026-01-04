using System;
using System.Linq;
using System.Text.RegularExpressions;
using Mirage;
using NuclearOption.Networking;
using Nuclei.Features;

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
    /// <returns> The Player object, if available. </returns>
    public static bool TryGetPlayer(this INetworkPlayer networkPlayer, out Player? player)
    {
        return PlayerHelper.TryGetPlayer(networkPlayer, out player);
    }

    /// <summary>
    ///     Try to find a player by name.
    /// </summary>
    /// <param name="playerName"> The name of the player to find. </param>
    /// <param name="playerObject"> The Player object, if available. </param>
    /// <returns></returns>
    public static bool TryFindPlayer(string playerName, out Player? playerObject)
    {
            return TryGetPlayer(Globals.AuthenticatedPlayers.FirstOrDefault(p =>
            {
                Player po;
                TryGetPlayer(p, out po);
                return StripStaffPrefix(po.PlayerName ?? "").ToLower()
                    .StartsWith(StripStaffPrefix(playerName).ToLower());
            }), out playerObject);
    }
    
    /// <summary>
    ///     Utility function to strip a player name of the staff tag, if they have it.
    /// </summary>
    /// <param name="playerName"> The player name. </param>
    /// <returns>Actual playername.</returns>
    public static string StripStaffPrefix(string playerName)
    {
        if (string.IsNullOrEmpty(playerName))
            return playerName;

        var pattern = $@"^\[\d*\]\s{Regex.Escape(NucleiConfig.StaffPrefix!.Value)}\s*";
        var cleanName = Regex.Replace(playerName, pattern, "", RegexOptions.IgnoreCase);

        return cleanName;
    }

    /// <summary>
    ///     Apply or remove the staff tag based on player permission level.
    /// </summary>
    /// <param name="playerObject"> The Player object. </param>
    /// <returns></returns>
    public static void ApplyOrRemoveStaffTag(Player playerObject)
    {
        if (!NucleiConfig.UseStaffPrefix!.Value || (!NucleiConfig.IsAdmin(playerObject.SteamID) &&
                                                   !NucleiConfig.IsOwner(playerObject.SteamID) &&
                                                   !NucleiConfig.IsModerator(playerObject.SteamID))) return;
        var newName = $"{NucleiConfig.StaffPrefix!.Value} {playerObject.PlayerName}";
        playerObject.PlayerName = newName;
    }

    private static int ID = 1;
    public static void ApplyID(Player player)
    {
        var newName = $"[{ID++}] {player.PlayerName}";
        player.PlayerName = newName;
    }
}
using NuclearOption.Networking;
using Nuclei.Helpers;

namespace Nuclei.Features;

public static class BanService
{
    /// <summary>
    ///     Verify that the player is not banned. If player in banlist, kick them off the server.
    /// </summary>
    /// <param name="player"> The player to verify. </param>
    public static void VerifyNotBanned(Player player)
    {
        var steamId = player.SteamID;
        if (NucleiConfig.IsBanned(steamId))
        {
            Nuclei.Logger?.LogInfo($"Player {player.PlayerName} is banned. Kicking...");
            ChatService.SendChatMessage($"Player  {player.PlayerName} is banned.");
            _ = Globals.NetworkManagerNuclearOptionInstance.KickPlayerAsync(player);
        }
    }
}
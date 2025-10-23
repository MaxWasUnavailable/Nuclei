using HarmonyLib;
using NuclearOption.Networking;
using Nuclei.Events;
using Nuclei.Features;
using Nuclei.Helpers;

namespace Nuclei.Patches;

[HarmonyPatch(typeof(MessageManager))]
[HarmonyPriority(Priority.First)]
[HarmonyWrapSafe]
internal static class MessageManagerPatches
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(MessageManager.JoinMessage))]
    private static bool JoinMessagePostfix(Player joinedPlayer)
    {
        // Check ban status
        var steamId = joinedPlayer.SteamID;
        if (NucleiConfig.IsBanned(steamId))
        {
            Nuclei.Logger?.LogInfo($"Player {joinedPlayer.PlayerName} is banned. Kicking...");
            _ = Globals.NetworkManagerNuclearOptionInstance.KickPlayerAsync(joinedPlayer);
            return false;
        }

        Nuclei.Logger?.LogInfo($"{joinedPlayer.PlayerName} joined the game");
        ChatService.SendChatMessage(NucleiConfig.WelcomeMessage!.Value, joinedPlayer);
        
        PlayerEvents.OnPlayerJoined(joinedPlayer);
        return true;
    }
    
    [HarmonyPostfix]
    [HarmonyPatch(nameof(MessageManager.DisconnectedMessage))]
    private static void DisconnectedMessagePostfix(Player player)
    {
        Nuclei.Logger?.LogInfo($"{player.PlayerName} left the game");
        
        PlayerEvents.OnPlayerLeft(player);
    }
}
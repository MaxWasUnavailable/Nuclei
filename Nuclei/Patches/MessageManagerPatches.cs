using HarmonyLib;
using Nuclei.Events;
using Nuclei.Features;

namespace Nuclei.Patches;

[HarmonyPatch(typeof(MessageManager))]
[HarmonyPriority(Priority.First)]
[HarmonyWrapSafe]
internal static class MessageManagerPatches
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(MessageManager.JoinMessage))]
    private static void JoinMessagePostfix(Player joinedPlayer)
    {
        Nuclei.Logger?.LogInfo($"{joinedPlayer.PlayerName} joined the game");
        ChatService.SendChatMessage(NucleiConfig.WelcomeMessage!.Value, joinedPlayer);
        
        PlayerEvents.OnPlayerJoined(joinedPlayer);
    }
    
    [HarmonyPrefix]
    [HarmonyPatch(nameof(MessageManager.JoinMessage))]
    private static bool JoinMessagePrefix(Player joinedPlayer)
    {
        if (!NucleiConfig.BannedPlayers!.Value.Contains(joinedPlayer.SteamID.ToString())) 
            return true;
        
        Nuclei.Logger?.LogInfo($"{joinedPlayer.PlayerName} ({joinedPlayer.SteamID}) tried to join the game but is banned");
        return false;
    }
    
    [HarmonyPostfix]
    [HarmonyPatch(nameof(MessageManager.DisconnectedMessage))]
    private static void DisconnectedMessagePostfix(Player player)
    {
        Nuclei.Logger?.LogInfo($"{player.PlayerName} left the game");
        
        PlayerEvents.OnPlayerLeft(player);
    }
}
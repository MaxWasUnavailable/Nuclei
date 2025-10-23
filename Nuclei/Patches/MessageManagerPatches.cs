using HarmonyLib;
using NuclearOption.Networking;
using Nuclei.Events;
using Nuclei.Features;
using Nuclei.Features.Commands.DefaultCommands;

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
    
    [HarmonyPostfix]
    [HarmonyPatch(nameof(MessageManager.DisconnectedMessage))]
    private static void DisconnectedMessagePostfix(Player player)
    {
        Nuclei.Logger?.LogInfo($"{player.PlayerName} left the game");
        
        PlayerEvents.OnPlayerLeft(player);
    }
}
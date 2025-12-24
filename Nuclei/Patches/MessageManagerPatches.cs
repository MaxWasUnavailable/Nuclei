using HarmonyLib;
using NuclearOption.Chat;
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
    private static void JoinMessagePostfix(Player joinedPlayer)
    {
        // TODO: move ban check to a dedicated service, hook into OnPlayerJoined event (or earlier?)
        var steamId = joinedPlayer.SteamID;
        if (NucleiConfig.IsBanned(steamId))
        {
            Nuclei.Logger?.LogInfo($"Player {joinedPlayer.PlayerName} is banned. Kicking...");
            _ = Globals.NetworkManagerNuclearOptionInstance.KickPlayerAsync(joinedPlayer);
            return;
        }

        Nuclei.Logger?.LogInfo($"{joinedPlayer.PlayerName} joined the game");
        MissionMessages.ShowMessage(NucleiConfig.WelcomeMessage!.Value,false,null,true);
        
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
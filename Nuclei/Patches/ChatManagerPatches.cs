using HarmonyLib;
using Mirage;
using Nuclei.Features;
using Nuclei.Features.Commands;
using Nuclei.Helpers;

namespace Nuclei.Patches;

[HarmonyPatch(typeof(ChatManager))]
[HarmonyPriority(Priority.First)]
[HarmonyWrapSafe]
internal static class ChatManagerPatches
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(ChatManager.UserCode_CmdSendChatMessage_1323305531))]
    private static bool UserCode_CmdSendChatMessage_1323305531Prefix(string message, bool allChat, INetworkPlayer sender)
    {
        if (!sender.TryGetPlayer(out var player)) 
            Nuclei.Logger?.LogWarning("Player component is null");

        if (message.StartsWith(NucleiConfig.CommandPrefix!.Value) && message.Length > 1)
            if (CommandService.TryExecuteCommand(player!, message.Remove(0, 1)))
                return false;

        Nuclei.Logger?.LogInfo(allChat
            ? $"{player!.PlayerName} sent message: {message}"
            : $"{player!.PlayerName} sent message in {player.HQ.faction.factionName} chat: {message}");

        return true;
    }
}
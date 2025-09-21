using HarmonyLib;
using Mirage;
using Nuclei.Features;
using Nuclei.Features.Commands;
using Nuclei.Helpers;
using System.Linq;
using System.Reflection;

namespace Nuclei.Patches;

[HarmonyPatch(typeof(ChatManager))]
[HarmonyPriority(Priority.First)]
[HarmonyWrapSafe]
internal static class ChatManagerPatches
{

    static MethodBase TargetMethod()
    {
        var t = typeof(ChatManager);
        var mi = AccessTools.GetDeclaredMethods(t)
            .FirstOrDefault(m => m.Name.StartsWith("UserCode_CmdSendChatMessage_", System.StringComparison.Ordinal));
        if (mi == null)
            throw new System.Exception("Could not find ChatManager.UserCode_CmdSendChatMessage_*");
        return mi;
    }
    static bool Prefix(ChatManager __instance, string message, bool allChat, INetworkPlayer sender)
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
using HarmonyLib;
using Mirage;
using NuclearOption.Chat;
using Nuclei.Abstractions.NO.Chat;
using Nuclei.Adapters.Players;
using Nuclei.Events.Events;

// ReSharper disable InconsistentNaming

namespace Nuclei.Patches.Patches;

[HarmonyPatch(typeof(ChatManager))]
[HarmonyPriority(Priority.First)]
[HarmonyWrapSafe]
internal static class ChatManagerPatches
{
    private static INetworkPlayerLookup Lookup { get; } = new NetworkPlayerLookup();    // TODO: switch to DI / static service locator when available?

    [HarmonyPrefix]
    [HarmonyPatch(nameof(ChatManager.CmdSendChatMessage))]
    [HarmonyPatch([typeof(string), typeof(bool), typeof(INetworkPlayer)])]
    private static bool CmdSendChatMessagePrefix(string message, bool allChat, INetworkPlayer sender, ref ChatMessageEvent? __state)
    {
        var payload = new ChatMessageEvent(new ChatMessage(Lookup.FromNetworkPlayer(sender), message, allChat));
        var shouldSendOriginal = true;

        ChatEvents.OnPreChatMessageSent(ref payload, ref shouldSendOriginal);

        __state = shouldSendOriginal ? payload : null;
        return shouldSendOriginal;
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(ChatManager.CmdSendChatMessage))]
    [HarmonyPatch([typeof(string), typeof(bool), typeof(INetworkPlayer)])]
    private static void CmdSendChatMessagePostfix(ChatMessageEvent? __state)
    {
        if (__state != null)
            ChatEvents.OnPostChatMessageSent(__state);
    }
}
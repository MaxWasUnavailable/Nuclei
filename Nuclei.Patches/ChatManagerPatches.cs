using HarmonyLib;
using Mirage;
using NuclearOption.Chat;
using Nuclei.Abstractions.Chat;
using Nuclei.Adapters.Players;
using Nuclei.Events.Events;

// ReSharper disable InconsistentNaming

namespace Nuclei.Patches;

[HarmonyPatch(typeof(ChatManager))]
[HarmonyPriority(Priority.First)]
[HarmonyWrapSafe]
internal static class ChatManagerPatches
{
    private static INetworkPlayerLookup Lookup { get; } = new NetworkPlayerLookup();

    [HarmonyPrefix]
    [HarmonyPatch(nameof(ChatManager.CmdSendChatMessage))]
    [HarmonyPatch([typeof(string), typeof(bool), typeof(INetworkPlayer)])]
    private static void CmdSendChatMessagePrefix(string message, bool allChat, INetworkPlayer sender, ref ChatMessageEvent? __state)
    {
        var playerInfo = Lookup.FromNetworkPlayer(sender);
        var chatMessage = new ChatMessage(playerInfo, message, allChat);
        __state = new ChatMessageEvent(chatMessage);
        ChatEvents.OnPreChatMessageSent(__state);
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
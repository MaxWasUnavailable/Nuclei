using HarmonyLib;

namespace Nuclei.Patches;

[HarmonyPatch(typeof(ChatManager))]
[HarmonyPriority(Priority.First)]
[HarmonyWrapSafe]
internal static class ChatManagerPatches
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(ChatManager.TargetReceiveMessage))]
    private static void TargetReceiveMessagePostfix(string message)
    {
        // TODO: Add player name & reformat log message
        Nuclei.Logger?.LogInfo($"Received message: {message}");
    }
}
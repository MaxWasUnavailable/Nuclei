using HarmonyLib;
using Steamworks;

namespace Nuclei.Patches;

[HarmonyPatch(typeof(CSteamAPIContext))]
[HarmonyPriority(Priority.First)]
[HarmonyWrapSafe]
internal static class CSteamAPIContextPatches
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(CSteamAPIContext.Init))]
    private static void InitPostfix()
    {
        Nuclei.Logger?.LogInfo("Steam API context initialized; attaching Nuclei to the official dedicated server.");
    }
}

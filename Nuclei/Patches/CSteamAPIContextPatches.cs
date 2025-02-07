using System;
using HarmonyLib;
using Nuclei.Features;
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
        try
        {
            Server.StartServer();
        }
        catch (Exception e)
        {
            Nuclei.Logger?.LogError($"Aborting server launch: Failed to start the server. For more information, see this error trace:\n{e}");
        }
    }
}
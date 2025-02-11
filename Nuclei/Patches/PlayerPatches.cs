using HarmonyLib;

namespace Nuclei.Patches;

[HarmonyPatch(typeof(Player))]
[HarmonyPriority(Priority.First)]
[HarmonyWrapSafe]
internal static class PlayerPatches
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(Player.CmdSetPlayerName))]
    private static void CmdSetPlayerNamePostfix(ref string playerName)
    {
        playerName = "Server";
    }
}
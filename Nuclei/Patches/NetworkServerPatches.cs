using HarmonyLib;
using Mirage;

namespace Nuclei.Patches;

[HarmonyPatch(typeof(NetworkServer))]
[HarmonyPriority(Priority.First)]
[HarmonyWrapSafe]
internal static class NetworkServerPatches
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(NetworkServer.StartServer))]
    private static void StartServerPrefix(ref NetworkClient localClient)
    {
        localClient.RunInBackground = true;
    }
}
using HarmonyLib;
using NuclearOption.DedicatedServer;
using NuclearOption.SavedMission;
using Nuclei.Events.Events;

namespace Nuclei.Patches;

[HarmonyPatch(typeof(DedicatedServerManager))]
[HarmonyPriority(Priority.First)]
[HarmonyWrapSafe]
internal static class DedicatedServerManagerPatches
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(DedicatedServerManager.StartServer))]
    private static void StartServerPrefix()
    {
        ServerEvents.OnPreServerStarted();
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(DedicatedServerManager.StartServer))]
    private static void StartServerPostfix()
    {
        ServerEvents.OnPostServerStarted();
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(DedicatedServerManager.LoadMissionMap))]
    [HarmonyPatch([typeof(Mission)])]
    private static void LoadMissionMapPrefix()
    {
        MissionEvents.OnPreMissionStarted();
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(DedicatedServerManager.LoadMissionMap))]
    [HarmonyPatch([typeof(Mission)])]
    private static void LoadMissionMapPostfix()
    {
        MissionEvents.OnPostMissionStarted();
    }
}


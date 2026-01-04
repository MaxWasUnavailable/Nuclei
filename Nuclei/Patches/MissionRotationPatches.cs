using HarmonyLib;
using Mirage;
using NuclearOption.Chat;
using NuclearOption.DedicatedServer;
using Nuclei.Features;
using Nuclei.Features.Commands;
using Nuclei.Helpers;

namespace Nuclei.Patches;

[HarmonyPatch(typeof(MissionRotation))]
[HarmonyPriority(Priority.First)]
[HarmonyWrapSafe]
internal static class MissionRotationPatches
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(MissionRotation.GetNext))]
    private static void GetNextPrefix()
    {
        MissionOptions nextMission = Globals.DedicatedServerManagerInstance.missionRotation.PeakNext();
        WeatherRandomizerService.RandomizeWeather(nextMission.Key.Name);
    }
}
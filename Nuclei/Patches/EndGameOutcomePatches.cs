using HarmonyLib;
using NuclearOption.SavedMission.ObjectiveV2.Outcomes;
using Nuclei.Events;
using Nuclei.Features;

namespace Nuclei.Patches;

[HarmonyPatch(typeof(EndGameOutcome))]
[HarmonyPriority(Priority.First)]
[HarmonyWrapSafe]
internal static class EndGameOutcomePatches
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(EndGameOutcome.Complete))]
    private static void CompletePostfix()
    {
        MissionEvents.OnMissionEnded(MissionService.CurrentMission!);
    }
}
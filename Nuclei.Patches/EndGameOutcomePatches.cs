using HarmonyLib;
using NuclearOption.SavedMission.ObjectiveV2.Outcomes;
using Nuclei.Events.Events;

namespace Nuclei.Patches;

[HarmonyPatch(typeof(EndGameOutcome))]
[HarmonyPriority(Priority.First)]
[HarmonyWrapSafe]
internal static class EndGameOutcomePatches
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(EndGameOutcome.Complete))]
    private static void CompletePrefix()
    {
        MissionEvents.OnPreMissionCompleted();
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(EndGameOutcome.Complete))]
    private static void CompletePostfix()
    {
        MissionEvents.OnPostMissionCompleted();
    }
}
using HarmonyLib;
using NuclearOption.SavedMission.ObjectiveV2.Outcomes;
using Nuclei.Events;
using Nuclei.Features;
using System.Reflection;

namespace Nuclei.Patches;

[HarmonyPatch]
[HarmonyPriority(Priority.First)]
[HarmonyWrapSafe]
internal static class EndGameOutcomePatches
{
    static MethodBase TargetMethod() =>
    AccessTools.Method("NuclearOption.SavedMission.ObjectiveV2.Outcomes.EndGameOutcome:Complete");

    [HarmonyPostfix]
    [HarmonyPatch]
    private static void Postfix()
    {
        MissionEvents.OnMissionEnded(MissionService.CurrentMission!);
    }
}
using System.Reflection;
using HarmonyLib;
using Nuclei.Events;
using Nuclei.Features;

namespace Nuclei.Patches;

[HarmonyPatch]
[HarmonyPriority(Priority.First)]
[HarmonyWrapSafe]
internal static class EndGameOutcomePatches
{
    private static MethodBase TargetMethod()
    {
        return AccessTools.Method("NuclearOption.SavedMission.ObjectiveV2.Outcomes.EndGameOutcome:Complete");
    }

    [HarmonyPostfix]
    [HarmonyPatch]
    private static void Postfix()
    {
        MissionEvents.OnMissionEnded(MissionService.CurrentMission!);
    }
}
using System;
using HarmonyLib;
using NuclearOption.SavedMission;
using Nuclei.Features;

[HarmonyPriority(Priority.First)]
[HarmonyWrapSafe]
[HarmonyPatch(typeof(MissionSaveLoad))]
public class MissionSaveLoadPatches
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(MissionSaveLoad.TryLoad))]
    private static void Postfix(
        MissionKey item,
        ref Mission mission,
        ref string error,
        ref bool __result)
    {
        if (!__result || mission == null) return;
        
        RandomizeWeather(ref mission);
        ModifyDifficulty(ref mission);
    }

    private static void ModifyDifficulty(ref Mission mission)
    {
        // This is set to scale for larger player counts better
        foreach (var f in mission.factions)
        {
            f.addAIPerEnemyPlayer = 0.80f;
            f.AIAircraftLimit = 8;
        }
    }

    private static void RandomizeWeather(ref Mission mission)
    {
        if (!NucleiConfig.RandomizeWeather!.Value) return;
        
        var rnd = new Random();
        mission.environment.timeOfDay = rnd.Next(3, 18);
        mission.environment.timeFactor = 8f;
        mission.environment.weatherIntensity = (float)(rnd.NextDouble() * 0.9);
        mission.environment.cloudAltitude = (float)(500 + rnd.NextDouble() * 1000);
        mission.environment.windSpeed = (float)(rnd.NextDouble() * 4);
        mission.environment.windTurbulence = (float)rnd.NextDouble();
        mission.environment.windHeading = rnd.Next(0, 360);
    }
}
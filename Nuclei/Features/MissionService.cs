using System;
using System.Collections.Generic;
using System.Linq;
using NuclearOption.SavedMission;
using NuclearOption.SavedMission.ObjectiveV2;
using Nuclei.Helpers;
using Random = UnityEngine.Random;

namespace Nuclei.Features;

/// <summary>
///     Manages missions on the server.
/// </summary>
public static class MissionService
{
    /// <summary>
    ///     The last mission that was started.
    /// </summary>
    public static Mission? LastMission { get; private set; }

    /// <summary>
    ///     The preselected mission key.
    /// </summary>
    public static MissionGroup.MissionKey? PreselectedMissionKey { get; private set; }

    /// <summary>
    ///     The current mission.
    /// </summary>
    public static Mission? CurrentMission => MissionManager.CurrentMission;

    /// <summary>
    ///     The current mission runner.
    /// </summary>
    public static MissionRunner? CurrentMissionRunner => MissionManager.Runner;

    /// <summary>
    ///     The current mission objectives.
    /// </summary>
    public static MissionObjectives? CurrentMissionObjectives => MissionManager.Objectives;

    /// <summary>
    ///     The current mission active objectives.
    /// </summary>
    public static List<Objective> CurrentMissionActiveObjectives => CurrentMissionRunner?.ActiveObjectives ?? [];

    /// <summary>
    ///     The current mission time.
    /// </summary>
    public static float CurrentMissionTime => Globals.MissionManagerInstance.missionTime;

    /// <summary>
    ///     Gets all Mission Keys as an IEnumerable.
    /// </summary>
    public static IEnumerable<MissionGroup.MissionKey> AllMissionKeys => MissionGroup.All.GetMissions();

    /// <summary>
    ///     Gets a mission by its key.
    /// </summary>
    /// <param name="key"> The mission key object of the mission. </param>
    /// <returns> The mission if found, otherwise null. </returns>
    public static Mission? GetMission(MissionGroup.MissionKey key)
    {
        return key.TryLoad(out var mission, out var errorString) ? mission : throw new Exception(errorString);
    }

    /// <summary>
    ///     Gets a list of Mission Keys filtered by the config.
    /// </summary>
    /// <returns> The list of mission keys. </returns>
    public static MissionGroup.MissionKey[] GetConfigMissionKeys()
    {
        return NucleiConfig.MissionsList.Select(m => AllMissionKeys.First(k => k.Name == m)).ToArray();
    }

    /// <summary>
    ///     Gets a random mission from the list of all missions. (Not filtered by the config)
    /// </summary>
    /// <param name="allowRepeat"> Whether to allow the same mission to be returned multiple times in a row. </param>
    /// <param name="allMissions"> Whether to get all missions or only the ones in the config. </param>
    /// <returns> The mission if found, otherwise null. </returns>
    public static Mission? GetRandomMission(bool allowRepeat = false, bool allMissions = false)
    {
        return GetRandomMission(allMissions ? AllMissionKeys.ToArray() : GetConfigMissionKeys(), allowRepeat);
    }

    /// <summary>
    ///     Gets a random mission from the provided list of missions.
    /// </summary>
    /// <param name="missions"> The list of missions to choose from. </param>
    /// <param name="allowRepeat"> Whether to allow the same mission to be returned multiple times in a row. </param>
    /// <returns></returns>
    public static Mission? GetRandomMission(MissionGroup.MissionKey[] missions, bool allowRepeat = false)
    {
        if (missions.Length == 0)
        {
            Nuclei.Logger?.LogWarning("No missions found. This should not happen.");
            return null;
        }
        if (!allowRepeat && missions.Length > 1 && LastMission != null)
            missions = missions.Where(m => m.Name != LastMission.Name).ToArray();

        return GetMission(missions[Random.Range(0, missions.Length)]);
    }

    /// <summary>
    ///     Select the given mission on the server.
    /// </summary>
    /// <param name="mission"> The mission to start. </param>
    /// <param name="checkIfSame"> Whether to check if the mission is the same as the current mission. </param>
    public static void SetMission(Mission mission, bool checkIfSame = false)
    {
        MissionManager.SetMission(mission, checkIfSame);
        LastMission = mission;
        Nuclei.Logger?.LogDebug($"Set mission: {mission.Name}");
    }

    /// <summary>
    ///     Validates that the configured missions actually exist.
    /// </summary>
    public static bool ValidateMissionConfig()
    {
        var valid = true;
        foreach (var missionName in NucleiConfig.MissionsList.Where(missionName => AllMissionKeys.All(k => k.Name != missionName)))
        {
            Nuclei.Logger?.LogError($"Mission '{missionName}' not found.");
            valid = false;
        }
        return valid;
    }

    /// <summary>
    ///     Return preselected mission if it exists, and clear it.
    /// </summary>
    /// <param name="mission"> The mission to return. </param>
    /// <returns> Whether the mission was found. </returns>
    public static bool TryGetConsumePreselectedMission(out Mission? mission)
    {
        if (PreselectedMissionKey == null)
        {
            mission = null;
            return false;
        }
        mission = GetMission(PreselectedMissionKey.Value)!;
        PreselectedMissionKey = null;
        return true;
    }
}
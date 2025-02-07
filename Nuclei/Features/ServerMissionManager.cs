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
public static class ServerMissionManager
{
    /// <summary>
    ///     Mission Key of the last mission that was started.
    /// </summary>
    public static MissionGroup.MissionKey? LastMission { get; } = null;

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
        return Nuclei.Instance!.MissionsList.Select(m => AllMissionKeys.First(k => k.Name == m)).ToArray();
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
        if (!allowRepeat && missions.Length > 1 && LastMission.HasValue) 
            missions = missions.Where(m => m.Name != LastMission.Value.Name).ToArray();

        return GetMission(missions[Random.Range(0, missions.Length)]);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using NuclearOption.DedicatedServer;
using NuclearOption.Networking;
using NuclearOption.Networking.Lobbies;
using NuclearOption.SavedMission;
using NuclearOption.SavedMission.ObjectiveV2;
using Nuclei.Enums;
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
    public static MissionKey? PreselectedMissionKey { get; private set; }

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
    public static float CurrentMissionTime => Globals.MissionManagerInstance.MissionTime;

    /// <summary>
    ///     Gets all Mission Keys as an IEnumerable.
    /// </summary>
    public static IEnumerable<MissionKey> AllMissionKeys => MissionGroup.All.GetMissions();

    /// <summary>
    ///     Gets a mission by its key.
    /// </summary>
    /// <param name="key"> The mission key object of the mission. </param>
    /// <returns> The mission if found, otherwise null. </returns>
    public static Mission? GetMission(MissionKey key)
    {
        return key.TryLoad(out var mission, out var errorString) ? mission : throw new Exception(errorString);
    }

    /// <summary>
    ///     Gets a mission by its mission object.
    /// </summary>
    /// <param name="mission"> The mission object </param>
    /// <returns> The mission if found, otherwise null. </returns>
    public static MissionKey GetMissionKey(Mission mission)
    {
        return AllMissionKeys.First(k => k.Name == mission.Name);
    }

    /// <summary>
    ///     Gets a list of Mission Keys filtered by the config.
    /// </summary>
    /// <returns> The list of mission keys. </returns>
    public static MissionKey[] GetConfigMissionKeys()
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
    ///     Gets the next sequential mission to be played. Loops back around to the first mission if at the end.
    /// </summary>
    /// <param name="allMissions"> Whether to allow getting from all missions or only the ones in the config. </param>
    /// <returns> The mission if found, otherwise null. </returns>
    public static Mission? GetNextSequentialMission(bool allMissions = false)
    {
        var missionKeys = allMissions ? AllMissionKeys.ToArray() : GetConfigMissionKeys();
        if (LastMission == null)
            return GetMission(missionKeys[0]);
        var index = Array.IndexOf(missionKeys, GetMissionKey(LastMission)) + 1;
        if (index >= missionKeys.Length)
            index = 0;
        return GetMission(missionKeys[index]);
    }

    /// <summary>
    ///     Gets the next mission to be played, based on the provided select mode.
    /// </summary>
    /// <param name="selectMode"> The mission select mode to use. </param>
    /// <param name="allMissions"> Whether to allow getting from all missions or only the ones in the config. </param>
    /// <returns> The mission if found, otherwise null. </returns>
    public static Mission? GetNextMission(MissionSelectMode selectMode, bool allMissions = false)
    {
        if (TryGetConsumePreselectedMission(out var mission))
            return mission;
        
        switch (selectMode)
        {
            case MissionSelectMode.Random:
                return GetRandomMission(true, allMissions);
            case MissionSelectMode.RandomNoRepeat:
                return GetRandomMission(false, allMissions);
            case MissionSelectMode.Sequential:
                return GetNextSequentialMission(allMissions);
            default:
                Nuclei.Logger?.LogError("Invalid mission select mode. Defaulting to random.");
                return GetRandomMission();
        }
    }

    /// <summary>
    ///     Gets a random mission from the provided list of missions.
    /// </summary>
    /// <param name="missions"> The list of missions to choose from. </param>
    /// <param name="allowRepeat"> Whether to allow the same mission to be returned multiple times in a row. </param>
    /// <returns></returns>
    public static Mission? GetRandomMission(MissionKey[] missions, bool allowRepeat = false)
    {
        if (missions.Length == 0)
        {
            Nuclei.Logger?.LogError("No missions found. This should not happen.");
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
    ///     Set a mission to be preselected for the next mission start.
    /// </summary>
    /// <param name="key"> The mission key to preselect. </param>
    public static void SetPreselectedMission(MissionKey key)
    {
        PreselectedMissionKey = key;
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
    
    
    // TODO: Deconstruct this into smaller, reusable functions
    /// <summary>
    ///     Starts the next mission in the mission rotation if m = null. If m is provided, it'll
    ///     load that mission instead
    /// </summary>
    public static async void StartNextMission(Player? player, MissionOptions? m = null)
    {
        
        try
        {
            var dsm = Globals.DedicatedServerManagerInstance;
            if (dsm == null)
            {
                Nuclei.Logger?.LogWarning("dsm is null");
                return;
            }

            var mr = dsm.missionRotation;
            if (mr == null)
            {
                Nuclei.Logger?.LogWarning("missionRotation is null");
                return;
            }

            var nextOpt = m ?? mr.GetNext();
            if (!nextOpt.Key.TryGetKey(out var key))
            {
                Nuclei.Logger?.LogWarning("Error: could not resolve mission key.");
                return;
            }

            if (!MissionSaveLoad.TryLoad(key, out var mission, out var err))
            {
                Nuclei.Logger?.LogWarning($"Load failed: {err}");
                return;
            }

            Nuclei.Logger?.LogInfo($"Loading next mission: {mission?.Name ?? "<unnamed>"}");
            if (player != null) ChatService.SendPrivateChatMessage("Loading next mission...", player);

            // Switch to main thread for Unity scene/lobby ops
            await UniTask.SwitchToMainThread();

            dsm.UpdateLobby(mission, false);
            var ok = await dsm.LoadNext(mission);
            if (!ok)
            {
                if (player != null) Nuclei.Logger?.LogError("Failed to load next mission.");
                return;
            }

            dsm.keyValues.SetKeyValue("start_time", LobbyInstance.CreateStartTime());
            dsm.currentMission = mission;
            dsm.currentMissionOption = nextOpt;
        }
        catch (Exception e)
        {
            Nuclei.Logger?.LogError(e);
            if (player != null) Nuclei.Logger?.LogError("Unexpected error while loading mission.");
        }
    }
}
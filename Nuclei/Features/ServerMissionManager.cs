using System.Collections.Generic;
using NuclearOption.SavedMission;
using NuclearOption.SavedMission.ObjectiveV2;
using Nuclei.Helpers;

namespace Nuclei.Features;

/// <summary>
///     Manages missions on the server.
/// </summary>
public static class ServerMissionManager
{
    /// <summary>
    ///     Name of the last mission that was started.
    /// </summary>
    public static Mission? LastMission { get; private set; } = null;

    /// <summary>
    ///     Starts a mission on the server.
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
    ///     Gets all missions as an IEnumerable.
    /// </summary>
    public static IEnumerable<MissionGroup.MissionKey> AllMissions => MissionGroup.All.GetMissions();
}
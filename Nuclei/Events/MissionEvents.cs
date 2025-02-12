using System;
using NuclearOption.SavedMission;

namespace Nuclei.Events;

/// <summary>
///     Declares mission-related events.
/// </summary>
public static class MissionEvents
{
    /// <summary>
    ///     Event handler for when a new mission has started.
    /// </summary>
    public static event Action<Mission>? NewMissionStarted;

    internal static void OnNewMissionStarted(Mission mission)
    {
        NewMissionStarted?.Invoke(mission);
    }

    /// <summary>
    ///     Event handler for when a mission has ended.
    /// </summary>
    public static event Action<Mission>? MissionEnded;

    internal static void OnMissionEnded(Mission mission)
    {
        MissionEnded?.Invoke(mission);
    }
}
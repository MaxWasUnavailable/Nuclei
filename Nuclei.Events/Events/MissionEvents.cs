using System;

namespace Nuclei.Events.Events;

/// <summary>
///     Declares mission-related events.
/// </summary>
public static class MissionEvents
{
    /// <summary>
    ///     Event handler fired before a mission starts.
    /// </summary>
    public static event Action? PreMissionStarted;

    /// <summary>
    ///     Event handler fired after a mission starts.
    /// </summary>
    public static event Action? PostMissionStarted;

    /// <summary>
    ///     Event handler fired before a mission ends.
    /// </summary>
    public static event Action? PreMissionEnded;

    /// <summary>
    ///     Event handler fired after a mission ends.
    /// </summary>
    public static event Action? PostMissionEnded;

    /// <summary>
    ///     Event handler fired before a mission completes.
    /// </summary>
    public static event Action? PreMissionCompleted;

    /// <summary>
    ///     Event handler fired after a mission completes.
    /// </summary>
    public static event Action? PostMissionCompleted;

    internal static void OnPreMissionStarted()
    {
        PreMissionStarted?.Invoke();
    }

    internal static void OnPostMissionStarted()
    {
        PostMissionStarted?.Invoke();
    }

    internal static void OnPreMissionEnded()
    {
        PreMissionEnded?.Invoke();
    }

    internal static void OnPostMissionEnded()
    {
        PostMissionEnded?.Invoke();
    }

    internal static void OnPreMissionCompleted()
    {
        PreMissionCompleted?.Invoke();
    }

    internal static void OnPostMissionCompleted()
    {
        PostMissionCompleted?.Invoke();
    }
}
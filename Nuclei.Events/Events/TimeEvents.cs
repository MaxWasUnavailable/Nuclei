using System;

namespace Nuclei.Events.Events;

/// <summary>
///     Time-related events for Nuclei.
/// </summary>
public static class TimeEvents
{
    /// <summary>
    ///     Event that triggers every second.
    /// </summary>
    public static event Action? EverySecond;

    /// <summary>
    ///     Event that triggers every minute.
    /// </summary>
    public static event Action? EveryMinute;

    /// <summary>
    ///     Event that triggers every 30 minutes.
    /// </summary>
    public static event Action? Every30Minutes;

    /// <summary>
    ///     Event that triggers every hour.
    /// </summary>
    public static event Action? EveryHour;

    internal static void OnEverySecond()
    {
        EverySecond?.Invoke();
    }

    internal static void OnEveryMinute()
    {
        EveryMinute?.Invoke();
    }


    internal static void OnEvery30Minutes()
    {
        Every30Minutes?.Invoke();
    }

    internal static void OnEveryHour()
    {
        EveryHour?.Invoke();
    }
}
using System;

namespace Nuclei.Events;

/// <summary>
///     Time-related events for Nuclei.
/// </summary>
public static class TimeEvents
{
    // every second, every 30 seconds, every minute, every 10 minutes, every 30 minutes, every hour
    
    /// <summary>
    ///     Event that triggers every second.
    /// </summary>
    public static event Action? EverySecond;
    
    /// <summary>
    ///     Event that triggers every 30 seconds.
    /// </summary>
    public static event Action? Every30Seconds;
    
    /// <summary>
    ///     Event that triggers every minute.
    /// </summary>
    public static event Action? EveryMinute;
    
    /// <summary>
    ///     Event that triggers every 10 minutes.
    /// </summary>
    public static event Action? Every10Minutes;
    
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

    internal static void OnEvery30Seconds()
    {
        Every30Seconds?.Invoke();
    }

    internal static void OnEveryMinute()
    {
        EveryMinute?.Invoke();
    }

    internal static void OnEvery10Minutes()
    {
        Every10Minutes?.Invoke();
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
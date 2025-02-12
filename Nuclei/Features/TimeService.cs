using System;
using Nuclei.Events;
using UnityEngine;

namespace Nuclei.Features;

/// <summary>
///     Time service for Nuclei. Handles triggering of time-based events.
/// </summary>
public class TimeService : MonoBehaviour
{
    private static int _lastTime;

    /// <summary>
    ///     The singleton instance of the time service.
    /// </summary>
    public static TimeService? Instance { get; private set; }

    /// <summary>
    ///     Initializes the time service.
    /// </summary>
    public static void Initialize()
    {
        if (Instance == null)
            Instance = new GameObject("TimeService").AddComponent<TimeService>();
    }

    /// <summary>
    ///     Destroys the time service.
    /// </summary>
    public static void Destroy()
    {
        if (Instance == null)
            return;
        Destroy(Instance.gameObject);
        Instance = null;
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this) 
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    private void FixedUpdate()
    {
        var currentTime = (int) Math.Round(Time.time);
        if (currentTime == _lastTime) 
            return;
        _lastTime = currentTime;
        
        TimeEvents.OnEverySecond();
        if (currentTime % 3600 == 0)
        {
            TimeEvents.OnEveryHour();
            TimeEvents.OnEvery30Minutes();
            TimeEvents.OnEvery10Minutes();
            TimeEvents.OnEveryMinute();
            TimeEvents.OnEvery30Seconds();
        }
        else if (currentTime % 1800 == 0)
        {
            TimeEvents.OnEvery30Minutes();
            TimeEvents.OnEvery10Minutes();
            TimeEvents.OnEveryMinute();
            TimeEvents.OnEvery30Seconds();
        }
        else if (currentTime % 600 == 0)
        {
            TimeEvents.OnEvery10Minutes();
            TimeEvents.OnEveryMinute();
            TimeEvents.OnEvery30Seconds();
        }
        else if (currentTime % 60 == 0)
        {
            TimeEvents.OnEveryMinute();
            TimeEvents.OnEvery30Seconds();
        }
        else if (currentTime % 30 == 0)
        {
            TimeEvents.OnEvery30Seconds();
        }
        Nuclei.Logger?.LogDebug($"Current time: {currentTime}");
    }
}
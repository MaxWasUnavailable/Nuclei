using System;
using System.Collections.Generic;
using System.Linq;
using Nuclei.Abstractions.Nuclei;
using Nuclei.Abstractions.Nuclei.Decorators;
using Nuclei.Events.Events;
using UnityEngine;
using ILogger = Nuclei.Abstractions.BepInEx.Logging.ILogger;
using Object = UnityEngine.Object;

namespace Nuclei.Core.Services;

/// <summary>
///     Converts tick updates into time-based events.
/// </summary>
public sealed class TimeSchedulerService : INucleiService
{
    private static readonly TimeSpan Second = TimeSpan.FromSeconds(1);

    private TimeSpan _accumulatedTime;
    private readonly object _sync = new();
    private readonly List<IntervalEntry> _intervals = [];
    private long _elapsedSeconds;
    private ILogger? _logger;

    private GameObject? _tickerGameObject;

    /// <inheritdoc />
    public void Initialize(INucleiContext context)
    {
        _logger = context.Logger.WithTimestamp().WithScope(nameof(TimeSchedulerService));

        RegisterInterval(TimeSpan.FromMinutes(1), TimeEvents.OnEveryMinute);
        RegisterInterval(TimeSpan.FromMinutes(30), TimeEvents.OnEvery30Minutes);
        RegisterInterval(TimeSpan.FromHours(1), TimeEvents.OnEveryHour);

        _tickerGameObject = TimeSchedulerTicker.Create(this);

        _logger?.Info("TimeScheduler initialized, default time events registered, and Ticker GameObject created.");
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _logger?.Info("TimeScheduler is being disposed. Clearing all registered intervals and destroying ticker GameObject.");
        lock (_sync)
        {
            _intervals.Clear();
        }

        if (_tickerGameObject)
        {
            Object.Destroy(_tickerGameObject);
            _tickerGameObject = null;
        }

        _logger?.Info("TimeScheduler disposed and all intervals cleared.");
    }

    /// <summary>
    ///     Registers a callback that runs on the specified interval.
    /// </summary>
    public IDisposable RegisterInterval(TimeSpan interval, Action callback)
    {
        if (interval < Second)
            throw new ArgumentOutOfRangeException(nameof(interval), "Interval must be at least 1 second.");

        if (callback == null)
            throw new ArgumentNullException(nameof(callback));

        var intervalSeconds = (long)interval.TotalSeconds;
        IntervalEntry entry;

        lock (_sync)
        {
            entry = new IntervalEntry(intervalSeconds, _elapsedSeconds + intervalSeconds, callback);
            _intervals.Add(entry);
        }

        return new IntervalRegistration(this, entry);
    }

    /// <summary>
    ///     Advances the scheduler using a delta time.
    /// </summary>
    internal void Tick(TimeSpan delta)
    {
        if (delta < TimeSpan.Zero)
        {
            _logger?.Warn($"Received negative delta time ({delta}). This is not expected and may cause issues with time-based events. Ignoring this tick.");
            return;
        }

        _accumulatedTime += delta;

        if (_accumulatedTime < Second)
            return;

        var secondsToProcess = (long)_accumulatedTime.TotalSeconds;
        _accumulatedTime -= TimeSpan.FromSeconds(secondsToProcess);

        for (var i = 0; i < secondsToProcess; i++)
            TickSecond();
    }

    private void TickSecond()
    {
        List<Action>? callbacks = null;

        lock (_sync)
        {
            _elapsedSeconds++;
            foreach (var entry in _intervals.Where(entry => _elapsedSeconds >= entry.NextDueSecond))
            {
                entry.NextDueSecond += entry.IntervalSeconds;
                callbacks ??= [];
                callbacks.Add(entry.Callback);
            }
        }

        TimeEvents.OnEverySecond();

        if (callbacks == null)
            return;

        foreach (var callback in callbacks)
            callback();
    }

    private sealed class IntervalEntry(long intervalSeconds, long nextDueSecond, Action callback)
    {
        public long IntervalSeconds { get; } = intervalSeconds;

        public long NextDueSecond { get; set; } = nextDueSecond;

        public Action Callback { get; } = callback;
    }

    private sealed class IntervalRegistration(TimeSchedulerService scheduler, IntervalEntry entry) : IDisposable
    {
        private bool _disposed;

        public void Dispose()
        {
            if (_disposed) return;

            lock (scheduler._sync)
            {
                scheduler._intervals.Remove(entry);
            }

            _disposed = true;
        }
    }
}

/// <summary>
///     MonoBehaviour responsible for ticking the TimeSchedulerService on Unity's FixedUpdate loop.
/// </summary>
[DisallowMultipleComponent]
internal sealed class TimeSchedulerTicker : MonoBehaviour
{
    private TimeSchedulerService? _scheduler;

    /// <summary>
    ///     Creates a new GameObject with the TimeSchedulerTicker component and initializes it with the provided scheduler.
    /// </summary>
    /// <param name="scheduler"> The TimeSchedulerService instance to tick. </param>
    /// <returns> The created GameObject containing the TimeSchedulerTicker. </returns>
    internal static GameObject Create(TimeSchedulerService scheduler)
    {
        var gameObject = new GameObject(nameof(TimeSchedulerService))
        {
            hideFlags = HideFlags.DontSave
        };

        DontDestroyOnLoad(gameObject);

        var ticker = gameObject.AddComponent<TimeSchedulerTicker>();
        ticker.Initialize(scheduler);

        return gameObject;
    }

    private void Initialize(TimeSchedulerService scheduler)
    {
        _scheduler = scheduler;
        enabled = true;
    }

    private void FixedUpdate()
    {
        _scheduler?.Tick(TimeSpan.FromSeconds(Time.fixedUnscaledDeltaTime));
    }
}

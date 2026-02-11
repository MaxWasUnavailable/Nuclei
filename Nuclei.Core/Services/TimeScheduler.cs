using System;
using System.Collections.Generic;
using System.Linq;
using Nuclei.Events.Events;

namespace Nuclei.Core.Services;

/// <summary>
///     Converts tick updates into time-based events.
/// </summary>
public sealed class TimeScheduler
{
    private static readonly TimeSpan Second = TimeSpan.FromSeconds(1);

    private readonly object _sync = new();
    private readonly List<IntervalEntry> _intervals = [];
    private long _elapsedSeconds;

    /// <summary>
    ///     Initializes a new instance of the <see cref="TimeScheduler" /> class and registers the default time events.
    /// </summary>
    public TimeScheduler()
    {
        RegisterInterval(TimeSpan.FromMinutes(1), TimeEvents.OnEveryMinute);
        RegisterInterval(TimeSpan.FromMinutes(30), TimeEvents.OnEvery30Minutes);
        RegisterInterval(TimeSpan.FromHours(1), TimeEvents.OnEveryHour);
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
    public void Tick(TimeSpan delta)
    {
        if (delta < Second)
            return;

        var secondsToProcess = (long)delta.TotalSeconds;
        for (var i = 0; i < secondsToProcess; i++)
            TickSecond();
    }

    private void TickSecond()
    {
        List<Action>? callbacks = null;

        lock (_sync)
        {
            _elapsedSeconds++;
            var totalSeconds = _elapsedSeconds;

            foreach (var entry in _intervals.Where(entry => totalSeconds >= entry.NextDueSecond))
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

    private sealed class IntervalRegistration(TimeScheduler scheduler, IntervalEntry entry) : IDisposable
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

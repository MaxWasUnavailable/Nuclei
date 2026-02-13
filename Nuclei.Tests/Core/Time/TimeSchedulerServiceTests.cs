using System;
using FluentAssertions;
using Nuclei.Core.Services;
using Nuclei.Events.Events;

namespace Nuclei.Tests.Core.Time;

/// <summary>
///     Tests for <see cref="TimeSchedulerService" />.
/// </summary>
public sealed class TimeSchedulerServiceTests
{
    [Test]
    public void Tick_WithSubSecondDelta_DoesNotFireEverySecond()
    {
        var scheduler = new TimeSchedulerService();
        var secondsFired = 0;

        TimeEvents.EverySecond += Handler;
        try
        {
            scheduler.Tick(TimeSpan.FromMilliseconds(500));
        }
        finally
        {
            TimeEvents.EverySecond -= Handler;
        }

        secondsFired.Should().Be(0);
        return;

        void Handler() => secondsFired++;
    }

    [Test]
    public void Tick_WithMultipleSubSecondDeltas_FiresEverySecondOnlyOnce()
    {
        var scheduler = new TimeSchedulerService();
        var secondsFired = 0;

        TimeEvents.EverySecond += Handler;
        try
        {
            scheduler.Tick(TimeSpan.FromMilliseconds(500));
            scheduler.Tick(TimeSpan.FromMilliseconds(500));
            scheduler.Tick(TimeSpan.FromMilliseconds(500));
        }
        finally
        {
            TimeEvents.EverySecond -= Handler;
        }

        secondsFired.Should().Be(1);
        return;

        void Handler() => secondsFired++;
    }

    [Test]
    public void Tick_WithMultipleSubSecondDeltas_RollsOverProperly_AndFiresEverySecondTwice()
    {
        var scheduler = new TimeSchedulerService();
        var secondsFired = 0;

        TimeEvents.EverySecond += Handler;
        try
        {
            scheduler.Tick(TimeSpan.FromMilliseconds(500));
            scheduler.Tick(TimeSpan.FromMilliseconds(500));
            scheduler.Tick(TimeSpan.FromMilliseconds(500));
            scheduler.Tick(TimeSpan.FromMilliseconds(500));
            scheduler.Tick(TimeSpan.FromMilliseconds(500));
        }
        finally
        {
            TimeEvents.EverySecond -= Handler;
        }

        secondsFired.Should().Be(2);
        return;

        void Handler() => secondsFired++;
    }

    [Test]
    public void Tick_WithMultipleSeconds_FiresEverySecondAndIntervalCallbacks()
    {
        var scheduler = new TimeSchedulerService();
        var secondsFired = 0;
        var intervalFired = 0;

        TimeEvents.EverySecond += Handler;
        try
        {
            using var registration = scheduler.RegisterInterval(TimeSpan.FromSeconds(2), () => intervalFired++);
            scheduler.Tick(TimeSpan.FromSeconds(5));
        }
        finally
        {
            TimeEvents.EverySecond -= Handler;
        }

        secondsFired.Should().Be(5);
        intervalFired.Should().Be(2);
        return;

        void Handler() => secondsFired++;
    }

    [Test]
    public void RegisterInterval_DisposeStopsFurtherCallbacks()
    {
        var scheduler = new TimeSchedulerService();
        var intervalFired = 0;

        var registration = scheduler.RegisterInterval(TimeSpan.FromSeconds(2), () => intervalFired++);
        scheduler.Tick(TimeSpan.FromSeconds(2));
        registration.Dispose();
        scheduler.Tick(TimeSpan.FromSeconds(2));

        intervalFired.Should().Be(1);
    }

    [Test]
    public void RegisterInterval_WithTooSmallInterval_Throws()
    {
        var scheduler = new TimeSchedulerService();

        var act = () => scheduler.RegisterInterval(TimeSpan.FromMilliseconds(500), () => { });

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Test]
    public void RegisterInterval_WithNullCallback_Throws()
    {
        var scheduler = new TimeSchedulerService();

        var act = () => scheduler.RegisterInterval(TimeSpan.FromSeconds(1), null!);

        act.Should().Throw<ArgumentNullException>();
    }
}


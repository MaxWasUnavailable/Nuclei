using System;
using Nuclei.Abstractions.NO.Factions;
using Nuclei.Abstractions.NO.Positions;
using Nuclei.Abstractions.NO.Units;

namespace Nuclei.Events.Events;

/// <summary>
///     Declares unit-related events.
/// </summary>
public static class UnitEvents
{
    /// <summary>
    ///     Fired before a unit is tracked by a faction.
    /// </summary>
    public static event Action<UnitTrackingEvent>? PreUnitTracked;

    /// <summary>
    ///     Fired after a unit is tracked by a faction.
    /// </summary>
    public static event Action<UnitTrackingEvent>? PostUnitTracked;

    /// <summary>
    ///     Fired before a unit is removed from tracking.
    /// </summary>
    public static event Action<UnitTrackingEvent>? PreUnitUntracked;

    /// <summary>
    ///     Fired after a unit is removed from tracking.
    /// </summary>
    public static event Action<UnitTrackingEvent>? PostUnitUntracked;

    /// <summary>
    ///     Fired before a faction registers a unit.
    /// </summary>
    public static event Action<FactionUnitEvent>? PreUnitRegistered;

    /// <summary>
    ///     Fired after a faction registers a unit.
    /// </summary>
    public static event Action<FactionUnitEvent>? PostUnitRegistered;

    /// <summary>
    ///     Fired before a faction removes a unit.
    /// </summary>
    public static event Action<FactionUnitEvent>? PreUnitRemoved;

    /// <summary>
    ///     Fired after a faction removes a unit.
    /// </summary>
    public static event Action<FactionUnitEvent>? PostUnitRemoved;

    /// <summary>
    ///     Fired before a unit changes faction.
    /// </summary>
    public static event Action<UnitFactionChangedEvent>? PreUnitFactionChanged;

    /// <summary>
    ///     Fired after a unit changes faction.
    /// </summary>
    public static event Action<UnitFactionChangedEvent>? PostUnitFactionChanged;

    internal static void OnPreUnitTracked(UnitTrackingEvent payload)
    {
        PreUnitTracked?.Invoke(payload);
    }

    internal static void OnPostUnitTracked(UnitTrackingEvent payload)
    {
        PostUnitTracked?.Invoke(payload);
    }

    internal static void OnPreUnitUntracked(UnitTrackingEvent payload)
    {
        PreUnitUntracked?.Invoke(payload);
    }

    internal static void OnPostUnitUntracked(UnitTrackingEvent payload)
    {
        PostUnitUntracked?.Invoke(payload);
    }

    internal static void OnPreUnitRegistered(FactionUnitEvent payload)
    {
        PreUnitRegistered?.Invoke(payload);
    }

    internal static void OnPostUnitRegistered(FactionUnitEvent payload)
    {
        PostUnitRegistered?.Invoke(payload);
    }

    internal static void OnPreUnitRemoved(FactionUnitEvent payload)
    {
        PreUnitRemoved?.Invoke(payload);
    }

    internal static void OnPostUnitRemoved(FactionUnitEvent payload)
    {
        PostUnitRemoved?.Invoke(payload);
    }

    internal static void OnPreUnitFactionChanged(UnitFactionChangedEvent payload)
    {
        PreUnitFactionChanged?.Invoke(payload);
    }

    internal static void OnPostUnitFactionChanged(UnitFactionChangedEvent payload)
    {
        PostUnitFactionChanged?.Invoke(payload);
    }
}

/// <summary>
///     Payload for unit tracking events.
/// </summary>
/// <param name="Faction">The faction HQ emitting the event.</param>
/// <param name="Unit">The unit involved.</param>
/// <param name="LastKnownPosition">The last known position, if available.</param>
public sealed record UnitTrackingEvent(IFactionInfo Faction, IUnitInfo Unit, IGlobalPosition? LastKnownPosition);

/// <summary>
///     Payload for faction unit registration events.
/// </summary>
/// <param name="Faction">The faction HQ emitting the event.</param>
/// <param name="Unit">The unit involved.</param>
public sealed record FactionUnitEvent(IFactionInfo Faction, IUnitInfo Unit);

/// <summary>
///     Payload for unit faction change events.
/// </summary>
/// <param name="Faction">The faction HQ emitting the event.</param>
/// <param name="Unit">The unit involved.</param>
public sealed record UnitFactionChangedEvent(IFactionInfo Faction, IUnitInfo Unit);

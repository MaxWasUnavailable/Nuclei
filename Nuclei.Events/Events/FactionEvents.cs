using System;
using Nuclei.Abstractions.Airbases;
using Nuclei.Abstractions.Factions;
using Nuclei.Abstractions.Zones;

namespace Nuclei.Events.Events;

/// <summary>
///     Declares faction-related events.
/// </summary>
public static class FactionEvents
{
    /// <summary>
    ///     Fired before an exclusion zone is registered.
    /// </summary>
    public static event Action<FactionExclusionZoneEvent>? PreExclusionZoneRegistered;

    /// <summary>
    ///     Fired after an exclusion zone is registered.
    /// </summary>
    public static event Action<FactionExclusionZoneEvent>? PostExclusionZoneRegistered;

    /// <summary>
    ///     Fired before the faction player roster changes.
    /// </summary>
    public static event Action<FactionRosterEvent>? PreFactionRosterChanged;

    /// <summary>
    ///     Fired after the faction player roster changes.
    /// </summary>
    public static event Action<FactionRosterEvent>? PostFactionRosterChanged;

    /// <summary>
    ///     Fired before an airbase is added to a faction.
    /// </summary>
    public static event Action<FactionAirbaseEvent>? PreAirbaseAdded;

    /// <summary>
    ///     Fired after an airbase is added to a faction.
    /// </summary>
    public static event Action<FactionAirbaseEvent>? PostAirbaseAdded;

    /// <summary>
    ///     Fired before an airbase is removed from a faction.
    /// </summary>
    public static event Action<FactionAirbaseEvent>? PreAirbaseRemoved;

    /// <summary>
    ///     Fired after an airbase is removed from a faction.
    /// </summary>
    public static event Action<FactionAirbaseEvent>? PostAirbaseRemoved;

    internal static void OnPreExclusionZoneRegistered(FactionExclusionZoneEvent payload)
    {
        PreExclusionZoneRegistered?.Invoke(payload);
    }

    internal static void OnPostExclusionZoneRegistered(FactionExclusionZoneEvent payload)
    {
        PostExclusionZoneRegistered?.Invoke(payload);
    }

    internal static void OnPreFactionRosterChanged(FactionRosterEvent payload)
    {
        PreFactionRosterChanged?.Invoke(payload);
    }

    internal static void OnPostFactionRosterChanged(FactionRosterEvent payload)
    {
        PostFactionRosterChanged?.Invoke(payload);
    }

    internal static void OnPreAirbaseAdded(FactionAirbaseEvent payload)
    {
        PreAirbaseAdded?.Invoke(payload);
    }

    internal static void OnPostAirbaseAdded(FactionAirbaseEvent payload)
    {
        PostAirbaseAdded?.Invoke(payload);
    }

    internal static void OnPreAirbaseRemoved(FactionAirbaseEvent payload)
    {
        PreAirbaseRemoved?.Invoke(payload);
    }

    internal static void OnPostAirbaseRemoved(FactionAirbaseEvent payload)
    {
        PostAirbaseRemoved?.Invoke(payload);
    }
}

/// <summary>
///     Payload for faction exclusion zone events.
/// </summary>
/// <param name="Faction">The faction HQ emitting the event.</param>
/// <param name="Zone">The exclusion zone registered.</param>
public sealed record FactionExclusionZoneEvent(IFactionInfo Faction, IExclusionZoneInfo Zone);

/// <summary>
///     Payload for faction roster change events.
/// </summary>
/// <param name="Faction">The faction HQ emitting the event.</param>
public sealed record FactionRosterEvent(IFactionInfo Faction);

/// <summary>
///     Payload for faction airbase events.
/// </summary>
/// <param name="Faction">The faction HQ emitting the event.</param>
/// <param name="Airbase">The airbase involved.</param>
public sealed record FactionAirbaseEvent(IFactionInfo Faction, IAirbaseInfo Airbase);

using System;
using Nuclei.Abstractions.Airbases;
using Nuclei.Abstractions.Factions;
using Nuclei.Abstractions.Players;
using Nuclei.Abstractions.Units;

namespace Nuclei.Events.Events;

/// <summary>
///     Declares combat-related events.
/// </summary>
public static class CombatEvents
{
    /// <summary>
    ///     Fired before a credit award is processed.
    /// </summary>
    public static event Action<CreditAwardedEvent>? PreCreditAwarded;

    /// <summary>
    ///     Fired after a credit award is processed.
    /// </summary>
    public static event Action<CreditAwardedEvent>? PostCreditAwarded;

    /// <summary>
    ///     Fired before a bomb failure is reported.
    /// </summary>
    public static event Action<BombFailureEvent>? PreBombFailed;

    /// <summary>
    ///     Fired after a bomb failure is reported.
    /// </summary>
    public static event Action<BombFailureEvent>? PostBombFailed;

    /// <summary>
    ///     Fired before a kill feed event is reported.
    /// </summary>
    public static event Action<KillFeedEvent>? PreKillFeed;

    /// <summary>
    ///     Fired after a kill feed event is reported.
    /// </summary>
    public static event Action<KillFeedEvent>? PostKillFeed;

    /// <summary>
    ///     Fired before a pilot capture event is reported.
    /// </summary>
    public static event Action<PilotCaptureEvent>? PrePilotCaptured;

    /// <summary>
    ///     Fired after a pilot capture event is reported.
    /// </summary>
    public static event Action<PilotCaptureEvent>? PostPilotCaptured;

    /// <summary>
    ///     Fired before a repair event is reported.
    /// </summary>
    public static event Action<RepairEvent>? PreRepairReported;

    /// <summary>
    ///     Fired after a repair event is reported.
    /// </summary>
    public static event Action<RepairEvent>? PostRepairReported;

    /// <summary>
    ///     Fired before a warhead destruction event is reported.
    /// </summary>
    public static event Action<WarheadDestroyedEvent>? PreWarheadDestroyed;

    /// <summary>
    ///     Fired after a warhead destruction event is reported.
    /// </summary>
    public static event Action<WarheadDestroyedEvent>? PostWarheadDestroyed;

    internal static void OnPreCreditAwarded(CreditAwardedEvent payload)
    {
        PreCreditAwarded?.Invoke(payload);
    }

    internal static void OnPostCreditAwarded(CreditAwardedEvent payload)
    {
        PostCreditAwarded?.Invoke(payload);
    }

    internal static void OnPreBombFailed(BombFailureEvent payload)
    {
        PreBombFailed?.Invoke(payload);
    }

    internal static void OnPostBombFailed(BombFailureEvent payload)
    {
        PostBombFailed?.Invoke(payload);
    }

    internal static void OnPreKillFeed(KillFeedEvent payload)
    {
        PreKillFeed?.Invoke(payload);
    }

    internal static void OnPostKillFeed(KillFeedEvent payload)
    {
        PostKillFeed?.Invoke(payload);
    }

    internal static void OnPrePilotCaptured(PilotCaptureEvent payload)
    {
        PrePilotCaptured?.Invoke(payload);
    }

    internal static void OnPostPilotCaptured(PilotCaptureEvent payload)
    {
        PostPilotCaptured?.Invoke(payload);
    }

    internal static void OnPreRepairReported(RepairEvent payload)
    {
        PreRepairReported?.Invoke(payload);
    }

    internal static void OnPostRepairReported(RepairEvent payload)
    {
        PostRepairReported?.Invoke(payload);
    }

    internal static void OnPreWarheadDestroyed(WarheadDestroyedEvent payload)
    {
        PreWarheadDestroyed?.Invoke(payload);
    }

    internal static void OnPostWarheadDestroyed(WarheadDestroyedEvent payload)
    {
        PostWarheadDestroyed?.Invoke(payload);
    }
}

/// <summary>
///     Payload for credit award events.
/// </summary>
/// <param name="Recipient">The recipient player.</param>
/// <param name="Target">The target unit.</param>
/// <param name="CreditAwarded">The credit amount awarded.</param>
/// <param name="ActionType">The reward type.</param>
public sealed record CreditAwardedEvent(IPlayerInfo Recipient, IUnitInfo Target, float CreditAwarded, string ActionType);

/// <summary>
///     Payload for bomb failure events.
/// </summary>
/// <param name="Bomb">The bomb unit.</param>
/// <param name="GForce">The reported G-force.</param>
public sealed record BombFailureEvent(IUnitInfo Bomb, float GForce);

/// <summary>
///     Payload for kill feed events.
/// </summary>
/// <param name="Killer">The killer unit.</param>
/// <param name="Killed">The killed unit.</param>
/// <param name="KilledType">The kill type reported.</param>
public sealed record KillFeedEvent(IUnitInfo Killer, IUnitInfo Killed, string KilledType);

/// <summary>
///     Payload for pilot capture events.
/// </summary>
/// <param name="Unit">The unit involved.</param>
/// <param name="Rescued">Whether the pilot was rescued.</param>
public sealed record PilotCaptureEvent(IUnitInfo Unit, bool Rescued);

/// <summary>
///     Payload for repair events.
/// </summary>
/// <param name="Unit">The unit involved.</param>
public sealed record RepairEvent(IUnitInfo Unit);

/// <summary>
///     Payload for warhead destruction events.
/// </summary>
/// <param name="Airbase">The airbase involved.</param>
/// <param name="Faction">The faction involved.</param>
/// <param name="Count">The number of warheads destroyed.</param>
public sealed record WarheadDestroyedEvent(IAirbaseInfo Airbase, IFactionInfo Faction, int Count);

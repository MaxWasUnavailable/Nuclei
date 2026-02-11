using System;
using Nuclei.Abstractions.NO.Factions;
using Nuclei.Abstractions.NO.Players;

namespace Nuclei.Events.Events;

/// <summary>
///     Declares announcement-related events.
/// </summary>
public static class AnnouncementEvents
{
    /// <summary>
    ///     Fired before a player-join announcement is shown.
    /// </summary>
    public static event Action<PlayerAnnouncementEvent>? PrePlayerJoinedAnnouncement;

    /// <summary>
    ///     Fired after a player-join announcement is shown.
    /// </summary>
    public static event Action<PlayerAnnouncementEvent>? PostPlayerJoinedAnnouncement;

    /// <summary>
    ///     Fired before a player-disconnect announcement is shown.
    /// </summary>
    public static event Action<PlayerAnnouncementEvent>? PrePlayerDisconnectedAnnouncement;

    /// <summary>
    ///     Fired after a player-disconnect announcement is shown.
    /// </summary>
    public static event Action<PlayerAnnouncementEvent>? PostPlayerDisconnectedAnnouncement;

    /// <summary>
    ///     Fired before a player-faction-join announcement is shown.
    /// </summary>
    public static event Action<PlayerFactionAnnouncementEvent>? PrePlayerJoinedFactionAnnouncement;

    /// <summary>
    ///     Fired after a player-faction-join announcement is shown.
    /// </summary>
    public static event Action<PlayerFactionAnnouncementEvent>? PostPlayerJoinedFactionAnnouncement;

    /// <summary>
    ///     Fired before a faction broadcast announcement is shown.
    /// </summary>
    public static event Action<TextAnnouncementEvent>? PreHqBroadcastAnnouncement;

    /// <summary>
    ///     Fired after a faction broadcast announcement is shown.
    /// </summary>
    public static event Action<TextAnnouncementEvent>? PostHqBroadcastAnnouncement;

    /// <summary>
    ///     Fired before a faction announcement is shown.
    /// </summary>
    public static event Action<HqTextAnnouncementEvent>? PreHqAnnouncement;

    /// <summary>
    ///     Fired after a faction announcement is shown.
    /// </summary>
    public static event Action<HqTextAnnouncementEvent>? PostHqAnnouncement;

    internal static void OnPrePlayerJoinedAnnouncement(PlayerAnnouncementEvent payload)
    {
        PrePlayerJoinedAnnouncement?.Invoke(payload);
    }

    internal static void OnPostPlayerJoinedAnnouncement(PlayerAnnouncementEvent payload)
    {
        PostPlayerJoinedAnnouncement?.Invoke(payload);
    }

    internal static void OnPrePlayerDisconnectedAnnouncement(PlayerAnnouncementEvent payload)
    {
        PrePlayerDisconnectedAnnouncement?.Invoke(payload);
    }

    internal static void OnPostPlayerDisconnectedAnnouncement(PlayerAnnouncementEvent payload)
    {
        PostPlayerDisconnectedAnnouncement?.Invoke(payload);
    }

    internal static void OnPrePlayerJoinedFactionAnnouncement(PlayerFactionAnnouncementEvent payload)
    {
        PrePlayerJoinedFactionAnnouncement?.Invoke(payload);
    }

    internal static void OnPostPlayerJoinedFactionAnnouncement(PlayerFactionAnnouncementEvent payload)
    {
        PostPlayerJoinedFactionAnnouncement?.Invoke(payload);
    }

    internal static void OnPreHqBroadcastAnnouncement(TextAnnouncementEvent payload)
    {
        PreHqBroadcastAnnouncement?.Invoke(payload);
    }

    internal static void OnPostHqBroadcastAnnouncement(TextAnnouncementEvent payload)
    {
        PostHqBroadcastAnnouncement?.Invoke(payload);
    }

    internal static void OnPreHqAnnouncement(HqTextAnnouncementEvent payload)
    {
        PreHqAnnouncement?.Invoke(payload);
    }

    internal static void OnPostHqAnnouncement(HqTextAnnouncementEvent payload)
    {
        PostHqAnnouncement?.Invoke(payload);
    }
}

/// <summary>
///     Payload for player announcement events.
/// </summary>
/// <param name="Player">The player involved.</param>
public sealed record PlayerAnnouncementEvent(IPlayerInfo Player);

/// <summary>
///     Payload for player faction announcement events.
/// </summary>
/// <param name="Player">The player involved.</param>
/// <param name="Faction">The faction involved.</param>
public sealed record PlayerFactionAnnouncementEvent(IPlayerInfo Player, IFactionInfo Faction);

/// <summary>
///     Payload for text-only announcements.
/// </summary>
/// <param name="Text">The announcement text.</param>
public sealed record TextAnnouncementEvent(string Text);

/// <summary>
///     Payload for faction text announcements.
/// </summary>
/// <param name="Faction">The faction involved.</param>
/// <param name="Text">The announcement text.</param>
public sealed record HqTextAnnouncementEvent(IFactionInfo Faction, string Text);

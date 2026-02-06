using System;
using Nuclei.Abstractions.Players;

namespace Nuclei.Events.Events;

/// <summary>
///     Declares player-related events.
/// </summary>
public static class PlayerEvents
{
    /// <summary>
    ///     Fired before a player connection is registered.
    /// </summary>
    public static event Action<PlayerConnectionEvent>? PrePlayerConnected;

    /// <summary>
    ///     Fired after a player connection is registered.
    /// </summary>
    public static event Action<PlayerConnectionEvent>? PostPlayerConnected;

    /// <summary>
    ///     Fired before a player is authenticated.
    /// </summary>
    public static event Action<PlayerAuthenticationEvent>? PrePlayerAuthenticated;

    /// <summary>
    ///     Fired after a player is authenticated.
    /// </summary>
    public static event Action<PlayerAuthenticationEvent>? PostPlayerAuthenticated;

    /// <summary>
    ///     Fired before a player is fully joined.
    /// </summary>
    public static event Action<PlayerJoinEvent>? PrePlayerJoined;

    /// <summary>
    ///     Fired after a player is fully joined.
    /// </summary>
    public static event Action<PlayerJoinEvent>? PostPlayerJoined;

    /// <summary>
    ///     Fired before a player leaves the server.
    /// </summary>
    public static event Action<PlayerLeaveEvent>? PrePlayerLeft;

    /// <summary>
    ///     Fired after a player leaves the server.
    /// </summary>
    public static event Action<PlayerLeaveEvent>? PostPlayerLeft;

    internal static void OnPrePlayerConnected(PlayerConnectionEvent payload)
    {
        PrePlayerConnected?.Invoke(payload);
    }

    internal static void OnPostPlayerConnected(PlayerConnectionEvent payload)
    {
        PostPlayerConnected?.Invoke(payload);
    }

    internal static void OnPrePlayerAuthenticated(PlayerAuthenticationEvent payload)
    {
        PrePlayerAuthenticated?.Invoke(payload);
    }

    internal static void OnPostPlayerAuthenticated(PlayerAuthenticationEvent payload)
    {
        PostPlayerAuthenticated?.Invoke(payload);
    }

    internal static void OnPrePlayerJoined(PlayerJoinEvent payload)
    {
        PrePlayerJoined?.Invoke(payload);
    }

    internal static void OnPostPlayerJoined(PlayerJoinEvent payload)
    {
        PostPlayerJoined?.Invoke(payload);
    }

    internal static void OnPrePlayerLeft(PlayerLeaveEvent payload)
    {
        PrePlayerLeft?.Invoke(payload);
    }

    internal static void OnPostPlayerLeft(PlayerLeaveEvent payload)
    {
        PostPlayerLeft?.Invoke(payload);
    }
}

/// <summary>
///     Event data for player connection events.
/// </summary>
/// <param name="Player"> The player involved in the event. </param>
public sealed record PlayerConnectionEvent(IPlayerInfo Player);

/// <summary>
///     Event data for player authentication events.
/// </summary>
/// <param name="Player"> The player involved in the event. </param>
public sealed record PlayerAuthenticationEvent(IPlayerInfo Player);

/// <summary>
///     Event data for player join events.
/// </summary>
/// <param name="Player"> The player involved in the event. </param>
public sealed record PlayerJoinEvent(IPlayerInfo Player);

/// <summary>
///     Event data for player leave events.
/// </summary>
/// <param name="Player"> The player involved in the event. </param>
public sealed record PlayerLeaveEvent(IPlayerInfo Player);

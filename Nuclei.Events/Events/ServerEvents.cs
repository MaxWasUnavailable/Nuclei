using System;

namespace Nuclei.Events.Events;

/// <summary>
///     Declares server-related events.
/// </summary>
public static class ServerEvents
{
    /// <summary>
    ///     Fired before the server starts.
    /// </summary>
    public static event Action? PreServerStarted;

    /// <summary>
    ///     Fired after the server starts.
    /// </summary>
    public static event Action? PostServerStarted;

    internal static void OnPreServerStarted()
    {
        PreServerStarted?.Invoke();
    }

    internal static void OnPostServerStarted()
    {
        PostServerStarted?.Invoke();
    }
}

using System;
using Nuclei.Features;

namespace Nuclei.Events;

/// <summary>
///     Declares server-related events.
/// </summary>
public static class ServerEvents
{
    /// <summary>
    ///     Event handler for when the server starts.
    /// </summary>
    public static event Action? ServerStarted;

    internal static void OnServerStarted()
    {
        ServerStarted?.Invoke();
        TimeService.Initialize();
    }

    /// <summary>
    ///     Event handler for when the server stops.
    /// </summary>
    public static event Action? ServerStopped;
    
    internal static void OnServerStopped()
    {
        ServerStopped?.Invoke();
    }
}

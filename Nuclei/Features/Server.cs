using Nuclei.Helpers;
using UnityEngine;

namespace Nuclei.Features;

/// <summary>
///     Server helpers for the official Nuclear Option dedicated server.
/// </summary>
public static class Server
{
    /// <summary>
    ///     Indicates whether the dedicated server network layer is active.
    /// </summary>
    public static bool IsServerRunning => Globals.NetworkManagerNuclearOptionInstance.Server?.Active ?? false;

    /// <summary>
    ///     The current mission time, calculated from the dedicated server's mission manager.
    /// </summary>
    public static double MissionTime => Globals.MissionManagerInstance.MissionTime;

    /// <summary>
    ///     Gets the current server FPS.
    /// </summary>
    public static double GetServerFPS()
    {
        return 1 / Time.unscaledDeltaTime;
    }

    /// <summary>
    ///     Stops the dedicated server process.
    /// </summary>
    public static void StopServer()
    {
        Nuclei.Logger?.LogInfo("Stopping server...");
        ChatService.SendChatMessage("Server is stopping...");
        Application.Quit();
    }

    /// <summary>
    ///     Mission restarts are handled by the official dedicated server runner.
    /// </summary>
    public static void StartOrRestartLobby()
    {
        Nuclei.Logger?.LogWarning("Mission restart is not implemented for the official dedicated server build.");
    }
}

using Nuclei.Helpers;
using UnityEngine;

namespace Nuclei.Features;

/// <summary>
///     Server functionality for Nuclei.
/// </summary>
public static class Server
{
    /// <summary>
    ///     Indicates whether the server is currently running.
    /// </summary>
    public static bool IsServerRunning => Globals.NetworkManagerNuclearOptionInstance.Server?.Active ?? false;

    /// <summary>
    ///     Stops the dedicated server.
    /// </summary>
    public static void StopServer()
    {
        Nuclei.Logger?.LogInfo("Stopping server...");
        
        EndMission();
        
        Nuclei.Logger?.LogInfo("Server stopped.");
        
        Application.Quit();
    }

    /// <summary>
    ///     Ends the current mission and returns the server to the main menu state.
    /// </summary>
    public static void EndMission()
    {
        Nuclei.Logger?.LogInfo("Ending mission...");
        
        if (!IsServerRunning)
        {
            Nuclei.Logger?.LogWarning("Server is not running.");
            return;
        }
        
        if (GameManager.gameState != GameManager.GameState.Multiplayer)
        {
            Nuclei.Logger?.LogWarning("Cannot end mission while not in game.");
            return;
        }
        
        ServerMessenger.SendHostEndedMessage();
        
        Globals.NetworkManagerNuclearOptionInstance.Stop(false);
        
        GameManager.SetGameState(GameManager.GameState.Menu);
        
        Nuclei.Logger?.LogInfo("Mission ended.");
    }

    /// <summary>
    ///     Starts the dedicated server.
    /// </summary>
    public static void StartServer()
    {
        if (IsServerRunning)
        {
            Nuclei.Logger?.LogWarning("Server is already running.");
            return;
        }
        
        Nuclei.Logger?.LogInfo("Starting server...");
        
        // TODO: remove this. It's a temporary test to see if the list of missions is being populated correctly.
        foreach (var missionKey in ServerMissionManager.AllMissions) 
            Nuclei.Logger?.LogInfo($"Found mission: {missionKey}");
        
        // TODO: Implement server startup logic here.
        
        Nuclei.Logger?.LogInfo("Server started.");
    }
}
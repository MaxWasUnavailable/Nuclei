using System;
using System.Threading.Tasks;
using NuclearOption.Networking;
using NuclearOption.SavedMission;
using Nuclei.Helpers;
using Steamworks;
using UnityEngine;
using UnityEngine.Audio;
using Object = UnityEngine.Object;

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
        
        MessageService.SendHostEndedMessage();
        
        Globals.NetworkManagerNuclearOptionInstance.Stop(false);
        
        GameManager.SetGameState(GameManager.GameState.Menu);
        
        Nuclei.Logger?.LogInfo("Mission ended.");
    }

    /// <summary>
    ///     Select a random mission on the server, based on the server config.
    /// </summary>
    public static void SelectRandomMission()
    {
        Nuclei.Logger?.LogInfo("Selecting random mission...");

        var mission = MissionService.GetRandomMission(Nuclei.Instance!.AllowRepeatMission!.Value);
        
        if (mission == null)
        {
            Nuclei.Logger?.LogWarning("Failed to get a random mission.");
            return;
        }
        
        SelectMission(mission);
    }

    /// <summary>
    ///     Select the given mission on the server.
    /// </summary>
    /// <param name="mission"> The mission to start. </param>
    public static void SelectMission(Mission mission)
    {
        Nuclei.Logger?.LogInfo($"Selected mission: {mission.Name}");
        
        if (IsServerRunning)
        {
            Nuclei.Logger?.LogWarning("Server is already running.");
            return;
        }
        
        if (GameManager.gameState == GameManager.GameState.Multiplayer)
        {
            Nuclei.Logger?.LogWarning("Cannot start mission while in game.");
            return;
        }

        MissionService.SetMission(mission);
    }

    /// <summary>
    ///     Creates a <see cref="HostOptions" /> object based on the server config and the currently selected mission.
    /// </summary>
    /// <returns> The created <see cref="HostOptions" /> object. </returns>
    public static HostOptions CreateHostOptionsForCurrentMission()
    {
        return CreateHostOptions(MissionService.CurrentMission!);
    }

    /// <summary>
    ///     Creates a <see cref="HostOptions" /> object based on the server config and a given mission.
    /// </summary>
    /// <param name="mission"> The mission to create the <see cref="HostOptions" /> object for. </param>
    /// <returns> The created <see cref="HostOptions" /> object. </returns>
    public static HostOptions CreateHostOptions(Mission mission)
    {
        return new HostOptions
        {
            SocketType = Nuclei.Instance!.UseSteamSocket!.Value ? SocketType.Steam : SocketType.UDP,
            MaxConnections = Nuclei.Instance!.MaxPlayers!.Value,
            Map = mission.MapKey,
            UdpPort = Nuclei.Instance!.UseSteamSocket!.Value ? null : Nuclei.Instance!.UdpPort!.Value
        };
    }

    /// <summary>
    ///     Starts a Steam lobby.
    /// </summary>
    public static void StartSteamLobby()
    {
        Nuclei.Logger?.LogInfo("Starting Steam lobby...");
        
        if (SteamMatchmaking.GetLobbyByIndex(0) != CSteamID.Nil)
        {
            Nuclei.Logger?.LogWarning("Steam lobby already exists.");
            return;
        }
        
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, Nuclei.Instance!.MaxPlayers!.Value);
        
        Nuclei.Logger?.LogInfo("Steam lobby started.");
    }

    /// <summary>
    ///     Starts a mission on the server, using the currently selected mission.
    /// </summary>
    public static void StartMission()
    {
        Nuclei.Logger?.LogInfo("Starting mission...");
        
        if (IsServerRunning)
        {
            Nuclei.Logger?.LogWarning("Server is already running.");
            return;
        }
        
        if (GameManager.gameState == GameManager.GameState.Multiplayer)
        {
            Nuclei.Logger?.LogWarning("Cannot start new mission while in game.");
            return;
        }
        
        Globals.NetworkManagerNuclearOptionInstance.StartHost(CreateHostOptionsForCurrentMission());
        
        Nuclei.Logger?.LogInfo("Mission started.");
    }

    /// <summary>
    ///     Starts or restarts the lobby, selecting a random mission first.
    /// </summary>
    public static void StartOrRestartLobby()
    {
        if (IsServerRunning)
        {
            Nuclei.Logger?.LogInfo("Already running, ending mission first...");
            EndMission();
        }

        if (MissionService.TryGetConsumePreselectedMission(out var mission))
            SelectMission(mission!);
        else
            SelectRandomMission();
        
        StartSteamLobby();
        StartMission();

        Resources.UnloadUnusedAssets();
    }
    
    private static void DisableAudio()
    {
        Nuclei.Logger?.LogDebug("Disabling audio...");
        
        Object.FindObjectOfType<AudioMixer>()?.SetFloat("MasterVolume", -80f);

        Nuclei.Logger?.LogDebug("Audio disabled.");
    }
    
    private static void SetupLocalPlayer()
    {
        Nuclei.Logger?.LogDebug("Setting up local player...");
        
        Globals.LocalPlayer.PlayerName = "Server";
        Globals.LocalPlayer.SteamID = 0;
        Globals.LocalPlayer.CmdSetPlayerName("Server");
        
        Nuclei.Logger?.LogDebug("Local player set up.");
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

        if (!MissionService.ValidateMissionConfig())
        {
            Nuclei.Logger?.LogError("Failed to validate mission config! Aborting server launch.");
            return;
        }
        
        StartOrRestartLobby();
        
        Nuclei.Logger?.LogInfo("Server started.");
    }
}
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NuclearOption.Networking;
using NuclearOption.SavedMission;
using Nuclei.Events;
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
    ///     The current mission time, calculated from the server's perspective.
    /// </summary>
    public static double MissionTime => Globals.LocalPlayer.NetworkTime.Time;
    
    private static int _lastMotDSent;
    
    private static void CheckSendMotD()
    {
        if (NucleiConfig.MotDFrequency!.Value <= 0)
            return;
        
        if (Time.time - _lastMotDSent < NucleiConfig.MotDFrequency!.Value && _lastMotDSent > 0)
            return;
        
        ChatService.SendMotD();
        _lastMotDSent = (int) Time.time;
    }

    private static void CheckMissionOverTime()
    {
        if (NucleiConfig.MissionDuration!.Value <= 0)
            return;
        
        if (MissionTime < NucleiConfig.MissionDuration!.Value)
            return;
        
        Nuclei.Logger?.LogInfo("Mission over-time reached, notifying players...");
        ChatService.SendChatMessage("Mission over-time reached, ending mission prematurely...");
        
        _ = SlowlyEndMission();
    }
    
    private static void OnMissionEnded(Mission mission)
    {
        Nuclei.Logger?.LogInfo($"Mission ended: {mission.Name}. Faction scores: {mission.factions.Aggregate("", (current, faction) => current + $"{faction.factionName}: {faction.FactionHQ.factionScore} ")}");
        
        _ = SlowlyEndMission();
    }
    
    private static async UniTask SlowlyEndMission()
    {
        ChatService.SendChatMessage("Ending mission and starting a new one in 30 seconds...");
        
        await Task.Delay(20000);
        
        ChatService.SendChatMessage("Ending mission and starting a new one in 10 seconds...");
        
        await Task.Delay(10000);
        
        Nuclei.Logger?.LogInfo("Ending mission and starting a new one...");

        await StartOrRestartLobby();
    }
    
    private static void TimeStatus()
    {
        Nuclei.Logger?.LogInfo($"Mission time: {MissionTime} seconds ({MissionTime / 60} minutes)");
        Nuclei.Logger?.LogInfo($"Remaining time: {NucleiConfig.MissionDuration!.Value - MissionTime} seconds ({(NucleiConfig.MissionDuration!.Value - MissionTime) / 60} minutes)");
        Nuclei.Logger?.LogInfo($"Server FPS: {GetServerFPS()}");
    }

    /// <summary>
    ///     Gets the current server FPS.
    /// </summary>
    /// <returns> The current server FPS. </returns>
    public static double GetServerFPS()
    {
        return 1 / Time.unscaledDeltaTime;
    }

    /// <summary>
    ///     Stops the dedicated server.
    /// </summary>
    public static void StopServer()
    {
        Nuclei.Logger?.LogInfo("Stopping server...");
        
        EndMission();
        
        Task.Delay(3000).ContinueWith(_ =>
        {
            Nuclei.Logger?.LogInfo("Server stopped.");
            Application.Quit();
        });
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
        
        MessageService.SendHostEndedMessage();
        
        Globals.NetworkManagerNuclearOptionInstance.Stop(false);
        
        SteamLobbyService.StopSteamLobby();
        
        Nuclei.Logger?.LogInfo("Mission ended.");
    }

    /// <summary>
    ///     Select the next mission on the server, based on the server config's mission list and MissionSelectMode.
    /// </summary>
    public static void SelectNextMission()
    {
        Nuclei.Logger?.LogInfo("Selecting next mission...");

        var mission = MissionService.GetNextMission(NucleiConfig.MissionSelectMode!.Value, NucleiConfig.UseAllMissions!.Value);
        
        if (mission == null)
        {
            Nuclei.Logger?.LogError("Failed to get a new mission.");
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
            SocketType = NucleiConfig.UseSteamSocket!.Value ? SocketType.Steam : SocketType.UDP,
            MaxConnections = NucleiConfig.MaxPlayers!.Value,
            Map = mission.MapKey,
            UdpPort = NucleiConfig.UseSteamSocket!.Value ? null : NucleiConfig.UdpPort!.Value
        };
    }

    /// <summary>
    ///     Starts a mission on the server, using the currently selected mission.
    /// </summary>
    public static async UniTask StartMission()
    {
        Nuclei.Logger?.LogInfo("Starting mission...");
        
        if (IsServerRunning)
        {
            Nuclei.Logger?.LogWarning("Server is already running.");
            return;
        }
        
        await Globals.NetworkManagerNuclearOptionInstance.StartHostAsync(CreateHostOptionsForCurrentMission());
        
        MissionEvents.OnNewMissionStarted(MissionService.CurrentMission!);
        
        Nuclei.Logger?.LogInfo("Mission started.");
    }

    /// <summary>
    ///     Starts or restarts the lobby, selecting a random mission first.
    /// </summary>
    public static async UniTask StartOrRestartLobby()
    {
        if (IsServerRunning)
        {
            Nuclei.Logger?.LogInfo("Already running, ending mission first...");
            EndMission();
            await UniTask.WaitUntil(() => !Globals.NetworkManagerNuclearOptionInstance.stopping);
        }
        
        GameManager.SetGameState(GameManager.GameState.Multiplayer);

        if (MissionService.TryGetConsumePreselectedMission(out var mission))
            SelectMission(mission!);
        else
            SelectNextMission();

        await SteamLobbyService.StartSteamLobby();
        await StartMission();
        await SteamLobbyService.SetPingData();
        SteamLobbyService.SetLobbyData();
        
        ApplyFramerateSettings();
        ApplyExperimentalSettings();
        
        if (NucleiConfig.MuteAfterStart!.Value)
            DisableAudio();

        Resources.UnloadUnusedAssets();
    }
    
    private static void DisableAudio()
    {
        Nuclei.Logger?.LogDebug("Disabling audio...");
        
        Globals.AudioMixerVolumeInstance.ChangeMixerVolume(AudioMixerVolume.Master, 0);

        Nuclei.Logger?.LogDebug("Audio disabled.");
    }

    private static void ApplyFramerateSettings()
    {
        QualitySettings.SetQualityLevel(0, true);
        QualitySettings.maxQueuedFrames = 0;
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = NucleiConfig.TargetFrameRate!.Value;
    }
    
    private static void ApplyExperimentalSettings()
    {
        if (NucleiConfig.UseUpdateForPhysicsUpdate!.Value)
            Physics.simulationMode = SimulationMode.Update;
        
        if (NucleiConfig.PhysicsUpdatesPerSecond!.Value != 60)
            Time.fixedDeltaTime = 1f / NucleiConfig.PhysicsUpdatesPerSecond!.Value;
    }

    /// <summary>
    ///     Starts the dedicated server.
    /// </summary>
    public static async UniTask StartServer()
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
        
        if (!SteamLobbyService.IsSteamAPIAvailable())
        {
            Nuclei.Logger?.LogError("Steam API is not available! Aborting server launch.");
            return;
        }
        
        await StartOrRestartLobby();
        
        TimeService.Initialize();
        
        TimeEvents.EveryMinute += CheckSendMotD;
        TimeEvents.EveryMinute += CheckMissionOverTime;
        TimeEvents.Every10Minutes += TimeStatus;
        MissionEvents.MissionEnded += OnMissionEnded;

        if (NucleiConfig.RefreshServerNamePeriodically!.Value)
        {
            Nuclei.Logger?.LogDebug("Hooked up periodic server name refresh event.");
            TimeEvents.Every10Minutes += SteamLobbyService.SetLobbyData;
        }
        
        Nuclei.Logger?.LogInfo($"Server started. (Took {Time.time} seconds)");
    }
}
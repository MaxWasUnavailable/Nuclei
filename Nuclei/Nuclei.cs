using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using Steamworks;
using UnityEngine;

namespace Nuclei;

/// <summary>
///     Main plugin class for Nuclei.
/// </summary>
[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class Nuclei : BaseUnityPlugin
{
    internal static Nuclei? Instance { get; private set; }
    internal new static ManualLogSource? Logger { get; private set; }
    private static Harmony? Harmony { get; set; }
    private static bool IsPatched { get; set; }
    
    internal const string GeneralSection = "General";
    internal const string TechnicalSection = "Technical";

    internal ConfigEntry<ushort>? MaxPlayers;
    internal const ushort DefaultMaxPlayers = 16;
    
    internal ConfigEntry<string>? ServerName;
    internal const string DefaultServerName = "Dedicated Nuclei Server";
    
    internal ConfigEntry<string>? ServerMessageOfTheDay;
    internal const string DefaultServerMessageOfTheDay = "Welcome to the server, [USERNAME]!";
    
    internal ConfigEntry<string>? Missions;
    internal const string DefaultMissions = "Escalation;Domination;Confrontation;Breakout;Carrier Duel;Altercation";
    
    internal ConfigEntry<uint>? MissionDuration;
    internal const uint DefaultMissionDuration = 60;
    
    internal ConfigEntry<bool>? AllowRepeatMission;
    internal const bool DefaultAllowRepeatMission = true;
    
    internal ConfigEntry<ushort>? UdpPort;
    internal const ushort DefaultUdpPort = 7777;
    
    internal ConfigEntry<bool>? UseSteamSocket;
    internal const bool DefaultUseSteamSocket = true;
    
    internal ConfigEntry<ELobbyType>? LobbyType;
    internal const ELobbyType DefaultLobbyType = ELobbyType.k_ELobbyTypePublic;
    
    internal List<string> MissionsList => Missions!.Value.Split(';').ToList();
    
    private void InitSettings()
    {
        Logger?.LogDebug("Loading settings...");
        
        MaxPlayers = Config.Bind(GeneralSection, "MaxPlayers", DefaultMaxPlayers, "The maximum number of players allowed in the server.");
        Logger?.LogDebug($"MaxPlayers: {MaxPlayers.Value}");
        
        ServerName = Config.Bind(GeneralSection, "ServerName", DefaultServerName, "The name of the server.");
        Logger?.LogDebug($"ServerName: {ServerName.Value}");
        
        ServerMessageOfTheDay = Config.Bind(GeneralSection, "ServerMessageOfTheDay", DefaultServerMessageOfTheDay, "The message of the day for the server. This is displayed when players join the server.");
        Logger?.LogDebug($"ServerMessageOfTheDay: {ServerMessageOfTheDay.Value}");
        
        Missions = Config.Bind(GeneralSection, "Missions", DefaultMissions, "The list of missions the server will cycle through. Separate missions with a semicolon.");
        Logger?.LogDebug($"Missions: {Missions.Value}");
        
        MissionDuration = Config.Bind(GeneralSection, "MissionDuration", DefaultMissionDuration, "The duration of each mission in minutes. The server will automatically switch to the next mission after this duration. Set to 0 to disable automatic mission switching.");
        Logger?.LogDebug($"MissionDuration: {MissionDuration.Value}");
        
        AllowRepeatMission = Config.Bind(GeneralSection, "AllowRepeatMission", DefaultAllowRepeatMission, "Whether to allow the same mission to be selected more than once in a row. Does not work if there is only one mission in the list.");
        Logger?.LogDebug($"AllowRepeatMission: {AllowRepeatMission.Value}");
        
        UdpPort = Config.Bind(TechnicalSection, "UdpPort", DefaultUdpPort, "The UDP port the server will listen on. Only change this if you know what you're doing.");
        Logger?.LogDebug($"UdpPort: {UdpPort.Value}");
        
        UseSteamSocket = Config.Bind(TechnicalSection, "UseSteamSocket", DefaultUseSteamSocket, "Whether to use the Steam socket type. Only change this if you know what you're doing.");
        Logger?.LogDebug($"UseSteamSocket: {UseSteamSocket.Value}");
        
        LobbyType = Config.Bind(TechnicalSection, "LobbyType", DefaultLobbyType, "The type of lobby to create when starting a Steam lobby.");
        Logger?.LogDebug($"LobbyType: {LobbyType.Value}");
        
        Logger?.LogDebug("Loaded settings!");
    }

    private void ValidateSettings()
    {
        Logger?.LogDebug("Validating settings...");
        
        if (MaxPlayers!.Value < 1)
        {
            Logger?.LogWarning("MaxPlayers must be at least 1! Resetting to default value.");
            MaxPlayers.Value = DefaultMaxPlayers;
        }
        
        if (string.IsNullOrWhiteSpace(ServerName!.Value))
        {
            Logger?.LogWarning("ServerName cannot be empty! Resetting to default value.");
            ServerName.Value = DefaultServerName;
        }
        
        if (string.IsNullOrWhiteSpace(Missions!.Value))
        {
            Logger?.LogWarning("Missions cannot be empty! Resetting to default value.");
            Missions.Value = DefaultMissions;
        }
        
        if (MissionsList.Count == 0)
        {
            Logger?.LogWarning("Missions cannot be empty! Resetting to default value.");
            Missions.Value = DefaultMissions;
        }
        
        if (MissionsList.Count == 1 && !AllowRepeatMission!.Value)
        {
            Logger?.LogWarning("AllowRepeatMission is disabled, but there is only one mission in the list! Enabling AllowRepeatMission.");
            AllowRepeatMission.Value = true;
        }
        
        if (UdpPort!.Value > 65535 && !UseSteamSocket!.Value)
        {
            Logger?.LogWarning("UdpPort must be between 0 and 65535! Resetting to default value.");
            UdpPort.Value = DefaultUdpPort;
        }
        
        Logger?.LogDebug("Settings validated!");
    }
    
    private static bool IsServerDedicated()
    {
        return Application.isBatchMode;
    }

    private void Awake()
    {
        Instance = this;
        
        Logger = base.Logger;
        
        Logger?.LogInfo($"Loading {PluginInfo.PLUGIN_NAME} v{PluginInfo.PLUGIN_VERSION}...");
        
        if (!IsServerDedicated())
        {
            Logger?.LogError("This plugin is intended for dedicated servers only! Aborting server initialisation. To run a dedicated server, use the -batchmode and -nographics command line arguments.");
            return;
        }

        try
        {
            InitSettings();
            ValidateSettings();
        }
        catch (ArgumentException e)
        {
            Logger?.LogError(
                $"Aborting server launch: Failed to load or validate settings. One of the settings might be the wrong type of value. For more information, see this error trace:\n{e}");
            return;
        }
        catch (Exception e)
        {
            Logger?.LogError($"Aborting server launch: Failed to load or validate settings. For more information, see this error trace:\n{e}");
            return;
        }

        PatchAll();

        if (IsPatched)
            Logger?.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        else
            Logger?.LogError($"Plugin {PluginInfo.PLUGIN_GUID} failed to load correctly!");
    }

    private static void PatchAll()
    {
        if (IsPatched)
        {
            Logger?.LogWarning("Already patched!");
            return;
        }

        Logger?.LogDebug("Patching...");

        Harmony ??= new Harmony(PluginInfo.PLUGIN_GUID);

        try
        {
            Harmony.PatchAll();
            IsPatched = true;
            Logger?.LogDebug("Patched!");
        }
        catch (Exception e)
        {
            Logger?.LogError($"Aborting server launch: Failed to Harmony patch the game. For more information, see this error trace:\n{e}");
        }
    }

    private void UnpatchSelf()
    {
        if (Harmony == null)
        {
            Logger?.LogError("Harmony instance is null!");
            return;
        }
        
        if (!IsPatched)
        {
            Logger?.LogWarning("Already unpatched!");
            return;
        }

        Logger?.LogDebug("Unpatching...");

        Harmony?.UnpatchSelf();
        IsPatched = false;

        Logger?.LogDebug("Unpatched!");
    }
}
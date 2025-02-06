using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

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

    private ConfigEntry<int> _maxPlayers;
    private ConfigEntry<string> _serverName;
    private ConfigEntry<string> _serverMessageOfTheDay;
    private ConfigEntry<List<string>> _missions;
    private ConfigEntry<int> _missionDuration;
    
    private void InitSettings()
    {
        Logger.LogDebug("Loading settings...");
        
        _maxPlayers = Config.Bind("Settings", "MaxPlayers", 16, "The maximum number of players allowed in the server.");
        Logger.LogDebug($"MaxPlayers: {_maxPlayers.Value}");
        
        _serverName = Config.Bind("Settings", "ServerName", "Dedicated Nuclei Server", "The name of the server.");
        Logger.LogDebug($"ServerName: {_serverName.Value}");
        
        _serverMessageOfTheDay = Config.Bind("Settings", "ServerMessageOfTheDay", "Welcome to the server, [USERNAME]!", "The message of the day for the server. This is displayed when players join the server.");
        Logger.LogDebug($"ServerMessageOfTheDay: {_serverMessageOfTheDay.Value}");
        
        _missions = Config.Bind("Settings", "Missions", new List<string> { "Escalation", "Domination", "Confrontation", "Breakout", "Carrier Duel", "Altercation" }, "The list of missions the server will cycle through.");
        Logger.LogDebug($"Missions: {string.Join("; ", _missions.Value)}");
        
        _missionDuration = Config.Bind("Settings", "MissionDuration", 60, "The duration of each mission in minutes. The server will automatically switch to the next mission after this duration. Set to 0 to disable automatic mission switching.");
        Logger.LogDebug($"MissionDuration: {_missionDuration.Value}");
        
        Logger.LogDebug("Loaded settings!");
    }

    private void ValidateSettings()
    {
        Logger.LogDebug("Validating settings...");
        
        if (_maxPlayers.Value < 1)
        {
            Logger.LogWarning("MaxPlayers must be at least 1! Resetting to default value.");
            _maxPlayers.Value = 16;
        }
        
        if (string.IsNullOrWhiteSpace(_serverName.Value))
        {
            Logger.LogWarning("ServerName cannot be empty! Resetting to default value.");
            _serverName.Value = "Dedicated Nuclei Server";
        }
        
        if (_missions.Value.Count == 0)
        {
            Logger.LogWarning("Missions cannot be empty! Resetting to default value.");
            _missions.Value = ["Escalation", "Domination", "Confrontation", "Breakout", "Carrier Duel", "Altercation"];
        }
        
        if (_missionDuration.Value < 0)
        {
            Logger.LogWarning("MissionDuration may not be negative! Setting to 0.");
            _missionDuration.Value = 0;
        }
        
        Logger.LogDebug("Settings validated!");
    }

    private void Awake()
    {
        Instance = this;
        
        Logger = base.Logger;
        
        Logger.LogInfo($"Loading {PluginInfo.PLUGIN_NAME} v{PluginInfo.PLUGIN_VERSION}...");

        InitSettings();
        ValidateSettings();

        Harmony = new Harmony(PluginInfo.PLUGIN_GUID);

        PatchAll();

        if (IsPatched)
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        else
            Logger.LogError($"Plugin {PluginInfo.PLUGIN_GUID} failed to load correctly!");
    }

    private void PatchAll()
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
            Logger?.LogError($"Failed to patch: {e}");
        }
    }

    private void UnpatchSelf()
    {
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
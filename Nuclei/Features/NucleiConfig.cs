using System.Collections.Generic;
using System.Linq;
using BepInEx.Configuration;
using Steamworks;

namespace Nuclei.Features;

/// <summary>
///     Configuration class for Nuclei.
/// </summary>
public static class NucleiConfig
{
    internal const string GeneralSection = "General";
    internal const string TechnicalSection = "Technical";

    internal static ConfigEntry<ushort>? MaxPlayers;
    internal const ushort DefaultMaxPlayers = 16;
    
    internal static ConfigEntry<string>? ServerName;
    internal const string DefaultServerName = "Dedicated Nuclei Server";
    
    internal static ConfigEntry<string>? ServerMessageOfTheDay;
    internal const string DefaultServerMessageOfTheDay = "Default message of the day.";
    
    internal static ConfigEntry<uint>? MotDFrequency;
    internal const uint DefaultMotDFrequency = 30;
    
    internal static ConfigEntry<string>? WelcomeMessage;
    internal const string DefaultWelcomeMessage = "Welcome to the server, {username}!";
    
    internal static ConfigEntry<string>? Missions;
    internal const string DefaultMissions = "Escalation;Domination;Confrontation;Breakout;Carrier Duel;Altercation";
    
    internal static ConfigEntry<uint>? MissionDuration;
    internal const uint DefaultMissionDuration = 60;
    
    internal static ConfigEntry<bool>? AllowRepeatMission;
    internal const bool DefaultAllowRepeatMission = true;
    
    internal static ConfigEntry<ushort>? UdpPort;
    internal const ushort DefaultUdpPort = 7777;
    
    internal static ConfigEntry<bool>? UseSteamSocket;
    internal const bool DefaultUseSteamSocket = true;
    
    internal static ConfigEntry<ELobbyType>? LobbyType;
    internal const ELobbyType DefaultLobbyType = ELobbyType.k_ELobbyTypePublic;

    internal static ConfigEntry<string>? Moderators;
    internal const string DefaultModerators = "";
    
    internal static ConfigEntry<string>? Admins;
    internal const string DefaultAdmins = "";
    
    internal static List<string> ModeratorsList => Moderators!.Value.Split(';').ToList();
    
    internal static List<string> AdminsList => Admins!.Value.Split(';').ToList();
    
    internal static List<string> MissionsList => Missions!.Value.Split(';').ToList();
    
    internal static void InitSettings(ConfigFile config)
    {
        Nuclei.Logger?.LogDebug("Loading settings...");
        
        MaxPlayers = config.Bind(GeneralSection, "MaxPlayers", DefaultMaxPlayers, "The maximum number of players allowed in the server.");
        Nuclei.Logger?.LogDebug($"MaxPlayers: {MaxPlayers.Value}");
        
        ServerName = config.Bind(GeneralSection, "ServerName", DefaultServerName, "The name of the server.");
        Nuclei.Logger?.LogDebug($"ServerName: {ServerName.Value}");
        
        ServerMessageOfTheDay = config.Bind(GeneralSection, "ServerMessageOfTheDay", DefaultServerMessageOfTheDay, "The message of the day for the server. This message is displayed periodically to all players.");
        Nuclei.Logger?.LogDebug($"ServerMessageOfTheDay: {ServerMessageOfTheDay.Value}");
        
        MotDFrequency = config.Bind(GeneralSection, "MotDFrequency", DefaultMotDFrequency, "The frequency in minutes at which the message of the day is displayed. Set to 0 to disable the message of the day.");
        Nuclei.Logger?.LogDebug($"MotDFrequency: {MotDFrequency.Value}");
        
        WelcomeMessage = config.Bind(GeneralSection, "WelcomeMessage", DefaultWelcomeMessage, "The message displayed to players when they join the server. Use {username} to insert the player's name.");
        Nuclei.Logger?.LogDebug($"WelcomeMessage: {WelcomeMessage.Value}");
        
        Missions = config.Bind(GeneralSection, "Missions", DefaultMissions, "The list of missions the server will cycle through. Separate missions with a semicolon.");
        Nuclei.Logger?.LogDebug($"Missions: {Missions.Value}");
        
        MissionDuration = config.Bind(GeneralSection, "MissionDuration", DefaultMissionDuration, "The duration of each mission in minutes. The server will automatically switch to the next mission after this duration. Set to 0 to disable automatic mission switching.");
        Nuclei.Logger?.LogDebug($"MissionDuration: {MissionDuration.Value}");
        
        AllowRepeatMission = config.Bind(GeneralSection, "AllowRepeatMission", DefaultAllowRepeatMission, "Whether to allow the same mission to be selected more than once in a row. Does not work if there is only one mission in the list.");
        Nuclei.Logger?.LogDebug($"AllowRepeatMission: {AllowRepeatMission.Value}");
        
        UdpPort = config.Bind(TechnicalSection, "UdpPort", DefaultUdpPort, "The UDP port the server will listen on. Only change this if you know what you're doing.");
        Nuclei.Logger?.LogDebug($"UdpPort: {UdpPort.Value}");
        
        UseSteamSocket = config.Bind(TechnicalSection, "UseSteamSocket", DefaultUseSteamSocket, "Whether to use the Steam socket type. Only change this if you know what you're doing.");
        Nuclei.Logger?.LogDebug($"UseSteamSocket: {UseSteamSocket.Value}");
        
        LobbyType = config.Bind(TechnicalSection, "LobbyType", DefaultLobbyType, "The type of lobby to create when starting a Steam lobby.");
        Nuclei.Logger?.LogDebug($"LobbyType: {LobbyType.Value}");
        
        Moderators = config.Bind(GeneralSection, "Moderators", DefaultModerators, "A list of moderators who have access to moderator commands. Separate steam IDs with a semicolon.");
        Nuclei.Logger?.LogDebug($"Moderators: {Moderators.Value}");
        
        Admins = config.Bind(GeneralSection, "Admins", DefaultAdmins, "A list of admins who have access to admin commands. Separate steam IDs with a semicolon.");
        Nuclei.Logger?.LogDebug($"Admins: {Admins.Value}");
        
        Nuclei.Logger?.LogDebug("Loaded settings!");
    }

    internal static void ValidateSettings()
    {
        Nuclei.Logger?.LogDebug("Validating settings...");
        
        if (MaxPlayers!.Value < 1)
        {
            Nuclei.Logger?.LogWarning("MaxPlayers must be at least 1! Resetting to default value.");
            MaxPlayers.Value = DefaultMaxPlayers;
        }
        
        if (string.IsNullOrWhiteSpace(ServerName!.Value))
        {
            Nuclei.Logger?.LogWarning("ServerName cannot be empty! Resetting to default value.");
            ServerName.Value = DefaultServerName;
        }
        
        if (string.IsNullOrWhiteSpace(Missions!.Value))
        {
            Nuclei.Logger?.LogWarning("Missions cannot be empty! Resetting to default value.");
            Missions.Value = DefaultMissions;
        }
        
        if (MissionsList.Count == 0)
        {
            Nuclei.Logger?.LogWarning("Missions cannot be empty! Resetting to default value.");
            Missions.Value = DefaultMissions;
        }
        
        if (MissionsList.Count == 1 && !AllowRepeatMission!.Value)
        {
            Nuclei.Logger?.LogWarning("AllowRepeatMission is disabled, but there is only one mission in the list! Enabling AllowRepeatMission.");
            AllowRepeatMission.Value = true;
        }
        
        if (UdpPort!.Value > 65535 && !UseSteamSocket!.Value)
        {
            Nuclei.Logger?.LogWarning("UdpPort must be between 0 and 65535! Resetting to default value.");
            UdpPort.Value = DefaultUdpPort;
        }
        
        if (MotDFrequency!.Value > MissionDuration!.Value && MotDFrequency!.Value != 0)
        {
            Nuclei.Logger?.LogWarning("MotDFrequency must be less than or equal to MissionDuration! Otherwise, the message of the day will never be displayed. Setting MotDFrequency to 0 (disabled).");
            MotDFrequency.Value = 0;
        }
        
        Nuclei.Logger?.LogDebug("Settings validated!");
    }
}
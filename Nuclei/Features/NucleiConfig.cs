using System.Collections.Generic;
using System.Linq;
using BepInEx.Configuration;
using Nuclei.Enums;
using Nuclei.Helpers;
using Steamworks;

namespace Nuclei.Features;

/// <summary>
///     Configuration class for Nuclei.
/// </summary>
public static class NucleiConfig
{
    internal const string GeneralSection = "General";
    internal const string TechnicalSection = "Technical";
    internal const string ExperimentalSection = "Experimental";

    internal static ConfigEntry<ushort>? MaxPlayers;
    internal const ushort DefaultMaxPlayers = 16;
    
    internal static ConfigEntry<string>? ServerName;
    internal const string DefaultServerName = "Dedicated Nuclei Server";
    
    internal static ConfigEntry<string>? MessageOfTheDay;
    internal const string DefaultMessageOfTheDay = "This server is running on Nuclei! Have fun!";
    
    internal static ConfigEntry<uint>? MotDFrequency;
    internal const uint DefaultMotDFrequency = 900;
    
    internal static ConfigEntry<string>? WelcomeMessage;
    internal const string DefaultWelcomeMessage = $"Welcome to the server, {DynamicPlaceholderUtils.PlayerNameCensored}!";
    
    internal static ConfigEntry<string>? Missions;
    internal const string DefaultMissions = "Escalation;Domination;Confrontation;Breakout;Carrier Duel;Altercation";
    
    internal static ConfigEntry<uint>? MissionDuration;
    internal const uint DefaultMissionDuration = 3600;
    
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
    
    internal static ConfigEntry<string>? Owner;
    internal const string DefaultOwner = "";
    
    internal static ConfigEntry<string>? BannedPlayers;
    internal const string DefaultBannedPlayers = "";
    
    internal static ConfigEntry<bool>? RefreshServerNamePeriodically;
    internal const bool DefaultRefreshServerNamePeriodically = true;
    
    internal static ConfigEntry<short>? TargetFrameRate;
    internal const short DefaultTargetFrameRate = 120;
    
    internal static ConfigEntry<bool>? MuteAfterStart;
    internal const bool DefaultMuteAfterStart = true;
    
    internal static ConfigEntry<ushort>? PhysicsUpdatesPerSecond;
    internal const ushort DefaultPhysicsUpdatesPerSecond = 60;

    internal static ConfigEntry<bool>? UseUpdateForPhysicsUpdate;
    internal const bool DefaultUseUpdateForPhysicsUpdate = false;

    internal static ConfigEntry<MissionSelectMode>? MissionSelectMode;
    internal const MissionSelectMode DefaultMissionSelectMode = Enums.MissionSelectMode.Random;

    internal static ConfigEntry<bool>? UseAllMissions;
    internal const bool DefaultUseAllMissions = false;
    
    internal static List<string> ModeratorsList => Moderators!.Value.Split(';').Where(m => !string.IsNullOrWhiteSpace(m)).ToList();
    
    internal static List<string> AdminsList => Admins!.Value.Split(';').Where(a => !string.IsNullOrWhiteSpace(a)).ToList();
    
    internal static List<string> BannedPlayersList => BannedPlayers!.Value.Split(';').Where(b => !string.IsNullOrWhiteSpace(b)).ToList();
    
    internal static List<string> MissionsList => Missions!.Value.Split(';').Where(m => !string.IsNullOrWhiteSpace(m)).ToList();
    
    internal static void InitSettings(ConfigFile config)
    {
        Nuclei.Logger?.LogDebug("Loading settings...");
        
        MaxPlayers = config.Bind(GeneralSection, "MaxPlayers", DefaultMaxPlayers, "The maximum number of players allowed in the server.");
        Nuclei.Logger?.LogDebug($"MaxPlayers: {MaxPlayers.Value}");
        
        ServerName = config.Bind(GeneralSection, "ServerName", DefaultServerName, "The name of the server.");
        Nuclei.Logger?.LogDebug($"ServerName: {ServerName.Value}");
        
        MessageOfTheDay = config.Bind(GeneralSection, "MessageOfTheDay", DefaultMessageOfTheDay, "The message of the day for the server. This message is displayed periodically to all players.");
        Nuclei.Logger?.LogDebug($"MessageOfTheDay: {MessageOfTheDay.Value}");
        
        MotDFrequency = config.Bind(GeneralSection, "MotDFrequency", DefaultMotDFrequency, "The frequency in seconds at which the message of the day is displayed. Set to 0 to disable the message of the day. Checks are done every minute.");
        Nuclei.Logger?.LogDebug($"MotDFrequency: {MotDFrequency.Value}");
        
        WelcomeMessage = config.Bind(GeneralSection, "WelcomeMessage", DefaultWelcomeMessage, "The message displayed to players when they join the server. See the readme for placeholders.");
        Nuclei.Logger?.LogDebug($"WelcomeMessage: {WelcomeMessage.Value}");
        
        Missions = config.Bind(GeneralSection, "Missions", DefaultMissions, "The list of missions the server will cycle through. Separate missions with a semicolon.");
        Nuclei.Logger?.LogDebug($"Missions: {Missions.Value}");
        
        MissionDuration = config.Bind(GeneralSection, "MissionDuration", DefaultMissionDuration, "The duration of each mission in seconds. The server will automatically switch to the next mission after this duration. Set to 0 to disable automatic mission switching. Checks are done every minute.");
        Nuclei.Logger?.LogDebug($"MissionDuration: {MissionDuration.Value}");
        
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
        
        Owner = config.Bind(GeneralSection, "Owner", DefaultOwner, "The Steam ID of the server owner. This player has access to all commands, and cannot be removed from the admin list.");
        Nuclei.Logger?.LogDebug($"Owner: {Owner.Value}");
        
        BannedPlayers = config.Bind(GeneralSection, "BannedPlayers", DefaultBannedPlayers, "A list of banned players. Separate steam IDs with a semicolon.");
        Nuclei.Logger?.LogDebug($"BannedPlayers: {BannedPlayers.Value}");
        
        RefreshServerNamePeriodically = config.Bind(GeneralSection, "RefreshServerNamePeriodically", DefaultRefreshServerNamePeriodically, "Whether to refresh the server name every 10 minutes. This is useful for servers that use dynamic placeholders in the server name.");
        Nuclei.Logger?.LogDebug($"RefreshServerNamePeriodically: {RefreshServerNamePeriodically.Value}");
        
        TargetFrameRate = config.Bind(TechnicalSection, "TargetFrameRate", DefaultTargetFrameRate, "The target frame rate of the server. Only change this if you know what you're doing. -1 for unlimited.");
        Nuclei.Logger?.LogDebug($"TargetFrameRate: {TargetFrameRate.Value}");
        
        MuteAfterStart = config.Bind(TechnicalSection, "MuteAfterStart", DefaultMuteAfterStart, "Whether to mute the server process after starting.");
        Nuclei.Logger?.LogDebug($"MuteAfterStart: {MuteAfterStart.Value}");
        
        PhysicsUpdatesPerSecond = config.Bind(ExperimentalSection, "PhysicsUpdatesPerSecond", DefaultPhysicsUpdatesPerSecond, "The number of physics updates per second. Only change this if you know what you're doing - this can break the game.");
        Nuclei.Logger?.LogDebug($"PhysicsUpdatesPerSecond: {PhysicsUpdatesPerSecond.Value}");
        
        UseUpdateForPhysicsUpdate = config.Bind(ExperimentalSection, "UseUpdateForPhysicsUpdate", DefaultUseUpdateForPhysicsUpdate, "Whether to use the Update method for physics updates. Only change this if you know what you're doing - this can negatively impact performance.");
        Nuclei.Logger?.LogDebug($"UseUpdateForPhysicsUpdate: {UseUpdateForPhysicsUpdate.Value}");
        
        MissionSelectMode = config.Bind(GeneralSection, "MissionSelectMode", DefaultMissionSelectMode, "The mode used to select the next mission. Random will select a random mission from the list, RandomNoRepeat will select a random mission without repeating the last one (if possible), and Sequential will select the next mission in the list.");
        Nuclei.Logger?.LogDebug($"MissionSelectMode: {MissionSelectMode.Value}");
        
        UseAllMissions = config.Bind(GeneralSection, "UseAllMissions", DefaultUseAllMissions, "Whether to use all missions available to the client (including tutorials, workshop items, custom missions, etc...) for mission selection. If false, only the missions in the config will be used.");
        Nuclei.Logger?.LogDebug($"UseAllMissions: {UseAllMissions.Value}");
        
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
        
        if (UdpPort!.Value > 65535)
        {
            Nuclei.Logger?.LogWarning("UdpPort must be between 0 and 65535! Resetting to default value.");
            UdpPort.Value = DefaultUdpPort;
        }
        
        if (TargetFrameRate!.Value < -1)
        {
            Nuclei.Logger?.LogWarning("TargetFrameRate cannot be less than -1! Setting to -1 (unlimited).");
            TargetFrameRate.Value = -1;
        }
        
        ValidateForUserErrors();
        
        Nuclei.Logger?.LogDebug("Settings validated!");
    }

    internal static void ValidateForUserErrors()
    {
        if (Owner!.Value == "")
        {
            Nuclei.Logger?.LogWarning("Owner is not set! It is recommended to set the owner in the config file.");
        }
        else
        {
            if (!ulong.TryParse(Owner.Value, out _))
                Nuclei.Logger?.LogWarning("Owner is not a valid Steam ID! Remove or correct it in the config file.");
            if (Owner.Value.Contains(";"))
                Nuclei.Logger?.LogWarning("Owner cannot be a list of Steam IDs! Remove or correct it in the config file. (Found a semicolon ';')");
        }
        
        if (ModeratorsList.Any(m => !ulong.TryParse(m, out _))) 
            Nuclei.Logger?.LogWarning("One or more moderators are not valid Steam IDs! Remove or correct them in the config file.");

        if (Moderators!.Value.EndsWith(";"))
        {
            Nuclei.Logger?.LogWarning("Moderators list ends with a semicolon ';'. Removing it.");
            Moderators.Value = Moderators.Value.TrimEnd(';');
        }
        
        if (Moderators!.Value.Contains(";;"))
        {
            Nuclei.Logger?.LogWarning("Moderators list contains multiple following semicolons ';;'. Fixing it.");
            Moderators.Value = Moderators.Value.Replace(";;", ";");
        }

        if (AdminsList.Any(a => !ulong.TryParse(a, out _)))
            Nuclei.Logger?.LogWarning("One or more admins are not valid Steam IDs! Remove or correct them in the config file.");

        if (Admins!.Value.EndsWith(";"))
        {
            Nuclei.Logger?.LogWarning("Admins list ends with a semicolon ';'. Removing it.");
            Admins.Value = Admins.Value.TrimEnd(';');
        }
        
        if (Admins!.Value.Contains(";;"))
        {
            Nuclei.Logger?.LogWarning("Admins list contains multiple following semicolons ';;'. Fixing it.");
            Admins.Value = Admins.Value.Replace(";;", ";");
        }
        
        if (BannedPlayers!.Value.EndsWith(";"))
        {
            Nuclei.Logger?.LogWarning("BannedPlayers list ends with a semicolon ';'. Removing it.");
            BannedPlayers.Value = BannedPlayers.Value.TrimEnd(';');
        }
        
        if (BannedPlayers!.Value.Contains(";;"))
        {
            Nuclei.Logger?.LogWarning("BannedPlayers list contains multiple following semicolons ';;'. Fixing it.");
            BannedPlayers.Value = BannedPlayers.Value.Replace(";;", ";");
        }
    }
    
    internal static void RemoveModerator(string steamId)
    {
        var moderatorsList = ModeratorsList;
        moderatorsList.Remove(steamId);
        Moderators!.Value = string.Join(";", moderatorsList);
    }
    
    internal static void AddModerator(string steamId)
    {
        var moderatorsList = ModeratorsList;
        if (moderatorsList.Contains(steamId))
            return;
        moderatorsList.Add(steamId);
        Moderators!.Value = string.Join(";", moderatorsList);
    }
    
    internal static void RemoveAdmin(string steamId)
    {
        var adminsList = AdminsList;
        adminsList.Remove(steamId);
        Admins!.Value = string.Join(";", adminsList);
    }
    
    internal static void AddAdmin(string steamId)
    {
        var adminsList = AdminsList;
        if (adminsList.Contains(steamId))
            return;
        adminsList.Add(steamId);
        Admins!.Value = string.Join(";", adminsList);
    }
    
    internal static void RemoveBannedPlayer(string steamId)
    {
        var bannedPlayersList = BannedPlayersList;
        bannedPlayersList.Remove(steamId);
        BannedPlayers!.Value = string.Join(";", bannedPlayersList);
    }
    
    internal static void AddBannedPlayer(string steamId)
    {
        var bannedPlayersList = BannedPlayersList;
        if (bannedPlayersList.Contains(steamId))
            return;
        bannedPlayersList.Add(steamId);
        BannedPlayers!.Value = string.Join(";", bannedPlayersList);
    }
    
    /// <summary>
    ///     Check if the given Steam ID is a moderator.
    /// </summary>
    /// <param name="steamId"> The Steam ID to check. </param>
    /// <returns> Whether the Steam ID is a moderator. </returns>
    public static bool IsModerator(ulong steamId)
    {
        return ModeratorsList.Contains(steamId.ToString());
    }
    
    /// <summary>
    ///     Check if the given Steam ID is an admin.
    /// </summary>
    /// <param name="steamId"> The Steam ID to check. </param>
    /// <returns> Whether the Steam ID is an admin. </returns>
    public static bool IsAdmin(ulong steamId)
    {
        return AdminsList.Contains(steamId.ToString());
    }
    
    /// <summary>
    ///     Check if the given Steam ID is the owner.
    /// </summary>
    /// <param name="steamId"> The Steam ID to check. </param>
    /// <returns> Whether the Steam ID is the owner. </returns>
    public static bool IsOwner(ulong steamId)
    {
        return Owner!.Value == steamId.ToString();
    }
    
    /// <summary>
    ///     Check if the given Steam ID is banned.
    /// </summary>
    /// <param name="steamId"> The Steam ID to check. </param>
    /// <returns> Whether the Steam ID is banned. </returns>
    public static bool IsBanned(ulong steamId)
    {
        return BannedPlayersList.Contains(steamId.ToString());
    }
}
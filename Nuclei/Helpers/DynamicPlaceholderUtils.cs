using NuclearOption.Networking;

namespace Nuclei.Helpers;

/// <summary>
///     Utility class for dynamic placeholders.
/// </summary>
public static class DynamicPlaceholderUtils
{
    /// <summary>
    ///     Placeholder for a player's username.
    /// </summary>
    public const string PlayerName = "{player_name}";

    /// <summary>
    ///     Placeholder for a player's username, censored.
    /// </summary>
    public const string PlayerNameCensored = "{player_name_censored}";

    /// <summary>
    ///     Placeholder for a player's Steam ID.
    /// </summary>
    public const string SteamID = "{steamid}";

    /// <summary>
    ///     Placeholder for the mission name.
    /// </summary>
    public const string MissionName = "{mission_name}";

    /// <summary>
    ///     Placeholder for the first faction's name.
    /// </summary>
    public const string Faction1Name = "{faction1_name}";

    /// <summary>
    ///     Placeholder for the second faction's name.
    /// </summary>
    public const string Faction2Name = "{faction2_name}";

    /// <summary>
    ///     Placeholder for the first faction's tag.
    /// </summary>
    public const string Faction1Tag = "{faction1_tag}";

    /// <summary>
    ///     Placeholder for the second faction's tag.
    /// </summary>
    public const string Faction2Tag = "{faction2_tag}";

    /// <summary>
    ///     Placeholder for the first faction's score.
    /// </summary>
    public const string Faction1Score = "{faction1_score}";

    /// <summary>
    ///     Placeholder for the second faction's score.
    /// </summary>
    public const string Faction2Score = "{faction2_score}";

    /// <summary>
    ///     Placeholder for all configured missions.
    /// </summary>
    public const string AllMissions = "{all_missions}";

    /// <summary>
    ///     Placeholder for showing up to 3 random missions, and (...) if there are more.
    /// </summary>
    public const string Random3MissionsEtc = "{random_3_missions_etc}";

    /// <summary>
    ///     Placeholder for the server name.
    /// </summary>
    public const string ServerName = "{server_name}";

    /// <summary>
    ///     Replaces dynamic placeholders in a string with the appropriate values.
    /// </summary>
    /// <param name="original"> The original string. </param>
    /// <param name="player"> The player to get the values from. Ignored if null. </param>
    /// <returns> The string with the placeholders replaced. </returns>
    public static string ReplaceDynamicPlaceholders(string original, Player? player = null)
    {
        if (player)
        {
            original = original.Replace(PlayerName, player!.PlayerName);
            original = original.Replace(PlayerNameCensored, player.GetNameOrCensored());
            original = original.Replace(SteamID, player.SteamID.ToString());
        }
        /*
        original = original.Replace(MissionName, MissionService.CurrentMission!.Name);
        if (MissionService.CurrentMission!.factions.Count > 0)
        {
            original = original.Replace(Faction1Name, MissionService.CurrentMission!.factions[0].FactionHQ.faction.factionName);
            original = original.Replace(Faction1Score, Mathf.RoundToInt(MissionService.CurrentMission!.factions[0].FactionHQ.factionScore).ToString());
            original = original.Replace(Faction1Tag, MissionService.CurrentMission!.factions[0].FactionHQ.faction.factionTag);
        }
        if (MissionService.CurrentMission!.factions.Count > 1)
        {
            original = original.Replace(Faction2Name, MissionService.CurrentMission!.factions[1].FactionHQ.faction.factionName);
            original = original.Replace(Faction2Score, Mathf.RoundToInt(MissionService.CurrentMission!.factions[1].FactionHQ.factionScore).ToString());
            original = original.Replace(Faction2Tag, MissionService.CurrentMission!.factions[1].FactionHQ.faction.factionTag);
        }

        var allMissions = NucleiConfig.MissionsList.Aggregate("", (current, mission) => current + ", " + mission).TrimStart(',', ' ');
        original = original.Replace(AllMissions, allMissions);
        
        var randomMissions = NucleiConfig.MissionsList.OrderBy(_ => Random.Range(0, 100)).Take(3).ToList();
        var randomMissionsString = randomMissions.Aggregate("", (current, mission) => current + ", " + mission).TrimStart(',', ' ');
        if (NucleiConfig.MissionsList.Count > 3) 
            randomMissionsString += ", (...)";
        original = original.Replace(Random3MissionsEtc, randomMissionsString);
        
        original = original.Replace(ServerName, NucleiConfig.ServerName!.Value);
        */
        return original;
    }
}
using Nuclei.Features;
using UnityEngine;

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
    ///     Placeholder for a player's faction name.
    /// </summary>
    public const string PlayerFactionName = "{player_faction_name}";

    /// <summary>
    ///     Placeholder for a player's faction tag.
    /// </summary>
    public const string PlayerFactionTag = "{player_faction_tag}";

    /// <summary>
    ///     Placeholder for the mission name.
    /// </summary>
    public const string MissionName = "{mission_name}";

    /// <summary>
    ///     Placeholder for the first faction's name.
    /// </summary>
    public const string Faction1 = "{faction1_name}";

    /// <summary>
    ///     Placeholder for the second faction's name.
    /// </summary>
    public const string Faction2 = "{faction2_name}";

    /// <summary>
    ///     Placeholder for the first faction's score.
    /// </summary>
    public const string Faction1Score = "{faction1_score}";

    /// <summary>
    ///     Placeholder for the second faction's score.
    /// </summary>
    public const string Faction2Score = "{faction2_score}";

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
            original = original.Replace(PlayerNameCensored, player.playerName_Censored);
            original = original.Replace(SteamID, player.SteamID.ToString());
            original = original.Replace(PlayerFactionName, player.HQ.faction.factionName);
            original = original.Replace(PlayerFactionTag, player.HQ.faction.factionTag);
        }
        
        original = original.Replace(MissionName, MissionService.CurrentMission!.Name);
        if (MissionService.CurrentMission!.factions.Count > 0)
        {
            original = original.Replace(Faction1, MissionService.CurrentMission!.factions[0].factionName);
            original = original.Replace(Faction1Score, Mathf.RoundToInt(MissionService.CurrentMission!.factions[0].FactionHQ.factionScore).ToString());
        }
        if (MissionService.CurrentMission!.factions.Count > 1)
        {
            original = original.Replace(Faction2, MissionService.CurrentMission!.factions[1].factionName);
            original = original.Replace(Faction2Score, Mathf.RoundToInt(MissionService.CurrentMission!.factions[1].FactionHQ.factionScore).ToString());
        }
        
        return original;
    }
}
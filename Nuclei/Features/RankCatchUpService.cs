using NuclearOption.Networking;
using Nuclei.Helpers;
using UnityEngine;
namespace Nuclei.Features;

public class RankCatchUpService
{
    public static void CatchUpPlayer(Player player)
    {
        if (player.GetAuthData().SaveData.Faction != null)
        {
            return; // Means that they already joined the server. No double-dipping!
        }
        var currentMissionTime = Time.timeSinceLevelLoad;
        var maxMissionTime = Globals.DedicatedServerManagerInstance.CurrentMissionOption.MaxTime;
        var percentComplete = (currentMissionTime / maxMissionTime) * 2;

        var rank = 0;
        var allocation = 0f;

        if (percentComplete < .20) return;
        else if (percentComplete >= .80)
        {
            rank = 5;
            allocation = 400f;
        }
        else if (percentComplete >= .60) 
        {
            rank = 4;
            allocation = 350f;
        }
        else if (percentComplete >= .40) 
        {
            rank = 3;
            allocation = 300f;
        }
        else if (percentComplete >= .40) 
        {
            rank = 2;
            allocation = 250f;
        }
        else if (percentComplete >= .20) 
        {
            rank = 1;
            allocation = 200f;
        }
        player.SetRank(rank, false);
        player.SetAllocation(player.Allocation + allocation);
        ChatService.SendPrivateChatMessage($"Late join - You have been promoted to Rank {rank} with +${allocation}m", player);
    }
}
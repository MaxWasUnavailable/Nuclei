using NuclearOption.Networking;
using Nuclei.Helpers;
using UnityEngine;
namespace Nuclei.Features;

public class RankCatchUpService
{
    public static void CatchUpPlayer(Player player)
    {
        var currentMissionTime = Time.timeSinceLevelLoad;
        var maxMissionTime = Globals.DedicatedServerManagerInstance.CurrentMissionOption.MaxTime;
        var percentComplete = currentMissionTime / maxMissionTime;

        if (percentComplete < .20) return;
        else if (percentComplete >= .80) 
        {
            player.SetRank(5, false);
            player.SetAllocation(player.Allocation + 300f);
        }
        else if (percentComplete >= .60) 
        {
            player.SetRank(4, false);
            player.SetAllocation(player.Allocation + 250f);
        }
        else if (percentComplete >= .40) 
        {
            player.SetRank(3, false);
            player.SetAllocation(player.Allocation + 200f);
        }
        else if (percentComplete >= .40) 
        {
            player.SetRank(2, false);
            player.SetAllocation(player.Allocation + 150f);
        }
        else if (percentComplete >= .20) 
        {
            player.SetRank(1, false);
            player.SetAllocation(player.Allocation + 100);
        }
    }
}
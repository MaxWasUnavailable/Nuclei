using System;
using System.Collections.Generic;
using System.Linq;
using NuclearOption.Networking;

namespace Nuclei.Features;

// Used with a timer service, it will check and balance team player count
public static class AutoBalanceService
{
    public static bool BalanceTeams()
    {
        var HQs = FactionRegistry.GetAllHQs().ToList();

        // new List<> required due to bug in GetPlayers() code if called multiple times
        var faction1Players = new List<Player>(HQs[0].GetPlayers(false));
        var faction2Players = new List<Player>(HQs[1].GetPlayers(false));

        double faction1PlayersCount = faction1Players.Count;
        double faction2PlayersCount = faction2Players.Count;

        double totalPlayers = faction1PlayersCount + faction2PlayersCount;

        if (HQs[0].preventJoin || HQs[1].preventJoin) return false; // If co-op, don't balance

        //get % of players of total for each faction
        double faction1Ratio = faction1PlayersCount / totalPlayers;
        double faction2Ratio = faction2PlayersCount / totalPlayers;

        // if the difference btwn the %'s is greater than the ratioMinimum, commence auto-balance
        // TODO: Make this configurable via Nuclei.Config
        double ratioMinimum = .10;
        double factionRatioDifference = Math.Abs(faction1Ratio - faction2Ratio);

        if (factionRatioDifference > ratioMinimum)
        {
            // dont balance if ratio reaches threshold but team is only diff by 1 players
            if (faction1PlayersCount - faction2PlayersCount == 1) return false; 


            // Formula meaning: Get the % of players, where % is the ratio difference. This number
            // is max players needed to move, except you need to div by 2 because each player moved
            // moves the ratio diff 2x.
            var numPlayersToMove = (int)(factionRatioDifference * totalPlayers / 2); 

            // Autobalance players from faction 1 to faction 2
            if (faction1PlayersCount > faction2PlayersCount)
            {
                MovePlayers(numPlayersToMove, faction1Players, HQs[1]);
                return true;
            }
            // Autobalance players from faction 2 to faction 1
            else
            {

                MovePlayers(numPlayersToMove, faction2Players, HQs[0]);
                return true;
            }
        }
        else return false;
    }

    private static void MovePlayers(int numPlayersToMove, List<Player> factionPlayers, FactionHQ factionToSet)
    {
        int factionPlayerCount = factionPlayers.Count;
        ChatService.SendChatMessage($"movePlayers factionPlayerCount: {factionPlayerCount}");
        HashSet<int> indices = [];
        Random rnd = new();
        for (int i = 0; i < numPlayersToMove; i++)
        {
            if (!indices.Add(rnd.Next(0, factionPlayerCount)))
                i--; // redo if random num generated is already in hashset
        }

        foreach (var x in indices)
        {

            // TODO: FIND WAY TO CHANGE FACTION MID GAME. functions below do not work
            //factionPlayers[x].SetFaction(factionToSet);
            //factionToSet.AddPlayer(factionPlayers[x]);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BepInEx.Configuration;
using Mirage;
using NuclearOption.Networking;
using Nuclei.Enums;
using Nuclei.Helpers;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace Nuclei.Features.Commands.DefaultCommands;

public class VoteKickCommand(ConfigFile config) : PermissionConfigurableCommand(config)
{
    public override string Name { get; } = "votekick";
    public override string Description { get; } = "lets you vote to kick a user from a list";
    public override string Usage { get; } = $"{NucleiConfig.CommandPrefixChar}votekick <[number] in their name from the Scoreboard> to select player";
    public override PermissionLevel DefaultPermissionLevel { get; } = PermissionLevel.Everyone;

    public override bool Validate(Player player, string[] args)
    {
        if (args.Length == 0)
        {
            return false;
        }
        if (args.Length > 1) return false;
        if ((args.Length == 1 && !int.TryParse(args[0], out _)) || (args.Length == 1 && int.Parse(args[0]) <= 0))
        {
            ChatService.SendPrivateChatMessage("Number invalid. Please Try again.", player);
            return false;
        }
        return true;
    }

    public override bool Execute(Player player, string[] args)
    {
        int idx = int.Parse(args[0]);
        var playerList = new List<INetworkPlayer>(Globals.AuthenticatedPlayers).Where(ip => PlayerHelper.TryGetPlayer(ip, out Player _)).Select(ip =>
        {
            ip.TryGetPlayer(out var p);
            return p;
        }).ToList();
        List<Player> l = playerList.Where(p => p.PlayerName.StartsWith($"[{idx}]")).ToList();
        if (l.Count == 0)
        {
            Nuclei.Logger?.LogError("Target player for votekick not found.");
            ChatService.SendPrivateChatMessage("Target player for votekick not found.", player);
            return false;
        }
        if (l.Count > 1)
        {
            Nuclei.Logger?.LogError("Not supposed to happen: Player with identical IDs");
            return false;
        }
        Player targetPlayer = l[0];
        string startingMessage = $"A vote to kick {targetPlayer.PlayerName} has been started.";

        void Action()
        {
            Globals.NetworkManagerNuclearOptionInstance.KickPlayerAsync(targetPlayer);
        }

        if (!VoteService.StartVote(player, startingMessage, Action))
        {
            ChatService.SendPrivateChatMessage("Cannot start a new votekick, please wait for current vote to expire.", player);
            return false;
        }
        playerList = null;
        return true;
    }

    public override bool Execute(string[] args)
    {
        throw new Exception("Requires Player object");
    }
}
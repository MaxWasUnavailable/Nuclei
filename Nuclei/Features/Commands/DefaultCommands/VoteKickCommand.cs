using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BepInEx.Configuration;
using Mirage;
using NuclearOption.Networking;
using Nuclei.Enums;
using Nuclei.Helpers;

namespace Nuclei.Features.Commands.DefaultCommands;

public class VoteKickCommand(ConfigFile config) : PermissionConfigurableCommand(config)
{
    private static List<INetworkPlayer> _playerList;
    public override string Name { get; } = "votekick";
    public override string Description { get; } = "lets you vote to kick a user using either their name or steam id.";
    public override string Usage { get; } = "votekick <name|steamid>";
    public override PermissionLevel DefaultPermissionLevel { get; } = PermissionLevel.Everyone;

    public override bool Validate(Player player, string[] args)
    {
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
        if (args.Length == 0)
        {
            _playerList = new List<INetworkPlayer>(Globals.AuthenticatedPlayers).Where(p => PlayerHelper.TryGetPlayer(p, out Player _)).ToList();
            ChatService.SendPrivateChatMessage($"Choose from the following players to kick ({NucleiConfig.CommandPrefixChar}votekick <number>):", player);
            // Get players
            for (int i = 0; i < _playerList.Count; i++)
            {
                Player p;
                if (PlayerUtils.TryGetPlayer(_playerList[i], out p))
                    ChatService.SendPrivateChatMessage($"{i + 1}: {PlayerUtils.StripStaffPrefix(p.PlayerName)}", player);
            }
            return true;
        }
        if (_playerList == null)
        {
            ChatService.SendPrivateChatMessage("Please use votekick without arguments to fetch the player list first.", player);
            return false;
        }

        int idx = int.Parse(args[0]);
        if (idx > _playerList.Count)
        {
            ChatService.SendPrivateChatMessage("Number invalid. Please Try again.", player);
            return false;
        }

        Player targetPlayer;
        if (!PlayerUtils.TryGetPlayer(_playerList[idx - 1], out targetPlayer))
        {
            Nuclei.Logger?.LogError("Target player for votekick not found.");
            return false;
        }
        string startingMessage = $"A vote to kick {targetPlayer.PlayerName} has been started.";
        Action action = () => Globals.NetworkManagerNuclearOptionInstance.KickPlayerAsync(targetPlayer);

        if (!VoteService.StartVote(player, startingMessage, action))
        {
            ChatService.SendPrivateChatMessage("Cannot start a new votekick, please wait for current vote to expire.", player);
            return false;
        }
        _playerList = null;
        return true;
    }

    public override bool Execute(string[] args)
    {
        throw new System.Exception("Requires Player object");
    }
}
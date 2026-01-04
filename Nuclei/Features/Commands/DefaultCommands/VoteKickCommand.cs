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
    private static Player? _lastPlayer;
    private static DateTime? _lastPlayerTime;
    private static List<INetworkPlayer>? _playerList;
    public override string Name { get; } = "votekick";
    public override string Description { get; } = "lets you vote to kick a user from a list";
    public override string Usage { get; } = $"{NucleiConfig.CommandPrefixChar}votekick to get list | {NucleiConfig.CommandPrefixChar}votekick <number> to select player";
    public override PermissionLevel DefaultPermissionLevel { get; } = PermissionLevel.Everyone;

    public override bool Validate(Player player, string[] args)
    {
        // Prevents multiple people from trying to initiate votekicks at the same time. Prevents race conditions when
        // a player leaves in between the times multiple players view the votekick player list
        
        // AddYears(1) is just random so that lastPlayerTime can be set to non-nullable, which is guaranteed
        if (_lastPlayer != player && _lastPlayer != null && DateTime.Now.Subtract(_lastPlayerTime ?? DateTime.Now.AddYears(1)).TotalSeconds < 30)
        {
            ChatService.SendPrivateChatMessage("Player currently starting a votekick. Please wait", player);
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
        if (args.Length == 0)
        {
            _lastPlayer = player;
            _lastPlayerTime = DateTime.Now;
            _playerList = new List<INetworkPlayer>(Globals.AuthenticatedPlayers).Where(p => PlayerHelper.TryGetPlayer(p, out Player _)).ToList();
            ChatService.SendPrivateChatMessage($"Choose from the following players to kick ({NucleiConfig.CommandPrefixChar}votekick <number>):", player);
            // Get players
            for (int i = 0; i < _playerList.Count; i++)
            {
                if (_playerList[i].TryGetPlayer(out var p))
                    ChatService.SendPrivateChatMessage($"{i + 1}: {PlayerUtils.StripStaffPrefix(p!.PlayerName)}", player);
            }
            return true;
        }
        if (_playerList == null)
        {
            ChatService.SendPrivateChatMessage($"Please type '{NucleiConfig.CommandPrefixChar}votekick' without arguments to fetch the player list first.", player);
            return false;
        }

        int idx = int.Parse(args[0]);
        if (idx > _playerList.Count)
        {
            ChatService.SendPrivateChatMessage("Number invalid. Please Try again.", player);
            return false;
        }

        if (!_playerList[idx - 1].TryGetPlayer(out var targetPlayer))
        {
            Nuclei.Logger?.LogError("Target player for votekick not found.");
            return false;
        }
        string startingMessage = $"A vote to kick {targetPlayer!.PlayerName} has been started.";

        void Action()
        {
            Globals.NetworkManagerNuclearOptionInstance.KickPlayerAsync(targetPlayer);
        }

        if (!VoteService.StartVote(player, startingMessage, Action))
        {
            ChatService.SendPrivateChatMessage("Cannot start a new votekick, please wait for current vote to expire.", player);
            return false;
        }
        _playerList = null;
        return true;
    }

    public override bool Execute(string[] args)
    {
        throw new Exception("Requires Player object");
    }
}
using System;
using System.Linq;
using System.Text;
using BepInEx.Configuration;
using NuclearOption.Networking;
using Nuclei.Enums;
using Nuclei.Features;
using Nuclei.Features.Commands;
using Nuclei.Helpers;
using VoteKick.Services;

namespace VoteKick.Commands;

public class VoteKickCommand(ConfigFile config) : PermissionConfigurableCommand(config)
{
    public override string Name { get; } = "votekick";
    public override string Description { get; } = "lets you vote to kick a user using either their name or steam id.";
    public override string Usage { get; } = "votekick <name|steamid>";
    public override PermissionLevel DefaultPermissionLevel { get; } = PermissionLevel.Everyone;
    
    public override bool Validate(Player player, string[] args)
    {
        if (args.Length >= 1) return true;
        
        var sb = new StringBuilder();
        sb.AppendLine("Choose a player to vote using either their name or steamid.");
        //sb.Append("Available reasons: ");
        //sb.Append(string.Join(", ", formattedReasons));
        ChatService.SendPrivateChatMessage(sb.ToString(), player);
        return false;
    }

    public override bool Execute(Player player, string[] args)
    {
        var target = String.Join(" ", args);
        /*
        var reason = args[1];
        if(!Enum.TryParse(reason, out KickReason reasonEnum))
        {
            ChatService.SendPrivateChatMessage("Could not parse reason.", player);
            return false;
        }
        */
        
        Player targetPlayer;

        PlayerUtils.TryFindPlayer(target, out targetPlayer);
        if (targetPlayer == null)
        {
            ChatService.SendPrivateChatMessage("Player not found.", player);
            return false;
        }
        else
        {
            if (!VoteKickService.StartVoteKick(player, targetPlayer))
            {
                ChatService.SendPrivateChatMessage("Cannot start a new votekick, please wait for current votekick to expire.", player);
                return false;
            }
        }
        return true;
    }

    public override bool Execute(string[] args)
    {
        throw new System.Exception("Requires Player object");
    }
}
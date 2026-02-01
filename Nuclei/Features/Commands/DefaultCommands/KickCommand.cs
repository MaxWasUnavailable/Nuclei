using System;
using BepInEx.Configuration;
using Cysharp.Threading.Tasks;
using NuclearOption.Networking;
using Nuclei.Enums;
using Nuclei.Helpers;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace Nuclei.Features.Commands.DefaultCommands;

/// <summary>
///     Command to kick a player from the server.
/// </summary>
public class KickCommand(ConfigFile config) : PermissionConfigurableCommand(config)
{
    public override string Name { get; } = "kick";
    public override string Description { get; } = "Kicks a player from the server.";
    public override string Usage { get; } = "kick <player_name>";

    public override bool Validate(Player player, string[] args)
    {
        return args.Length == 1;
    }

    public override bool Execute(Player player, string[] args)
    {
        var target = args[0];

        if (PlayerUtils.TryFindPlayer(target, out var targetPlayer))
        {
            if (targetPlayer == player)
            {
                ChatService.SendPrivateChatMessage("You can't kick yourself.", player);
                return false;
            }

            _ = Globals.NetworkManagerNuclearOptionInstance.KickPlayerAsync(targetPlayer);
            Nuclei.Logger?.LogInfo($"Player {target} kicked from the server.");
            return true;
        }

        ChatService.SendPrivateChatMessage($"Player {target} not found.", player);
        Nuclei.Logger?.LogWarning($"Player {target} not found.");
        return false;
    }
    
    public override bool Execute(string[] args)
    {
        var target = args[0];

        if (PlayerUtils.TryFindPlayer(target, out var targetPlayer))
        {
            _ = Globals.NetworkManagerNuclearOptionInstance.KickPlayerAsync(targetPlayer);
            Nuclei.Logger?.LogInfo($"Player {target} was kicked from the server.");
            ChatService.SendChatMessage($"Player {target} was kicked from the server.");
            return true;
        }

        Nuclei.Logger?.LogWarning($"Player {target} not found.");
        return false;
    }

    public override PermissionLevel DefaultPermissionLevel { get; } = PermissionLevel.Moderator;
}
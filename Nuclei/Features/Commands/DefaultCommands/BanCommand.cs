using BepInEx.Configuration;
using NuclearOption.Networking;
using Nuclei.Enums;
using Nuclei.Helpers;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace Nuclei.Features.Commands.DefaultCommands;

/// <summary>
///     Command to ban a player from the server.
/// </summary>
public class BanCommand(ConfigFile config) : PermissionConfigurableCommand(config)
{
    public override string Name { get; } = "ban";
    public override string Description { get; } = "Bans a player from the server.";
    public override string Usage { get; } = "ban <player_name>";

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
                ChatService.SendPrivateChatMessage("You can't ban yourself.", player);
                return false;
            }

            NucleiConfig.AddBannedPlayer(targetPlayer!.SteamID.ToString());
            _ = Globals.NetworkManagerNuclearOptionInstance.KickPlayerAsync(targetPlayer);
            Nuclei.Logger?.LogInfo($"Player {target} banned from the server.");
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

            NucleiConfig.AddBannedPlayer(targetPlayer!.SteamID.ToString());
            _ = Globals.NetworkManagerNuclearOptionInstance.KickPlayerAsync(targetPlayer);
            Nuclei.Logger?.LogInfo($"Player {target} banned from the server.");
            return true;
        }

        Nuclei.Logger?.LogWarning($"Player {target} not found.");
        return false;
    }


    public override PermissionLevel DefaultPermissionLevel { get; } = PermissionLevel.Moderator;
}
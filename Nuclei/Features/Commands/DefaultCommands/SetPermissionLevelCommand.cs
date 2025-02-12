using BepInEx.Configuration;
using Nuclei.Helpers;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace Nuclei.Features.Commands.DefaultCommands;

/// <summary>
///     Command to set the permission level of a player.
/// </summary>
public class SetPermissionLevelCommand(ConfigFile config) : PermissionConfigurableCommand(config)
{
    public override string Name { get; } = "setpermissionlevel";
    public override string Description { get; } = "Set the permission level of a player.";
    public override string Usage { get; } = "setpermissionlevel <player_name> <permission_level>";

    public override bool Validate(Player player, string[] args)
    {
        return args.Length == 2 && PermissionLevelUtils.TryParsePermissionLevel(args[1], out _);
    }

    public override void Execute(Player player, string[] args)
    {
        var target = args[0];
        PermissionLevelUtils.TryParsePermissionLevel(args[1], out var permissionLevel);

        if (PlayerUtils.TryFindPlayer(target, out var targetPlayer))
        {
            var targetSteamID = player.SteamID.ToString();
            switch (permissionLevel)
            {
                case PermissionLevel.Admin:
                    NucleiConfig.AddAdmin(targetSteamID);
                    NucleiConfig.RemoveModerator(targetSteamID);
                    break;
                case PermissionLevel.Moderator:
                    NucleiConfig.RemoveAdmin(targetSteamID);
                    NucleiConfig.AddModerator(targetSteamID);
                    break;
                default:
                case PermissionLevel.Everyone:
                    NucleiConfig.RemoveAdmin(targetSteamID);
                    NucleiConfig.RemoveModerator(targetSteamID);
                    break;
            }
            Nuclei.Logger?.LogInfo($"Set permission level of {target} to {permissionLevel}.");
        }
        else
        {
            ChatService.SendPrivateChatMessage($"Player {target} not found.", player);
            Nuclei.Logger?.LogWarning($"Player {target} not found.");
        }
    }
    
    public override PermissionLevel DefaultPermissionLevel { get; } = PermissionLevel.Admin;
}
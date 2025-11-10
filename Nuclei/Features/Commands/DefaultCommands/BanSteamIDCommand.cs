using BepInEx.Configuration;
using NuclearOption.Networking;
using Nuclei.Enums;
using Nuclei.Helpers;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace Nuclei.Features.Commands.DefaultCommands;

/// <summary>
///     Command to ban a steam id from the server. Useful if player is offline.
/// </summary>
public class BanSteamIDCommand(ConfigFile config) : PermissionConfigurableCommand(config)
{
    public override string Name { get; } = "bansteamid";
    public override string Description { get; } = "Bans a steamid from the server.";
    public override string Usage { get; } = "bansteamid <steamid>";

    public override bool Validate(Player player, string[] args)
    {
        return args.Length == 1;
    }

    public override bool Execute(Player player, string[] args)
    {
        var target = args[0];
        NucleiConfig.AddBannedPlayer(target.ToString());
        Nuclei.Logger?.LogInfo($"SteamID {target} banned from the server.");
        return true;
    }

    public override bool Execute(string[] args)
    {
        var target = args[0];
        NucleiConfig.AddBannedPlayer(target.ToString());
        Nuclei.Logger?.LogInfo($"SteamID {target} banned from the server.");
        return true;
    }


    public override PermissionLevel DefaultPermissionLevel { get; } = PermissionLevel.Moderator;
}
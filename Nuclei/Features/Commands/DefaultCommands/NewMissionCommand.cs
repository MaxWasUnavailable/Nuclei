using BepInEx.Configuration;
using Nuclei.Enums;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Nuclei.Features.Commands.DefaultCommands;

/// <summary>
///     Command to end the current mission and start a new one.
/// </summary>
public class NewMissionCommand(ConfigFile config) : PermissionConfigurableCommand(config)
{
    public override string Name { get; } = "newmission";
    public override string Description { get; } = "Ends the current mission and starts a new one.";
    public override string Usage { get; } = "newmission";
    public override bool Validate(Player player, string[] args)
    {
        return true;
    }

    public override void Execute(Player player, string[] args)
    {
        _ = Server.StartOrRestartLobby();
    }

    public override PermissionLevel DefaultPermissionLevel { get; } = PermissionLevel.Admin;
}
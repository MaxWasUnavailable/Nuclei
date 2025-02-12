using BepInEx.Configuration;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace Nuclei.Features.Commands.DefaultCommands;

/// <summary>
///     Command to stop the server.
/// </summary>
public class StopCommand(ConfigFile config) : PermissionConfigurableCommand(config)
{
    public override string Name { get; } = "stop";
    public override string Description { get; } = "Stop the server.";
    public override string Usage { get; } = "stop";

    public override bool Validate(Player player, string[] args)
    {
        return args.Length == 0;
    }

    public override void Execute(Player player, string[] args)
    {
        Server.StopServer();
    }
    
    public override PermissionLevel DefaultPermissionLevel { get; } = PermissionLevel.Owner;
}
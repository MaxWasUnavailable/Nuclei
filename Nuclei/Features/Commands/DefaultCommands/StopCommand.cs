using BepInEx.Configuration;
using NuclearOption.Networking;
using Nuclei.Enums;
using Nuclei.Helpers;

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

    public override bool Execute(Player player, string[] args)
    {
        ChatService.SendPrivateChatMessage("Stopping server...", player);
        Globals.NetworkManagerNuclearOptionInstance.Server.Stop();
        return true;
    }
    public override bool Execute( string[] args)
    {
        Nuclei.Logger?.LogInfo("Stopping server...");
        Globals.NetworkManagerNuclearOptionInstance.Server.Stop();
        return true;
    }
    
    public override PermissionLevel DefaultPermissionLevel { get; } = PermissionLevel.Owner;
}
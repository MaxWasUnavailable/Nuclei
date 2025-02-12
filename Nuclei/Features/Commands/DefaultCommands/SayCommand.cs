using BepInEx.Configuration;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace Nuclei.Features.Commands.DefaultCommands;

/// <summary>
///     Command to broadcast a message to all players from the server.
/// </summary>
public class SayCommand(ConfigFile config) : PermissionConfigurableCommand(config)
{
    public override string Name { get; } = "say";
    public override string Description { get; } = "Broadcast a message to all players from the server.";
    public override string Usage { get; } = "say <message>";
    public override PermissionLevel DefaultPermissionLevel { get; } = PermissionLevel.Moderator;

    public override bool Validate(Player player, string[] args)
    {
        return args.Length > 0;
    }

    public override void Execute(Player player, string[] args)
    {
        var message = string.Join(" ", args);
        ChatManager.SendChatMessage($"{message}", true);
    }
}
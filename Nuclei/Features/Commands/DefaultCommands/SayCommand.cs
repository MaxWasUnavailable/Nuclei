using BepInEx.Configuration;
using Nuclei.Enums;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace Nuclei.Features.Commands.DefaultCommands;

/// <summary>
///     Command to broadcast a message to all players, through the server account.
/// </summary>
public class SayCommand(ConfigFile config) : PermissionConfigurableCommand(config)
{
    public override string Name { get; } = "say";
    public override string Description { get; } = "Broadcast a message to all players, through the server account.";
    public override string Usage { get; } = "say <message>";

    public override bool Validate(Player player, string[] args)
    {
        return args.Length > 0;
    }

    public override void Execute(Player player, string[] args)
    {
        var message = string.Join(" ", args);
        ChatService.SendChatMessage($"{message}");
    }
    
    public override PermissionLevel DefaultPermissionLevel { get; } = PermissionLevel.Moderator;
}
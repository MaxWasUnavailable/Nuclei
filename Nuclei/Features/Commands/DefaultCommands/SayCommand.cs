#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace Nuclei.Features.Commands.DefaultCommands;

/// <summary>
///     Command to broadcast a message to all players from the server.
/// </summary>
public class SayCommand : ICommand
{
    public string Name { get; } = "say";
    public string Description { get; } = "Broadcast a message to all players from the server.";
    public string Usage { get; } = "say <message>";
    public PermissionLevel PermissionLevel { get; } = PermissionLevel.Moderator;
    public bool Validate(Player player, string[] args)
    {
        return args.Length > 0;
    }

    public void Execute(Player player, string[] args)
    {
        var message = string.Join(" ", args);
        ChatManager.SendChatMessage($"{message}", true);
    }
}
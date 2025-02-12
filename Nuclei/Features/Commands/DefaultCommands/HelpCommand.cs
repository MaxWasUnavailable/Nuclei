using System.Linq;
using BepInEx.Configuration;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace Nuclei.Features.Commands.DefaultCommands;

/// <summary>
///     Command to get help on other commands, or get the list of commands you have access to.
/// </summary>
public class HelpCommand(ConfigFile config) : PermissionConfigurableCommand(config)
{
    public override string Name { get; } = "help";
    public override string Description { get; } = "Get help on other commands, or get the list of commands you have access to.";
    public override string Usage { get; } = "help [command name]";

    public override bool Validate(Player player, string[] args)
    {
        return args.Length <= 1;
    }

    public override void Execute(Player player, string[] args)
    {
        if (args.Length == 0)
        {
            var accessibleCommands = CommandService.GetCommands().Where(c => c.PermissionLevel <= CommandService.GetPlayerPermissionLevel(player)).ToList();
            var commandNames = accessibleCommands.Select(c => c.Name).ToList();
            ChatService.SendPrivateChatMessage($"You have access to the following commands: {string.Join(", ", commandNames)}", player);
            return;
        }
        
        var commandName = args[0];
        if (!CommandService.TryGetCommand(commandName, out var command))
        {
            ChatService.SendPrivateChatMessage($"Command '{commandName}' not found.", player);
            return;
        }
        
        ChatService.SendPrivateChatMessage($"Command '{command.Name}': {command.Description}", player);
    }
    
    public override PermissionLevel DefaultPermissionLevel { get; } = PermissionLevel.Everyone;
}
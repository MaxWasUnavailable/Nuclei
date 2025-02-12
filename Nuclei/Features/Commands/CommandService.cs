using System.Collections.Generic;
using System.Linq;

namespace Nuclei.Features.Commands;

/// <summary>
///     Service for handling commands.
/// </summary>
public static class CommandService
{
    private static readonly List<ICommand> Commands = [];

    /// <summary>
    ///     Register a command.
    /// </summary>
    /// <param name="command"> The command to register. </param>
    public static void RegisterCommand(ICommand command)
    {
        Commands.Add(command);
    }

    /// <summary>
    ///     Get the permission level of a player.
    /// </summary>
    /// <param name="player"> The player. </param>
    /// <returns> The permission level of the player. </returns>
    public static PermissionLevel GetPlayerPermissionLevel(Player player)
    {
        if (player.IsHost)
            return PermissionLevel.Admin;

        return 0; // TODO: Implement permission levels from config (?)
    }

    /// <summary>
    ///     Try to get a command by name.
    /// </summary>
    /// <param name="commandName"> The name of the command. </param>
    /// <param name="command"> The command, if available. </param>
    /// <returns></returns>
    public static bool TryGetCommand(string commandName, out ICommand command)
    {
        command = Commands.Find(c => c.Name.ToLower() == commandName);
        return command != null;
    }

    /// <summary>
    ///     Try to execute a command.
    /// </summary>
    /// <param name="player"> The player executing the command. </param>
    /// <param name="commandName"> The name of the command. </param>
    /// <param name="args"> The arguments for the command. </param>
    /// <returns></returns>
    public static bool TryExecuteCommand(Player player, string commandName, string[] args)
    {
        if (!TryGetCommand(commandName, out var command))
        {
            Nuclei.Logger?.LogWarning($"Command {commandName} not found");
            return false;
        }
        
        if (GetPlayerPermissionLevel(player) < command.PermissionLevel)
        {
            Nuclei.Logger?.LogWarning($"Player {player.PlayerName} does not have permission to execute command {commandName}");
            return false;
        }
        
        if (command.Validate(player, args))
        {
            command.Execute(player, args);
            Nuclei.Logger?.LogInfo($"Command {commandName} executed successfully by {player.PlayerName} with argument(s): {string.Join(", ", args)}");
        }
        else
        {
            Nuclei.Logger?.LogWarning($"Validation for command {commandName} ran by {player.PlayerName} failed with argument(s): {string.Join(", ", args)}");
            return false;
        }
        return true;
    }

    /// <summary>
    ///     Try to execute a command.
    /// </summary>
    /// <param name="player"> The player executing the command. </param>
    /// <param name="message"> The message containing the command. </param>
    /// <returns> Whether the command was executed successfully. </returns>
    public static bool TryExecuteCommand(Player player, string message)
    {
        var split = message.Split(' ');
        var commandName = split[0];
        var args = split.Skip(1).ToArray();
        
        return TryExecuteCommand(player, commandName, args);
    }
}
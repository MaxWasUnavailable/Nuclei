namespace Nuclei.Features.Commands;

/// <summary>
///     Interface for commands.
/// </summary>
public interface ICommand
{
    /// <summary>
    ///     The command name.
    /// </summary>
    string Name { get; }

    /// <summary>
    ///     The command description.
    /// </summary>
    string Description { get; }

    /// <summary>
    ///     The command usage.
    /// </summary>
    string Usage { get; }

    /// <summary>
    ///     The command permission level. Users with levels higher than this can execute the command.
    /// </summary>
    PermissionLevel PermissionLevel { get; }

    /// <summary>
    ///     Validate the command arguments.
    /// </summary>
    /// <param name="player"> The player executing the command. </param>
    /// <param name="args"> The command arguments. </param>
    /// <returns> Whether the command arguments are valid. </returns>
    bool Validate(Player player, string[] args);

    /// <summary>
    ///     The command action.
    /// </summary>
    /// <param name="player"> The player executing the command. </param>
    /// <param name="args"> The command arguments. </param>
    void Execute(Player player, string[] args);
}
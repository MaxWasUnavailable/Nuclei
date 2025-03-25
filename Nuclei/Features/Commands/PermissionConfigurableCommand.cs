using BepInEx.Configuration;
using Nuclei.Enums;

namespace Nuclei.Features.Commands;

/// <summary>
///     Base class for commands that can be configured with a permission level.
/// </summary>
public abstract class PermissionConfigurableCommand : ICommand
{
    private const string CommandConfigSection = "Commands";

    /// <inheritdoc />
    public abstract string Name { get; }

    /// <inheritdoc />
    public abstract string Description { get; }

    /// <inheritdoc />
    public abstract string Usage { get; }

    /// <inheritdoc />
    public abstract bool Validate(Player player, string[] args);

    /// <inheritdoc />
    public abstract bool Execute(Player player, string[] args);

    /// <inheritdoc />
    public PermissionLevel PermissionLevel => PermissionLevelConfig.Value;

    /// <summary>
    ///     The command permission level configuration.
    /// </summary>
    private ConfigEntry<PermissionLevel> PermissionLevelConfig { get; }

    /// <summary>
    ///     The default permission level required to execute the command.
    /// </summary>
    public abstract PermissionLevel DefaultPermissionLevel { get; }

    /// <summary>
    ///     Constructor for the base command.
    /// </summary>
    /// <param name="config"> BepInEx configuration file. </param>
    protected PermissionConfigurableCommand(ConfigFile config)
    {
        PermissionLevelConfig = config.Bind(CommandConfigSection, Name, DefaultPermissionLevel, $"The permission level required to execute the {Name} command.");
    }
}
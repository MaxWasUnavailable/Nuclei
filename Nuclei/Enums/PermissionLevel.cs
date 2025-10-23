namespace Nuclei.Enums;

/// <summary>
///     The permission level required to execute a command.
/// </summary>
public enum PermissionLevel
{
    /// <summary>
    ///     Everyone can execute the command.
    /// </summary>
    Everyone = 0,

    /// <summary>
    ///     Only moderators and above can execute the command.
    /// </summary>
    Moderator = 1,

    /// <summary>
    ///     Only admins can execute the command.
    /// </summary>
    Admin = 2,

    /// <summary>
    ///     Only the owner can execute the command.
    /// </summary>
    Owner = 3
}
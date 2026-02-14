using Nuclei.Abstractions.BepInEx.Config;
using Nuclei.Abstractions.BepInEx.Logging;

namespace Nuclei.Core.Config;

/// <summary>
///     Config for Nuclei's core.
/// </summary>
///
/// TODO: rework config system to be more flexible (handle prefixed config categories, service-specific config, auto-handle config entry + default value, etc.)
public static class NucleiConfig
{
    internal const string GeneralSection = "General";

    private static ILogger? Logger { get; set; }

    internal static IConfigEntry<string>? CommandPrefix;
    internal const string DefaultCommandPrefix = "/";

    internal static IConfigEntry<string>? MessageOfTheDay;
    internal const string DefaultMessageOfTheDay = "This server is running on Nuclei! Have fun!";

    internal static IConfigEntry<uint>? MotDFrequency;
    internal const uint DefaultMotDFrequency = 900;

    internal static IConfigEntry<string>? WelcomeMessage;
    internal const string DefaultWelcomeMessage = "Welcome to the server, {player_name_censored}!";

    internal static char CommandPrefixChar => CommandPrefix!.Value[0];

    public static void Initialize(IConfigProvider config, ILogger logger)
    {
        Logger = logger;
        Logger.Debug("Loading settings...");

        CommandPrefix = config.Bind(GeneralSection, "CommandPrefix", DefaultCommandPrefix, "The prefix used to identify commands. Must be a single character.");
        Logger.Debug($"CommandPrefix: {CommandPrefix.Value}");

        MessageOfTheDay = config.Bind(GeneralSection, "MessageOfTheDay", DefaultMessageOfTheDay, "The message of the day for the server. This message is displayed periodically to all players. See the readme for placeholders.");
        Logger.Debug($"MessageOfTheDay: {MessageOfTheDay.Value}");

        MotDFrequency = config.Bind(GeneralSection, "MotDFrequency", DefaultMotDFrequency, "The frequency in seconds at which the message of the day is displayed. Set to 0 to disable the message of the day. Checks are done every minute.");
        Logger.Debug($"MotDFrequency: {MotDFrequency.Value}");

        WelcomeMessage = config.Bind(GeneralSection, "WelcomeMessage", DefaultWelcomeMessage, "The message displayed to players when they join the server. See the readme for placeholders.");
        Logger.Debug($"WelcomeMessage: {WelcomeMessage.Value}");

        Logger.Debug("Loaded settings!");
    }

    public static void ValidateSettings()
    {
        Logger?.Debug("Validating settings...");

        if (CommandPrefix != null && CommandPrefix.Value.Length != 1)
        {
            Logger?.Warn("CommandPrefix must be a single character! Resetting to default value.");
            CommandPrefix.Value = DefaultCommandPrefix;
        }

        if (MessageOfTheDay != null && string.IsNullOrWhiteSpace(MessageOfTheDay.Value))
        {
            Logger?.Warn("MessageOfTheDay cannot be empty! Resetting to default value.");
            MessageOfTheDay.Value = DefaultMessageOfTheDay;
        }

        if (WelcomeMessage != null && string.IsNullOrWhiteSpace(WelcomeMessage.Value))
        {
            Logger?.Warn("WelcomeMessage cannot be empty! Resetting to default value.");
            WelcomeMessage.Value = DefaultWelcomeMessage;
        }

        Logger?.Debug("Settings validated!");
    }
}

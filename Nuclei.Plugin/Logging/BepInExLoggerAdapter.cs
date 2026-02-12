using System;
using BepInEx.Logging;
using Nuclei.Abstractions.BepInEx.Logging;

namespace Nuclei.Plugin.Logging;

/// <summary>
///     An adapter that allows using a BepInEx <see cref="ManualLogSource" /> as an <see cref="ILogger" />.
/// </summary>
/// <param name="logger"> The BepInEx logger.</param>
internal sealed class BepInExLoggerAdapter(ManualLogSource logger) : ILogger
{
    /// <summary>
    ///     Logs a debug message.
    /// </summary>
    /// <param name="message"> The message to log. </param>
    public void Debug(string message)
    {
        logger.LogDebug(message);
    }

    /// <summary>
    ///     Logs an informational message.
    /// </summary>
    /// <param name="message"> The message to log. </param>
    public void Info(string message)
    {
        logger.LogInfo(message);
    }

    /// <summary>
    ///     Logs a warning message.
    /// </summary>
    /// <param name="message"> The message to log. </param>
    public void Warn(string message)
    {
        logger.LogWarning(message);
    }

    /// <summary>
    ///     Logs an error message.
    /// </summary>
    /// <param name="message"> The message to log. </param>
    public void Error(string message)
    {
        logger.LogError(message);
    }

    /// <summary>
    ///     Logs an error message with an associated exception.
    /// </summary>
    /// <param name="message"> The message to log. </param>
    /// <param name="exception"> The exception to log. </param>
    public void Error(string message, Exception exception)
    {
        logger.LogError($"{message}{Environment.NewLine}{exception}");
    }
}

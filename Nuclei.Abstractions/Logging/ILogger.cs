using System;

namespace Nuclei.Abstractions.Logging;

/// <summary>
///     Defines the interface for a logger.
/// </summary>
public interface ILogger
{
    /// <summary>
    ///     Logs a debug message.
    /// </summary>
    /// <param name="message"> The message to log. </param>
    void Debug(string message);

    /// <summary>
    ///     Logs an informational message.
    /// </summary>
    /// <param name="message"> The message to log. </param>
    void Info(string message);

    /// <summary>
    ///     Logs a warning message.
    /// </summary>
    /// <param name="message"> The message to log. </param>
    void Warn(string message);

    /// <summary>
    ///     Logs an error message.
    /// </summary>
    /// <param name="message"> The message to log. </param>
    void Error(string message);

    /// <summary>
    ///     Logs an error message with an associated exception.
    /// </summary>
    /// <param name="exception"> The exception to log. </param>
    /// <param name="message"> The message to log. </param>
    void Error(Exception exception, string message);
}

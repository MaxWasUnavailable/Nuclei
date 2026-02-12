using System;
using Nuclei.Abstractions.BepInEx.Logging;

namespace Nuclei.Abstractions.Nuclei.Decorators;

/// <summary>
///     Base class for logger decorators. Provides a virtual decoration method.
/// </summary>
/// <param name="innerLogger"> The logger to decorate. </param>
public abstract class AbstractLoggerDecorator(ILogger innerLogger) : ILogger
{
    /// <summary>
    ///     Gets the inner logger that is being decorated. This is useful for accessing the underlying logger instance.
    /// </summary>
    public ILogger InnerLogger { get; } = innerLogger;

    /// <summary>
    ///     Decorates the given log message. This method is called by the concrete logger decorator implementations to
    ///     modify the log message before it is passed to the inner logger.
    /// </summary>
    /// <param name="message"> The log message to decorate. </param>
    /// <returns> The decorated log message. </returns>
    protected virtual string DecorateMessage(string message)
    {
        return message;
    }

    /// <inheritdoc />
    public void Debug(string message)
    {
        InnerLogger.Debug(DecorateMessage(message));
    }

    /// <inheritdoc />
    public void Info(string message)
    {
        InnerLogger.Info(DecorateMessage(message));
    }

    /// <inheritdoc />
    public void Warn(string message)
    {
        InnerLogger.Warn(DecorateMessage(message));
    }

    /// <inheritdoc />
    public void Error(string message)
    {
        InnerLogger.Error(DecorateMessage(message));
    }

    /// <inheritdoc />
    public void Error(string message, Exception exception)
    {
        InnerLogger.Error(DecorateMessage(message), exception);
    }
}
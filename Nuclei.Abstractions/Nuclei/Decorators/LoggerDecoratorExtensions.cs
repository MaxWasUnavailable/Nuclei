using System;
using Nuclei.Abstractions.BepInEx.Logging;

namespace Nuclei.Abstractions.Nuclei.Decorators;

/// <summary>
///     Extension methods for <see cref="ILogger" /> that provide convenient ways to create decorated logger instances.
/// </summary>
public static class LoggerDecoratorExtensions
{
    /// <param name="logger"> The logger to decorate. </param>
    extension(ILogger logger)
    {
        /// <summary>
        ///     Decorates the given logger with a scope prefix. This is useful for distinguishing log messages from
        ///     different projects / modules.
        /// </summary>
        /// <param name="scope"> The name of the scope to add to each log message. </param>
        /// <returns> A new logger instance that decorates the given logger with the specified scope. </returns>
        public ILogger WithScope(string scope)
        {
            return new ScopeLoggerDecorator(logger, scope);
        }

        /// <summary>
        ///     Decorates the given logger with a timestamp. This is useful for adding timestamps to log messages.
        /// </summary>
        /// <param name="timestampFormat"> The timestamp format string. Defaults to "HH:mm:ss.fff". </param>
        /// <param name="timestampProvider"> Optional timestamp provider override. Defaults to using the current UTC time. </param>
        /// <returns> A new logger instance that decorates the given logger with timestamps. </returns>
        public ILogger WithTimestamp(string? timestampFormat = null, Func<DateTimeOffset>? timestampProvider = null)
        {
            return new TimestampLoggerDecorator(logger, timestampFormat, timestampProvider);
        }
    }

    /// <summary>
    ///     Gets the root logger from a decorated logger instance. This is useful for unwrapping loggers to access
    ///     the underlying root logger.
    /// </summary>
    /// <param name="logger"> The decorated logger instance. </param>
    /// <returns> The root logger instance that is wrapped by the decorators. </returns>
    public static ILogger GetRootLogger(ILogger logger)
    {
        var current = logger;
        while (current is AbstractLoggerDecorator decorator)
        {
            var next = decorator.InnerLogger;
            if (ReferenceEquals(next, current)) break;

            current = next;
        }

        return current;
    }
}
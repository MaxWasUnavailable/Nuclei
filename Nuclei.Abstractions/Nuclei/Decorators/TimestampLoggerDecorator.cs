using System;
using Nuclei.Abstractions.BepInEx.Logging;

namespace Nuclei.Abstractions.Nuclei.Decorators;

/// <summary>
///     A logger decorator that adds a timestamp to each log message.
/// </summary>
public class TimestampLoggerDecorator(
    ILogger innerLogger,
    string? timestampFormat = "HH:mm:ss.fff",
    Func<DateTimeOffset>? timestampProvider = null)
    : AbstractLoggerDecorator(innerLogger)
{
    private readonly string _timestampFormat =
        (string.IsNullOrWhiteSpace(timestampFormat) ? "HH:mm:ss.fff" : timestampFormat)!;
    private readonly Func<DateTimeOffset> _timestampProvider = timestampProvider ?? (() => DateTimeOffset.UtcNow);

    /// <inheritdoc />
    protected override string DecorateMessage(string message)
    {
        return $"[{_timestampProvider().ToString(_timestampFormat)}] {message}";
    }
}
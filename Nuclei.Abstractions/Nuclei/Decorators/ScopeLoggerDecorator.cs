using Nuclei.Abstractions.BepInEx.Logging;

namespace Nuclei.Abstractions.Nuclei.Decorators;

/// <summary>
///     A logger decorator that adds the specified scope to each log message. This is useful for distinguishing log
///     messages from different projects / modules when they are all being logged to the same destination.
/// </summary>
public class ScopeLoggerDecorator(ILogger innerLogger, string scope) : AbstractLoggerDecorator(innerLogger)
{
    /// <inheritdoc />
    protected override string DecorateMessage(string message)
    {
        return $"[{scope}] {message}";
    }
}
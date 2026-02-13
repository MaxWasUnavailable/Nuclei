using Nuclei.Abstractions.BepInEx.Config;
using Nuclei.Abstractions.BepInEx.Logging;

namespace Nuclei.Abstractions.Nuclei;

/// <summary>
///     Interface for a Nuclei context.
///     A context provides access to a service provider and other shared resources.
/// </summary>
public interface INucleiContext
{
    /// <summary>
    ///     Gets the service provider for this context.
    /// </summary>
    IServiceProvider ServiceProvider { get; }

    /// <summary>
    ///     Gets the logger for this context.
    /// </summary>
    ILogger Logger { get; }

    /// <summary>
    ///     Gets the config provider for this context.
    /// </summary>
    IConfigProvider Config { get; }
}
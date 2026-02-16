using System.Linq;
using Nuclei.Abstractions.BepInEx.Config;
using Nuclei.Abstractions.BepInEx.Logging;
using Nuclei.Abstractions.Nuclei;
using IServiceProvider = Nuclei.Abstractions.Nuclei.IServiceProvider;

namespace Nuclei.Core;

/// <summary>
///     Implementation of <see cref="INucleiContext" />.
/// </summary>
public class NucleiContext(IServiceProvider serviceProvider, ILogger logger, IConfigProvider config) : INucleiContext
{
    /// <inheritdoc />
    public IServiceProvider ServiceProvider { get; } = serviceProvider;

    /// <inheritdoc />
    public ILogger Logger { get; } = logger;

    /// <inheritdoc />
    public IConfigProvider Config { get; } = config;

    /// <summary>
    ///     Initializes all services registered in the <see cref="ServiceProvider" />.
    /// </summary>
    internal void InitializeServices()
    {
        foreach (var service in DependsOnAttribute.OrderServices(ServiceProvider.GetAll().ToList()))
            service.Initialize(this);
    }
}
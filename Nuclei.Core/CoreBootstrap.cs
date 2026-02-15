using Nuclei.Abstractions.BepInEx.Config;
using Nuclei.Abstractions.BepInEx.Logging;
using Nuclei.Abstractions.Nuclei;
using Nuclei.Abstractions.Nuclei.Decorators;
using Nuclei.Core.Config;
using Nuclei.Core.Services;
using Nuclei.Core.Services.TimeScheduler;
using IServiceProvider = Nuclei.Abstractions.Nuclei.IServiceProvider;

namespace Nuclei.Core;

/// <summary>
///     Static class responsible for initializing Nuclei's core components, such as the context, services, and other shared
///     resources.
/// </summary>
public static class CoreBootstrap
{
    /// <summary>
    ///     Initializes Nuclei's context & services.
    /// </summary>
    public static INucleiContext Initialize(ILogger logger, IConfigProvider config)
    {
        var bootstrapLogger = logger.WithTimestamp().WithScope(nameof(Core));

        bootstrapLogger.Debug("Starting Nuclei bootstrap process...");

        bootstrapLogger.Debug("Initializing Nuclei config.");
        NucleiConfig.Initialize(config, logger);
        NucleiConfig.ValidateSettings();
        bootstrapLogger.Debug("Nuclei config initialized successfully.");

        bootstrapLogger.Debug("Initializing Nuclei context.");
        var context = InitializeContext(config, logger);
        bootstrapLogger.Debug("Nuclei context initialized successfully.");

        bootstrapLogger.Info("Core components initialized successfully.");

        return context;
    }

    private static IServiceProvider BuildServiceProvider()
    {
        var serviceRegistry = new ServiceRegistry();

        serviceRegistry.Register(new TimeSchedulerService());

        return serviceRegistry;
    }


    private static INucleiContext InitializeContext(IConfigProvider config, ILogger logger)
    {
        var context = new NucleiContext(BuildServiceProvider(), logger, config);

        context.InitializeServices();

        return context;
    }
}
using System.Collections.Generic;

namespace Nuclei.Abstractions.Nuclei;

/// <summary>
///     Interface for service providers. A service provider is a component that can provide Nuclei Services.
/// </summary>
public interface IServiceProvider
{
    /// <summary>
    ///     Gets all registered services.
    /// </summary>
    /// <returns>A collection of all registered services.</returns>
    ICollection<INucleiService> GetAll();

    /// <summary>
    ///     Gets a registered service of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the service to get.</typeparam>
    /// <returns>The registered service of the specified type, or null if no such service is registered.</returns>
    T Get<T>() where T : class, INucleiService;

    /// <summary>
    ///     Tries to get a registered service of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the service to get.</typeparam>
    /// <param name="service">
    ///     When this method returns, contains the registered service of the specified type, if it is found;
    ///     otherwise, null.
    /// </param>
    /// <returns>true if the service was found; otherwise, false.</returns>
    bool TryGet<T>(out T service) where T : class, INucleiService;
}
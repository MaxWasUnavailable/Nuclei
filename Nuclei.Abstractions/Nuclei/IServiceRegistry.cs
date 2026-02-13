using System;

namespace Nuclei.Abstractions.Nuclei;

/// <summary>
///     Interface for service registries. A service registry is a component that can register (& dispose of) Nuclei
///     Services.
/// </summary>
public interface IServiceRegistry : IDisposable
{
    /// <summary>
    ///     Registers a service with the provider.
    /// </summary>
    /// <param name="service">The service to register.</param>
    void Register<T>(T service) where T : class, INucleiService;
}
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Nuclei.Abstractions.Nuclei;
using IServiceProvider = Nuclei.Abstractions.Nuclei.IServiceProvider;

namespace Nuclei.Core;

/// <summary>
///     Provider for Nuclei services, allows for registration and retrieval of services.
/// </summary>
public class ServiceRegistry : IServiceProvider, IServiceRegistry
{
    private readonly ConcurrentDictionary<Type, INucleiService> _services = new();

    /// <inheritdoc />
    public void Register<T>(T service) where T : class, INucleiService
    {
        if (service == null)
            throw new ArgumentNullException(nameof(service));

        var type = service.GetType();
        if (!_services.TryAdd(type, service))
            throw new InvalidOperationException($"Service of type {type.FullName} is already registered.");
    }

    /// <inheritdoc />
    public ICollection<INucleiService> GetAll()
    {
        return _services.Values;
    }

    /// <inheritdoc />
    public T Get<T>() where T : class, INucleiService
    {
        return TryGet<T>(out var service)
            ? service
            : throw new KeyNotFoundException($"Service of type {typeof(T).FullName} is not registered.");
    }

    /// <inheritdoc />
    public bool TryGet<T>(out T service) where T : class, INucleiService
    {
        var type = typeof(T);
        if (_services.TryGetValue(type, out var foundService))
        {
            service = (foundService as T)!;
            return true;
        }

        service = null!;
        return false;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        foreach (var service in _services.Values)
            service.Dispose();
        _services.Clear();
    }
}
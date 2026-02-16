using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Nuclei.Abstractions.Nuclei;

/// <summary>
///     Declares service dependencies for initialization ordering.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class DependsOnAttribute : Attribute
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="DependsOnAttribute" /> class with the specified dependencies.
    /// </summary>
    /// <param name="dependencies"> The types of services that the annotated service depends on. </param>
    public DependsOnAttribute(params Type[] dependencies)
    {
        Dependencies = dependencies;
    }

    /// <summary>
    ///     Gets the types of services that the annotated service depends on.
    /// </summary>
    // ReSharper disable once MemberCanBePrivate.Global
    public Type[] Dependencies { get; }

    /// <summary>
    ///     Orders the given services based on their declared dependencies.
    /// </summary>
    /// <param name="services"> The services to order. </param>
    /// <returns> The given services ordered such that each service appears after all of its dependencies. </returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if a service declares a dependency that is not present in the given
    ///     list, or if a dependency cycle is detected.
    /// </exception>
    public static IReadOnlyList<INucleiService> OrderServices(IReadOnlyList<INucleiService> services)
    {
        var serviceMap = services.ToDictionary(service => service.GetType(), service => service);
        var dependents = new Dictionary<Type, List<Type>>();
        var dependencyCount = new Dictionary<Type, int>();

        foreach (var service in services)
        {
            var type = service.GetType();
            dependencyCount[type] = 0;
            dependents[type] = [];
        }

        foreach (var service in services)
        {
            var serviceType = service.GetType();
            if (serviceType.GetCustomAttribute(typeof(DependsOnAttribute), true) is not DependsOnAttribute attribute)
                continue;

            foreach (var dependency in attribute.Dependencies)
            {
                if (!serviceMap.ContainsKey(dependency))
                    throw new InvalidOperationException(
                        $"Service dependency '{dependency.FullName}' is not registered for '{serviceType.FullName}'.");

                dependents[dependency].Add(serviceType);
                dependencyCount[serviceType]++;
            }
        }

        var ready = dependencyCount
            .Where(pair => pair.Value == 0)
            .Select(pair => pair.Key)
            .OrderBy(type => type.FullName, StringComparer.Ordinal)
            .ToList();

        var ordered = new List<INucleiService>(services.Count);
        while (ready.Count > 0)
        {
            var next = ready[0];
            ready.RemoveAt(0);
            ordered.Add(serviceMap[next]);

            foreach (var dependent in dependents[next])
            {
                dependencyCount[dependent]--;
                if (dependencyCount[dependent] != 0)
                    continue;

                ready.Add(dependent);
                ready.Sort((left, right) => StringComparer.Ordinal.Compare(left.FullName, right.FullName));
            }
        }

        if (ordered.Count == services.Count)
            return ordered;

        var remaining = string.Join(", ", dependencyCount.Where(pair => pair.Value > 0).Select(pair => pair.Key.FullName));
        throw new InvalidOperationException($"Service dependency cycle detected: {remaining}");
    }
}
using System;

namespace Nuclei.Abstractions.Nuclei;

/// <summary>
///     Interface for Nuclei services. Services are components that handle & expose specific functionality to the Nuclei
///     ecosystem.
/// </summary>
public interface INucleiService : IDisposable
{
    /// <summary>
    ///     Initializes the service with the provided context.
    /// </summary>
    /// <param name="context">
    ///     The context to initialize the service with.
    /// </param>
    void Initialize(INucleiContext context);
}
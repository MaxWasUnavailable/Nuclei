using Nuclei.Abstractions.NO.Units;

namespace Nuclei.Adapters.Units;

/// <summary>
///     Resolves unit abstractions from identifiers.
/// </summary>
public interface IUnitLookup
{
    /// <summary>
    ///     Resolves a unit abstraction from an identifier.
    /// </summary>
    /// <param name="id">The persistent unit identifier.</param>
    /// <returns>The unit abstraction.</returns>
    IUnitInfo FromPersistentId(PersistentID id);
}


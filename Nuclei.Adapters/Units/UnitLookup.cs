using Nuclei.Abstractions.Units;

namespace Nuclei.Adapters.Units;

/// <summary>
///     Default adapter for resolving unit info using the game registry.
/// </summary>
public sealed class UnitLookup : IUnitLookup
{
    /// <inheritdoc />
    public IUnitInfo FromPersistentId(PersistentID id)
    {
        if (!UnitRegistry.TryGetUnit(id, out var unit) || !unit)
            return new UnitInfo(id.ToString(), string.Empty);

        var name = unit.unitName ?? unit.name;
        return new UnitInfo(id.ToString(), name);

    }
}


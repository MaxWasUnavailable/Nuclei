namespace Nuclei.Abstractions.Zones;

/// <summary>
///     Defines an exclusion zone.
/// </summary>
public interface IExclusionZoneInfo
{
    /// <summary>
    ///     The identifier of the source unit.
    /// </summary>
    string SourceId { get; }
}


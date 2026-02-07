namespace Nuclei.Abstractions.Zones;

/// <summary>
///     Defines an exclusion zone.
/// </summary>
/// <param name="SourceId">The identifier of the source unit.</param>
public sealed record ExclusionZoneInfo(string SourceId) : IExclusionZoneInfo;

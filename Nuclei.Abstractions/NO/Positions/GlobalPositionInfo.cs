namespace Nuclei.Abstractions.NO.Positions;

/// <summary>
///     Minimal global position payload.
/// </summary>
/// <param name="X">X coordinate.</param>
/// <param name="Y">Y coordinate.</param>
/// <param name="Z">Z coordinate.</param>
public sealed record GlobalPositionInfo(float X, float Y, float Z) : IGlobalPosition;


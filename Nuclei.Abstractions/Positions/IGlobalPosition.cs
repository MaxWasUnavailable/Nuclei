namespace Nuclei.Abstractions.Positions;

/// <summary>
///     Defines a world-space position in global coordinates.
/// </summary>
public interface IGlobalPosition
{
    /// <summary>
    ///     X coordinate.
    /// </summary>
    float X { get; }

    /// <summary>
    ///     Y coordinate.
    /// </summary>
    float Y { get; }

    /// <summary>
    ///     Z coordinate.
    /// </summary>
    float Z { get; }
}


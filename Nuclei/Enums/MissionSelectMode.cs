namespace Nuclei.Enums;

/// <summary>
///     The mode for selecting a mission.
/// </summary>
public enum MissionSelectMode
{
    /// <summary>
    ///     Select a mission at random.
    /// </summary>
    Random = 0,

    /// <summary>
    ///     Select a mission at random, but do not repeat the same mission twice in a row.
    /// </summary>
    RandomNoRepeat = 1,

    /// <summary>
    ///     Select the next mission in the list and loop back to the start if the end is reached.
    /// </summary>
    Sequential = 2
}
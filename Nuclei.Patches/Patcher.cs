using HarmonyLib;

namespace Nuclei.Patches;

/// <summary>
///     The main entry point for the patching process.
/// </summary>
internal static class Patcher
{
    /// <summary>
    ///     Applies all patches in the assembly.
    /// </summary>
    public static void ApplyPatches(Harmony harmony)
    {
        harmony.PatchAll();
    }
}
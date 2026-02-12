using System;
using System.Linq;
using HarmonyLib;
using Nuclei.Abstractions.BepInEx.Logging;
using Nuclei.Abstractions.Nuclei.Decorators;

namespace Nuclei.Patches;

/// <summary>
///     The main entry point for the patching process.
/// </summary>
internal static class Patcher
{
    /// <summary>
    ///     Applies all patches in the assembly.
    /// </summary>
    public static void ApplyPatches(Harmony harmony, ILogger logger)
    {
        var patchLogger = logger.WithScope(nameof(Patcher));

        var patchTypes = typeof(Patcher).Assembly
            .GetTypes()
            .Where(t => t.GetCustomAttributes(typeof(HarmonyPatch), true).Length > 0)
            .ToArray();

        var errorCount = 0;

        foreach (var patchType in patchTypes)
            try
            {
                var processor = new PatchClassProcessor(harmony, patchType);
                processor.Patch();
                patchLogger.Debug($"Successfully applied Harmony patches in: {patchType.FullName}");
            }
            catch (Exception e)
            {
                errorCount++;
                patchLogger.Warn($"Failed to apply Harmony patch: {patchType.FullName}");
                patchLogger.Error("Harmony patch failure", e);
            }

        if (errorCount > 0)
            throw new Exception($"Failed to apply {errorCount} Harmony patches.");
    }
}
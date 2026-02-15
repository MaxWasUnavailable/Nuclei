using System;
using BepInEx;
using HarmonyLib;
using Nuclei.Abstractions.Nuclei;
using Nuclei.Abstractions.Nuclei.Decorators;
using Nuclei.Core;
using Nuclei.Patches;
using Nuclei.Plugin.Config;
using Nuclei.Plugin.Logging;
using ILogger = Nuclei.Abstractions.BepInEx.Logging.ILogger;

namespace Nuclei.Plugin;

/// <summary>
///     Main plugin class for Nuclei.
/// </summary>
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Nuclei : BaseUnityPlugin
{
    internal static Nuclei? Instance { get; private set; }

    private static Harmony? Harmony { get; set; }
    private static bool IsHarmonyPatched { get; set; }

    private ILogger _instanceLogger = null!;

    private INucleiContext? _nucleiContext;

    private void Awake()
    {
        DontDestroyOnLoad(this);

        var bepInExLogger = new BepInExLoggerAdapter(Logger);

        _instanceLogger = bepInExLogger.WithTimestamp().WithScope(nameof(Nuclei));

        _instanceLogger.Info($"Loading {MyPluginInfo.PLUGIN_NAME} v{MyPluginInfo.PLUGIN_VERSION}...");

        try
        {
            _nucleiContext = CoreBootstrap.Initialize(bepInExLogger, new BepInExConfigProvider(Config));
        }
        catch (Exception e)
        {
            _instanceLogger.Error("Nuclei failed to initialize correctly. For more information, see this error trace:", e);
            return;
        }

        PatchAll(bepInExLogger);

        if (IsHarmonyPatched)
            _instanceLogger.Info($"Plugin {MyPluginInfo.PLUGIN_GUID} loaded successfully.");
        else
            _instanceLogger.Error($"Plugin {MyPluginInfo.PLUGIN_GUID} loaded with patching errors. Nuclei might not work properly.");
    }

    private void Start()
    {
        Instance = Info.Instance as Nuclei;
        _instanceLogger.Debug("Nuclei instance is set!");
    }

    private void PatchAll(ILogger logger)
    {
        if (IsHarmonyPatched)
        {
            _instanceLogger.Warn("Already patched!");
            return;
        }

        _instanceLogger.Debug("Patching...");

        Harmony ??= new Harmony(MyPluginInfo.PLUGIN_GUID);

        try
        {
            Harmony.PatchAll();
            Patcher.ApplyPatches(Harmony, logger);

            _instanceLogger.Debug("Finished patching!");
        }
        catch (Exception e)
        {
            _instanceLogger.Error("Failed to apply all Harmony patches. Nuclei might not work properly. This could be the result of a game update. For more information, see this error trace:", e);
        }

        IsHarmonyPatched = true;
    }

    private void UnpatchSelf()
    {
        if (Harmony == null)
        {
            _instanceLogger.Error("Harmony instance is null!");
            return;
        }

        if (!IsHarmonyPatched)
        {
            _instanceLogger.Warn("Already unpatched!");
            return;
        }

        _instanceLogger.Debug("Unpatching...");

        Harmony.UnpatchSelf();
        IsHarmonyPatched = false;

        _instanceLogger.Debug("Unpatched!");
    }

    private void OnDestroy()
    {
        _instanceLogger.Warn("Nuclei is being destroyed. Unpatching Harmony patches...");

        UnpatchSelf();
    }
}
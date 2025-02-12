using System;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nuclei.Features;
using Nuclei.Features.Commands;
using Nuclei.Features.Commands.DefaultCommands;
using UnityEngine;

namespace Nuclei;

/// <summary>
///     Main plugin class for Nuclei.
/// </summary>
[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class Nuclei : BaseUnityPlugin
{
    internal static Nuclei? Instance { get; private set; }
    internal new static ManualLogSource? Logger { get; private set; }
    private static Harmony? Harmony { get; set; }
    private static bool IsPatched { get; set; }
    
    private static bool IsServerDedicated()
    {
        return Application.isBatchMode;
    }

    private void Awake()
    {
        Instance = this;
        
        Logger = base.Logger;
        
        Logger?.LogInfo($"Loading {PluginInfo.PLUGIN_NAME} v{PluginInfo.PLUGIN_VERSION}...");
        
        if (!IsServerDedicated())
        {
            Logger?.LogError("This plugin is intended for dedicated servers only! Aborting server initialisation. To run a dedicated server, use the -batchmode and -nographics command line arguments. If you're running the game as a player, you can safely ignore this message.");
            return;
        }

        try
        {
            NucleiConfig.InitSettings(Config);
            NucleiConfig.ValidateSettings();
        }
        catch (ArgumentException e)
        {
            Logger?.LogError(
                $"Aborting server launch: Failed to load or validate settings. One of the settings might be the wrong type of value. For more information, see this error trace:\n{e}");
            return;
        }
        catch (Exception e)
        {
            Logger?.LogError($"Aborting server launch: Failed to load or validate settings. For more information, see this error trace:\n{e}");
            return;
        }

        PatchAll();
        
        CommandService.RegisterCommand(new SayCommand(Config));
        CommandService.RegisterCommand(new NewMissionCommand(Config));
        CommandService.RegisterCommand(new KickCommand(Config));

        if (IsPatched)
            Logger?.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        else
            Logger?.LogError($"Plugin {PluginInfo.PLUGIN_GUID} failed to load correctly!");
    }

    private static void PatchAll()
    {
        if (IsPatched)
        {
            Logger?.LogWarning("Already patched!");
            return;
        }

        Logger?.LogDebug("Patching...");

        Harmony ??= new Harmony(PluginInfo.PLUGIN_GUID);

        try
        {
            Harmony.PatchAll();
            IsPatched = true;
            Logger?.LogDebug("Patched!");
        }
        catch (Exception e)
        {
            Logger?.LogError($"Aborting server launch: Failed to Harmony patch the game. For more information, see this error trace:\n{e}");
        }
    }

    private void UnpatchSelf()
    {
        if (Harmony == null)
        {
            Logger?.LogError("Harmony instance is null!");
            return;
        }
        
        if (!IsPatched)
        {
            Logger?.LogWarning("Already unpatched!");
            return;
        }

        Logger?.LogDebug("Unpatching...");

        Harmony?.UnpatchSelf();
        IsPatched = false;

        Logger?.LogDebug("Unpatched!");
    }
}
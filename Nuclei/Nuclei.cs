using System;
using System.Linq;
using System.Threading;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using NuclearOption.Networking;
using Nuclei.Events;
using Nuclei.Features;
using Nuclei.Features.Commands;
using Nuclei.Features.Commands.DefaultCommands;
using Nuclei.Helpers;
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
    private ConsoleManager? _console;
    
    private void Awake()
    {
        Instance = this;
        
        Logger = base.Logger;
        var unityCtx = SynchronizationContext.Current ?? new SynchronizationContext(); 
        _console = new ConsoleManager(unityCtx, HandleConsoleCommand);
        _console.Start();
        
        Logger?.LogInfo($"Loading {PluginInfo.PLUGIN_NAME} v{PluginInfo.PLUGIN_VERSION}...");
        

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
        CommandService.RegisterCommand(new BanCommand(Config));
        CommandService.RegisterCommand(new StopCommand(Config));
        CommandService.RegisterCommand(new SetPermissionLevelCommand(Config));
        CommandService.RegisterCommand(new HelpCommand(Config));
        CommandService.RegisterCommand(new NextMissionCommand(Config));
        CommandService.RegisterCommand(new BanSteamIDCommand(Config));
        CommandService.RegisterCommand(new ListCommand(Config));

        PlayerEvents.PlayerJoined += OnPlayerJoin;

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
    
    // TODO: Move these somewhere else?
    private static void HandleConsoleCommand(string rawLine, string[] args)
    {
        Logger?.LogInfo($"> {rawLine}");
        if (args.Length == 0) return;

        var cmd = args[0].ToLowerInvariant();
        CommandService.TryExecuteCommand(cmd, args.Skip(1).ToArray());
    }

    private void OnPlayerJoin(Player player)
    {
        BanService.VerifyNotBanned(player);
        PlayerUtils.ApplyOrRemoveStaffTag(player);
    }
}
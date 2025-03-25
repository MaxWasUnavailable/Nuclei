using Cysharp.Threading.Tasks;
using Nuclei.Helpers;
using Steamworks;
using UnityEngine;

namespace Nuclei.Features;

/// <summary>
///     Steam lobby wrapper service for Nuclei.
/// </summary>
public static class SteamLobbyService
{
    private const string KeyHostAddress = "HostAddress";
    private const string KeyName = "name";
    private const string KeyVersion = "version";
    private const string KeyHostPing = "HostPing";

    /// <summary>
    ///     Starts a Steam lobby.
    /// </summary>
    public static async UniTask StartSteamLobby()
    {
        Nuclei.Logger?.LogInfo("Starting Steam lobby...");

        var result = await Globals.SteamLobbyInstance.Steam_CreateLobbyAsync(NucleiConfig.LobbyType!.Value,
            NucleiConfig.MaxPlayers!.Value);
        
        if (result.m_eResult != EResult.k_EResultOK)
        {
            Nuclei.Logger?.LogError("Failed to create Steam lobby.");
            return;
        }
        
        Nuclei.Logger?.LogInfo("Steam lobby started.");
    }
    internal static void SetLobbyData()
    {
        SteamMatchmaking.SetLobbyData(Globals.LobbySteamID, KeyHostAddress, SteamUser.GetSteamID().ToString());
        SteamMatchmaking.SetLobbyData(Globals.LobbySteamID, KeyVersion, Application.version);
        UpdateLobbyName();
    }

    /// <summary>
    ///     Updates the lobby name.
    /// </summary>
    /// <remarks> It is useful to re-run this periodically in case of dynamic placeholders. </remarks>
    public static void UpdateLobbyName()
    {
        SteamMatchmaking.SetLobbyData(Globals.LobbySteamID, KeyName, DynamicPlaceholderUtils.ReplaceDynamicPlaceholders(NucleiConfig.ServerName!.Value));
    }

    internal static async UniTask SetPingData()
    {
        if (!await Globals.SteamLobbyInstance.WaitForLocalLocation())
        {
            Nuclei.Logger?.LogError("Failed to await Steam Lobby local location fetch for ping data. Ping will be empty.");
            return;
        }
        if (SteamLobby.GetLocalLocation(out var location))
            SteamMatchmaking.SetLobbyData(Globals.LobbySteamID, KeyHostPing, location);
        else
            Nuclei.Logger?.LogError("Failed to get local location for ping data. Ping will be empty.");
    }
    
    /// <summary>
    ///     Stops the Steam lobby.
    /// </summary>
    public static void StopSteamLobby()
    {
        Nuclei.Logger?.LogDebug("Stopping Steam lobby...");
        SteamMatchmaking.LeaveLobby(Globals.LobbySteamID);
        Nuclei.Logger?.LogDebug("Steam lobby stopped.");
    }
    
    /// <summary>
    ///     Checks if the Steam API is available.
    /// </summary>
    /// <returns> True if the Steam API is available, false otherwise. </returns>
    public static bool IsSteamAPIAvailable()
    {
        return SteamAPI.IsSteamRunning();
    }
    
}
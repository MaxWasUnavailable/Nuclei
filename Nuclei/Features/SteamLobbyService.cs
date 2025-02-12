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
        SteamMatchmaking.SetLobbyData(Globals.LobbySteamID, KeyName, NucleiConfig.ServerName!.Value);
        SteamMatchmaking.SetLobbyData(Globals.LobbySteamID, KeyVersion, Application.version);
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
    
}
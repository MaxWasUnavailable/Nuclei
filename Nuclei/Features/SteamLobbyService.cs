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

    private static CSteamID? _lobbyId;

    /// <summary>
    ///     Starts a Steam lobby.
    /// </summary>
    public static async UniTask StartSteamLobby()
    {
        Nuclei.Logger?.LogInfo("Starting Steam lobby...");

        var result = await Globals.SteamLobbyInstance.Steam_CreateLobbyAsync(Nuclei.Instance!.LobbyType!.Value,
            Nuclei.Instance!.MaxPlayers!.Value);
        
        if (result.m_eResult != EResult.k_EResultOK)
        {
            Nuclei.Logger?.LogError("Failed to create Steam lobby.");
            return;
        }
        
        _lobbyId = new CSteamID(result.m_ulSteamIDLobby);
        
        SetLobbyData();
        
        Nuclei.Logger?.LogInfo("Steam lobby started.");
    }

    private static void SetLobbyData()
    {
        SteamMatchmaking.SetLobbyData(_lobbyId!.Value, KeyHostAddress, SteamUser.GetSteamID().ToString());
        SteamMatchmaking.SetLobbyData(_lobbyId!.Value, KeyName, Nuclei.Instance!.ServerName!.Value);
        SteamMatchmaking.SetLobbyData(_lobbyId!.Value, KeyVersion, Application.version);
    }
    
}
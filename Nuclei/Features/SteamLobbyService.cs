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
        
        SetLobbyData(new CSteamID(result.m_ulSteamIDLobby));
        
        Nuclei.Logger?.LogInfo("Steam lobby started.");
    }

    internal static void SetLobbyData(CSteamID lobbyId)
    {
        SteamMatchmaking.SetLobbyData(lobbyId, KeyHostAddress, SteamUser.GetSteamID().ToString());
        SteamMatchmaking.SetLobbyData(lobbyId, KeyName, NucleiConfig.ServerName!.Value);
        SteamMatchmaking.SetLobbyData(lobbyId, KeyVersion, Application.version);
    }
    
}
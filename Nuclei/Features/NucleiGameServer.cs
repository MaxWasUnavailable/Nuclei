using Steamworks;
using UnityEngine;

namespace Nuclei.Features;

/// <summary>
///     Steam Game Server for Nuclei.
/// </summary>
public class NucleiGameServer : MonoBehaviour
{
    /// <summary>
    ///     The current version of the server.
    /// </summary>
    public static string Version => Application.version;

    /// <summary>
    ///     Primary server port.
    /// </summary>
    public static ushort GamePort => 27015;

    /// <summary>
    ///     Query port.
    /// </summary>
    public static ushort QueryPort => 27016;
    
    protected Callback<SteamServersConnected_t> m_CallbackSteamServersConnected;

    private bool m_bInitialized;
    private bool m_bConnectedToSteam;

    private void OnSteamServersConnected(SteamServersConnected_t pLogonSuccess) {
        Debug.Log("SpaceWarServer connected to Steam successfully");
        m_bConnectedToSteam = true;

        // log on is not finished until OnPolicyResponse() is called

        // Tell Steam about our server details
        SendUpdatedServerDetailsToSteam();
    }

    private void OnEnable()
    {
        m_CallbackSteamServersConnected = Callback<SteamServersConnected_t>.CreateGameServer(OnSteamServersConnected);

        m_bInitialized = false;
        m_bConnectedToSteam = false;

        m_bInitialized = GameServer.Init(0, GamePort, QueryPort, EServerMode.eServerModeAuthenticationAndSecure, Version);
        if (!m_bInitialized)
        {
            Nuclei.Logger?.LogError("SteamGameServer_Init call failed");
            return;
        }

        SteamGameServer.LogOnAnonymous();

        Nuclei.Logger?.LogInfo("SteamGameServer_Init success");
    }
    
    private void OnDisable() {
        if(!m_bInitialized) 
            return;

        m_CallbackSteamServersConnected.Dispose();
        SteamGameServer.LogOff();
        
        GameServer.Shutdown();
        m_bInitialized = false;
        
        Nuclei.Logger?.LogInfo("SteamGameServer_Shutdown");
    }
    
    private void Update() {
        if(!m_bInitialized) 
            return;

        GameServer.RunCallbacks();

        if(m_bConnectedToSteam) 
            SendUpdatedServerDetailsToSteam();
    }
    
    private const string KeyHostAddress = "HostAddress";
    private const string KeyName = "name";
    private const string KeyVersion = "version";
    private const string KeyHostPing = "HostPing";
    
    void SendUpdatedServerDetailsToSteam() {
        SteamGameServer.SetMaxPlayerCount(NucleiConfig.MaxPlayers!.Value);
        SteamGameServer.SetPasswordProtected(false);
        SteamGameServer.SetServerName(NucleiConfig.ServerName!.Value);
        
        SteamGameServer.SetKeyValue(KeyHostAddress, SteamGameServer.GetSteamID().ToString());
        SteamGameServer.SetKeyValue(KeyName, NucleiConfig.ServerName!.Value);
        SteamGameServer.SetKeyValue(KeyVersion, Version);
    }
}
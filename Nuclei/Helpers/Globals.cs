using System;
using NuclearOption.Networking;
using Steamworks;

namespace Nuclei.Helpers;

/// <summary>
///     Has property getters for various Nuclear Option values that are used throughout the mod.
/// </summary>
public static class Globals
{
    /// <summary>
    ///     Gets the instance of the <see cref="NetworkManagerNuclearOption" /> class.
    /// </summary>
    public static NetworkManagerNuclearOption NetworkManagerNuclearOptionInstance => NetworkManagerNuclearOption.i ?? throw new NullReferenceException("NetworkManagerNuclearOption instance is null.");

    /// <summary>
    ///     Gets the instance of the <see cref="MissionManager" /> class.
    /// </summary>
    public static MissionManager MissionManagerInstance => MissionManager.i ?? throw new NullReferenceException("MissionManager instance is null.");

    /// <summary>
    ///     Gets the instance of the <see cref="ChatManager" /> class.
    /// </summary>
    public static ChatManager ChatManagerInstance => ChatManager.i ?? throw new NullReferenceException("ChatManager instance is null.");

    /// <summary>
    ///     Gets the instance of the <see cref="SteamLobby" /> class.
    /// </summary>
    public static SteamLobby SteamLobbyInstance => SteamLobby.instance ?? throw new NullReferenceException("SteamLobby instance is null.");

    /// <summary>
    ///     Gets the local player. (The host / server)
    /// </summary>
    public static Player LocalPlayer => GameManager.LocalPlayer ?? throw new NullReferenceException("Local player is null.");

    /// <summary>
    ///     Gets the instance of the <see cref="AudioMixerVolume" /> class.
    /// </summary>
    public static AudioMixerVolume AudioMixerVolumeInstance => AudioMixerVolume.i ?? throw new NullReferenceException("AudioMixerVolume instance is null.");

    /// <summary>
    ///     Gets the name of the local player. (The host / server)
    /// </summary>
    public static string HostName => LocalPlayer.PlayerName;

    /// <summary>
    ///     Gets the Steam ID of the lobby.
    /// </summary>
    public static CSteamID LobbySteamID => SteamLobbyInstance._currentLobbyID;
}
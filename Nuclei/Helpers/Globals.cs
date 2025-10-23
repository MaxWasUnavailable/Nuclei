using System;
using System.Collections.Generic;
using Mirage;
using NuclearOption.Networking;

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
    ///     Gets the instance of the <see cref="AudioMixerVolume" /> class.
    /// </summary>
    public static AudioMixerVolume AudioMixerVolumeInstance => SoundManager.i.Volumes ?? throw new NullReferenceException("SoundManager AudioMixerVolume instance is null.");


    /// <summary>
    ///     Get a read-only list of all authenticated players.
    /// </summary>
    public static IReadOnlyList<INetworkPlayer> AuthenticatedPlayers => NetworkManagerNuclearOptionInstance.Server.AuthenticatedPlayers;
}
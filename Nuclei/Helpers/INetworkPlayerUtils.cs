using Mirage;
using Steamworks;

namespace Nuclei.Helpers;

/// <summary>
///     Helper class for INetworkPlayer-related operations.
/// </summary>
// ReSharper disable once InconsistentNaming
public static class INetworkPlayerUtils
{
    /// <summary>
    ///     Get the Steam ID of an INetworkPlayer object.
    /// </summary>
    /// <param name="networkPlayer"> The INetworkPlayer object. </param>
    /// <returns> The Steam ID of the player. </returns>
    public static CSteamID GetSteamID(this INetworkPlayer networkPlayer)
    {
        return Globals.NetworkManagerNuclearOptionInstance.authenticator.GetSteamId(networkPlayer);
    }

    /// <summary>
    ///     Get the Steam ID of an INetworkPlayer object as a ulong.
    /// </summary>
    /// <param name="networkPlayer"> The INetworkPlayer object. </param>
    /// <returns> The Steam ID of the player as a ulong. </returns>
    public static ulong GetSteamIDUlong(this INetworkPlayer networkPlayer)
    {
        return networkPlayer.GetSteamID().m_SteamID;
    }
}
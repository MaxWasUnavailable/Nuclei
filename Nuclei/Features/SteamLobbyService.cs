using Cysharp.Threading.Tasks;

namespace Nuclei.Features;

/// <summary>
///     Compatibility shim. Steam server advertisement is handled by the official dedicated server.
/// </summary>
public static class SteamLobbyService
{
    public static UniTask StartSteamLobby()
    {
        Nuclei.Logger?.LogDebug("Skipping Nuclei Steam lobby creation; official dedicated server owns advertisement.");
        return UniTask.CompletedTask;
    }

    internal static void SetLobbyData()
    {
        Nuclei.Logger?.LogDebug("Skipping Nuclei Steam lobby data update; official dedicated server owns advertisement.");
    }

    public static void UpdateLobbyName()
    {
        Nuclei.Logger?.LogDebug("Skipping Nuclei Steam lobby name update; official dedicated server owns advertisement.");
    }

    internal static UniTask SetPingData()
    {
        Nuclei.Logger?.LogDebug("Skipping Nuclei Steam lobby ping data update; official dedicated server owns advertisement.");
        return UniTask.CompletedTask;
    }

    public static void StopSteamLobby()
    {
        Nuclei.Logger?.LogDebug("Skipping Nuclei Steam lobby shutdown; official dedicated server owns advertisement.");
    }

    public static bool IsSteamAPIAvailable()
    {
        return true;
    }
}

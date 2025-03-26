using HarmonyLib;
using Mirage;
using Mirage.Authentication;
using Nuclei.Events;
using Nuclei.Features;
using Nuclei.Helpers;

namespace Nuclei.Patches;

[HarmonyPatch(typeof(NetworkServer))]
[HarmonyPriority(Priority.First)]
[HarmonyWrapSafe]
internal static class NetworkServerPatches
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(NetworkServer.StartServer))]
    private static void StartServerPrefix(ref NetworkClient localClient)
    {
        localClient.RunInBackground = true;
    }
    
    [HarmonyPostfix]
    [HarmonyPatch(nameof(NetworkServer.StartServer))]
    private static void StartServerPostfix()
    {
        ServerEvents.OnServerStarted();
    }
    
    [HarmonyPostfix]
    [HarmonyPatch(nameof(NetworkServer.Stop))]
    private static void StopPostfix()
    {
        ServerEvents.OnServerStopped();
    }
    
    [HarmonyPrefix]
    [HarmonyPatch(nameof(NetworkServer.AuthenticationSuccess))]
    private static void AuthenticationSuccessPrefix(ref INetworkPlayer player, AuthenticationResult result)
    {
        var steamId = player.GetSteamIDUlong();
        
        if (!NucleiConfig.IsBanned(steamId)) 
            return;

        Nuclei.Logger?.LogInfo($"Player with Steam ID {steamId} tried to join the game but is banned.");
        player.Disconnect();
    }
}
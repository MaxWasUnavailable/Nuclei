using HarmonyLib;
using Mirage;
using NuclearOption.Networking;
using Nuclei.Features;

namespace Nuclei.Patches;

// TODO: find way to display proper player name
[HarmonyPatch(typeof(NetworkManagerNuclearOption))]
[HarmonyPriority(Priority.First)]
[HarmonyWrapSafe]
internal static class NetworkManagerNuclearOptionPatches
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(NetworkManagerNuclearOption.LogServerConnected))]
    private static void LogServerConnectedPrefix(INetworkPlayer player)
    {
        Nuclei.Logger?.LogInfo($"Player {player.Address} connecting to the server...");
    }
    
    [HarmonyPrefix]
    [HarmonyPatch(nameof(NetworkManagerNuclearOption.LogServerDisconnected))]
    private static void LogServerDisconnectedPrefix(INetworkPlayer player)
    {
        Nuclei.Logger?.LogInfo($"Player {player.Address} disconnecting from the server...");
    }
    
    [HarmonyPostfix]
    [HarmonyPatch(nameof(NetworkManagerNuclearOption.LogServerConnected))]
    private static void LogServerConnectedPostfix(INetworkPlayer player)
    {
        Nuclei.Logger?.LogInfo($"Player {player.Address} connected to the server.");
        ChatService.SendChatMessage(Nuclei.Instance!.ServerMessageOfTheDay!.Value);
    }
    
    [HarmonyPostfix]
    [HarmonyPatch(nameof(NetworkManagerNuclearOption.LogServerDisconnected))]
    private static void LogServerDisconnectedPostfix(INetworkPlayer player)
    {
        Nuclei.Logger?.LogInfo($"Player {player.Address} disconnected from the server.");
    }
    
    [HarmonyPrefix]
    [HarmonyPatch(nameof(NetworkManagerNuclearOption.OnClientAuthenticated))]
    private static void OnClientAuthenticatedPrefix(INetworkPlayer player)
    {
        Nuclei.Logger?.LogInfo($"Player {player.Address} authenticating.");
        if (player.IsHost) 
            Nuclei.Logger?.LogInfo($"Player {player.Address} is the host.");
    }
    
    [HarmonyPrefix]
    [HarmonyPatch(nameof(NetworkManagerNuclearOption.LogClientConnected))]
    private static void LogClientConnectedPrefix(INetworkPlayer player)
    {
        Nuclei.Logger?.LogInfo($"Client {player.Address} connecting to the server...");
    }
    
    [HarmonyPostfix]
    [HarmonyPatch(nameof(NetworkManagerNuclearOption.LogClientConnected))]
    private static void LogClientConnectedPostfix(INetworkPlayer player)
    {
        Nuclei.Logger?.LogInfo($"Client {player.Address} connected to the server.");
    }
}
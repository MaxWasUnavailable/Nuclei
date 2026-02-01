using BepInEx.Configuration;
using Mirage;
using NuclearOption.Networking;
using Nuclei.Enums;
using Nuclei.Helpers;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace Nuclei.Features.Commands.DefaultCommands;

/// <summary>
///     Command to list currently connected players.    
/// </summary>
public class ListCommand(ConfigFile config) : PermissionConfigurableCommand(config)
{
    public override string Name { get; } = "list";
    public override string Description { get; } = "List all connected players.";
    public override string Usage { get; } = "list";

    public override bool Validate(Player player, string[] args)
    {
        return true;
    }

    public override bool Execute(Player player, string[] args)
    {
        var players = Globals.AuthenticatedPlayers;
        var playerNames = "";
        foreach (INetworkPlayer item in players)
        {
            Player p;
            item.TryGetPlayer(out p);
            if (p != null)
            {
                playerNames += $"{p.PlayerName}, ";
            }
        }
        string finalMessage = $"[{players.Count - 1}] ";
        finalMessage += playerNames;
        ChatService.SendPrivateChatMessage($"{finalMessage}", player);
        return true;
    }

    public override bool Execute(string[] args)
    {
        var players = Globals.AuthenticatedPlayers;
        var playerNames = "";
        foreach (INetworkPlayer item in players)
        {
            Player p;
            item.TryGetPlayer(out p);
            if (p != null)
            {
                playerNames += $"{p.PlayerName}, ";
            }
        }
        string finalMessage = $"[{players.Count - 1}] ";
        finalMessage += playerNames;
        Nuclei.Logger?.LogInfo($"{finalMessage}");
        return true;
    }

    public override PermissionLevel DefaultPermissionLevel { get; } = PermissionLevel.Everyone;
}
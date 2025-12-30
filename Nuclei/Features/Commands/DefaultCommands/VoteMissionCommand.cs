using System;
using System.Collections.Generic;
using System.Text;
using BepInEx.Configuration;
using NuclearOption.DedicatedServer;
using NuclearOption.Networking;
using Nuclei.Enums;
using Nuclei.Helpers;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace Nuclei.Features.Commands.DefaultCommands;

public class VoteMissionCommand(ConfigFile config) : PermissionConfigurableCommand(config)
{
    private static List<MissionOptions>? _fetchedMissions;
    public override string Name { get; } = "votemission";
    public override string Description { get; } = "lets you vote the next mission";
    public override string Usage { get; } = "votemission to get list of missions. votemission <number> to start a vote for that mission";
    public override PermissionLevel DefaultPermissionLevel { get; } = PermissionLevel.Everyone;
    
    public override bool Validate(Player player, string[] args)
    {
        if (args.Length >= 1) return true;
        
        var sb = new StringBuilder();
        sb.AppendLine("Choose from the following missions: ");
        _fetchedMissions = Globals.DedicatedServerManagerInstance.missionRotation.allMissions;

        // Get missions
        for (int i = 0; i < _fetchedMissions.Count; i++)
        {
            sb.AppendLine($"{(i + 1).ToString()}: {_fetchedMissions[i].Key.Name}");
        }
        ChatService.SendPrivateChatMessage(sb.ToString(), player);
        return false;
    }

    public override bool Execute(Player player, string[] args)
    {
        try
        {
            if (_fetchedMissions == null)
            {
                ChatService.SendPrivateChatMessage("Please use votemission without arguments to fetch the mission list first.", player);
                return false;
            }

            int i = int.Parse(args[0]);
            if (i <= 0 || i >= _fetchedMissions.Count) throw new FormatException();

            Action a = () => Globals.DedicatedServerManagerInstance.missionRotation.OverrideNext(_fetchedMissions[i]);

            if (!VoteService.StartVote(player, $"Mission vote for {_fetchedMissions[i].Key} has been started", a))
            {
                ChatService.SendPrivateChatMessage("Cannot start a new mission vote, please wait for current vote to expire.", player);
                return false;
            }
            return true;
        }
        catch (FormatException)
        {
            ChatService.SendPrivateChatMessage("That was not a valid number. Please try again.", player);
            return false;
        }
    }

    public override bool Execute(string[] args)
    {
        throw new System.Exception("Requires Player object");
    }
}
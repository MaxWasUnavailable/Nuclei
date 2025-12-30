

using System;
using System.Collections.Generic;
using BepInEx.Configuration;
using NuclearOption.DedicatedServer;
using NuclearOption.Networking;
using Nuclei.Enums;
using Nuclei.Helpers;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace Nuclei.Features.Commands.DefaultCommands;

public class VoteSkipCommand(ConfigFile config) : PermissionConfigurableCommand(config)
{
    private static List<MissionOptions>? _fetchedMissions;
    public override string Name { get; } = "voteskip";
    public override string Description { get; } = "Let you skip this mission by voting";
    public override string Usage { get; } = "voteskip to initiate a vote to skip the current mission";
    public override PermissionLevel DefaultPermissionLevel { get; } = PermissionLevel.Everyone;
    
    public override bool Validate(Player player, string[] args)
    {
        if (args.Length > 0) ChatService.SendPrivateChatMessage("Arguments not needed for this command. Voting anyway...", player);
        return true;
    }

    public override bool Execute(Player player, string[] args)
    {
        Action a = () => MissionService.StartNextMission(player);

        if (!VoteService.StartVote(player, $"A vote to skip the current mission has been started", a))
        {
            ChatService.SendPrivateChatMessage("Cannot start a new mission skip vote, please wait for current vote to expire.", player);
            return false;
        }
        return true;
    }

    public override bool Execute(string[] args)
    {
        throw new System.Exception("Requires Player object");
    }
}
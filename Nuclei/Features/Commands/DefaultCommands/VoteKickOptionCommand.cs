using BepInEx.Configuration;
using NuclearOption.Networking;
using Nuclei.Enums;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace Nuclei.Features.Commands.DefaultCommands;

public class VoteOptionCommand(ConfigFile config) : PermissionConfigurableCommand(config)
{
    public override string Name { get; } = "vote";
    public override string Description { get; } = "adds a vote to the current vote";
    public override string Usage { get; } = "vote";
    public override PermissionLevel DefaultPermissionLevel { get; } = PermissionLevel.Everyone;
    
    public override bool Validate(Player player, string[] args)
    {
        return args.Length == 0;
    }

    public override bool Execute(Player player, string[] args)
    {
        VoteService.HandleVote(player);
        return false;
    }

    public override bool Execute(string[] args)
    {
        throw new System.Exception("Requires Player object");
    }
}
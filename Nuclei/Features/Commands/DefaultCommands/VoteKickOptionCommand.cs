using BepInEx.Configuration;
using NuclearOption.Networking;
using Nuclei.Enums;
using Nuclei.Features.Commands;
using VoteKick.Services;

namespace VoteKick.Commands;

public class VoteKickOptionCommand(ConfigFile config) : PermissionConfigurableCommand(config)
{
    public override string Name { get; } = "vote";
    public override string Description { get; } = "adds a vote to the current vote kick";
    public override string Usage { get; } = "vote";
    public override PermissionLevel DefaultPermissionLevel { get; } = PermissionLevel.Everyone;
    
    public override bool Validate(Player player, string[] args)
    {
        return args.Length == 0;
    }

    public override bool Execute(Player player, string[] args)
    {
        VoteKickService.HandleVote(player);
        return false;
    }

    public override bool Execute(string[] args)
    {
        throw new System.Exception("Requires Player object");
    }
}
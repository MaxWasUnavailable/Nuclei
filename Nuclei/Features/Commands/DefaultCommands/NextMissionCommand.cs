using BepInEx.Configuration;
using Cysharp.Threading.Tasks;
using NuclearOption.Networking;
using NuclearOption.Networking.Lobbies;
using NuclearOption.SavedMission;
using Nuclei.Enums;
using Nuclei.Helpers;
using System;

namespace Nuclei.Features.Commands.DefaultCommands;

public class NextMissionCommand(ConfigFile config) : PermissionConfigurableCommand(config)
{
    public override string Name => "nextmission";
    public override string Description => "Ends the current mission and starts the next one in the rotation.";
    public override string Usage => "nextmission";
    public override PermissionLevel DefaultPermissionLevel => PermissionLevel.Moderator;


    public override bool Validate(Player player, string[] args) => true;

    public override bool Execute(Player player, string[] args)
    {
        MissionService.StartNextMission(player); 
        return true;
    }

    public override bool Execute(string[] args)
    {
        MissionService.StartNextMission(null); 
        return true;
    }
}

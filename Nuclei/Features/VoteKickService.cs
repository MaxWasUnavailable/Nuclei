using System.Collections.Generic;
using System.Timers;
using HarmonyLib;
using Mirage.Serialization;
using NuclearOption.Networking;
using Nuclei.Features;
using Nuclei.Helpers;

namespace VoteKick.Services;

public static class VoteKickService
{
    private static VoteKickSession _activeVoteKick;

    /// <summary>
    /// start a vote-kick session for target player
    /// </summary>
    /// <param name="initiator"></param>
    /// <param name="targetPlayer"></param>
    /// <returns></returns>
    public static bool StartVoteKick(Player initiator, Player targetPlayer)
    {
        if (_activeVoteKick != null) return false; // vote in progress
        _activeVoteKick = new VoteKickSession(initiator, targetPlayer);
        _activeVoteKick.Start();
        return true;
    }

    /// <summary>
    /// handles a vote from the vote command
    /// </summary>
    /// <param name="voter"></param>
    public static void HandleVote(Player voter)
    {
        if (_activeVoteKick == null)
        {
            var commandPrefix = (char) AccessTools.Property(typeof(NucleiConfig), "CommandPrefixChar").GetValue(null);
            ChatService.SendPrivateChatMessage($"A vote kick session has not been started, use {commandPrefix}\"votekick\" command to start one.", voter);
            return;
        }
        _activeVoteKick.AddVote(voter);
    }

    public static void StopVoteKick()
    {
        _activeVoteKick = null;
    }
}

public class VoteKickSession
{
    private readonly Player _targetPlayer;
    private readonly Timer _timer;
    private HashSet<ulong> _voters;
    private int _timeLeft;
    private int _voteThreshold; // don't want threshold changing as players leave or join
    
    private readonly int DEFAULT_VOTING_WINDOW = NucleiConfig.KickTimeout!.Value; 

    public VoteKickSession(Player initiator, Player targetPlayer)
    {
        _targetPlayer = targetPlayer;
        _voteThreshold = VoteThreshold();
        _timeLeft = DEFAULT_VOTING_WINDOW;
        _timer = new Timer(1000);
        _timer.Elapsed += OnTimerTick;
        _voters = [];
        AddVote(initiator);
    }

    public void Start()
    {
        var commandPrefix = "/";
        MissionMessages.ShowMessage($"A vote to kick {_targetPlayer.PlayerName} has been started.", false, null, true);
        MissionMessages.ShowMessage($"Use {commandPrefix}vote to join. You have {_timeLeft} seconds to cast your vote. ({_voters.Count}/{_voteThreshold} votes)", false, null, true);
        _timer.Start();
    }
    
    /// <summary>
    /// Will add a vote to the vote kick if the player is not already in the hashset.
    /// </summary>
    /// <param name="voter"></param>
    public void AddVote(Player voter)
    {
        if (_voters.Add(voter.SteamID))
        {
            MissionMessages.ShowMessage("Voter's steamID added successfully", false, null, true);
            MissionMessages.ShowMessage($"{voter.PlayerName} has voted to kick {_targetPlayer.PlayerName}. ({_voters.Count}/{_voteThreshold} votes)", false, null, true);

            if (_voters.Count >= _voteThreshold)
            {
                FinaliseVote(true);
            }
        }
        else
        {
            ChatService.SendPrivateChatMessage("You have already voted.", voter);
        }
    }

    /// <summary>
    /// Callback that is called every timer tick which is set to 1 second (1000)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnTimerTick(object sender, ElapsedEventArgs e)
    {
        _timeLeft--;

        if ((_timeLeft % 10 == 0 && _timeLeft > 0) || _timeLeft < 10) // every ten seconds or below 10 seconds every tick
        {
            MissionMessages.ShowMessage($"Vote to kick {_targetPlayer.PlayerName} ends in {_timeLeft} seconds. ({_voters.Count}/{_voteThreshold} votes)", false, null, true);
        }
        
        if (_timeLeft <= 0)
        {
            FinaliseVote(false);
        }
    }
    
    private void FinaliseVote(bool thresholdMet)
    {
        _timer.Stop();
        _timer.Dispose();
        
        if (thresholdMet)
        {
            MissionMessages.ShowMessage($"The vote to kick {_targetPlayer.PlayerName} has passed!", false, null, true);
            Globals.NetworkManagerNuclearOptionInstance.KickPlayerAsync(_targetPlayer);
        }
        else
        {
            MissionMessages.ShowMessage($"The vote to kick {_targetPlayer.PlayerName} has failed. ({_voters.Count}/{_voteThreshold} votes)", false, null, true);
        }
        VoteKickService.StopVoteKick();
    }

    private int VoteThreshold()
    {
        var threshold = NucleiConfig.KickThreshold.Value;
        var totalPlayers = Globals.AuthenticatedPlayers.Count;
        return (int)(totalPlayers * threshold);
    }
}
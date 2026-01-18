using System;
using System.Collections.Generic;
using System.Timers;
using HarmonyLib;
using NuclearOption.Networking;
using Nuclei.Helpers;

namespace Nuclei.Features;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
public static class VoteService
{
    private static VoteSession? _activeVote;

    /// <summary>
    /// start a vote-kick session for target player
    /// </summary>
    /// <param name="initiator"></param>
    /// <param name="startingMessage"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static bool StartVote(Player initiator, string startingMessage, Action action)
    {
        if (_activeVote != null)
        {
            return false; // vote in progress
        }

        _activeVote = new VoteSession(initiator, startingMessage, action);
        _activeVote.Start();
        return true;
    }

    /// <summary>
    /// handles a vote from the vote command
    /// </summary>
    /// <param name="voter"></param>
    public static void HandleVote(Player voter, bool votedYes)
    {
        if (_activeVote == null)
        {
            var commandPrefix = (char) AccessTools.Property(typeof(NucleiConfig), "CommandPrefixChar").GetValue(null);
            ChatService.SendPrivateChatMessage($"A vote session has not been started, use a vote command to start one.", voter);
        }
        else _activeVote.AddVote(voter, votedYes);
    }

    public static void StopVoteKick()
    {
        _activeVote = null;
    }
}

public class VoteSession
{
    private readonly Player _initiator;
    private readonly Timer _timer;
    private HashSet<ulong> _yesVoters;
    private HashSet<ulong> _noVoters;
    private readonly string _startingMessage;
    private int _timeLeft;
    private int _voteThreshold; // don't want threshold changing as players leave or join

    // Function to call when vote succeeds
    private Action _action;
    
    private static readonly int DEFAULT_VOTING_WINDOW = NucleiConfig.KickTimeout!.Value; 

    public VoteSession(Player initiator, string startingMessage, Action action)
    {
        _initiator = initiator;
        _voteThreshold = VoteThreshold();
        _timeLeft = DEFAULT_VOTING_WINDOW;
        _timer = new Timer(1000);
        _timer.Elapsed += OnTimerTick;
        _yesVoters = [];
        _noVoters = [];
        _startingMessage = startingMessage;
        _action = action;
    }

    public void Start()
    {
        ChatService.SendChatMessage(_startingMessage);
        ChatService.SendChatMessage($"Type '{NucleiConfig.CommandPrefixChar}y' to vote yes, '{NucleiConfig.CommandPrefixChar}n' to vote no. You have {_timeLeft} seconds to cast your vote. ({_yesVoters.Count}/{_voteThreshold} YES votes, {_noVoters.Count}/{_voteThreshold} NO votes).");
        _timer.Start();
        AddVote(_initiator, true);
    }

    /// <summary>
    /// Will add a vote to the vote kick if the player is not already in the hashset.
    /// </summary>
    /// <param name="voter"></param>
    public void AddVote(Player voter, bool votedYes)
    {
        if (votedYes)
        {
            if (_yesVoters.Add(voter.SteamID))
            {
                ChatService.SendChatMessage(
                    $"{voter.PlayerName} has voted. ({_yesVoters.Count}/{_voteThreshold} YES votes, {_noVoters.Count}/{_voteThreshold} NO votes).");

                if (_yesVoters.Count >= _voteThreshold)
                {
                    ChatService.SendChatMessage("YES votes have reached a majority.");
                    FinaliseVote(true);
                }
            }
            else
            {
                ChatService.SendPrivateChatMessage("You have already voted.", voter);
            }
        }
        else
        {
            if (_noVoters.Add(voter.SteamID))
            {
                ChatService.SendChatMessage(
                    $"{voter.PlayerName} has voted. ({_yesVoters.Count}/{_voteThreshold} YES votes, {_noVoters.Count}/{_voteThreshold} NO votes).");

                if (_noVoters.Count >= _voteThreshold)
                {
                    ChatService.SendChatMessage("NO votes have reached a majority.");
                    FinaliseVote(false);
                }
            }
            else
            {
                ChatService.SendPrivateChatMessage("You have already voted.", voter);
            }
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
            ChatService.SendChatMessage($"Vote ends in {_timeLeft} seconds. Type `{NucleiConfig.CommandPrefixChar}y` to vote YES, '{NucleiConfig.CommandPrefixChar}n' for NO. ({_yesVoters.Count}/{_voteThreshold} YES votes, {_noVoters.Count}/{_voteThreshold} NO votes).");
        }
        
        if (_timeLeft <= 0)
        {
            FinaliseVote(false);
        }
    }
    
    
    /// <summary>
    /// Checks if vote threshold is met, then calls the action function associated
    /// </summary>
    /// <param name="sender"></param>
    private void FinaliseVote(bool thresholdMet)
    {
        _timer.Stop();
        _timer.Dispose();
        VoteService.StopVoteKick();
        
        if (thresholdMet)
        {
            ChatService.SendChatMessage($"The vote has passed!");
            _action();
        }
        else
        {
            ChatService.SendChatMessage($"The vote has failed. ({_yesVoters.Count}/{_voteThreshold} YES votes, {_noVoters.Count}/{_voteThreshold} NO votes)");
        }
    }

    private int VoteThreshold()
    {
        var threshold = NucleiConfig.KickThreshold.Value;
        var totalPlayers = Globals.AuthenticatedPlayers.Count;
        return (int)(totalPlayers * threshold);
    }
}
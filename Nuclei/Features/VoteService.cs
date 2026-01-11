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
    public static void HandleVote(Player voter)
    {
        if (_activeVote == null)
        {
            var commandPrefix = (char) AccessTools.Property(typeof(NucleiConfig), "CommandPrefixChar").GetValue(null);
            ChatService.SendPrivateChatMessage($"A vote session has not been started, use a vote command to start one.", voter);
        }
        else _activeVote.AddVote(voter);
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
    private HashSet<ulong> _voters;
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
        _voters = [];
        _startingMessage = startingMessage;
        _action = action;
    }

    public void Start()
    {
        var commandPrefix = "/";
        ChatService.SendChatMessage(_startingMessage);
        MissionMessages.ShowMessage($"Use {commandPrefix}vote to join. You have {_timeLeft} seconds to cast your vote. ({_voters.Count}/{_voteThreshold} votes)", false, null, true);
        _timer.Start();
        AddVote(_initiator);
    }
    
    /// <summary>
    /// Will add a vote to the vote kick if the player is not already in the hashset.
    /// </summary>
    /// <param name="voter"></param>
    public void AddVote(Player voter)
    {
        if (_voters.Add(voter.SteamID))
        {
            ChatService.SendChatMessage("Voter's steamID added successfully");
            ChatService.SendChatMessage($"{voter.PlayerName} has voted. ({_voters.Count}/{_voteThreshold} votes)");

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
            MissionMessages.ShowMessage($"Vote ends in {_timeLeft} seconds. ({_voters.Count}/{_voteThreshold} votes)", false, null, true);
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
            ChatService.SendChatMessage($"The vote has failed. ({_voters.Count}/{_voteThreshold} votes)");

        }
    }

    private int VoteThreshold()
    {
        var threshold = NucleiConfig.KickThreshold.Value;
        var totalPlayers = Globals.AuthenticatedPlayers.Count;
        return (int)(totalPlayers * threshold);
    }
}
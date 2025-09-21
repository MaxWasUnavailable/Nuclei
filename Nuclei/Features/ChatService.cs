using Mirage;
using NuclearOption.Networking;
using Nuclei.Helpers;
using System;
using static Rewired.Controller;

namespace Nuclei.Features;

/// <summary>
///     Manages server chat functionality.
/// </summary>
public static class ChatService
{
    private static bool CanSend(string message, bool ignoreEmpty = false, bool ignoreRateLimit = false)
    {
        if (string.IsNullOrWhiteSpace(message) && !ignoreEmpty)
        {
            Nuclei.Logger?.LogWarning("Cannot send empty chat message.");
            return false;
        }
        if (!Globals.ChatManagerInstance)
        {
            Nuclei.Logger?.LogWarning("Chat manager instance is null.");
            return false;
        }

        if (ignoreRateLimit)
            return true;
        
        try
        {
            return ChatManager.CanSend(message, true, true);
        }
        catch (ArgumentException e)
        {
            Nuclei.Logger?.LogError($"Cannot send chat message: {e.Message}");
            return false;
        }
    }

    /// <summary>
    ///     Sanitizes a chat message to prevent command injection.
    /// </summary>
    /// <param name="message"> The message to sanitize. </param>
    /// <returns> The sanitized message. </returns>
    private static string SanitizeMessage(this string message)
    {
        return message.TrimStart(NucleiConfig.CommandPrefixChar);
    }
    
    /// <summary>
    ///     Pre-processes a chat message by replacing dynamic placeholders &amp; sanitizing it.
    /// </summary>
    /// <param name="message"> The message to pre-process. </param>
    /// <param name="player"> Player to use for chat message variables </param>
    /// <returns> The pre-processed message. </returns>
    private static string PreProcessMessage(this string message, Player? player = null)
    {
        return DynamicPlaceholderUtils.ReplaceDynamicPlaceholders(message, player).SanitizeMessage();
    }

    /// <summary>
    ///     Sends a chat message to all clients.
    /// </summary>
    /// <param name="message"> The message to send. </param>
    /// <param name="player"> Player to use for chat message variables </param>
    public static void SendChatMessage(string message, Player? player = null)
    {
        var actualMessage = message.PreProcessMessage(player);
        
        if (!CanSend(actualMessage, ignoreRateLimit: true))
        {
            Nuclei.Logger?.LogWarning("Cannot send chat message.");
            return;
        }
        
        /* This uses the recipient as the "sender". TargetReceiveMessage requires this.
           No way around it, devs have said that this will be changed in upcoming patch.
         */
        int sent = 0;
        foreach (var conn in Globals.MissionManagerInstance.Server.AuthenticatedPlayers)
        {
            try
            {
                if (!conn.TryGetPlayer<Player>(out var recipient) || recipient == null)
                    continue;
                 
                Globals.ChatManagerInstance.TargetReceiveMessage(conn, actualMessage, recipient, true);
                sent++;
            }
            catch (Exception ex)
            {
                Nuclei.Logger.LogError($"[Admin] Broadcast to a connection failed: {ex}");
            }
        }

    }

    /// <summary>
    ///     Sends a private chat message to a player.
    /// </summary>
    /// <param name="message"> The message to send. </param>
    /// <param name="targetPlayer"> The player to send the message to. </param>
    public static void SendPrivateChatMessage(string message, Player targetPlayer)
    {
        var actualMessage = message.PreProcessMessage(targetPlayer);
        
        if (!CanSend(actualMessage, ignoreRateLimit: true))
        {
            Nuclei.Logger?.LogWarning("Cannot send private chat message.");
            return;
        }

        Globals.ChatManagerInstance.TargetReceiveMessage(targetPlayer.Owner, actualMessage, targetPlayer, true);
        Nuclei.Logger?.LogInfo($"Sent private message to {targetPlayer.PlayerName}: {actualMessage}");
    }

    /// <summary>
    ///     Sends the message of the day to all clients.
    /// </summary>
    public static void SendMotD()
    {
        var actualMotD = NucleiConfig.MessageOfTheDay!.Value.PreProcessMessage();
        
        if (!CanSend(actualMotD, ignoreRateLimit: true))
        {
            Nuclei.Logger?.LogWarning("Cannot send message of the day.");
            return;
        }

        int sent = 0;
        foreach (var conn in Globals.MissionManagerInstance.Server.AuthenticatedPlayers)
        {
            try
            {
                // skip the dedicated server’s host connection if present
                if (conn.IsHost) continue;

                if (!conn.TryGetPlayer<Player>(out var recipient) || recipient == null)
                    continue;

                // Targeted ClientRpc: one message per client, no duplicates

                Globals.ChatManagerInstance.TargetReceiveMessage(conn, actualMotD, recipient, true);
                sent++;
            }
            catch (Exception ex)
            {
                Nuclei.Logger.LogError($"[Admin] Broadcast to a connection failed: {ex}");
            }
        }
    }
}
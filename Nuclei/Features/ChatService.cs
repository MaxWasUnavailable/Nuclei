using System;
using Nuclei.Helpers;

namespace Nuclei.Features;

/// <summary>
///     Manages server chat functionality.
/// </summary>
public static class ChatService
{
    private static bool CanSend(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            Nuclei.Logger?.LogWarning("Cannot send empty chat message.");
            return false;
        }
        if (Globals.ChatManagerInstance == null)
        {
            Nuclei.Logger?.LogWarning("Chat manager instance is null.");
            return false;
        }

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
    ///     Fills chat message variables.
    /// </summary>
    /// <param name="message"> The message to fill. </param>
    /// <param name="player"> The player to use for filling player-specific variables. </param>
    /// <returns></returns>
    public static string FillChatMessageVariables(string message, Player? player = null)
    {
        if (player != null)
        {
            message = message.Replace("{username}", player.PlayerName);
        }
        
        return message;
    }

    /// <summary>
    ///     Sends a chat message to all clients.
    /// </summary>
    /// <param name="message"> The message to send. </param>
    /// <param name="player"> The player sending the message. </param>
    public static void SendChatMessage(string message, Player? player = null)
    {
        if (!CanSend(message))
        {
            Nuclei.Logger?.LogWarning("Cannot send chat message.");
            return;
        }
        
        Globals.ChatManagerInstance.CmdSendChatMessage(FillChatMessageVariables(message, player), true);
    }
}
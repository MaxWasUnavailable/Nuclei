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
        return ChatManager.CanSend(message, true, true);
    }

    /// <summary>
    ///     Sends a chat message to all clients.
    /// </summary>
    /// <param name="message"> The message to send. </param>
    public static void SendChatMessage(string message)
    {
        if (!CanSend(message))
        {
            Nuclei.Logger?.LogWarning("Cannot send chat message.");
            return;
        }
        
        Globals.ChatManagerInstance.CmdSendChatMessage(message, true);
    }
}
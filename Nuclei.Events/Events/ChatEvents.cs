using System;
using Nuclei.Abstractions.NO.Chat;

namespace Nuclei.Events.Events;

/// <summary>
///     Declares chat-related events.
/// </summary>
public static class ChatEvents
{
    /// <summary>
    ///     Event handler fired before a chat message is processed.
    /// </summary>
    public static event ChatMessageEventHandler? PreChatMessageSent;

    /// <summary>
    ///     Event handler fired after a chat message is processed.
    /// </summary>
    public static event Action<ChatMessageEvent>? PostChatMessageSent;

    internal static void OnPreChatMessageSent(ref ChatMessageEvent message, ref bool shouldSendOriginal)
    {
        PreChatMessageSent?.Invoke(ref message, ref shouldSendOriginal);
    }

    internal static void OnPostChatMessageSent(ChatMessageEvent message)
    {
        PostChatMessageSent?.Invoke(message);
    }
}

/// <summary>
///     Data for a chat message event.
/// </summary>
/// <param name="Message">The chat message associated with the event.</param>
public sealed record ChatMessageEvent(ChatMessage Message);

/// <summary>
///     Delegate for cancelable chat message events.
/// </summary>
/// <param name="message">The chat message associated with the event.</param>
/// <param name="shouldSendOriginal">
///     Whether the original message should be sent. If False, prevent patched method from running.
/// </param>
public delegate void ChatMessageEventHandler(ref ChatMessageEvent message, ref bool shouldSendOriginal);

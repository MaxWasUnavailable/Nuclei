using System;
using Nuclei.Abstractions.Chat;

namespace Nuclei.Events.Events;

/// <summary>
///     Declares chat-related events.
/// </summary>
public static class ChatEvents
{
    /// <summary>
    ///     Event handler fired before a chat message is processed.
    /// </summary>
    public static event Action<ChatMessageEvent>? PreChatMessageSent;

    /// <summary>
    ///     Event handler fired after a chat message is processed.
    /// </summary>
    public static event Action<ChatMessageEvent>? PostChatMessageSent;

    internal static void OnPreChatMessageSent(ChatMessageEvent message)
    {
        PreChatMessageSent?.Invoke(message);
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

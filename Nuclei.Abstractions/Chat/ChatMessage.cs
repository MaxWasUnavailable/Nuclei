using Nuclei.Abstractions.Players;

namespace Nuclei.Abstractions.Chat;

/// <summary>
///     Data for a chat message event.
/// </summary>
public sealed record ChatMessage(IPlayerInfo Sender, string Message, bool AllChat);


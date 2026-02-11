using Nuclei.Abstractions.NO.Players;

namespace Nuclei.Abstractions.NO.Chat;

/// <summary>
///     Data for a chat message event.
/// </summary>
public sealed record ChatMessage(IPlayerInfo Sender, string Message, bool AllChat);


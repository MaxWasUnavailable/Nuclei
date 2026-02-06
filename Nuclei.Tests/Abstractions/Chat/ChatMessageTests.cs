using FluentAssertions;
using Nuclei.Abstractions.Chat;
using Nuclei.Abstractions.Players;

namespace Nuclei.Tests.Abstractions.Chat;

/// <summary>
///     Tests for <see cref="ChatMessage" />.
/// </summary>
public sealed class ChatMessageTests
{
    [Test]
    public void Constructor_SetsProperties()
    {
        var sender = new PlayerInfo("Ace", 123UL);

        var message = new ChatMessage(sender, "Hello", true);

        message.Sender.Should().Be(sender);
        message.Message.Should().Be("Hello");
        message.AllChat.Should().BeTrue();
    }

    [Test]
    public void Equality_WithSameValues_IsEqual()
    {
        var sender = new PlayerInfo("Ace", 123UL);
        var first = new ChatMessage(sender, "Hello", false);
        var second = new ChatMessage(new PlayerInfo("Ace", 123UL), "Hello", false);

        first.Should().Be(second);
    }

    [Test]
    public void Equality_WithDifferentValues_IsNotEqual()
    {
        var sender = new PlayerInfo("Ace", 123UL);
        var first = new ChatMessage(sender, "Hello", false);
        var second = new ChatMessage(sender, "Different", false);

        first.Should().NotBe(second);
    }
}


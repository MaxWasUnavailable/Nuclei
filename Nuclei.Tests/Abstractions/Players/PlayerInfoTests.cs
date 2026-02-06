using FluentAssertions;
using Nuclei.Abstractions.Players;

namespace Nuclei.Tests.Abstractions.Players;

/// <summary>
///     Tests for <see cref="PlayerInfo" />.
/// </summary>
public sealed class PlayerInfoTests
{
    [Test]
    public void Constructor_SetsProperties()
    {
        var info = new PlayerInfo("Ace", 123UL);

        info.Name.Should().Be("Ace");
        info.SteamId.Should().Be(123UL);
    }

    [Test]
    public void Equality_WithSameValues_IsEqual()
    {
        var first = new PlayerInfo("Ace", 123UL);
        var second = new PlayerInfo("Ace", 123UL);

        first.Should().Be(second);
        first.GetHashCode().Should().Be(second.GetHashCode());
    }

    [Test]
    public void Equality_WithDifferentValues_IsNotEqual()
    {
        var first = new PlayerInfo("Ace", 123UL);
        var second = new PlayerInfo("Viper", 456UL);

        first.Should().NotBe(second);
    }

    [Test]
    public void Constructor_AllowsNullName()
    {
        var info = new PlayerInfo(null, 0UL);

        info.Name.Should().BeNull();
        info.SteamId.Should().Be(0UL);
    }
}

